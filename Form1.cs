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
            LogMessage("���Դ��� API ���...");
            if (apiHandle != IntPtr.Zero)
            {
                LogMessage("����Ѵ��ڡ��������١�", true);
                return;
            }
            var result = ShiTuCApi.ShiTuApi_Create(out apiHandle);
            if (result == ShiTuApiResultCode.SHITU_API_SUCCESS && apiHandle != IntPtr.Zero)
            {
                LogMessage("API ��������ɹ���");
            }
            else
            {
                LogMessage($"���� API ���ʧ�ܡ�����: {result}, ��Ϣ: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
                apiHandle = IntPtr.Zero;
            }
            UpdateStatusLabel();
        }

        private void btnDestroyHandle_Click(object sender, EventArgs e)
        {
            LogMessage("�������� API ���...");
            if (apiHandle == IntPtr.Zero)
            {
                LogMessage("����Ѿ��� null��");
                return;
            }
            var result = ShiTuCApi.ShiTuApi_Destroy(ref apiHandle);
            if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
            {
                LogMessage("API ������ٳɹ���");
            }
            else
            {
                LogMessage($"���� API ���ʧ�ܡ�����: {result}, ��Ϣ: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
                apiHandle = IntPtr.Zero;
            }
            UpdateStatusLabel();
        }

        private void btnBrowseConfig_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "YAML �����ļ� (*.yaml;*.yml)|*.yaml;*.yml|�����ļ� (*.*)|*.*", Title = "ѡ�������ļ�" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                    txtConfigPath.Text = ofd.FileName;
            }
        }

        private void btnInitialize_Click(object sender, EventArgs e)
        {
            if (apiHandle == IntPtr.Zero) { LogMessage("���ȴ��������", true); return; }
            if (string.IsNullOrWhiteSpace(txtConfigPath.Text) || !File.Exists(txtConfigPath.Text))
            {
                LogMessage("��ѡ��һ����Ч�������ļ�·����", true);
                return;
            }

            LogMessage($"ʹ�������ļ� '{txtConfigPath.Text}' ��ʼ�� API (������ڲ����֤״̬)...");

            try
            {
                var result = ShiTuCApi.ShiTuApi_InitializeWithLicense(apiHandle, txtConfigPath.Text);

                if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
                {
                    LogMessage("API ��ʼ���ɹ���");
                    btnGetLicenseInfo_Click(sender, e);
                }
                else
                {
                    LogMessage($"API ��ʼ��ʧ�ܡ�����: {result}, ��Ϣ: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
                    btnGetLicenseInfo_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"�ڵ��ñ��ش���ʱ�������ش���: {ex.GetType().Name} - {ex.Message}", true);
                LogMessage($"��ͨ����ζ�� C# P/Invoke ������ C++ DLL ����������ƥ�䣬����DLL�������������ȱʧ��", true);
            }

            UpdateStatusLabel();
        }
        #endregion

        #region Index Operations
        private void btnBrowseImageList_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "�ı��ļ� (*.txt)|*.txt|�����ļ� (*.*)|*.*", Title = "ѡ��ͼ���б��ļ�" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                    txtImageListPath.Text = ofd.FileName;
            }
        }

        private async void btnBuildIndex_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0)
            {
                LogMessage("API δ��ʼ��������Ч��", true);
                return;
            }
            if (isBusy) return;

            if (string.IsNullOrWhiteSpace(txtImageListPath.Text) || !File.Exists(txtImageListPath.Text))
            {
                LogMessage("��ѡ��һ����Ч��ͼ���б��ļ�·����", true);
                return;
            }

            SetBusyState(true, "���ڹ�������...");
            LogMessage($"���б��ļ���ʼ�첽��������: {txtImageListPath.Text}...");

            try
            {
                await Task.Run(() =>
                {
                    var result = ShiTuCApi.ShiTuApi_BuildIndexFromFile(apiHandle, txtImageListPath.Text, progressCallbackInstance, IntPtr.Zero);
                    this.Invoke(new Action(() =>
                    {
                        if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
                        {
                            LogMessage("��������������ɡ�");
                            if (progressBarBuild.Maximum > 0) progressBarBuild.Value = progressBarBuild.Maximum;
                            btnGetInfo_Click(null, null);
                        }
                        else
                        {
                            LogMessage($"��������ʧ�ܡ�����: {result}, ��Ϣ: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
                        }
                    }));
                });
                LogMessage("���������ѽ�����");
            }
            catch (Exception ex)
            {
                LogMessage($"��������ʱ�����쳣: {ex.Message}", true);
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
                LogMessage("API δ��ʼ��������Ч��", true);
                return;
            }

            LogMessage("��������...");
            var result = ShiTuCApi.ShiTuApi_SaveIndex(apiHandle);
            if (result == ShiTuApiResultCode.SHITU_API_SUCCESS) LogMessage("��������ɹ���");
            else LogMessage($"��������ʧ�ܡ�����: {result}, ��Ϣ: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
        }

        private void btnExportIndexToCsv_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0)
            {
                LogMessage("API δ��ʼ��������Ч��", true);
                return;
            }

            using (var sfd = new SaveFileDialog { Filter = "CSV �ļ� (*.csv)|*.csv", Title = "API ��������Ϊ CSV", FileName = "api_exported_index.csv" })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;

                LogMessage($"���ڵ��� API ֱ�ӵ���������: {sfd.FileName}...");
                var result = ShiTuCApi.ShiTuApi_ExportLabels(apiHandle, sfd.FileName);

                if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
                {
                    LogMessage("API ���������ɹ���");
                }
                else
                {
                    LogMessage($"API ��������ʧ�ܡ�����: {result}, ��Ϣ: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
                }
            }
        }
        #endregion

        #region Search Operations
        private void btnBrowseQueryImage_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "ͼ���ļ�(*.BMP;*.JPG;*.JPEG;*.PNG)|*.BMP;*.JPG;*.JPEG;*.GIF;*.PNG|�����ļ� (*.*)|*.*", Title = "ѡ���ѯͼ��" })
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
                        LogMessage($"����Ԥ��ͼ��ʧ��: {ex.Message}", true);
                    }
                }
            }
        }

        private async void btnSearchFolder_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0) { LogMessage("API δ��ʼ��������Ч��", true); return; }
            if (isBusy) return;

            using (var fbd = new FolderBrowserDialog { Description = "ѡ�����ͼƬ���ļ��н�����������" })
            {
                if (fbd.ShowDialog() != DialogResult.OK) return;

                string folderPath = fbd.SelectedPath;
                var validExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".bmp" };
                var imageFiles = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => validExtensions.Contains(Path.GetExtension(f)))
                    .ToList();

                if (imageFiles.Count == 0)
                {
                    LogMessage("��ѡ�ļ�����δ�ҵ�֧�ֵ�ͼƬ�ļ���", true);
                    return;
                }

                SetBusyState(true, "������������...");
                LogMessage($"��ʼ���������ļ���: {folderPath} ({imageFiles.Count} ��ͼƬ)");
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
                                progress.Report(($"({imageCounter}/{imageFiles.Count}) ��������: {fileName}", null));

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

                    LogMessage($"����������ɡ�����ѱ��浽: {csvOutputPath}");
                }
                catch (Exception ex)
                {
                    LogMessage($"�������������з������ش���: {ex.Message}", true);
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
                LogMessage($"�����ɹ����ҵ� {numResults} �������");
                DisplaySearchResults(resultsPtr, numResults);
            }
            else
            {
                LogMessage($"����ʧ�ܡ�����: {result}, ��Ϣ: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
                ClearResultsDisplay();
            }
        }

        private void btnSearchPath_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0)
            {
                LogMessage("API δ��ʼ��������Ч��", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtQueryImagePath.Text) || !File.Exists(txtQueryImagePath.Text))
            {
                LogMessage("��ѡ��һ����Ч�Ĳ�ѯͼ��·����", true);
                return;
            }

            LogMessage($"��·������: {txtQueryImagePath.Text}...");
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
                LogMessage("API δ��ʼ��������Ч��", true);
                return;
            }
            if (pictureBoxQuery.Image == null)
            {
                LogMessage("���ȼ���һ��ͼƬ�Ի�ȡ���ݡ�", true);
                return;
            }

            LogMessage("��Ԥ��ͼ���м������ݲ�����...");
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
                LogMessage("API δ��ʼ��������Ч��", true);
                return;
            }
            var dimResult = ShiTuCApi.ShiTuApi_GetIndexDimension(apiHandle, out int dim);
            var countResult = ShiTuCApi.ShiTuApi_GetIndexNumItems(apiHandle, out ulong count);
            lblDimension.Text = $"ά��: {(dimResult == ShiTuApiResultCode.SHITU_API_SUCCESS ? dim.ToString() : "����")}";
            lblItemCount.Text = $"����: {(countResult == ShiTuApiResultCode.SHITU_API_SUCCESS ? count.ToString() : "����")}";
        }

        private void btnDeleteId_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0)
            {
                LogMessage("API δ��ʼ��������Ч��", true);
                return;
            }
            if (!long.TryParse(txtDeleteId.Text, out long idToDelete))
            {
                LogMessage("ID��ʽ��Ч��", true);
                return;
            }
            LogMessage($"����ɾ�� ID: {idToDelete}...");
            var result = ShiTuCApi.ShiTuApi_DeleteItem(apiHandle, idToDelete);
            if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
            {
                LogMessage($"DeleteItem ������ɡ�");
                btnGetInfo_Click(sender, e);
            }
            else
            {
                LogMessage($"DeleteItem ʧ�ܡ�����: {result}, ��Ϣ: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
            }
        }

        private void btnDeleteIds_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0)
            {
                LogMessage("API δ��ʼ��������Ч��", true);
                return;
            }

            var idsToDelete = new List<long>();
            string input = txtDeleteIds.Text.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                using (var ofd = new OpenFileDialog { Filter = "ID �б��ļ� (*.txt)|*.txt|�����ļ� (*.*)|*.*", Title = "ѡ�����ID���ı��ļ�" })
                {
                    if (ofd.ShowDialog() != DialogResult.OK)
                    {
                        LogMessage("����ȡ����", false);
                        return;
                    }
                    try
                    {
                        input = File.ReadAllText(ofd.FileName);
                        LogMessage($"���ļ� '{Path.GetFileName(ofd.FileName)}' ����ID�б�");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"��ȡID�ļ�ʧ��: {ex.Message}", true);
                        return;
                    }
                }
            }

            var idStrings = input.Split(new[] { ',', '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var idStr in idStrings)
            {
                if (long.TryParse(idStr.Trim(), out long id)) idsToDelete.Add(id);
                else LogMessage($"����: ��Ч��ID��ʽ '{idStr}'", true);
            }

            if (idsToDelete.Count == 0) { LogMessage("δ�������κ���ЧID��", true); return; }

            LogMessage($"����ɾ�� {idsToDelete.Count} ��ID...");
            var result = ShiTuCApi.ShiTuApi_DeleteItems(apiHandle, idsToDelete.ToArray(), (ulong)idsToDelete.Count, out ulong numDeleted);

            if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
            {
                LogMessage($"DeleteItems ������ɡ�API����ɾ���� {numDeleted} ����Ŀ��");
                btnGetInfo_Click(sender, e);
            }
            else
            {
                LogMessage($"DeleteItems ʧ�ܡ�����: {result}, ��Ϣ: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
            }
        }

        private void btnGetLabelById_Click(object sender, EventArgs e)
        {
            if (ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 0)
            {
                LogMessage("API δ��ʼ��������Ч��", true);
                return;
            }
            if (!long.TryParse(txtDeleteId.Text, out long idToQuery))
            {
                LogMessage("������һ����Ч��ID�Խ��в�ѯ��", true);
                return;
            }

            LogMessage($"���ڲ�ѯ ID: {idToQuery} �ı�ǩ...");
            IntPtr labelPtr = IntPtr.Zero;
            try
            {
                var result = ShiTuCApi.ShiTuApi_GetLabelById(apiHandle, idToQuery, out labelPtr);
                if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
                {
                    if (labelPtr != IntPtr.Zero)
                    {
                        string label = Marshal.PtrToStringUTF8(labelPtr);
                        LogMessage($"��ѯ�ɹ���ID: {idToQuery}, ��ǩ: '{label}'");
                    }
                    else
                    {
                        LogMessage($"��ѯ��ɡ�ID: {idToQuery} �������в����ڡ�");
                    }
                }
                else
                {
                    LogMessage($"��ѯ��ǩʧ�ܡ�����: {result}, ��Ϣ: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
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
            if (apiHandle == IntPtr.Zero) { LogMessage("���ȴ��������", true); return; }

            LogMessage("���ڻ�ȡ���֤��Ϣ...");
            CShiTuLicenseInfo licenseInfo = default;
            try
            {
                var result = ShiTuCApi.ShiTuApi_GetLicenseInfo(apiHandle, out licenseInfo);
                if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"״̬: {licenseInfo.GetStatusString()}");
                    if (!string.IsNullOrEmpty(licenseInfo.GetMachineCode())) sb.AppendLine($"������: {licenseInfo.GetMachineCode()}");
                    if (licenseInfo.is_permanent == 1) sb.AppendLine("����: �������֤");
                    else sb.AppendLine($"����: {licenseInfo.GetExpiryDate()} | ʣ��: {licenseInfo.GetRemainingDays()} ��");
                    if (!string.IsNullOrEmpty(licenseInfo.GetErrorMessage())) sb.AppendLine($"��Ϣ: {licenseInfo.GetErrorMessage()}");

                    this.Invoke(new Action(() => txtLicenseInfo.Text = sb.ToString()));
                    LogMessage("���֤��Ϣ��ȡ�ɹ���");
                }
                else
                {
                    LogMessage($"��ȡ���֤��Ϣʧ�ܡ�����: {result}, ��Ϣ: {ShiTuCApi.ShiTuApi_GetLastErrorMsg()}", true);
                }
            }
            finally
            {
                ShiTuCApi.ShiTuApi_FreeLicenseInfoContents(ref licenseInfo);
            }
        }

        private void btnCopyMachineCode_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLicenseInfo.Text)) { LogMessage("���Ȼ�ȡ���֤��Ϣ��", true); return; }
            var match = Regex.Match(txtLicenseInfo.Text, @"������:\s*(\S+)");
            if (match.Success)
            {
                Clipboard.SetText(match.Groups[1].Value);
                LogMessage($"������ '{match.Groups[1].Value}' �Ѹ��Ƶ������塣");
            }
            else
            {
                LogMessage("δ����Ϣ���ҵ������롣", true);
            }
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            if (apiHandle == IntPtr.Zero) { LogMessage("���ȴ��������", true); return; }
            if (string.IsNullOrWhiteSpace(txtActivationKey.Text)) { LogMessage("������Ҫ��������֤��Կ��", true); return; }

            string keyToActivate = txtActivationKey.Text.Trim();
            LogMessage("���Լ������֤��Կ...");

            var result = ShiTuCApi.ShiTuApi_ActivateLicenseKey(apiHandle, keyToActivate);

            if (result == ShiTuApiResultCode.SHITU_API_SUCCESS)
            {
                LogMessage("���֤����ɹ���");
                txtActivationKey.Text = "";
                btnGetLicenseInfo_Click(sender, e);
            }
            else
            {
                if (result != ShiTuApiResultCode.SHITU_API_SUCCESS)
                {
                    string genericError = "���֤����ʧ�ܡ�����������Կ����ϵ����֧�֡�";
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
                    LogMessage($"��������: {current}/{total} - {message}");
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
            if (busy) { lblStatus.Text = $"״̬: {message}"; lblStatus.ForeColor = Color.Orange; }
            else { UpdateStatusLabel(); }
        }

        private void LogMessage(string message, bool isError = false)
        {
            if (txtLog.InvokeRequired) { txtLog.BeginInvoke(new Action(() => LogMessage(message, isError))); }
            else
            {
                string prefix = isError ? "[����] " : "[��Ϣ] ";
                txtLog.AppendText($"{DateTime.Now:HH:mm:ss} {prefix}{message}{Environment.NewLine}");
            }
        }

        private void UpdateStatusLabel()
        {
            if (lblStatus.InvokeRequired) { lblStatus.BeginInvoke(new Action(UpdateStatusLabel)); }
            else
            {
                bool isInitialized = apiHandle != IntPtr.Zero && ShiTuCApi.ShiTuApi_IsInitialized(apiHandle) == 1;
                lblStatus.Text = isInitialized ? "״̬: �ѳ�ʼ��" : "״̬: δ��ʼ��";
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

                // 1. ʹ�� YamlDotNet ��ȫ�ؽ��������ļ�
                string visDir = null;
                try
                {
                    string yamlContent = File.ReadAllText(configPath);

                    // ���� Deserializer ����������C#����û�е�����
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
                    LogMessage($"����YAML�����ļ�ʧ��: {ex.Message}", true);
                    return;
                }

                if (string.IsNullOrEmpty(visDir))
                {
                    // ���YAML�ļ���û�����������Ͳ��ٳ��Լ���
                    return;
                }

                // 2. �����·��ת��Ϊ����·��
                if (!Path.IsPathRooted(visDir))
                {
                    visDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, visDir);
                }

                if (!Directory.Exists(visDir))
                {
                    // �ڵ�һ������ʱ��Ŀ¼���ܻ������ڣ����ĵȴ�һ��
                    // ������ΪC++д�ļ���C#���ļ�֮����ܴ���΢С���ӳ�
                    Task.Delay(100).Wait(); // �ȴ�100����
                    if (!Directory.Exists(visDir))
                    {
                        // ����������ڣ��ͷ���
                        return;
                    }
                }

                // 3. �ҵ�Ŀ¼�����µ� .jpg �ļ�������
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
                LogMessage($"���ؿ��ӻ����ͼʧ��: {ex.Message}", true);
                pictureBoxResult.Image = null;
                lastVisualizationPath = null;
            }
        }

        private void pictureBoxResult_Click(object sender, EventArgs e)
        {
            if (pictureBoxResult.Image == null) return;

            Form imagePopup = new Form
            {
                Text = $"Ԥ��: {Path.GetFileName(lastVisualizationPath ?? "���ͼ")}",
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
                MessageBox.Show("����ִ�к�̨�������Ժ�رա�", "�������ڽ���", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (apiHandle != IntPtr.Zero)
            {
                LogMessage("����رգ����� API ���...");
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