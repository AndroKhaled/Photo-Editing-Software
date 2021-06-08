namespace CvExample
{
	partial class DlgVideoSrc
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
			this.buttonFile = new System.Windows.Forms.Button();
			this.radioWebcamInline = new System.Windows.Forms.RadioButton();
			this.radioWebcamExtern = new System.Windows.Forms.RadioButton();
			this.radioFile = new System.Windows.Forms.RadioButton();
			this.textBoxFile = new System.Windows.Forms.TextBox();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonFile
			// 
			this.buttonFile.Enabled = false;
			this.buttonFile.Location = new System.Drawing.Point(213, 84);
			this.buttonFile.Name = "buttonFile";
			this.buttonFile.Size = new System.Drawing.Size(36, 23);
			this.buttonFile.TabIndex = 5;
			this.buttonFile.Text = "...";
			this.buttonFile.UseVisualStyleBackColor = true;
			this.buttonFile.Click += new System.EventHandler(this.buttonFile_Click);
			// 
			// radioWebcamInline
			// 
			this.radioWebcamInline.AutoSize = true;
			this.radioWebcamInline.Checked = true;
			this.radioWebcamInline.Location = new System.Drawing.Point(13, 15);
			this.radioWebcamInline.Name = "radioWebcamInline";
			this.radioWebcamInline.Size = new System.Drawing.Size(102, 17);
			this.radioWebcamInline.TabIndex = 1;
			this.radioWebcamInline.TabStop = true;
			this.radioWebcamInline.Text = "Webcam (Inline)";
			this.radioWebcamInline.UseVisualStyleBackColor = true;
			this.radioWebcamInline.CheckedChanged += new System.EventHandler(this.radioWebcamInline_CheckedChanged);
			// 
			// radioWebcamExtern
			// 
			this.radioWebcamExtern.AutoSize = true;
			this.radioWebcamExtern.Location = new System.Drawing.Point(13, 39);
			this.radioWebcamExtern.Name = "radioWebcamExtern";
			this.radioWebcamExtern.Size = new System.Drawing.Size(148, 17);
			this.radioWebcamExtern.TabIndex = 2;
			this.radioWebcamExtern.Text = "Webcam (extern Window)";
			this.radioWebcamExtern.UseVisualStyleBackColor = true;
			this.radioWebcamExtern.CheckedChanged += new System.EventHandler(this.radioWebcamExtern_CheckedChanged);
			// 
			// radioFile
			// 
			this.radioFile.AutoSize = true;
			this.radioFile.Location = new System.Drawing.Point(13, 63);
			this.radioFile.Name = "radioFile";
			this.radioFile.Size = new System.Drawing.Size(41, 17);
			this.radioFile.TabIndex = 3;
			this.radioFile.Text = "File";
			this.radioFile.UseVisualStyleBackColor = true;
			this.radioFile.CheckedChanged += new System.EventHandler(this.radioFile_CheckedChanged);
			// 
			// textBoxFile
			// 
			this.textBoxFile.Enabled = false;
			this.textBoxFile.Location = new System.Drawing.Point(13, 86);
			this.textBoxFile.Name = "textBoxFile";
			this.textBoxFile.Size = new System.Drawing.Size(194, 20);
			this.textBoxFile.TabIndex = 4;
			this.textBoxFile.Text = "select File...";
			// 
			// buttonOk
			// 
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(174, 135);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(75, 23);
			this.buttonOk.TabIndex = 6;
			this.buttonOk.Text = "Ok";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(93, 135);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// DlgVideoSrc
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(261, 171);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonFile);
			this.Controls.Add(this.textBoxFile);
			this.Controls.Add(this.radioFile);
			this.Controls.Add(this.radioWebcamExtern);
			this.Controls.Add(this.radioWebcamInline);
			this.Name = "DlgVideoSrc";
			this.Text = "Select Video Source";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		public System.Windows.Forms.RadioButton radioWebcamInline;
		public System.Windows.Forms.RadioButton radioWebcamExtern;
		public System.Windows.Forms.RadioButton radioFile;
		public System.Windows.Forms.TextBox textBoxFile;
		public System.Windows.Forms.Button buttonFile;
	}
}