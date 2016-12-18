namespace ms_denserDepth
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabPage_info = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button_exportDLO = new System.Windows.Forms.Button();
            this.button_importDLO = new System.Windows.Forms.Button();
            this.pictureBox_app = new System.Windows.Forms.PictureBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.App = new System.Windows.Forms.Button();
            this.button_labeled = new System.Windows.Forms.Button();
            this.button_showFeature = new System.Windows.Forms.Button();
            this.button_Stereo = new System.Windows.Forms.Button();
            this.textBox_shell = new System.Windows.Forms.TextBox();
            this.button_importPC = new System.Windows.Forms.Button();
            this.button_Object = new System.Windows.Forms.Button();
            this.button_showDenser = new System.Windows.Forms.Button();
            this.button_showDepth = new System.Windows.Forms.Button();
            this.button_showMerged = new System.Windows.Forms.Button();
            this.button_showEdge = new System.Windows.Forms.Button();
            this.button_showOrigin = new System.Windows.Forms.Button();
            this.imageBox_main = new Emgu.CV.UI.ImageBox();
            this.button_getRT = new System.Windows.Forms.Button();
            this.label_numImgUnit = new System.Windows.Forms.Label();
            this.label_numImg = new System.Windows.Forms.Label();
            this.button_getResult = new System.Windows.Forms.Button();
            this.button_labeling = new System.Windows.Forms.Button();
            this.button_denser = new System.Windows.Forms.Button();
            this.button_canny = new System.Windows.Forms.Button();
            this.button_deleteImg = new System.Windows.Forms.Button();
            this.button_runSfM = new System.Windows.Forms.Button();
            this.button_importImg = new System.Windows.Forms.Button();
            this.listView_imgs = new System.Windows.Forms.ListView();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_app)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox_main)).BeginInit();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage_info
            // 
            this.tabPage_info.BackColor = System.Drawing.Color.DimGray;
            this.tabPage_info.Location = new System.Drawing.Point(4, 22);
            this.tabPage_info.Name = "tabPage_info";
            this.tabPage_info.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_info.Size = new System.Drawing.Size(1128, 777);
            this.tabPage_info.TabIndex = 2;
            this.tabPage_info.Text = "Info";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.DimGray;
            this.tabPage2.Controls.Add(this.button_exportDLO);
            this.tabPage2.Controls.Add(this.button_importDLO);
            this.tabPage2.Controls.Add(this.pictureBox_app);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1128, 777);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Application";
            // 
            // button_exportDLO
            // 
            this.button_exportDLO.Location = new System.Drawing.Point(594, 57);
            this.button_exportDLO.Name = "button_exportDLO";
            this.button_exportDLO.Size = new System.Drawing.Size(120, 44);
            this.button_exportDLO.TabIndex = 4;
            this.button_exportDLO.Text = "Export Object";
            this.button_exportDLO.UseVisualStyleBackColor = true;
            // 
            // button_importDLO
            // 
            this.button_importDLO.Location = new System.Drawing.Point(594, 7);
            this.button_importDLO.Name = "button_importDLO";
            this.button_importDLO.Size = new System.Drawing.Size(120, 44);
            this.button_importDLO.TabIndex = 2;
            this.button_importDLO.Text = "Import Object";
            this.button_importDLO.UseVisualStyleBackColor = true;
            // 
            // pictureBox_app
            // 
            this.pictureBox_app.Location = new System.Drawing.Point(8, 7);
            this.pictureBox_app.Name = "pictureBox_app";
            this.pictureBox_app.Size = new System.Drawing.Size(580, 569);
            this.pictureBox_app.TabIndex = 0;
            this.pictureBox_app.TabStop = false;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.DimGray;
            this.tabPage1.Controls.Add(this.App);
            this.tabPage1.Controls.Add(this.button_labeled);
            this.tabPage1.Controls.Add(this.button_showFeature);
            this.tabPage1.Controls.Add(this.button_Stereo);
            this.tabPage1.Controls.Add(this.textBox_shell);
            this.tabPage1.Controls.Add(this.button_importPC);
            this.tabPage1.Controls.Add(this.button_Object);
            this.tabPage1.Controls.Add(this.button_showDenser);
            this.tabPage1.Controls.Add(this.button_showDepth);
            this.tabPage1.Controls.Add(this.button_showMerged);
            this.tabPage1.Controls.Add(this.button_showEdge);
            this.tabPage1.Controls.Add(this.button_showOrigin);
            this.tabPage1.Controls.Add(this.imageBox_main);
            this.tabPage1.Controls.Add(this.button_getRT);
            this.tabPage1.Controls.Add(this.label_numImgUnit);
            this.tabPage1.Controls.Add(this.label_numImg);
            this.tabPage1.Controls.Add(this.button_getResult);
            this.tabPage1.Controls.Add(this.button_labeling);
            this.tabPage1.Controls.Add(this.button_denser);
            this.tabPage1.Controls.Add(this.button_canny);
            this.tabPage1.Controls.Add(this.button_deleteImg);
            this.tabPage1.Controls.Add(this.button_runSfM);
            this.tabPage1.Controls.Add(this.button_importImg);
            this.tabPage1.Controls.Add(this.listView_imgs);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1128, 777);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Denser";
            // 
            // App
            // 
            this.App.Location = new System.Drawing.Point(853, 719);
            this.App.Name = "App";
            this.App.Size = new System.Drawing.Size(250, 40);
            this.App.TabIndex = 27;
            this.App.Text = "Application Start";
            this.App.UseVisualStyleBackColor = true;
            this.App.Click += new System.EventHandler(this.App_Click);
            // 
            // button_labeled
            // 
            this.button_labeled.Location = new System.Drawing.Point(501, 719);
            this.button_labeled.Name = "button_labeled";
            this.button_labeled.Size = new System.Drawing.Size(80, 40);
            this.button_labeled.TabIndex = 26;
            this.button_labeled.Text = "Labeled Image";
            this.button_labeled.UseVisualStyleBackColor = true;
            this.button_labeled.Click += new System.EventHandler(this.button_labeled_Click);
            // 
            // button_showFeature
            // 
            this.button_showFeature.Location = new System.Drawing.Point(313, 719);
            this.button_showFeature.Name = "button_showFeature";
            this.button_showFeature.Size = new System.Drawing.Size(80, 40);
            this.button_showFeature.TabIndex = 25;
            this.button_showFeature.Text = "Feature";
            this.button_showFeature.UseVisualStyleBackColor = true;
            this.button_showFeature.Click += new System.EventHandler(this.button_showFeature_Click);
            // 
            // button_Stereo
            // 
            this.button_Stereo.Location = new System.Drawing.Point(980, 133);
            this.button_Stereo.Name = "button_Stereo";
            this.button_Stereo.Size = new System.Drawing.Size(123, 44);
            this.button_Stereo.TabIndex = 24;
            this.button_Stereo.Text = "Stereo";
            this.button_Stereo.UseVisualStyleBackColor = true;
            this.button_Stereo.Click += new System.EventHandler(this.button_Stereo_Click);
            // 
            // textBox_shell
            // 
            this.textBox_shell.BackColor = System.Drawing.SystemColors.InfoText;
            this.textBox_shell.ForeColor = System.Drawing.SystemColors.Control;
            this.textBox_shell.Location = new System.Drawing.Point(853, 662);
            this.textBox_shell.Multiline = true;
            this.textBox_shell.Name = "textBox_shell";
            this.textBox_shell.Size = new System.Drawing.Size(250, 38);
            this.textBox_shell.TabIndex = 23;
            // 
            // button_importPC
            // 
            this.button_importPC.Location = new System.Drawing.Point(980, 302);
            this.button_importPC.Name = "button_importPC";
            this.button_importPC.Size = new System.Drawing.Size(123, 45);
            this.button_importPC.TabIndex = 22;
            this.button_importPC.Text = "Get PC";
            this.button_importPC.UseVisualStyleBackColor = true;
            this.button_importPC.Click += new System.EventHandler(this.button_importPC_Click);
            // 
            // button_Object
            // 
            this.button_Object.Location = new System.Drawing.Point(689, 719);
            this.button_Object.Name = "button_Object";
            this.button_Object.Size = new System.Drawing.Size(80, 40);
            this.button_Object.TabIndex = 21;
            this.button_Object.Text = "Detected Obj";
            this.button_Object.UseVisualStyleBackColor = true;
            this.button_Object.Click += new System.EventHandler(this.button_Object_Click);
            // 
            // button_showDenser
            // 
            this.button_showDenser.Location = new System.Drawing.Point(595, 719);
            this.button_showDenser.Name = "button_showDenser";
            this.button_showDenser.Size = new System.Drawing.Size(80, 40);
            this.button_showDenser.TabIndex = 20;
            this.button_showDenser.Text = "Denser  Depth";
            this.button_showDenser.UseVisualStyleBackColor = true;
            this.button_showDenser.Click += new System.EventHandler(this.button_showDenser_Click);
            // 
            // button_showDepth
            // 
            this.button_showDepth.Location = new System.Drawing.Point(408, 719);
            this.button_showDepth.Name = "button_showDepth";
            this.button_showDepth.Size = new System.Drawing.Size(80, 40);
            this.button_showDepth.TabIndex = 19;
            this.button_showDepth.Text = "Depth";
            this.button_showDepth.UseVisualStyleBackColor = true;
            this.button_showDepth.Click += new System.EventHandler(this.button_showDepth_Click);
            // 
            // button_showMerged
            // 
            this.button_showMerged.Location = new System.Drawing.Point(216, 719);
            this.button_showMerged.Name = "button_showMerged";
            this.button_showMerged.Size = new System.Drawing.Size(80, 40);
            this.button_showMerged.TabIndex = 18;
            this.button_showMerged.Text = "Orgn+Edge";
            this.button_showMerged.UseVisualStyleBackColor = true;
            this.button_showMerged.Click += new System.EventHandler(this.button_showMerged_Click);
            // 
            // button_showEdge
            // 
            this.button_showEdge.Location = new System.Drawing.Point(120, 719);
            this.button_showEdge.Name = "button_showEdge";
            this.button_showEdge.Size = new System.Drawing.Size(80, 40);
            this.button_showEdge.TabIndex = 17;
            this.button_showEdge.Text = "Edge";
            this.button_showEdge.UseVisualStyleBackColor = true;
            this.button_showEdge.Click += new System.EventHandler(this.button_showEdge_Click);
            // 
            // button_showOrigin
            // 
            this.button_showOrigin.Location = new System.Drawing.Point(24, 719);
            this.button_showOrigin.Name = "button_showOrigin";
            this.button_showOrigin.Size = new System.Drawing.Size(80, 40);
            this.button_showOrigin.TabIndex = 16;
            this.button_showOrigin.Text = "Origianl";
            this.button_showOrigin.UseVisualStyleBackColor = true;
            this.button_showOrigin.Click += new System.EventHandler(this.button_showOrigin_Click);
            // 
            // imageBox_main
            // 
            this.imageBox_main.Location = new System.Drawing.Point(24, 20);
            this.imageBox_main.Name = "imageBox_main";
            this.imageBox_main.Size = new System.Drawing.Size(800, 680);
            this.imageBox_main.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageBox_main.TabIndex = 2;
            this.imageBox_main.TabStop = false;
            this.imageBox_main.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imageBox_main_MouseDown);
            this.imageBox_main.MouseMove += new System.Windows.Forms.MouseEventHandler(this.imageBox_main_MouseMove);
            this.imageBox_main.MouseUp += new System.Windows.Forms.MouseEventHandler(this.imageBox_main_MouseUp);
            // 
            // button_getRT
            // 
            this.button_getRT.Location = new System.Drawing.Point(980, 244);
            this.button_getRT.Name = "button_getRT";
            this.button_getRT.Size = new System.Drawing.Size(123, 47);
            this.button_getRT.TabIndex = 15;
            this.button_getRT.Text = "Get RT Info";
            this.button_getRT.UseVisualStyleBackColor = true;
            this.button_getRT.Click += new System.EventHandler(this.button_getRT_Click);
            // 
            // label_numImgUnit
            // 
            this.label_numImgUnit.AutoSize = true;
            this.label_numImgUnit.ForeColor = System.Drawing.Color.White;
            this.label_numImgUnit.Location = new System.Drawing.Point(941, 576);
            this.label_numImgUnit.Name = "label_numImgUnit";
            this.label_numImgUnit.Size = new System.Drawing.Size(17, 12);
            this.label_numImgUnit.TabIndex = 14;
            this.label_numImgUnit.Text = "장";
            // 
            // label_numImg
            // 
            this.label_numImg.AutoSize = true;
            this.label_numImg.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label_numImg.Location = new System.Drawing.Point(927, 575);
            this.label_numImg.Name = "label_numImg";
            this.label_numImg.Size = new System.Drawing.Size(12, 12);
            this.label_numImg.TabIndex = 13;
            this.label_numImg.Text = "n";
            this.label_numImg.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button_getResult
            // 
            this.button_getResult.Location = new System.Drawing.Point(980, 525);
            this.button_getResult.Name = "button_getResult";
            this.button_getResult.Size = new System.Drawing.Size(123, 45);
            this.button_getResult.TabIndex = 11;
            this.button_getResult.Text = "Save Layer";
            this.button_getResult.UseVisualStyleBackColor = true;
            this.button_getResult.Click += new System.EventHandler(this.button_getResult_Click);
            // 
            // button_labeling
            // 
            this.button_labeling.Location = new System.Drawing.Point(980, 470);
            this.button_labeling.Name = "button_labeling";
            this.button_labeling.Size = new System.Drawing.Size(123, 45);
            this.button_labeling.TabIndex = 10;
            this.button_labeling.Text = "Object Labeling";
            this.button_labeling.UseVisualStyleBackColor = true;
            this.button_labeling.Click += new System.EventHandler(this.button_labeling_Click);
            // 
            // button_denser
            // 
            this.button_denser.Location = new System.Drawing.Point(980, 358);
            this.button_denser.Name = "button_denser";
            this.button_denser.Size = new System.Drawing.Size(123, 45);
            this.button_denser.TabIndex = 9;
            this.button_denser.Text = "Depth Denser";
            this.button_denser.UseVisualStyleBackColor = true;
            this.button_denser.Click += new System.EventHandler(this.button_denser_Click);
            // 
            // button_canny
            // 
            this.button_canny.Location = new System.Drawing.Point(980, 414);
            this.button_canny.Name = "button_canny";
            this.button_canny.Size = new System.Drawing.Size(123, 45);
            this.button_canny.TabIndex = 8;
            this.button_canny.Text = "Edge Detection";
            this.button_canny.UseVisualStyleBackColor = true;
            this.button_canny.Click += new System.EventHandler(this.button_canny_Click);
            // 
            // button_deleteImg
            // 
            this.button_deleteImg.Location = new System.Drawing.Point(980, 78);
            this.button_deleteImg.Name = "button_deleteImg";
            this.button_deleteImg.Size = new System.Drawing.Size(123, 45);
            this.button_deleteImg.TabIndex = 7;
            this.button_deleteImg.Text = "Delete Images";
            this.button_deleteImg.UseVisualStyleBackColor = true;
            this.button_deleteImg.Click += new System.EventHandler(this.button_deleteImg_Click);
            // 
            // button_runSfM
            // 
            this.button_runSfM.Location = new System.Drawing.Point(980, 187);
            this.button_runSfM.Name = "button_runSfM";
            this.button_runSfM.Size = new System.Drawing.Size(123, 45);
            this.button_runSfM.TabIndex = 6;
            this.button_runSfM.Text = "Run SfM";
            this.button_runSfM.UseVisualStyleBackColor = true;
            this.button_runSfM.Click += new System.EventHandler(this.button_runSfM_Click);
            // 
            // button_importImg
            // 
            this.button_importImg.Location = new System.Drawing.Point(980, 20);
            this.button_importImg.Name = "button_importImg";
            this.button_importImg.Size = new System.Drawing.Size(123, 45);
            this.button_importImg.TabIndex = 5;
            this.button_importImg.Text = "Import Images";
            this.button_importImg.UseVisualStyleBackColor = true;
            this.button_importImg.Click += new System.EventHandler(this.button_importImg_Click);
            // 
            // listView_imgs
            // 
            this.listView_imgs.Location = new System.Drawing.Point(853, 20);
            this.listView_imgs.Name = "listView_imgs";
            this.listView_imgs.Size = new System.Drawing.Size(106, 550);
            this.listView_imgs.TabIndex = 3;
            this.listView_imgs.UseCompatibleStateImageBehavior = false;
            this.listView_imgs.View = System.Windows.Forms.View.List;
            this.listView_imgs.SelectedIndexChanged += new System.EventHandler(this.listView_imgs_SelectedIndexChanged);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage_info);
            this.tabControl.Location = new System.Drawing.Point(1, 4);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1136, 803);
            this.tabControl.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(1137, 807);
            this.Controls.Add(this.tabControl);
            this.Name = "Form1";
            this.Text = "denserDepth";
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_app)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox_main)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage_info;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button_exportDLO;
        private System.Windows.Forms.Button button_importDLO;
        private System.Windows.Forms.PictureBox pictureBox_app;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button_labeled;
        private System.Windows.Forms.Button button_showFeature;
        private System.Windows.Forms.Button button_Stereo;
        private System.Windows.Forms.TextBox textBox_shell;
        private System.Windows.Forms.Button button_importPC;
        private System.Windows.Forms.Button button_Object;
        private System.Windows.Forms.Button button_showDenser;
        private System.Windows.Forms.Button button_showDepth;
        private System.Windows.Forms.Button button_showMerged;
        private System.Windows.Forms.Button button_showEdge;
        private System.Windows.Forms.Button button_showOrigin;
        private Emgu.CV.UI.ImageBox imageBox_main;
        private System.Windows.Forms.Button button_getRT;
        private System.Windows.Forms.Label label_numImgUnit;
        private System.Windows.Forms.Label label_numImg;
        private System.Windows.Forms.Button button_getResult;
        private System.Windows.Forms.Button button_labeling;
        private System.Windows.Forms.Button button_denser;
        private System.Windows.Forms.Button button_canny;
        private System.Windows.Forms.Button button_deleteImg;
        private System.Windows.Forms.Button button_runSfM;
        private System.Windows.Forms.Button button_importImg;
        private System.Windows.Forms.ListView listView_imgs;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.Button App;
    }
}

