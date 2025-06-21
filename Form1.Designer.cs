namespace ShiTuApiTestApp
{
    partial class Form1
    {

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code


        private void InitializeComponent()
        {
            btnCreateHandle = new Button();
            btnDestroyHandle = new Button();
            lblStatus = new Label();
            groupBox1 = new GroupBox();
            btnInitialize = new Button();
            btnBrowseConfig = new Button();
            txtConfigPath = new TextBox();
            label1 = new Label();
            groupBox2 = new GroupBox();
            progressBarBuild = new ProgressBar();
            btnSaveIndex = new Button();
            btnBuildIndex = new Button();
            btnBrowseImageList = new Button();
            txtImageListPath = new TextBox();
            label2 = new Label();
            groupBox3 = new GroupBox();
            pictureBoxResult = new PictureBox();
            btnSearchFolder = new Button();
            btnSearchData = new Button();
            pictureBoxQuery = new PictureBox();
            listViewResults = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            btnSearchPath = new Button();
            btnBrowseQueryImage = new Button();
            txtQueryImagePath = new TextBox();
            label3 = new Label();
            groupBox4 = new GroupBox();
            lblItemCount = new Label();
            lblDimension = new Label();
            btnGetInfo = new Button();
            groupBox5 = new GroupBox();
            btnGetLabelById = new Button();
            btnDeleteIds = new Button();
            txtDeleteIds = new TextBox();
            btnDeleteId = new Button();
            txtDeleteId = new TextBox();
            label5 = new Label();
            groupBoxExportIndex = new GroupBox();
            btnExportIndexToCsv = new Button();
            txtLog = new TextBox();
            label8 = new Label();
            groupBoxLicense = new GroupBox();
            btnActivate = new Button();
            txtActivationKey = new TextBox();
            btnCopyMachineCode = new Button();
            txtLicenseInfo = new TextBox();
            btnGetLicenseInfo = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxResult).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxQuery).BeginInit();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBoxExportIndex.SuspendLayout();
            groupBoxLicense.SuspendLayout();
            SuspendLayout();
            // 
            // btnCreateHandle
            // 
            btnCreateHandle.Location = new Point(15, 13);
            btnCreateHandle.Name = "btnCreateHandle";
            btnCreateHandle.Size = new Size(133, 29);
            btnCreateHandle.TabIndex = 0;
            btnCreateHandle.Text = "创建句柄";
            btnCreateHandle.UseVisualStyleBackColor = true;
            btnCreateHandle.Click += btnCreateHandle_Click;
            // 
            // btnDestroyHandle
            // 
            btnDestroyHandle.Location = new Point(154, 13);
            btnDestroyHandle.Name = "btnDestroyHandle";
            btnDestroyHandle.Size = new Size(133, 29);
            btnDestroyHandle.TabIndex = 1;
            btnDestroyHandle.Text = "销毁句柄";
            btnDestroyHandle.UseVisualStyleBackColor = true;
            btnDestroyHandle.Click += btnDestroyHandle_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatus.Location = new Point(308, 17);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(119, 20);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "状态: 未初始化";
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(btnInitialize);
            groupBox1.Controls.Add(btnBrowseConfig);
            groupBox1.Controls.Add(txtConfigPath);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(15, 49);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(647, 76);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "初始化";
            // 
            // btnInitialize
            // 
            btnInitialize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnInitialize.Location = new Point(522, 25);
            btnInitialize.Name = "btnInitialize";
            btnInitialize.Size = new Size(119, 29);
            btnInitialize.TabIndex = 3;
            btnInitialize.Text = "初始化 API";
            btnInitialize.UseVisualStyleBackColor = true;
            btnInitialize.Click += btnInitialize_Click;
            // 
            // btnBrowseConfig
            // 
            btnBrowseConfig.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseConfig.Location = new Point(472, 25);
            btnBrowseConfig.Name = "btnBrowseConfig";
            btnBrowseConfig.Size = new Size(42, 29);
            btnBrowseConfig.TabIndex = 2;
            btnBrowseConfig.Text = "浏览...";
            btnBrowseConfig.UseVisualStyleBackColor = true;
            btnBrowseConfig.Click += btnBrowseConfig_Click;
            // 
            // txtConfigPath
            // 
            txtConfigPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtConfigPath.Location = new Point(152, 26);
            txtConfigPath.Name = "txtConfigPath";
            txtConfigPath.Size = new Size(313, 27);
            txtConfigPath.TabIndex = 1;
            txtConfigPath.Text = "config.yaml";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 29);
            label1.Name = "label1";
            label1.Size = new Size(103, 20);
            label1.TabIndex = 0;
            label1.Text = "配置文件路径:";
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(progressBarBuild);
            groupBox2.Controls.Add(btnSaveIndex);
            groupBox2.Controls.Add(btnBuildIndex);
            groupBox2.Controls.Add(btnBrowseImageList);
            groupBox2.Controls.Add(txtImageListPath);
            groupBox2.Controls.Add(label2);
            groupBox2.Location = new Point(15, 134);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(647, 106);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "索引构建";
            // 
            // progressBarBuild
            // 
            progressBarBuild.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBarBuild.Location = new Point(135, 65);
            progressBarBuild.Name = "progressBarBuild";
            progressBarBuild.Size = new Size(323, 29);
            progressBarBuild.TabIndex = 5;
            // 
            // btnSaveIndex
            // 
            btnSaveIndex.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSaveIndex.Location = new Point(521, 65);
            btnSaveIndex.Name = "btnSaveIndex";
            btnSaveIndex.Size = new Size(119, 29);
            btnSaveIndex.TabIndex = 4;
            btnSaveIndex.Text = "保存索引";
            btnSaveIndex.UseVisualStyleBackColor = true;
            btnSaveIndex.Click += btnSaveIndex_Click;
            // 
            // btnBuildIndex
            // 
            btnBuildIndex.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBuildIndex.Location = new Point(521, 27);
            btnBuildIndex.Name = "btnBuildIndex";
            btnBuildIndex.Size = new Size(119, 29);
            btnBuildIndex.TabIndex = 3;
            btnBuildIndex.Text = "构建索引";
            btnBuildIndex.UseVisualStyleBackColor = true;
            btnBuildIndex.Click += btnBuildIndex_Click;
            // 
            // btnBrowseImageList
            // 
            btnBrowseImageList.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseImageList.Location = new Point(472, 27);
            btnBrowseImageList.Name = "btnBrowseImageList";
            btnBrowseImageList.Size = new Size(42, 29);
            btnBrowseImageList.TabIndex = 2;
            btnBrowseImageList.Text = "浏览...";
            btnBrowseImageList.UseVisualStyleBackColor = true;
            btnBrowseImageList.Click += btnBrowseImageList_Click;
            // 
            // txtImageListPath
            // 
            txtImageListPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtImageListPath.Location = new Point(135, 28);
            txtImageListPath.Name = "txtImageListPath";
            txtImageListPath.Size = new Size(330, 27);
            txtImageListPath.TabIndex = 1;
            txtImageListPath.Text = "image_list.txt";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 31);
            label2.Name = "label2";
            label2.Size = new Size(103, 20);
            label2.TabIndex = 0;
            label2.Text = "图像列表路径:";
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox3.Controls.Add(pictureBoxResult);
            groupBox3.Controls.Add(btnSearchFolder);
            groupBox3.Controls.Add(btnSearchData);
            groupBox3.Controls.Add(pictureBoxQuery);
            groupBox3.Controls.Add(listViewResults);
            groupBox3.Controls.Add(btnSearchPath);
            groupBox3.Controls.Add(btnBrowseQueryImage);
            groupBox3.Controls.Add(txtQueryImagePath);
            groupBox3.Controls.Add(label3);
            groupBox3.Location = new Point(15, 257);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(647, 406);
            groupBox3.TabIndex = 5;
            groupBox3.TabStop = false;
            groupBox3.Text = "图像检索";
            // 
            // pictureBoxResult
            // 
            pictureBoxResult.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxResult.Cursor = Cursors.Hand;
            pictureBoxResult.Location = new Point(238, 63);
            pictureBoxResult.Name = "pictureBoxResult";
            pictureBoxResult.Size = new Size(146, 130);
            pictureBoxResult.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxResult.TabIndex = 8;
            pictureBoxResult.TabStop = false;
            pictureBoxResult.Click += pictureBoxResult_Click;
            // 
            // btnSearchFolder
            // 
            btnSearchFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSearchFolder.Location = new Point(511, 101);
            btnSearchFolder.Name = "btnSearchFolder";
            btnSearchFolder.Size = new Size(130, 29);
            btnSearchFolder.TabIndex = 7;
            btnSearchFolder.Text = "按文件夹搜索";
            btnSearchFolder.UseVisualStyleBackColor = true;
            btnSearchFolder.Click += btnSearchFolder_Click;
            // 
            // btnSearchData
            // 
            btnSearchData.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSearchData.Location = new Point(521, 63);
            btnSearchData.Name = "btnSearchData";
            btnSearchData.Size = new Size(119, 29);
            btnSearchData.TabIndex = 6;
            btnSearchData.Text = "按数据搜索";
            btnSearchData.UseVisualStyleBackColor = true;
            btnSearchData.Click += btnSearchData_Click;
            // 
            // pictureBoxQuery
            // 
            pictureBoxQuery.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxQuery.Location = new Point(11, 63);
            pictureBoxQuery.Name = "pictureBoxQuery";
            pictureBoxQuery.Size = new Size(146, 130);
            pictureBoxQuery.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxQuery.TabIndex = 5;
            pictureBoxQuery.TabStop = false;
            // 
            // listViewResults
            // 
            listViewResults.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listViewResults.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4, columnHeader5 });
            listViewResults.FullRowSelect = true;
            listViewResults.GridLines = true;
            listViewResults.Location = new Point(11, 203);
            listViewResults.Name = "listViewResults";
            listViewResults.Size = new Size(628, 197);
            listViewResults.TabIndex = 4;
            listViewResults.UseCompatibleStateImageBehavior = false;
            listViewResults.View = View.Details;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "ID";
            columnHeader1.Width = 80;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "标签";
            columnHeader2.Width = 150;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "分数";
            columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "距离";
            columnHeader4.Width = 100;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "边界框";
            columnHeader5.Width = 100;
            // 
            // btnSearchPath
            // 
            btnSearchPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSearchPath.Location = new Point(521, 27);
            btnSearchPath.Name = "btnSearchPath";
            btnSearchPath.Size = new Size(119, 29);
            btnSearchPath.TabIndex = 3;
            btnSearchPath.Text = "按路径搜索";
            btnSearchPath.UseVisualStyleBackColor = true;
            btnSearchPath.Click += btnSearchPath_Click;
            // 
            // btnBrowseQueryImage
            // 
            btnBrowseQueryImage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseQueryImage.Location = new Point(472, 27);
            btnBrowseQueryImage.Name = "btnBrowseQueryImage";
            btnBrowseQueryImage.Size = new Size(42, 29);
            btnBrowseQueryImage.TabIndex = 2;
            btnBrowseQueryImage.Text = "浏览...";
            btnBrowseQueryImage.UseVisualStyleBackColor = true;
            btnBrowseQueryImage.Click += btnBrowseQueryImage_Click;
            // 
            // txtQueryImagePath
            // 
            txtQueryImagePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtQueryImagePath.Location = new Point(105, 28);
            txtQueryImagePath.Name = "txtQueryImagePath";
            txtQueryImagePath.Size = new Size(361, 27);
            txtQueryImagePath.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 31);
            label3.Name = "label3";
            label3.Size = new Size(73, 20);
            label3.TabIndex = 0;
            label3.Text = "查询图像:";
            // 
            // groupBox4
            // 
            groupBox4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            groupBox4.Controls.Add(lblItemCount);
            groupBox4.Controls.Add(lblDimension);
            groupBox4.Controls.Add(btnGetInfo);
            groupBox4.Location = new Point(680, 13);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(285, 103);
            groupBox4.TabIndex = 6;
            groupBox4.TabStop = false;
            groupBox4.Text = "索引信息";
            // 
            // lblItemCount
            // 
            lblItemCount.AutoSize = true;
            lblItemCount.Location = new Point(132, 65);
            lblItemCount.Name = "lblItemCount";
            lblItemCount.Size = new Size(53, 20);
            lblItemCount.TabIndex = 2;
            lblItemCount.Text = "数量: -";
            // 
            // lblDimension
            // 
            lblDimension.AutoSize = true;
            lblDimension.Location = new Point(132, 31);
            lblDimension.Name = "lblDimension";
            lblDimension.Size = new Size(53, 20);
            lblDimension.TabIndex = 1;
            lblDimension.Text = "维度: -";
            // 
            // btnGetInfo
            // 
            btnGetInfo.Location = new Point(8, 27);
            btnGetInfo.Name = "btnGetInfo";
            btnGetInfo.Size = new Size(106, 58);
            btnGetInfo.TabIndex = 0;
            btnGetInfo.Text = "获取信息";
            btnGetInfo.UseVisualStyleBackColor = true;
            btnGetInfo.Click += btnGetInfo_Click;
            // 
            // groupBox5
            // 
            groupBox5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            groupBox5.Controls.Add(btnGetLabelById);
            groupBox5.Controls.Add(btnDeleteIds);
            groupBox5.Controls.Add(txtDeleteIds);
            groupBox5.Controls.Add(btnDeleteId);
            groupBox5.Controls.Add(txtDeleteId);
            groupBox5.Controls.Add(label5);
            groupBox5.Location = new Point(680, 122);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(285, 139);
            groupBox5.TabIndex = 7;
            groupBox5.TabStop = false;
            groupBox5.Text = "删除操作";
            // 
            // btnGetLabelById
            // 
            btnGetLabelById.Location = new Point(161, 62);
            btnGetLabelById.Name = "btnGetLabelById";
            btnGetLabelById.Size = new Size(112, 29);
            btnGetLabelById.TabIndex = 4;
            btnGetLabelById.Text = "查询此 ID";
            btnGetLabelById.UseVisualStyleBackColor = true;
            btnGetLabelById.Click += btnGetLabelById_Click;
            // 
            // btnDeleteIds
            // 
            btnDeleteIds.Location = new Point(161, 97);
            btnDeleteIds.Name = "btnDeleteIds";
            btnDeleteIds.Size = new Size(112, 29);
            btnDeleteIds.TabIndex = 5;
            btnDeleteIds.Text = "删除ID(文件)";
            btnDeleteIds.UseVisualStyleBackColor = true;
            btnDeleteIds.Click += btnDeleteIds_Click;
            // 
            // txtDeleteIds
            // 
            txtDeleteIds.Location = new Point(8, 62);
            txtDeleteIds.Multiline = true;
            txtDeleteIds.Name = "txtDeleteIds";
            txtDeleteIds.PlaceholderText = "ID,以逗号/换行分隔, 或点击按钮选择文件";
            txtDeleteIds.ScrollBars = ScrollBars.Vertical;
            txtDeleteIds.Size = new Size(140, 68);
            txtDeleteIds.TabIndex = 3;
            // 
            // btnDeleteId
            // 
            btnDeleteId.Location = new Point(161, 27);
            btnDeleteId.Name = "btnDeleteId";
            btnDeleteId.Size = new Size(112, 29);
            btnDeleteId.TabIndex = 2;
            btnDeleteId.Text = "删除此 ID";
            btnDeleteId.UseVisualStyleBackColor = true;
            btnDeleteId.Click += btnDeleteId_Click;
            // 
            // txtDeleteId
            // 
            txtDeleteId.Location = new Point(81, 28);
            txtDeleteId.Name = "txtDeleteId";
            txtDeleteId.Size = new Size(67, 27);
            txtDeleteId.TabIndex = 1;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(8, 31);
            label5.Name = "label5";
            label5.Size = new Size(62, 20);
            label5.TabIndex = 0;
            label5.Text = "单个 ID:";
            // 
            // groupBoxExportIndex
            // 
            groupBoxExportIndex.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            groupBoxExportIndex.Controls.Add(btnExportIndexToCsv);
            groupBoxExportIndex.Location = new Point(680, 267);
            groupBoxExportIndex.Name = "groupBoxExportIndex";
            groupBoxExportIndex.Size = new Size(285, 67);
            groupBoxExportIndex.TabIndex = 13;
            groupBoxExportIndex.TabStop = false;
            groupBoxExportIndex.Text = "导出索引";
            // 
            // btnExportIndexToCsv
            // 
            btnExportIndexToCsv.Location = new Point(8, 27);
            btnExportIndexToCsv.Name = "btnExportIndexToCsv";
            btnExportIndexToCsv.Size = new Size(270, 29);
            btnExportIndexToCsv.TabIndex = 0;
            btnExportIndexToCsv.Text = "导出为 CSV...";
            btnExportIndexToCsv.UseVisualStyleBackColor = true;
            btnExportIndexToCsv.Click += btnExportIndexToCsv_Click;
            // 
            // txtLog
            // 
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            txtLog.Location = new Point(680, 558);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(284, 105);
            txtLog.TabIndex = 10;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Location = new Point(680, 535);
            label8.Name = "label8";
            label8.Size = new Size(43, 20);
            label8.TabIndex = 11;
            label8.Text = "日志:";
            // 
            // groupBoxLicense
            // 
            groupBoxLicense.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            groupBoxLicense.Controls.Add(btnActivate);
            groupBoxLicense.Controls.Add(txtActivationKey);
            groupBoxLicense.Controls.Add(btnCopyMachineCode);
            groupBoxLicense.Controls.Add(txtLicenseInfo);
            groupBoxLicense.Controls.Add(btnGetLicenseInfo);
            groupBoxLicense.Location = new Point(680, 340);
            groupBoxLicense.Name = "groupBoxLicense";
            groupBoxLicense.Size = new Size(285, 192);
            groupBoxLicense.TabIndex = 12;
            groupBoxLicense.TabStop = false;
            groupBoxLicense.Text = "许可证信息与激活";
            // 
            // btnActivate
            // 
            btnActivate.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnActivate.Location = new Point(192, 123);
            btnActivate.Name = "btnActivate";
            btnActivate.Size = new Size(86, 29);
            btnActivate.TabIndex = 4;
            btnActivate.Text = "激活";
            btnActivate.UseVisualStyleBackColor = true;
            btnActivate.Click += btnActivate_Click;
            // 
            // txtActivationKey
            // 
            txtActivationKey.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtActivationKey.Location = new Point(8, 124);
            txtActivationKey.Name = "txtActivationKey";
            txtActivationKey.PlaceholderText = "在此输入许可证密钥";
            txtActivationKey.Size = new Size(177, 27);
            txtActivationKey.TabIndex = 3;
            // 
            // btnCopyMachineCode
            // 
            btnCopyMachineCode.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCopyMachineCode.Location = new Point(154, 157);
            btnCopyMachineCode.Name = "btnCopyMachineCode";
            btnCopyMachineCode.Size = new Size(124, 29);
            btnCopyMachineCode.TabIndex = 2;
            btnCopyMachineCode.Text = "复制机器码";
            btnCopyMachineCode.UseVisualStyleBackColor = true;
            btnCopyMachineCode.Click += btnCopyMachineCode_Click;
            // 
            // txtLicenseInfo
            // 
            txtLicenseInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLicenseInfo.Location = new Point(8, 27);
            txtLicenseInfo.Multiline = true;
            txtLicenseInfo.Name = "txtLicenseInfo";
            txtLicenseInfo.ReadOnly = true;
            txtLicenseInfo.ScrollBars = ScrollBars.Vertical;
            txtLicenseInfo.Size = new Size(270, 91);
            txtLicenseInfo.TabIndex = 1;
            // 
            // btnGetLicenseInfo
            // 
            btnGetLicenseInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnGetLicenseInfo.Location = new Point(8, 157);
            btnGetLicenseInfo.Name = "btnGetLicenseInfo";
            btnGetLicenseInfo.Size = new Size(140, 29);
            btnGetLicenseInfo.TabIndex = 0;
            btnGetLicenseInfo.Text = "获取/刷新信息";
            btnGetLicenseInfo.UseVisualStyleBackColor = true;
            btnGetLicenseInfo.Click += btnGetLicenseInfo_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(978, 675);
            Controls.Add(groupBoxLicense);
            Controls.Add(label8);
            Controls.Add(txtLog);
            Controls.Add(groupBoxExportIndex);
            Controls.Add(groupBox5);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(lblStatus);
            Controls.Add(btnDestroyHandle);
            Controls.Add(btnCreateHandle);
            MinimumSize = new Size(996, 722);
            Name = "Form1";
            Text = "识途 C API 测试器 (带许可证)";
            FormClosing += Form1_FormClosing;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxResult).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxQuery).EndInit();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBoxExportIndex.ResumeLayout(false);
            groupBoxLicense.ResumeLayout(false);
            groupBoxLicense.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnCreateHandle;
        private System.Windows.Forms.Button btnDestroyHandle;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnInitialize;
        private System.Windows.Forms.Button btnBrowseConfig;
        private System.Windows.Forms.TextBox txtConfigPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSaveIndex;
        private System.Windows.Forms.Button btnBuildIndex;
        private System.Windows.Forms.Button btnBrowseImageList;
        private System.Windows.Forms.TextBox txtImageListPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnSearchPath;
        private System.Windows.Forms.Button btnBrowseQueryImage;
        private System.Windows.Forms.TextBox txtQueryImagePath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar progressBarBuild;
        private System.Windows.Forms.ListView listViewResults;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.PictureBox pictureBoxQuery;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnGetInfo;
        private System.Windows.Forms.Label lblItemCount;
        private System.Windows.Forms.Label lblDimension;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnDeleteIds;
        private System.Windows.Forms.TextBox txtDeleteIds;
        private System.Windows.Forms.Button btnDeleteId;
        private System.Windows.Forms.TextBox txtDeleteId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnSearchData;
        private System.Windows.Forms.GroupBox groupBoxLicense;
        private System.Windows.Forms.Button btnCopyMachineCode;
        private System.Windows.Forms.TextBox txtLicenseInfo;
        private System.Windows.Forms.Button btnGetLicenseInfo;
        private System.Windows.Forms.TextBox txtActivationKey;
        private System.Windows.Forms.Button btnActivate;
        private System.Windows.Forms.Button btnSearchFolder;
        private System.Windows.Forms.GroupBox groupBoxExportIndex;
        private System.Windows.Forms.Button btnExportIndexToCsv;
        private System.Windows.Forms.Button btnGetLabelById;
        private System.Windows.Forms.PictureBox pictureBoxResult;
    }
}