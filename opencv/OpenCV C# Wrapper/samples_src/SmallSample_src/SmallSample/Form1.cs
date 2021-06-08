using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using openCV;

namespace SmallSample
{

	/// <summary>
	/// Form1 class
	/// </summary>
	public partial class Form1 : Form
	{
		/// <summary>
		/// Delegate for Trackbar changed event (used for trackbar 1)
		/// </summary>
		private cvlib.OnTrackbarChangeCallback onChange1;

		/// <summary>
		/// Delegate for Trackbar changed event (used for trackbar 2)
		/// </summary>
		private cvlib.OnTrackbarChangeCallback onChange2;


		/// <summary>
		/// Delegate for Mouse events
		/// </summary>
		private cvlib.OnMouseCallback onMouse;

		/// <summary>
		/// Delegate for error event
		/// </summary>
		private cvlib.OnErrorCallback onError;
		
		/// <summary>
		/// Image variables
		/// </summary>
		private IplImage img, gray;

		/// <summary>
		/// status of the trackbars
		/// </summary>
		private int value1, value2;

		/// <summary>
		/// Initializes a new instance of the <see cref="Form1"/> class.
		/// </summary>
		public Form1()
		{
			// windows form desinger generated code
			InitializeComponent();
	
			// instantiate callbacks
			onChange1 = new cvlib.OnTrackbarChangeCallback(OnChange1);
			onChange2 = new cvlib.OnTrackbarChangeCallback(OnChange2);
			onMouse = new cvlib.OnMouseCallback(OnMouse);
			onError = new cvlib.OnErrorCallback(OnError);

			// set our Erro mode to Parent (call error handler, but dont quit)
			cvlib.CvSetErrMode(cvlib.CV_ErrModeParent);

			// set error event handler callback method
			cvlib.CvRedirectError(onError);

			// create output window
			cvlib.CvNamedWindow("MyWindow", cvlib.CV_WINDOW_AUTOSIZE);
			
			// create two trackbars for lower and higher tresholds
			cvlib.CvCreateTrackbar("trsh1:", "MyWindow", ref value1, 255, onChange1);
			cvlib.CvCreateTrackbar("trsh2:", "MyWindow", ref value2, 255, onChange2);

			// set mouse event handler callback method  
			cvlib.CvSetMouseCallback("MyWindow", onMouse, IntPtr.Zero);
		}

		/// <summary>
		/// Handles the Click event of the buttonExit control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonExit_Click(object sender, EventArgs e)
		{
			// release unmanaged resources
			if (img.imageData != IntPtr.Zero) cvlib.CvReleaseImage(ref img);
			if (gray.imageData != IntPtr.Zero) cvlib.CvReleaseImage(ref gray);

			// close the application
			this.Close();
		}

		/// <summary>
		/// Handles the Click event of the buttonFile control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void buttonFile_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				// Load image from file
				// try-catch for the case that image type is not valid
				// cvLoadImage does not raise an error here but returns
				// null pointer
				try
				{
					img = cvlib.CvLoadImage(openFileDialog1.FileName, cvlib.CV_LOAD_IMAGE_COLOR);
					
					// check for ipl errors
					if (cvlib.CvGetErrStatus() != 0 || img.ptr == IntPtr.Zero)
					{
						if (img.imageData != IntPtr.Zero) cvlib.CvReleaseImage(ref img);

						// Set status always back
						cvlib.CvSetErrStatus(0);
						return;
					}
				}
				catch
				{
					MessageBox.Show("Filtype not valid.", "Error while loading a file.", 
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// Create new image
				gray = cvlib.CvCreateImage(new CvSize(img.width, img.height), (int)cvlib.IPL_DEPTH_8U, 1);		
				
				// check for errors (this should be placed in a common method
				if (cvlib.CvGetErrStatus() != 0)
				{
					if (img.imageData != IntPtr.Zero) cvlib.CvReleaseImage(ref img);
					if (gray.imageData != IntPtr.Zero) cvlib.CvReleaseImage(ref gray);
					cvlib.CvSetErrStatus(0);
					return;
				}

				// assign iplimage to the picture box control
				this.pictureBox1.BackgroundImage = (Bitmap)img;

				// process the canny edge detector
				Process();
			}
		}


		/// <summary>
		/// Processes this instance.
		/// </summary>
		private void Process()
		{
			// create gray level image from source
			cvlib.CvCvtColor(ref img, ref gray, cvlib.CV_BGR2GRAY);

			// apply the operator
			cvlib.CvCanny(ref gray, ref gray, value1, value2, 3);
			
			// check for errors
			if (cvlib.CvGetErrStatus() != 0)
			{
				cvlib.CvReleaseImage(ref img);

				// Set status always back
				cvlib.CvSetErrStatus(0);
				return;
			}

			// shwo the result in external window
			cvlib.CvShowImage("MyWindow", ref gray);
		}

		/// <summary>
		/// Called when [change1].
		/// </summary>
		/// <param name="val">The val of the trackbar.</param>
		private void OnChange1(int val)
		{
			value1 = val;
			Process();
		}

		/// <summary>
		/// Called when [change2].
		/// </summary>
		/// <param name="val">The val of the trackbar.</param>
		private void OnChange2(int val)
		{
			value2 = val;
			Process();
		}


		/// <summary>
		/// Called when [mouse].
		/// </summary>
		/// <param name="evnt">The evnt (eg mouse down)</param>
		/// <param name="x">x-coordinate of mouse.</param>
		/// <param name="y">y-coordinate of mouse.</param>
		/// <param name="flags">Some flags (eg. mouse down)</param>
		/// <param name="param">Optional Parameter</param>
		private void OnMouse(int evnt, int x, int y, int flags, IntPtr param)
		{
			labelValue.Text = 
				"Event: " + evnt.ToString() + 
				"\nx=" + x.ToString() + 
				"\ny=" + y.ToString() + 
				"\nFlags: " + flags.ToString();
		}


		/// <summary>
		/// Called when [error].
		/// </summary>
		/// <param name="status">Error status: 0 if no error, > 0 encoded error message</param>
		/// <param name="func_name">The func_name where the error occured.</param>
		/// <param name="err_msg">The error text.</param>
		/// <param name="file_name">The file name where the error occured.</param>
		/// <param name="line">The line in the file where the error occured.</param>
		/// <returns></returns>
		private int OnError(int status, string func_name,
										string err_msg, string file_name, int line)
		{
			MessageBox.Show("Status: " + cvlib.CvErrorStr(status) + 
				"\nIn Function: " + func_name + 
				"\nMessage: " + err_msg + 
				"\nIn File: " + file_name + 
				"\nOn Line: " + line.ToString(), "CV-Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return 0;
		}
	}
}