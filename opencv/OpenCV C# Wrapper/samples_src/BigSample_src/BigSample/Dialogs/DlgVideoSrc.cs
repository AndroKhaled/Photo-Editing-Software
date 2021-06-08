using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CvExample
{
	public partial class DlgVideoSrc : Form
	{
		public DlgVideoSrc()
		{
			InitializeComponent();
		}

		private void buttonFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();

			// Initialisieren Datei-Dialog
			ofd.InitialDirectory = "C:\\Dokumente und Einstellungen\\Wartung\\Eigene Dateien\\Eigene Videos";
			ofd.Filter = "Avi files (avi)|*.avi; | Windows Media File (wmf)|*.wmf; | All Files (*.*) | *.*;";
			ofd.FilterIndex = 3;
			ofd.RestoreDirectory = true;

			// Datei Dialog
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				textBoxFile.Text = ofd.FileName;
			}
			else return;
		}

		private void radioFile_CheckedChanged(object sender, EventArgs e)
		{
			if (radioFile.Checked)
			{
				textBoxFile.Enabled = true;
				buttonFile.Enabled = true;
			}
		}

		private void radioPxc_CheckedChanged(object sender, EventArgs e)
		{
			//if (radioPxc.Checked)
			//{
			//  textBoxFile.Enabled = false;
			//  buttonFile.Enabled = false;
			//}
		}

		private void radioWebcamInline_CheckedChanged(object sender, EventArgs e)
		{
			if (radioWebcamInline.Checked)
			{
				textBoxFile.Enabled = false;
				buttonFile.Enabled = false;
			}
		}

		private void radioWebcamExtern_CheckedChanged(object sender, EventArgs e)
		{
			if (radioWebcamExtern.Checked)
			{
				textBoxFile.Enabled = false;
				buttonFile.Enabled = false;
			}
		}
	}
}