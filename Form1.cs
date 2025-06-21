using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ShiTuApiTestApp.ShiTuCApi;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ShiTuApiTestApp
{   
    public class GlobalConfigForVis
    {
        
        [YamlMember(Alias = "visualization_output_dir")]
        public string VisualizationOutputDir { get; set; }
    }

    public class YamlConfigForVis
    {
        [YamlMember(Alias = "Global")]
        public GlobalConfigForVis Global { get; set; }
    }

    public partial class Form1 : Form
    {
        private IntPtr apiHandle = IntPtr.Zero;
        private bool isBusy = false;
        private string lastVisualizationPath = null;
        private readonly ShiTuCApi.CShiTuProgressCallback progressCallbackInstance;

        public Form1()
        {
            InitializeComponent();
            progressCallbackInstance = new ShiTuCApi.CShiTuProgressCallback(ProgressCallbackHandler);
            UpdateStatusLabel();
            Console.SetOut(new TextBoxStreamWriter(txtLog));
            Console.SetError(new TextBoxStreamWriter(txtLog));
        }

        #region Handle & Initialization
        private void btnCreateHandle_Click(object sender, EventArgs e)
        {
            LogMessage("尝试创建 API 句柄...");
            if (apiHandle != IntPtr.Zero)
            {
                LogMessage("句柄已存在。请先销毁。", true);
                return;
            }
            var result = ShiTuCApi.ShiTuApi_Create(out apiHandle);
            if (result == ShiTuApiResultCode.SHITU_API_SUCCESS && apiHandle != IntPtr.Zero)
            {
                LogMessage("API 句柄创建成功。");
            }
            else
            {
                LogMessage($"创建 API 句柄失败。代码: {result}, 消息: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
                apiHandle = IntPtr.Zero;
            }
            UpdateStatusLabel();
        }

        private void btnDestroyHandle_Click(object sender, EventArgs e)
        {
            LogMessage("尝试销毁 API 句柄...");
            if (apiHandle == IntPtr.Zero)
            {
                LogMessage("句柄已经是 null。");
                return;
            }
            var result = ShiTuCApi.ShiTuApi_Destroy(ref apiHandle);
            if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
            {
                LogMessage("API 句柄销毁成功。");
            }
            else
            {
                LogMessage($"销毁 API 句柄失败。代码: {result}, 消息: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
                apiHandle = IntPtr.Zero;
            }
            UpdateStatusLabel();
        }

        private void btnBrowseConfig_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "YAML 配置文件 (*.yaml;*.yml)|*.yaml;*.yml|所有文件 (*.*)|*.*", Title = "选择配置文件" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                    txtConfigPath.Text = ofd.FileName;
            }
        }

        private void btnInitialize_Click(object sender, EventArgs e)
        {
            if (apiHandle == IntPtr.Zero) { LogMessage("请先创建句柄。", true); return; }
            if (string.IsNullOrWhiteSpace(txtConfigPath.Text) || !File.Exists(txtConfigPath.Text))
            {
                LogMessage("请选择一个有效的配置文件路径。", true);
                return;
            }

            LogMessage($"使用配置文件 '{txtConfigPath.Text}' 初始化 API (将检查内部许可证状态)...");

            try
            {
                var result = ShiTuCApi.ShiTuApi_InitializeWithLicense(apiHandle, txtConfigPath.Text);

                if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
                {
                    LogMessage("API 初始化成功。");
                    btnGetLicenseInfo_Click(sender, e);
                }
                else
                {
                    LogMessage($"API 初始化失败。代码: {result}, 消息: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
                    btnGetLicenseInfo_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"在调用本地代码时发生严重错误: {ex.GetType().Name} - {ex.Message}", true);
                LogMessage($"这通常意味着 C# P/Invoke 声明与 C++ DLL 导出函数不匹配，或者DLL本身或其依赖项缺失。", true);
            }

            UpdateStatusLabel();
        }
        #endregion

        #region Index Operations
        private void btnBrowseImageList_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*", Title = "选择图像列表文件" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                    txtImageListPath.Text = ofd.FileName;
            }
        }

        private async void btnBuildIndex_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0)
            {
                LogMessage("API 未初始化或句柄无效。", true);
                return;
            }
            if (isBusy) return;

            if (string.IsNullOrWhiteSpace(txtImageListPath.Text) || !File.Exists(txtImageListPath.Text))
            {
                LogMessage("请选择一个有效的图像列表文件路径。", true);
                return;
            }

            SetBusyState(true, "正在构建索引...");
            LogMessage($"从列表文件开始异步构建索引: {txtImageListPath.Text}...");

            try
            {
                await Task.Run(() =>
                {
                    var result = ShiTuCApi.ShiTuApi_BuildIndexFromFile(apiHandle, txtImageListPath.Text, progressCallbackInstance, IntPtr.Zero);
                    this.Invoke(new Action(() =>
                    {
                        if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
                        {
                            LogMessage("索引构建命令完成。");
                            if (progressBarBuild.Maximum > 0) progressBarBuild.Value = progressBarBuild.Maximum;
                            btnGetInfo_Click(null, null);
                        }
                        else
                        {
                            LogMessage($"索引构建失败。代码: {result}, 消息: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
                        }
                    }));
                });
                LogMessage("构建任务已结束。");
            }
            catch (Exception ex)
            {
                LogMessage($"构建索引时发生异常: {ex.Message}", true);
            }
            finally
            {
                SetBusyState(false);
            }
        }

        private void btnSaveIndex_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0)
            {
                LogMessage("API 未初始化或句柄无效。", true);
                return;
            }

            LogMessage("保存索引...");
            var result = ShiTuCApi.ShiTuApi_SaveIndex(apiHandle);
            if (result == ShiTuApiResultCode.SHITU_API_SUCCESS) LogMessage("索引保存成功。");
            else LogMessage($"保存索引失败。代码: {result}, 消息: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
        }

        private void btnExportIndexToCsv_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0)
            {
                LogMessage("API 未初始化或句柄无效。", true);
                return;
            }

            using (var sfd = new SaveFileDialog { Filter = "CSV 文件 (*.csv)|*.csv", Title = "API 导出索引为 CSV", FileName = "api_exported_index.csv" })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;

                LogMessage($"正在调用 API 直接导出索引到: {sfd.FileName}...");
                var result = ShiTuCApi.ShiTuApi_ExportLabels(apiHandle, sfd.FileName);

                if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
                {
                    LogMessage("API 导出索引成功。");
                }
                else
                {
                    LogMessage($"API 导出索引失败。代码: {result}, 消息: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
                }
            }
        }
        #endregion

        #region Search Operations
        private void btnBrowseQueryImage_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "图像文件(*.BMP;*.JPG;*.JPEG;*.PNG)|*.BMP;*.JPG;*.JPEG;*.GIF;*.PNG|所有文件 (*.*)|*.*", Title = "选择查询图像" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtQueryImagePath.Text = ofd.FileName;
                    try
                    {
                        byte[] imageBytes = File.ReadAllBytes(ofd.FileName);
                        using (var ms = new MemoryStream(imageBytes))
                        {
                            if (pictureBoxQuery.Image != null) pictureBoxQuery.Image.Dispose();
                            pictureBoxQuery.Image = Image.FromStream(ms);
                        }
                    }
                    catch (Exception ex)
                    {
                        pictureBoxQuery.Image = null;
                        LogMessage($"加载预览图像失败: {ex.Message}", true);
                    }
                }
            }
        }

        private async void btnSearchFolder_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0) { LogMessage("API 未初始化或句柄无效。", true); return; }
            if (isBusy) return;

            using (var fbd = new FolderBrowserDialog { Description = "选择包含图片的文件夹进行批量搜索" })
            {
                if (fbd.ShowDialog() != DialogResult.OK) return;

                string folderPath = fbd.SelectedPath;
                var validExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".bmp" };
                var imageFiles = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => validExtensions.Contains(Path.GetExtension(f)))
                    .ToList();

                if (imageFiles.Count == 0)
                {
                    LogMessage("所选文件夹中未找到支持的图片文件。", true);
                    return;
                }

                SetBusyState(true, "正在批量搜索...");
                LogMessage($"开始批量搜索文件夹: {folderPath} ({imageFiles.Count} 张图片)");
                ClearResultsDisplay();
                string csvOutputPath = Path.Combine(folderPath, $"search_results_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

                IProgress<(string message, ListViewItem item)> progress = new System.Progress<(string message, ListViewItem item)>(update =>
                {
                    if (!string.IsNullOrEmpty(update.message))
                    {
                        LogMessage(update.message);
                    }
                    if (update.item != null)
                    {
                        var groupName = update.item.Group.Name;
                        var group = listViewResults.Groups[groupName] ?? listViewResults.Groups.Add(groupName, groupName);
                        update.item.Group = group;
                        listViewResults.Items.Add(update.item);
                    }
                });

                try
                {
                    await Task.Run(() =>
                    {
                        using (var csvWriter = new StreamWriter(csvOutputPath, false, Encoding.UTF8))
                        {
                            csvWriter.WriteLine("ImageFile,ResultNo,MatchID,MatchLabel,Score,Distance,BoundingBox");
                            int imageCounter = 0;

                            foreach (var imageFile in imageFiles)
                            {
                                imageCounter++;
                                var fileName = Path.GetFileName(imageFile);
                                progress.Report(($"({imageCounter}/{imageFiles.Count}) 正在搜索: {fileName}", null));

                                IntPtr resultsPtr = IntPtr.Zero;
                                int numResults = 0;
                                try
                                {
                                    var result = ShiTuApi_SearchByPath(apiHandle, imageFile, -1f, -1f, -1, out resultsPtr, out numResults);

                                    if (result == ShiTuApiResultCode.SHITU_API_SUCCESS && numResults > 0)
                                    {
                                        int structSize = Marshal.SizeOf(typeof(CShiTuSearchResult));
                                        for (int i = 0; i < numResults; i++)
                                        {
                                            IntPtr currentPtr = IntPtr.Add(resultsPtr, i * structSize);
                                            var cResult = (CShiTuSearchResult)Marshal.PtrToStructure(currentPtr, typeof(CShiTuSearchResult));

                                            var lvi = new ListViewItem(new[] { cResult.id.ToString(), cResult.GetLabelString() ?? "", cResult.score.ToString("F6"), cResult.distance.ToString("F6"), cResult.box.ToString() });
                                            lvi.Group = new ListViewGroup(fileName, fileName);

                                            string escapedLabel = $"\"{cResult.GetLabelString()?.Replace("\"", "\"\"") ?? ""}\"";
                                            string csvLine = $"\"{fileName}\",{i + 1},{cResult.id},{escapedLabel},{cResult.score:F6},{cResult.distance:F6},\"{cResult.box}\"";
                                            csvWriter.WriteLine(csvLine);

                                            progress.Report((string.Empty, lvi));
                                        }
                                    }
                                }
                                finally
                                {
                                    if (resultsPtr != IntPtr.Zero) ShiTuApi_FreeResults(resultsPtr, numResults);
                                }
                                this.Invoke(new Action(() => LoadLatestVisualization()));
                            }
                        }
                    });

                    LogMessage($"批量搜索完成。结果已保存到: {csvOutputPath}");
                }
                catch (Exception ex)
                {
                    LogMessage($"批量搜索过程中发生严重错误: {ex.Message}", true);
                }
                finally
                {
                    SetBusyState(false);
                }
            }
        }

        private void ProcessAndDisplayResults(ShiTuApiResultCode result, IntPtr resultsPtr, int numResults)
        {
            if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
            {
                LogMessage($"搜索成功。找到 {numResults} 个结果。");
                DisplaySearchResults(resultsPtr, numResults);
            }
            else
            {
                LogMessage($"搜索失败。代码: {result}, 消息: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
                ClearResultsDisplay();
            }
        }

        private void btnSearchPath_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0)
            {
                LogMessage("API 未初始化或句柄无效。", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtQueryImagePath.Text) || !File.Exists(txtQueryImagePath.Text))
            {
                LogMessage("请选择一个有效的查询图像路径。", true);
                return;
            }

            LogMessage($"按路径搜索: {txtQueryImagePath.Text}...");
            IntPtr resultsPtr = IntPtr.Zero;
            int numResults = 0;
            try
            {
                var result = ShiTuCApi.ShiTuApi_SearchByPath(apiHandle, txtQueryImagePath.Text, -1.0f, -1.0f, -1, out resultsPtr, out numResults);
                ProcessAndDisplayResults(result, resultsPtr, numResults);
                LoadLatestVisualization();
            }
            finally
            {
                if (resultsPtr != IntPtr.Zero) ShiTuApi_FreeResults(resultsPtr, numResults);
            }
        }

        private void btnSearchData_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0)
            {
                LogMessage("API 未初始化或句柄无效。", true);
                return;
            }
            if (pictureBoxQuery.Image == null)
            {
                LogMessage("请先加载一张图片以获取数据。", true);
                return;
            }

            LogMessage("从预览图像中加载数据并搜索...");
            byte[] imageData;
            int width, height, stride;

            using (var bmp = new Bitmap(pictureBoxQuery.Image))
            using (var bmpCloned = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format24bppRgb))
            {
                width = bmpCloned.Width;
                height = bmpCloned.Height;
                BitmapData bmpData = bmpCloned.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bmpCloned.PixelFormat);
                stride = bmpData.Stride;
                imageData = new byte[stride * height];
                Marshal.Copy(bmpData.Scan0, imageData, 0, imageData.Length);
                bmpCloned.UnlockBits(bmpData);
            }

            IntPtr resultsPtr = IntPtr.Zero;
            int numResults = 0;
            try
            {
                var result = ShiTuCApi.ShiTuApi_SearchByData(apiHandle, imageData, width, height, 3, stride, ShiTuApiImageFormat.SHITU_API_IMAGE_FORMAT_BGR, -1f, -1f, -1, out resultsPtr, out numResults);
                ProcessAndDisplayResults(result, resultsPtr, numResults);
                LoadLatestVisualization();
            }
            finally
            {
                if (resultsPtr != IntPtr.Zero) ShiTuApi_FreeResults(resultsPtr, numResults);
            }
        }
        #endregion

        #region Info & Deletion
        private void btnGetInfo_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0)
            {
                LogMessage("API 未初始化或句柄无效。", true);
                return;
            }
            var dimResult = ShiTuCApi.ShiTuApi_GetIndexDimension(apiHandle, out int dim);
            var countResult = ShiTuCApi.ShiTuApi_GetIndexNumItems(apiHandle, out ulong count);
            lblDimension.Text = $"维度: {(dimResult == ShiTuApiResultCode.SHITU_API_SUCCESS ? dim.ToString() : "错误")}";
            lblItemCount.Text = $"数量: {(countResult == ShiTuApiResultCode.SHITU_API_SUCCESS ? count.ToString() : "错误")}";
        }

        private void btnDeleteId_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0)
            {
                LogMessage("API 未初始化或句柄无效。", true);
                return;
            }
            if (!long.TryParse(txtDeleteId.Text, out long idToDelete))
            {
                LogMessage("ID格式无效。", true);
                return;
            }
            LogMessage($"尝试删除 ID: {idToDelete}...");
            var result = ShiTuCApi.ShiTuApi_DeleteItem(apiHandle, idToDelete);
            if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
            {
                LogMessage($"DeleteItem 命令完成。");
                btnGetInfo_Click(sender, e);
            }
            else
            {
                LogMessage($"DeleteItem 失败。代码: {result}, 消息: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
            }
        }

        private void btnDeleteIds_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0)
            {
                LogMessage("API 未初始化或句柄无效。", true);
                return;
            }

            var idsToDelete = new List<long>();
            string input = txtDeleteIds.Text.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                using (var ofd = new OpenFileDialog { Filter = "ID 列表文件 (*.txt)|*.txt|所有文件 (*.*)|*.*", Title = "选择包含ID的文本文件" })
                {
                    if (ofd.ShowDialog() != DialogResult.OK)
                    {
                        LogMessage("操作取消。", false);
                        return;
                    }
                    try
                    {
                        input = File.ReadAllText(ofd.FileName);
                        LogMessage($"从文件 '{Path.GetFileName(ofd.FileName)}' 加载ID列表。");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"读取ID文件失败: {ex.Message}", true);
                        return;
                    }
                }
            }

            var idStrings = input.Split(new[] { ',', '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var idStr in idStrings)
            {
                if (long.TryParse(idStr.Trim(), out long id)) idsToDelete.Add(id);
                else LogMessage($"警告: 无效的ID格式 '{idStr}'", true);
            }

            if (idsToDelete.Count == 0) { LogMessage("未解析到任何有效ID。", true); return; }

            LogMessage($"尝试删除 {idsToDelete.Count} 个ID...");
            var result = ShiTuCApi.ShiTuApi_DeleteItems(apiHandle, idsToDelete.ToArray(), (ulong)idsToDelete.Count, out ulong numDeleted);

            if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
            {
                LogMessage($"DeleteItems 命令完成。API报告删除了 {numDeleted} 个条目。");
                btnGetInfo_Click(sender, e);
            }
            else
            {
                LogMessage($"DeleteItems 失败。代码: {result}, 消息: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
            }
        }

        private void btnGetLabelById_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0)
            {
                LogMessage("API 未初始化或句柄无效。", true);
                return;
            }
            if (!long.TryParse(txtDeleteId.Text, out long idToQuery))
            {
                LogMessage("请输入一个有效的ID以进行查询。", true);
                return;
            }

            LogMessage($"正在查询 ID: {idToQuery} 的标签...");
            IntPtr labelPtr = IntPtr.Zero;
            try
            {
                var result = ShiTuCApi.ShiTuApi_GetLabelById(apiHandle, idToQuery, out labelPtr);
                if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
                {
                    if (labelPtr != IntPtr.Zero)
                    {
                        string label = Marshal.PtrToStringUTF8(labelPtr);
                        LogMessage($"查询成功。ID: {idToQuery}, 标签: '{label}'");
                    }
                    else
                    {
                        LogMessage($"查询完成。ID: {idToQuery} 在索引中不存在。");
                    }
                }
                else
                {
                    LogMessage($"查询标签失败。代码: {result}, 消息: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
                }
            }
            finally
            {
                if (labelPtr != IntPtr.Zero)
                {
                    ShiTuCApi.ShiTuApi_FreeString(labelPtr);
                }
            }
        }
        #endregion

        #region License Operations
        private void btnGetLicenseInfo_Click(object sender, EventArgs e)
        {
            if (apiHandle == IntPtr.Zero) { LogMessage("请先创建句柄。", true); return; }

            LogMessage("正在获取许可证信息...");
            CShiTuLicenseInfo licenseInfo = default;
            try
            {
                var result = ShiTuCApi.ShiTuApi_GetLicenseInfo(apiHandle, out licenseInfo);
                if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"状态: {licenseInfo.GetStatusString()}");
                    if (!string.IsNullOrEmpty(licenseInfo.GetMachineCode())) sb.AppendLine($"机器码: {licenseInfo.GetMachineCode()}");
                    if (licenseInfo.is_permanent == 1) sb.AppendLine("类型: 永久许可证");
                    else sb.AppendLine($"到期: {licenseInfo.GetExpiryDate()} | 剩余: {licenseInfo.GetRemainingDays()} 天");
                    if (!string.IsNullOrEmpty(licenseInfo.GetErrorMessage())) sb.AppendLine($"消息: {licenseInfo.GetErrorMessage()}");

                    this.Invoke(new Action(() => txtLicenseInfo.Text = sb.ToString()));
                    LogMessage("许可证信息获取成功。");
                }
                else
                {
                    LogMessage($"获取许可证信息失败。代码: {result}, 消息: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
                }
            }
            finally
            {
                ShiTuCApi.ShiTuApi_FreeLicenseInfoContents(ref licenseInfo);
            }
        }

        private void btnCopyMachineCode_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLicenseInfo.Text)) { LogMessage("请先获取许可证信息。", true); return; }
            var match = Regex.Match(txtLicenseInfo.Text, @"机器码:\s*(\S+)");
            if (match.Success)
            {
                Clipboard.SetText(match.Groups[1].Value);
                LogMessage($"机器码 '{match.Groups[1].Value}' 已复制到剪贴板。");
            }
            else
            {
                LogMessage("未在信息中找到机器码。", true);
            }
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            if (apiHandle == IntPtr.Zero) { LogMessage("请先创建句柄。", true); return; }
            if (string.IsNullOrWhiteSpace(txtActivationKey.Text)) { LogMessage("请输入要激活的许可证密钥。", true); return; }

            string keyToActivate = txtActivationKey.Text.Trim();
            LogMessage("尝试激活许可证密钥...");

            var result = ShiTuCApi.ShiTuApi_ActivateLicenseKey(apiHandle, keyToActivate);

            if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
            {
                LogMessage("许可证激活成功！");
                txtActivationKey.Text = "";
                btnGetLicenseInfo_Click(sender, e);
            }
            else
            {
                if (result != ShiTuApiResultCode.SHITU_API_SUCCESS)
                {
                    string genericError = "许可证激活失败。请检查您的密钥或联系技术支持。";
                    LogMessage(genericError, true);
                }
                btnGetLicenseInfo_Click(sender, e);
            }
        }
        #endregion

        #region Helpers & Closing
        private void ProgressCallbackHandler(int current, int total, IntPtr messagePtr, IntPtr userData)
        {
            string message = Marshal.PtrToStringUTF8(messagePtr) ?? "";
            if (progressBarBuild.InvokeRequired)
            {
                progressBarBuild.BeginInvoke(new Action(() => {
                    if (total > 0)
                    {
                        progressBarBuild.Maximum = total;
                        progressBarBuild.Value = Math.Min(current, total);
                    }
                    LogMessage($"构建进度: {current}/{total} - {message}");
                }));
            }
        }

        private void SetBusyState(bool busy, string message = "")
        {
            this.isBusy = busy;
            if (this.InvokeRequired) { this.Invoke(new Action(() => SetBusyState(busy, message))); return; }

            var controlsToToggle = new Control[] { btnInitialize, btnBuildIndex, btnSaveIndex, btnSearchPath, btnSearchData, btnSearchFolder, btnDeleteId, btnGetLabelById, btnDeleteIds, btnExportIndexToCsv, btnActivate };
            foreach (var ctrl in controlsToToggle) { ctrl.Enabled = !busy; }

            this.Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
            if (busy) { lblStatus.Text = $"状态: {message}"; lblStatus.ForeColor = Color.Orange; }
            else { UpdateStatusLabel(); }
        }

        private void LogMessage(string message, bool isError = false)
        {
            if (txtLog.InvokeRequired) { txtLog.BeginInvoke(new Action(() => LogMessage(message, isError))); }
            else
            {
                string prefix = isError ? "[错误] " : "[信息] ";
                txtLog.AppendText($"{DateTime.Now:HH:mm:ss} {prefix}{message}{Environment.NewLine}");
            }
        }

        private void UpdateStatusLabel()
        {
            if (lblStatus.InvokeRequired) { lblStatus.BeginInvoke(new Action(UpdateStatusLabel)); }
            else
            {
                bool isInitialized = apiHandle != IntPtr.Zero && ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 1;
                lblStatus.Text = isInitialized ? "状态: 已初始化" : "状态: 未初始化";
                lblStatus.ForeColor = isInitialized ? Color.Green : Color.Red;
            }
        }

        private void ClearResultsDisplay()
        {
            if (listViewResults.InvokeRequired) { listViewResults.BeginInvoke(new Action(ClearResultsDisplay)); }
            else
            {
                listViewResults.Items.Clear();
                listViewResults.Groups.Clear();
            }
        }

        private void DisplaySearchResults(IntPtr resultsPtr, int numResults)
        {
            if (listViewResults.InvokeRequired) { listViewResults.BeginInvoke(new Action(() => DisplaySearchResults(resultsPtr, numResults))); return; }
            ClearResultsDisplay();
            if (resultsPtr == IntPtr.Zero || numResults <= 0) return;

            listViewResults.BeginUpdate();
            try
            {
                int structSize = Marshal.SizeOf(typeof(CShiTuSearchResult));
                for (int i = 0; i < numResults; i++)
                {
                    IntPtr currentPtr = IntPtr.Add(resultsPtr, i * structSize);
                    var cResult = (CShiTuSearchResult)Marshal.PtrToStructure(currentPtr, typeof(CShiTuSearchResult));
                    var lvi = new ListViewItem(new[] { cResult.id.ToString(), cResult.GetLabelString() ?? "", cResult.score.ToString("F6"), cResult.distance.ToString("F6"), cResult.box.ToString() });
                    listViewResults.Items.Add(lvi);
                }
            }
            finally
            {
                listViewResults.EndUpdate();
            }
        }
        private void LoadLatestVisualization()
        {
            try
            {
                string configPath = txtConfigPath.Text;
                if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
                {
                    return; 
                }

                // 1. 使用 YamlDotNet 安全地解析配置文件
                string visDir = null;
                try
                {
                    string yamlContent = File.ReadAllText(configPath);

                    // 构建 Deserializer 并让它忽略C#类中没有的属性
                    var deserializer = new DeserializerBuilder()
                        .IgnoreUnmatchedProperties() 
                        .Build();

                    var config = deserializer.Deserialize<YamlConfigForVis>(yamlContent);

                    if (config?.Global?.VisualizationOutputDir != null)
                    {
                        visDir = config.Global.VisualizationOutputDir;
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"解析YAML配置文件失败: {ex.Message}", true);
                    return;
                }

                if (string.IsNullOrEmpty(visDir))
                {
                    // 如果YAML文件里没有这个配置项，就不再尝试加载
                    return;
                }

                // 2. 将相对路径转换为绝对路径
                if (!Path.IsPathRooted(visDir))
                {
                    visDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, visDir);
                }

                if (!Directory.Exists(visDir))
                {
                    // 在第一次运行时，目录可能还不存在，耐心等待一下
                    // 这是因为C++写文件和C#读文件之间可能存在微小的延迟
                    Task.Delay(100).Wait(); // 等待100毫秒
                    if (!Directory.Exists(visDir))
                    {
                        // 如果还不存在，就放弃
                        return;
                    }
                }

                // 3. 找到目录下最新的 .jpg 文件并加载
                var latestFile = new DirectoryInfo(visDir)
                    .GetFiles("vis_*.jpg")
                    .OrderByDescending(f => f.LastWriteTime)
                    .FirstOrDefault();

                if (latestFile != null)
                {
                    lastVisualizationPath = latestFile.FullName;

                    byte[] imageBytes = File.ReadAllBytes(lastVisualizationPath);
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        if (pictureBoxResult.Image != null) pictureBoxResult.Image.Dispose();
                        pictureBoxResult.Image = Image.FromStream(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"加载可视化结果图失败: {ex.Message}", true);
                pictureBoxResult.Image = null;
                lastVisualizationPath = null;
            }
        }

        private void pictureBoxResult_Click(object sender, EventArgs e)
        {
            if (pictureBoxResult.Image == null) return;

            Form imagePopup = new Form
            {
                Text = $"预览: {Path.GetFileName(lastVisualizationPath ?? "结果图")}",
                Size = new System.Drawing.Size(800, 600),
                StartPosition = FormStartPosition.CenterParent,
                WindowState = FormWindowState.Normal
            };

            PictureBox popupPictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = (Bitmap)pictureBoxResult.Image.Clone()
            };

            imagePopup.Controls.Add(popupPictureBox);

            imagePopup.FormClosed += (s, args) =>
            {
                popupPictureBox.Image.Dispose();
                imagePopup.Dispose();
            };

            imagePopup.Show(this);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isBusy)
            {
                e.Cancel = true;
                MessageBox.Show("正在执行后台任务，请稍后关闭。", "操作正在进行", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (apiHandle != IntPtr.Zero)
            {
                LogMessage("窗体关闭，销毁 API 句柄...");
                ShiTuCApi.ShiTuApi_Destroy(ref apiHandle);
            }
        }
        #endregion
    }

    public class TextBoxStreamWriter : TextWriter
    {
        private readonly TextBox _output;
        public TextBoxStreamWriter(TextBox output) { _output = output; }
        public override void Write(char value)
        {
            if (_output.InvokeRequired) { _output.BeginInvoke(new Action(() => _output.AppendText(value.ToString()))); }
            else { _output.AppendText(value.ToString()); }
        }
        public override Encoding Encoding => Encoding.UTF8;
    }
}