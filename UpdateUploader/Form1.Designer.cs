namespace UpdateUploader
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this._uploadButton = new System.Windows.Forms.Button();
            this._upLoadProgress = new System.Windows.Forms.ProgressBar();
            this._loadChangelog = new System.Windows.Forms.Button();
            this._descriptionBox = new System.Windows.Forms.RichTextBox();
            this._applicationPathBox = new System.Windows.Forms.TextBox();
            this._versionBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._nameBox = new System.Windows.Forms.TextBox();
            this._loadVersion = new System.Windows.Forms.Button();
            this._openApplicationPath = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this._userNameBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this._repositoryBox = new System.Windows.Forms.TextBox();
            this._appPathDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // _uploadButton
            // 
            this._uploadButton.Location = new System.Drawing.Point(458, 391);
            this._uploadButton.Name = "_uploadButton";
            this._uploadButton.Size = new System.Drawing.Size(75, 23);
            this._uploadButton.TabIndex = 0;
            this._uploadButton.Text = "Hochladen";
            this._uploadButton.UseVisualStyleBackColor = true;
            this._uploadButton.Click += new System.EventHandler(this.UploadButton_Click_Async);
            // 
            // _upLoadProgress
            // 
            this._upLoadProgress.Location = new System.Drawing.Point(12, 391);
            this._upLoadProgress.Name = "_upLoadProgress";
            this._upLoadProgress.Size = new System.Drawing.Size(435, 23);
            this._upLoadProgress.TabIndex = 1;
            // 
            // _loadChangelog
            // 
            this._loadChangelog.Location = new System.Drawing.Point(458, 151);
            this._loadChangelog.Name = "_loadChangelog";
            this._loadChangelog.Size = new System.Drawing.Size(75, 38);
            this._loadChangelog.TabIndex = 2;
            this._loadChangelog.Text = "Changelog Laden";
            this._loadChangelog.UseVisualStyleBackColor = true;
            this._loadChangelog.Click += new System.EventHandler(this.LoadChangeLog_Click);
            // 
            // _descriptionBox
            // 
            this._descriptionBox.Location = new System.Drawing.Point(73, 151);
            this._descriptionBox.Name = "_descriptionBox";
            this._descriptionBox.Size = new System.Drawing.Size(374, 234);
            this._descriptionBox.TabIndex = 3;
            this._descriptionBox.Text = "";
            // 
            // _applicationPathBox
            // 
            this._applicationPathBox.Location = new System.Drawing.Point(118, 58);
            this._applicationPathBox.Name = "_applicationPathBox";
            this._applicationPathBox.Size = new System.Drawing.Size(329, 20);
            this._applicationPathBox.TabIndex = 4;
            this._applicationPathBox.TextChanged += new System.EventHandler(this.TextChangedImpl);
            // 
            // _versionBox
            // 
            this._versionBox.Location = new System.Drawing.Point(73, 116);
            this._versionBox.Name = "_versionBox";
            this._versionBox.Size = new System.Drawing.Size(374, 20);
            this._versionBox.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Version:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Anwendungs Pfand:";
            // 
            // _nameBox
            // 
            this._nameBox.Location = new System.Drawing.Point(73, 84);
            this._nameBox.Name = "_nameBox";
            this._nameBox.Size = new System.Drawing.Size(374, 20);
            this._nameBox.TabIndex = 9;
            // 
            // _loadVersion
            // 
            this._loadVersion.Location = new System.Drawing.Point(458, 106);
            this._loadVersion.Name = "_loadVersion";
            this._loadVersion.Size = new System.Drawing.Size(75, 39);
            this._loadVersion.TabIndex = 10;
            this._loadVersion.Text = "Version laden";
            this._loadVersion.UseVisualStyleBackColor = true;
            this._loadVersion.Click += new System.EventHandler(this.LoadVersion_Click);
            // 
            // _openApplicationPath
            // 
            this._openApplicationPath.Location = new System.Drawing.Point(458, 56);
            this._openApplicationPath.Name = "_openApplicationPath";
            this._openApplicationPath.Size = new System.Drawing.Size(75, 23);
            this._openApplicationPath.TabIndex = 11;
            this._openApplicationPath.Text = "öffnen";
            this._openApplicationPath.UseVisualStyleBackColor = true;
            this._openApplicationPath.Click += new System.EventHandler(this.OpenApplicationPath_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Nutzer Name:";
            // 
            // _userNameBox
            // 
            this._userNameBox.Location = new System.Drawing.Point(98, 22);
            this._userNameBox.Name = "_userNameBox";
            this._userNameBox.Size = new System.Drawing.Size(133, 20);
            this._userNameBox.TabIndex = 13;
            this._userNameBox.TextChanged += new System.EventHandler(this.TextChangedImpl);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(237, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Repository:";
            // 
            // _repositoryBox
            // 
            this._repositoryBox.Location = new System.Drawing.Point(303, 22);
            this._repositoryBox.Name = "_repositoryBox";
            this._repositoryBox.Size = new System.Drawing.Size(144, 20);
            this._repositoryBox.TabIndex = 15;
            this._repositoryBox.TextChanged += new System.EventHandler(this.TextChangedImpl);
            // 
            // _appPathDialog
            // 
            this._appPathDialog.Description = "Bitte Anwendungspfad Auswählen";
            this._appPathDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this._appPathDialog.ShowNewFolderButton = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 427);
            this.Controls.Add(this._repositoryBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._userNameBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._openApplicationPath);
            this.Controls.Add(this._loadVersion);
            this.Controls.Add(this._nameBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._versionBox);
            this.Controls.Add(this._applicationPathBox);
            this.Controls.Add(this._descriptionBox);
            this.Controls.Add(this._loadChangelog);
            this.Controls.Add(this._upLoadProgress);
            this.Controls.Add(this._uploadButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _uploadButton;
        private System.Windows.Forms.ProgressBar _upLoadProgress;
        private System.Windows.Forms.Button _loadChangelog;
        private System.Windows.Forms.RichTextBox _descriptionBox;
        private System.Windows.Forms.TextBox _applicationPathBox;
        private System.Windows.Forms.TextBox _versionBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _nameBox;
        private System.Windows.Forms.Button _loadVersion;
        private System.Windows.Forms.Button _openApplicationPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _userNameBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox _repositoryBox;
        private System.Windows.Forms.FolderBrowserDialog _appPathDialog;
    }
}

