//**********************************************************
// File name: $ MaiForm.cs $
// Author:		$ Heiko Kieﬂling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Main file for demontrating usage of cvlib.dll.
//						You must copy cvlib.dll and all OpenCV dll's in
//						your release and debug folder else the appl.
//						will not execute.
// 
// License:		There is no explicit license attached. Feel free
//						to use the code how you like but without any warranty.
//						If you include the code in your own projects and/or
//						redistribute pls. include this header.
//
// History:		Rev. 1.0 (beta), hki - initial revision
//						Rev. 1.1, hki
//**********************************************************

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Drawing.Imaging;
using openCV;
using System.Runtime.InteropServices;
using System.Collections;
using Tools;

namespace CvExample
{
	/// <summary>
	/// Indikation of what type of 'video' has been selected
	/// </summary>
	public enum typeOfVideo 
	{
		/// <summary>
		/// USB Webcam will be shown in Picture Box Window
		/// </summary>
		inlineVFW,

		/// <summary>
		/// PXC 200 Framgrabber used (if available)
		/// </summary>
		inlineGrabber,

		/// <summary>
		/// USB Webcam will be shown in external OpenCV Window
		/// </summary>
		externVFW,

		/// <summary>
		/// A video from a file (only indeo codec) will be shown in Picture box
		/// </summary>
		file
	};

	/// <summary>
	/// The main object
	/// </summary>
	public partial class MainForm: Form
	{
		#region Member
		#region Function Table
		/// <summary>
		/// actually supported image processing functions
		/// If you want to add a new function this is the
		/// first place where you add the name.
		/// </summary>
		private string[] FunctionTable = new string[] 
		{
			"Canny",
			"Dilate", 
			"Erode",
			"Corner Harris",
			"Sobel",
			"Laplace", 
			"Watershed Transformation",
			"Pyramid Segmentation",
			"Calibration (Teach)",
			"Calibration (Correct)",
			"HAAR Classifier (Faces)",
			"Find Contours",
			"Hough Lines",
			"Hough Circles",
			"Convert2Gray",
			"FitLine",
			"FitEllipse",
			"Scanline Histograms",
			"Equalize Histogram",
			"Hue-Sat Histogram",
			"Good Features to Track",
			"Singular Value Transformation",
			"Matrix Multiplication",
			"Perspective Transform",
			"Random",
			"Moments",
			"Flood Fill",
			"Template Matching"
		};
		#endregion

		#region Form Designer
		/// <summary>
		/// Private form designer members
		/// </summary>
		private MenuStrip menuStrip;
		private StatusStrip statusStrip;
		private ToolStripStatusLabel toolStripStatusLabelFrameRate;
		private System.ComponentModel.IContainer components;
		private ToolStripStatusLabel toolStripStatusLabelPos;
		private ToolStripMenuItem toolStripMenuItemFile;
		private ToolStripMenuItem loadImageToolStripMenuItem;
		private ToolStripMenuItem toolStripMenuItemTabSchlieﬂen;
		private ToolStripMenuItem beendenToolStripMenuItem;
		private TabControl tabControlPictures;
		private TextBox textBox;
		private ToolStripComboBox comboBoxOperations;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripMenuItem executeToolStripMenuItem;
		private ToolStripMenuItem videoToolStripMenuItem1;
		private ToolStripMenuItem toolStripMenuItemOptionen;
		private ToolStripMenuItem openResultInNewTabpageToolStripMenuItem;
		private ToolStripMenuItem saveImageToolStripMenuItem;
		private FolderBrowserDialog folderBrowserDialog1;
		private ContextMenuStrip contextMenuStrip1;
		private ToolStripMenuItem clearOutputToolStripMenuItem;
		private ToolStripMenuItem videoSourceToolStripMenuItem1;
		#endregion

		#region User defined
		/// <summary>
		/// List of points collected by click on the picture box used
		/// for input the watershed algorithm
		/// </summary>
		public List<Point> watershedPoints = null;

		/// <summary>
		/// Used for aquiring images
		/// </summary>
		private System.Windows.Forms.Timer timerGrab = null;

		/// <summary>
		/// used for calcualtion FPS
		/// </summary>
		private System.Windows.Forms.Timer timerFPS = null;

		/// <summary>
		/// A dynamic dialog for adding controls to control
		/// the image processing operations
		/// </summary>
		public DlgParams dlg = null;
		
		/// <summary>
		/// OpenCV Video Capture Structure
		/// </summary>
		private CvCapture videoCapture;

		/// <summary>
		/// Used to detect if OpenCV window is open and needs
		/// to destroy
		/// </summary>
		private int extWindowHandle;

		/// <summary>
		/// Indicates if mouse coordinates handeld as watersheet input points
		/// and will be drawn
		/// </summary>
		private bool drawWatershedPoints;

		/// <summary>
		/// the selected Video type (see enum above)
		/// </summary>
		private typeOfVideo vidoType = typeOfVideo.inlineVFW;
		
		/// <summary>
		/// name of the selected video file (when video from file)
		/// </summary>
		private string videoFile = "";

		/// <summary>
		/// some counter
		/// </summary>
		private int fps, fcnt, nFrames;
		#endregion
		#endregion

		#region Constructor
		/// <summary>
		/// Main form ctor
		/// </summary>
		public MainForm()
		{
			//set some defaults
			watershedPoints = new List<Point>();
			extWindowHandle = 0;
			drawWatershedPoints = false;
			vidoType = typeOfVideo.inlineVFW;
			fps = 0;
			InitializeComponent();
		}
		#endregion

		#region Destructor
		/// <summary>
		/// Garbage Collector
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			// release our Bitmap ressources (picture boxes, tab pages)
			foreach (TabPage page in tabControlPictures.TabPages)
			{
				((PictureBox)page.Controls[0]).Image.Dispose();
			}

			// release video capture
			if (videoCapture.ptr != IntPtr.Zero)
			{
				cvlib.CvReleaseCapture(ref videoCapture);
				videoCapture.ptr = IntPtr.Zero;
			}

			// close openCV Window when open
			if (extWindowHandle != 0)
			{
				cvlib.CvDestroyWindow("Capture");
				extWindowHandle = 0;
			}

			// dispose form components and call base class
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.toolStripMenuItemFile = new System.Windows.Forms.ToolStripMenuItem();
			this.loadImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.videoSourceToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.beendenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemOptionen = new System.Windows.Forms.ToolStripMenuItem();
			this.openResultInNewTabpageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.videoToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemTabSchlieﬂen = new System.Windows.Forms.ToolStripMenuItem();
			this.comboBoxOperations = new System.Windows.Forms.ToolStripComboBox();
			this.executeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabelPos = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabelFrameRate = new System.Windows.Forms.ToolStripStatusLabel();
			this.tabControlPictures = new System.Windows.Forms.TabControl();
			this.textBox = new System.Windows.Forms.TextBox();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.clearOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.timerGrab = new System.Windows.Forms.Timer(this.components);
			this.timerFPS = new System.Windows.Forms.Timer(this.components);
			this.menuStrip.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.AllowMerge = false;
			this.menuStrip.AutoSize = false;
			this.menuStrip.BackColor = System.Drawing.Color.LightGray;
			this.menuStrip.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("menuStrip.BackgroundImage")));
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemFile,
            this.toolStripMenuItemOptionen,
            this.videoToolStripMenuItem1,
            this.toolStripMenuItemTabSchlieﬂen,
            this.comboBoxOperations,
            this.executeToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(792, 35);
			this.menuStrip.TabIndex = 16;
			this.menuStrip.Text = "menuStrip1";
			// 
			// toolStripMenuItemFile
			// 
			this.toolStripMenuItemFile.BackColor = System.Drawing.Color.Transparent;
			this.toolStripMenuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadImageToolStripMenuItem,
            this.saveImageToolStripMenuItem,
            this.videoSourceToolStripMenuItem1,
            this.toolStripSeparator3,
            this.beendenToolStripMenuItem});
			this.toolStripMenuItemFile.Font = new System.Drawing.Font("Andale Sans UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.toolStripMenuItemFile.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
			this.toolStripMenuItemFile.Name = "toolStripMenuItemFile";
			this.toolStripMenuItemFile.Padding = new System.Windows.Forms.Padding(4, 0, 10, 0);
			this.toolStripMenuItemFile.Size = new System.Drawing.Size(47, 31);
			this.toolStripMenuItemFile.Text = "File...";
			this.toolStripMenuItemFile.ToolTipText = "Open or save a new Image File or Spline Set";
			// 
			// loadImageToolStripMenuItem
			// 
			this.loadImageToolStripMenuItem.AutoToolTip = true;
			this.loadImageToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("loadImageToolStripMenuItem.Image")));
			this.loadImageToolStripMenuItem.Name = "loadImageToolStripMenuItem";
			this.loadImageToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.loadImageToolStripMenuItem.Text = "Load Image...";
			this.loadImageToolStripMenuItem.ToolTipText = "Load Image from file.";
			this.loadImageToolStripMenuItem.Click += new System.EventHandler(this.LoadImageToolStripMenuItem_Click);
			// 
			// saveImageToolStripMenuItem
			// 
			this.saveImageToolStripMenuItem.Name = "saveImageToolStripMenuItem";
			this.saveImageToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.saveImageToolStripMenuItem.Text = "Save Image...";
			this.saveImageToolStripMenuItem.ToolTipText = "Save Image to file.";
			this.saveImageToolStripMenuItem.Click += new System.EventHandler(this.SaveImageToolStripMenuItem_Click);
			// 
			// videoSourceToolStripMenuItem1
			// 
			this.videoSourceToolStripMenuItem1.Name = "videoSourceToolStripMenuItem1";
			this.videoSourceToolStripMenuItem1.Size = new System.Drawing.Size(154, 22);
			this.videoSourceToolStripMenuItem1.Text = "Video Source...";
			this.videoSourceToolStripMenuItem1.Click += new System.EventHandler(this.videoSourceToolStripMenuItem1_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(151, 6);
			// 
			// beendenToolStripMenuItem
			// 
			this.beendenToolStripMenuItem.Name = "beendenToolStripMenuItem";
			this.beendenToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.beendenToolStripMenuItem.Text = "Exit";
			this.beendenToolStripMenuItem.ToolTipText = "Exit the application.";
			// 
			// toolStripMenuItemOptionen
			// 
			this.toolStripMenuItemOptionen.BackColor = System.Drawing.Color.Transparent;
			this.toolStripMenuItemOptionen.Checked = true;
			this.toolStripMenuItemOptionen.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItemOptionen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openResultInNewTabpageToolStripMenuItem});
			this.toolStripMenuItemOptionen.Font = new System.Drawing.Font("Andale Sans UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.toolStripMenuItemOptionen.Name = "toolStripMenuItemOptionen";
			this.toolStripMenuItemOptionen.Size = new System.Drawing.Size(66, 31);
			this.toolStripMenuItemOptionen.Text = "Settings...";
			// 
			// openResultInNewTabpageToolStripMenuItem
			// 
			this.openResultInNewTabpageToolStripMenuItem.Checked = true;
			this.openResultInNewTabpageToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.openResultInNewTabpageToolStripMenuItem.Name = "openResultInNewTabpageToolStripMenuItem";
			this.openResultInNewTabpageToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.openResultInNewTabpageToolStripMenuItem.Text = "Open Result in new Tabpage";
			this.openResultInNewTabpageToolStripMenuItem.Click += new System.EventHandler(this.OpenResultInNewTabpageToolStripMenuItem_Click);
			// 
			// videoToolStripMenuItem1
			// 
			this.videoToolStripMenuItem1.BackColor = System.Drawing.Color.Transparent;
			this.videoToolStripMenuItem1.Font = new System.Drawing.Font("Andale Sans UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.videoToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("videoToolStripMenuItem1.Image")));
			this.videoToolStripMenuItem1.Name = "videoToolStripMenuItem1";
			this.videoToolStripMenuItem1.Size = new System.Drawing.Size(90, 31);
			this.videoToolStripMenuItem1.Text = "Start Video";
			this.videoToolStripMenuItem1.Click += new System.EventHandler(this.VideoStartStopToolStripMenuItem_Click);
			// 
			// toolStripMenuItemTabSchlieﬂen
			// 
			this.toolStripMenuItemTabSchlieﬂen.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolStripMenuItemTabSchlieﬂen.AutoSize = false;
			this.toolStripMenuItemTabSchlieﬂen.AutoToolTip = true;
			this.toolStripMenuItemTabSchlieﬂen.BackColor = System.Drawing.Color.Transparent;
			this.toolStripMenuItemTabSchlieﬂen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.toolStripMenuItemTabSchlieﬂen.Font = new System.Drawing.Font("Andale Sans UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.toolStripMenuItemTabSchlieﬂen.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
			this.toolStripMenuItemTabSchlieﬂen.Name = "toolStripMenuItemTabSchlieﬂen";
			this.toolStripMenuItemTabSchlieﬂen.Size = new System.Drawing.Size(94, 21);
			this.toolStripMenuItemTabSchlieﬂen.Text = "Close Tabpage";
			this.toolStripMenuItemTabSchlieﬂen.ToolTipText = "Schlieﬂt das aktuelle Bild.";
			this.toolStripMenuItemTabSchlieﬂen.Click += new System.EventHandler(this.tabSchlieﬂenToolStripMenuItem_Click);
			// 
			// comboBoxOperations
			// 
			this.comboBoxOperations.AutoCompleteCustomSource.AddRange(new string[] {
            "Select Operation...",
            "Canny Edge Detector",
            "Dilate",
            "Erode"});
			this.comboBoxOperations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxOperations.Items.AddRange(new object[] {
            "Select Operation..."});
			this.comboBoxOperations.Name = "comboBoxOperations";
			this.comboBoxOperations.Size = new System.Drawing.Size(170, 31);
			this.comboBoxOperations.SelectedIndexChanged += new System.EventHandler(this.comboBoxOperations_SelectedIndexChanged);
			// 
			// executeToolStripMenuItem
			// 
			this.executeToolStripMenuItem.BackColor = System.Drawing.Color.Transparent;
			this.executeToolStripMenuItem.Font = new System.Drawing.Font("Andale Sans UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.executeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("executeToolStripMenuItem.Image")));
			this.executeToolStripMenuItem.Name = "executeToolStripMenuItem";
			this.executeToolStripMenuItem.Size = new System.Drawing.Size(53, 31);
			this.executeToolStripMenuItem.Text = "Run";
			this.executeToolStripMenuItem.Click += new System.EventHandler(this.MenuItemProcess_Click);
			// 
			// statusStrip
			// 
			this.statusStrip.BackColor = System.Drawing.Color.Transparent;
			this.statusStrip.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("statusStrip.BackgroundImage")));
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelPos,
            this.toolStripStatusLabelFrameRate});
			this.statusStrip.Location = new System.Drawing.Point(0, 614);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(792, 22);
			this.statusStrip.TabIndex = 17;
			this.statusStrip.Text = "statusStrip";
			// 
			// toolStripStatusLabelPos
			// 
			this.toolStripStatusLabelPos.Name = "toolStripStatusLabelPos";
			this.toolStripStatusLabelPos.Size = new System.Drawing.Size(0, 17);
			// 
			// toolStripStatusLabelFrameRate
			// 
			this.toolStripStatusLabelFrameRate.Name = "toolStripStatusLabelFrameRate";
			this.toolStripStatusLabelFrameRate.Size = new System.Drawing.Size(0, 17);
			// 
			// tabControlPictures
			// 
			this.tabControlPictures.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
									| System.Windows.Forms.AnchorStyles.Left)
									| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlPictures.Location = new System.Drawing.Point(0, 35);
			this.tabControlPictures.Name = "tabControlPictures";
			this.tabControlPictures.SelectedIndex = 0;
			this.tabControlPictures.Size = new System.Drawing.Size(792, 478);
			this.tabControlPictures.TabIndex = 18;
			// 
			// textBox
			// 
			this.textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(239)))), ((int)(((byte)(240)))));
			this.textBox.ContextMenuStrip = this.contextMenuStrip1;
			this.textBox.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.textBox.ForeColor = System.Drawing.Color.Green;
			this.textBox.Location = new System.Drawing.Point(0, 512);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox.Size = new System.Drawing.Size(792, 102);
			this.textBox.TabIndex = 20;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearOutputToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(146, 26);
			// 
			// clearOutputToolStripMenuItem
			// 
			this.clearOutputToolStripMenuItem.Name = "clearOutputToolStripMenuItem";
			this.clearOutputToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
			this.clearOutputToolStripMenuItem.Text = "Clear output";
			this.clearOutputToolStripMenuItem.Click += new System.EventHandler(this.clearOutputToolStripMenuItem_Click);
			// 
			// timerGrab
			// 
			this.timerGrab.Tick += new System.EventHandler(this.TimerGrab_Tick);
			// 
			// timerFPS
			// 
			this.timerFPS.Tick += new System.EventHandler(this.TimerFPS_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.ClientSize = new System.Drawing.Size(792, 636);
			this.Controls.Add(this.textBox);
			this.Controls.Add(this.tabControlPictures);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.menuStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip;
			this.Name = "MainForm";
			this.Text = "(Vision).[c#]";
			this.Load += new System.EventHandler(this.StartForm_Load);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		#region Main Function
		[STAThread]
		static void Main() 
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		#endregion

		#region Main Form Load
		/// <summary>
		/// Double-Bpuffer und schelle Zeichenroutinen aktivieren
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void StartForm_Load(object sender, System.EventArgs e)
		{
			// some useful details for 3D and flicker free drawing
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);

			// initialize function combobox
			comboBoxOperations.Items.AddRange(FunctionTable);
			comboBoxOperations.SelectedIndex = 0;

			// load start image
			IplImage img;
			try
			{
				img = cvlib.CvLoadImage("start.jpg", cvlib.CV_LOAD_IMAGE_COLOR);
			}
			catch
			{
				return;
			}

			// set start image and write welcome text
			CreateNewTab("Start.jpg", img, true);
			WriteLine("Welcome to (vision).[c#] V1.0 beta.", true, true); 
		}
		#endregion

		//////////////////////

		#region Draw & Mouse Events
		/// <summary>
		/// Draw a Overlay.
		/// watershedPoints if (Tab)
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">not used</param>
		private void PictBox_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			try
			{
			  if (drawWatershedPoints)
			  {
			    SolidBrush bRed = new SolidBrush(Color.Red);
			    foreach (Point pt in watershedPoints)
			      g.FillEllipse(bRed, pt.X - 2, pt.Y - 2, 4, 4);
			    bRed.Dispose();
			  }
			}
			catch (Exception ex)
			{
			  WriteLine("Draw: " + ex.Message, true, true);
			}
		}

		/// <summary>
		/// Mouse Ereignis Klick links: Punkt setzen
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PictBox_MouseClick(object sender, MouseEventArgs e)
		{
			if (tabControlPictures.SelectedTab == null) return;

			Point pt;
			if (comboBoxOperations.SelectedItem.ToString().CompareTo("Watershed Transformation") == 0)
			{
				if (watershedPoints == null) return;

				// Draw Point only if not alredy one on that pos
				if (e.Button == MouseButtons.Left)
				{
					// Punkt mit Mousekoordinaten pr¸fen und ggf. 
					// zu Point Liste hinzuf¸gen
					if ((pt = findNearestPoint(e.X, e.Y, watershedPoints)).X < 0)
						watershedPoints.Add(new Point(e.X, e.Y));
				}
				else
				{
					// Lˆschen des Punktes

					if (e.Button == MouseButtons.Right)
						if ((pt = findNearestPoint(e.X, e.Y, watershedPoints)).X >= 0)
							watershedPoints.Remove(pt);
				}
			}

			// Alles neu zeichnen
			tabControlPictures.SelectedTab.Controls[0].Invalidate();
			//menuStrip.Focus();
		}

		/// <summary>
		/// Mouse Ereignis move: Show Coordinates on statusstrip
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PictBox_MouseMove(object sender, MouseEventArgs e)
		{
			if (tabControlPictures.SelectedTab == null) return;

			toolStripStatusLabelPos.ForeColor = Color.Red;
			toolStripStatusLabelPos.Text = "X= " + e.X + " Y= " + e.Y;
		}

		/// <summary>
		/// Mouse Ereignis double click, Parameter anzeigen
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PictBox_MouseDblClick(object sender, MouseEventArgs e)
		{
			if (dlg != null && !dlg.Visible)
			{
				comboBoxOperations_SelectedIndexChanged(null, null);
			}
		}

		/// <summary>
		/// Hilfsfunktion
		/// Suchen, ob ind er N‰he der Mousekoordinaten ein Kontrollpunkt gesetzt ist
		/// </summary>
		/// <param name="x">Startpunkt X</param>
		/// <param name="y">Startpunkt Y</param>
		/// <returns>Index in der Liste der Kontrollpunkte</returns>
		private Point findNearestPoint(int x, int y, List<Point> watershedPoints)
		{
			int k;

			for (int i = x - 5; i < x + 5; i++)
				for (int j = y - 5; j < y + 5; j++)
					for (k = 0; k < watershedPoints.Count; k++)
						if (watershedPoints[k].X == i && watershedPoints[k].Y == j) return watershedPoints[k];
			return new Point(-1, -1);
		}
		#endregion

		//////////////////////

		#region Tab-Pages
		/// <summary>
		/// Creates a new tab Page and loads the image in container
		/// </summary>
		/// <param name="name">name of image</param>
		/// <param name="img">image</param>
		/// <param name="dispose">if true image will be disposed after drawing</param>
		private void CreateNewTab(string name, IplImage img, bool dispose)
		{
			// Datenstrukturen / Tab-Seiten
			if (img.imageData == IntPtr.Zero) return;
			TabPage myTabPage = new TabPage(name);
			myTabPage.BorderStyle = BorderStyle.None;
			myTabPage.BackColor = Color.White;
			myTabPage.AutoScroll = true;
			PictureBox pb = new PictureBox();
			pb.BorderStyle = BorderStyle.FixedSingle;
			pb.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			pb.Image = cvlib.ToBitmap(img, dispose);
			myTabPage.Controls.Add(pb);
			tabControlPictures.TabPages.Add(myTabPage);
			tabControlPictures.SelectTab(tabControlPictures.TabCount - 1);
			tabControlPictures.SelectedTab.Controls[0].MouseMove += new System.Windows.Forms.MouseEventHandler(PictBox_MouseMove);
			tabControlPictures.SelectedTab.Controls[0].MouseClick += new System.Windows.Forms.MouseEventHandler(PictBox_MouseClick);
			tabControlPictures.SelectedTab.Controls[0].MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(PictBox_MouseDblClick);
			tabControlPictures.SelectedTab.Controls[0].Paint += new System.Windows.Forms.PaintEventHandler(PictBox_Paint);
			SetApplicationSize(img.width, img.height);
		}

		/// <summary>
		/// Tab-Seite wird geschlossen
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabSchlieﬂenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// It could be shown, this case could happen :-)
			if (tabControlPictures.SelectedTab == null) return;

			// Release Bitmap
			if (((PictureBox)tabControlPictures.SelectedTab.Controls[0]).Image != null)
				((PictureBox)tabControlPictures.SelectedTab.Controls[0]).Image.Dispose();
			
			// Remove tabpage
			tabControlPictures.TabPages.Remove(tabControlPictures.SelectedTab);
			
			// If we have a tab page, set application size to fit the image
			if (tabControlPictures.TabCount > 0)
			{
				tabControlPictures.SelectedIndex = tabControlPictures.TabPages.Count - 1;
				SetApplicationSize(((PictureBox)(tabControlPictures.SelectedTab.Controls[0])).Image.Width,
					((PictureBox)(tabControlPictures.SelectedTab.Controls[0])).Image.Height);
			}
		}

		/// <summary>
		/// toogle indicator
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OpenResultInNewTabpageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			openResultInNewTabpageToolStripMenuItem.Checked = !openResultInNewTabpageToolStripMenuItem.Checked;
		}

		/// <summary>
		/// Read current image from picturebox and convert
		/// to ipl image
		/// </summary>
		/// <param name="dispose">if true Bitmap will be disposed</param>
		/// <returns>The converted image</returns>
		private IplImage GetCurrentImage(bool dispose)
		{
			try
			{
				return cvlib.ToIplImage((Bitmap)((PictureBox)tabControlPictures.SelectedTab.Controls[0]).Image, dispose);
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		/// Read current image from Picturebox
		/// </summary>
		/// <returns>The converted image</returns>
		private Bitmap GetCurrentImage()
		{
			return (Bitmap)((PictureBox)tabControlPictures.SelectedTab.Controls[0]).Image;	
		}

		/// <summary>
		/// Set image in current tab and picturebox
		/// </summary>
		/// <param name="name">New name of the tab</param>
		/// <param name="img">Image to be shown</param>
		/// <param name="dispose">if true IplImage will be released after conversion to Bitmap</param>
		private void SetCurrentImage(string name, IplImage img, bool dispose)
		{
			if (openResultInNewTabpageToolStripMenuItem.Checked)
			{
				CreateNewTab(name, img, dispose);
			}
			else
			{
				if (img.imageData == IntPtr.Zero) return;
				((PictureBox)tabControlPictures.SelectedTab.Controls[0]).Image.Dispose();
				((PictureBox)tabControlPictures.SelectedTab.Controls[0]).Image = cvlib.ToBitmap(img, dispose);
				SetApplicationSize(img.width, img.height); // should normally be the same size
			}
		}
		#endregion

		//////////////////////

		#region Load / Save File
		/// <summary>
		///Load Image File and create image
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LoadImageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string name = "";
			OpenFileDialog ofd = new OpenFileDialog();
			IplImage img;

			// Initialisieren Datei-Dialog
			ofd.Filter = "JPEG files (jpg, jpeg, jpe)|*.jpg;*.jpeg;*jpe|" + 
									 "Windows Bitmaps (bmp, dib)|*.bmp;*.dib;|" + 
									 "Portable Network Graphics (png)|*.png;|" +
									 "Portable image format (pbm, pgm, ppm)|*.pbm;*.pgm;*.ppm|" + 
									 "Sun raster (sr, ras)|*.sr;*.ras|" +
									 "All Files (*.*)|*.jpg;*.jpeg;*jpe;*.bmp;*.dib;*.png;*.pbm;*.pgm;*.ppm;*.sr;*.ras";
			ofd.FilterIndex = 6;

			// Datei Dialog
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				try
				{
					img = cvlib.CvLoadImage(ofd.FileName, cvlib.CV_LOAD_IMAGE_COLOR);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
					return;
				}
				name = ofd.FileName.Substring(ofd.FileName.LastIndexOf("\\") + 1);
			}
			else return;

			CreateNewTab(name, img, true);
		}

		/// <summary>
		/// Save image file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveImageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (tabControlPictures.SelectedTab == null) return;

			SaveFileDialog sfd = new SaveFileDialog();
			IplImage img;

			// Initialisieren Datei-Dialog
			sfd.Filter = "JPEG files (jpg, jpeg, jpe)|*.jpg;*.jpeg;*jpe|" +
									 "Windows Bitmaps (bmp, dib)|*.bmp;*.dib;|" +
									 "Portable Network Graphics (png)|*.png;|" +
									 "Portable image format (pbm, pgm, ppm)|*.pbm;*.pgm;*.ppm|" +
									 "Sun raster (sr, ras)|*.sr;*.ras|" +
									 "All Files (*.*)|*.jpg;*.jpeg;*jpe;*.bmp;*.dib;*.png;*.pbm;*.pgm;*.ppm;*.sr;*.ras";
			sfd.FilterIndex = 2;
			sfd.AddExtension = true;
			sfd.DefaultExt = "bmp";
			sfd.FileName = "*.bmp";

			// Datei Dialog
			if (sfd.ShowDialog() == DialogResult.OK)
			{
				try
				{
					img = GetCurrentImage(false);
					cvlib.CvSaveImage(sfd.FileName, ref img);
				}
				catch(Exception ex)
				{
					MessageBox.Show("Saving of Image failed: " + ex.Message);
					return;
				}
			}
			else return;
		}
		#endregion

		#region Dialog Video Source
		/// <summary>
		/// Show dialog where the user select the video source
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void videoSourceToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			DlgVideoSrc dlg = new DlgVideoSrc();

			switch (vidoType)
			{
				case typeOfVideo.externVFW:
					dlg.radioWebcamExtern.Checked = true;
					break;
				case typeOfVideo.file:
					dlg.radioFile.Checked = true;
					dlg.textBoxFile.Enabled = true;
					dlg.buttonFile.Enabled = true;
					break;
				default:
					dlg.radioWebcamInline.Checked = true;
					break;
			}
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				if (dlg.radioWebcamInline.Checked) vidoType = typeOfVideo.inlineVFW;
				else
					if (dlg.radioWebcamExtern.Checked) vidoType = typeOfVideo.externVFW;
					else
					{
						vidoType = typeOfVideo.file;
						videoFile = dlg.textBoxFile.Text;
					}

			}
		}
		#endregion

		//////////////////////

		#region Run
		/// <summary>
		/// This is the main "Loop" where images will be directed to
		/// the appropriate image processing function. For each
		/// function the neccessary class will be first created.
		/// </summary>
		/// <param name="image">The iplImage from the picturebox in actual
		/// tab page.</param>
		private void Process(IplImage image)
		{
			try
			{
				#region Operations
				#region Perspective Transform
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Perspective Transform") == 0)
				{
					PerspectiveTransform ptr = new PerspectiveTransform(this);
					IplImage res = ptr.TransformAffine(image);
					cvlib.CvReleaseImage(ref res); // res equal image
				}
				#endregion

				#region Filter
				#region Sobel Filter
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Sobel") == 0)
				{
					ImageFilters fil = new ImageFilters(this);
					IplImage sobel = fil.Sobel(image);
					SetCurrentImage("Sobel", sobel, true);
					return;
				}
				#endregion

				#region Laplace Filter
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Laplace") == 0)
				{
					IplImage laplace;
					ImageFilters fil = new ImageFilters(this);
					laplace = fil.Laplace(image);
					SetCurrentImage("Laplace", laplace, true);
					return;
				}
				#endregion

				#region Canny Filter
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Canny") == 0)
				{
					IplImage canny;
					ImageFilters fil = new ImageFilters(this);
					canny = fil.Canny(image);
					SetCurrentImage("Edges", canny, true);
					return;
				}
				#endregion

				#region Corner Harris
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Corner Harris") == 0)
				{
					IplImage harris;
					ImageFilters fil = new ImageFilters(this);
					harris = fil.CornerHarris(image);
					SetCurrentImage("Corners", harris, true);
					return;
				}
				#endregion

				#region Dilate
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Dilate") == 0)
				{
					IplImage dilate;
					ImageFilters fil = new ImageFilters(this);
					dilate = fil.Dilate(image);
					SetCurrentImage("Dilate", dilate, true);
					return;
				}
				#endregion

				#region Erode
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Erode") == 0)
				{
					IplImage erode;
					ImageFilters fil = new ImageFilters(this);
					erode = fil.Erode(image);
					SetCurrentImage("Erode", erode, true);
					return;
				}
				#endregion
				#endregion

				#region Color Conversion
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Convert2Gray") == 0)
				{
					IplImage gray;
					ColorConvert cc = new ColorConvert(this);
					gray = cc.Convert2Gray(image);
					SetCurrentImage("Gray", gray, true);
					return;
				}
				#endregion

				#region HAAR Classifier (Faces)
				if (((string)comboBoxOperations.SelectedItem).CompareTo("HAAR Classifier (Faces)") == 0)
				{
					IplImage haarImage;
					Classifier cl = new Classifier(this);
					haarImage = cl.HaarClassifier(image);
					SetCurrentImage("Faces", haarImage, true);
					return;
				}
				#endregion

				#region Good Features to Track
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Good Features to Track") == 0)
				{
					FeatureDetector fd = new FeatureDetector(this);
					image = fd.GoodFeaturesToTrack(image);
					SetCurrentImage("Features", image, true);
					return;
				}
				#endregion

				#region Calibration
				
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Calibration (Teach)") == 0)
				{
					// new instance of calibration class
					Calibration calibClass = new Calibration(this);
					
					// optional event handler (not used here)
					calibClass.Calibrated += new Calibration.CalibratedHandler(OnCalibrationCalibrated);

					// Calculate the camera params
					// the method writes the params to a file in exe directory
					int found = 0;
					image = calibClass.FindParams(image, ref found);
					if (found == 0)
					{
						WriteLine("No images or corners found.", true, false);
						cvlib.CvReleaseImage(ref image);
						return;
					}

					WriteLine("Calculation of camerera undistort params sucessfull.", true, false);
					WriteLine("The result has been written to file: \"CamCalibParams.txt\" in your execution directory.", true, false);
					WriteLine("This file will be used for undistortion.", true, false);

					SetCurrentImage("Corners", image, true);
				}

				if (((string)comboBoxOperations.SelectedItem).CompareTo("Calibration (Correct)") == 0)
				{
					// new instance
					Calibration calibClass = new Calibration(this);

					// Undistort
					image = calibClass.Undistort(image);

					// show corners
					SetCurrentImage("Undistort", image, true);
					return;
				}
				#endregion

				#region Mathematics / Approximation
				#region Singular Value Transformation

				if (((string)comboBoxOperations.SelectedItem).CompareTo("Singular Value Transformation") == 0)
				{
					// not needed here
					cvlib.CvReleaseImage(ref image);

					MathExamples me = new MathExamples(this);
					me.SVD();
					return;
				}
				#endregion

				#region Matrix Multiplication
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Matrix Multiplication") == 0)
				{
					cvlib.CvReleaseImage(ref image);

					MathExamples me = new MathExamples(this);
					me.Mul();
					return;
				}
				#endregion

				#region FitLine
				if (((string)comboBoxOperations.SelectedItem).CompareTo("FitLine") == 0)
				{
					MathExamples me = new MathExamples(this);
					image = me.FitLine(image);
					SetCurrentImage("Line", image, true);
					return;
				}
				#endregion

				#region FitEllipse
				if (((string)comboBoxOperations.SelectedItem).CompareTo("FitEllipse") == 0)
				{
					MathExamples me = new MathExamples(this);
					image = me.FitEllipse(image);
					SetCurrentImage("Ellipse", image, true);
					return;
				}
				#endregion
				#endregion

				#region Hough
				#region Hough Lines
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Hough Lines") == 0)
				{
					HoughTransform h = new HoughTransform(this);
					image = h.HoughLines(image);
					SetCurrentImage("Hough Lines", image, true);
					return;
				}
				#endregion

				#region Hough Circles
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Hough Circles") == 0)
				{
					HoughTransform h = new HoughTransform(this);
					image = h.HoughCircles(image);
					SetCurrentImage("Hough Circles", image, true);
					return;
				}
				#endregion
				#endregion

				#region Segment
				#region Pyramid Segmentation
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Pyramid Segmentation") == 0)
				{
					Segmentation sg = new Segmentation(this);
					image = sg.PyrSegment(image);
					SetCurrentImage("Pyramid", image, true);
					return;
				}
				#endregion

				#region Watershed Transformation
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Watershed Transformation") == 0)
				{
					Segmentation sg = new Segmentation(this);
					image = sg.Watershed(image);
					SetCurrentImage("Wshed", image, true);
					return;
				}
				#endregion
				#endregion

				#region Find Contours
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Find Contours") == 0)
				{
					Conturfinder cf = new Conturfinder(this);
					image = cf.FindContoursCvLib(image);
					SetCurrentImage("Contours", image, true);
					return;
				}
				#endregion

				#region Histograms
				#region Histogram Lines
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Scanline Histograms") == 0)
				{
					Histograms h = new Histograms(this);
					image = h.HistLine(image);
					SetCurrentImage("Hist", image, true);
					return;
				}
				#endregion

				#region Hue-Sat Histogram
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Hue-Sat Histogram") == 0)
				{
					Histograms h = new Histograms(this);
					h.HSHistogram(image);
					return;
				}
				#endregion

				#region Equalize Histogram
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Equalize Histogram") == 0)
				{
					Histograms h = new Histograms(this);
					image = h.EqualizeHistogram(image);
					SetCurrentImage("Equalized Histogram Image", image, true);
				}
				#endregion
				#endregion

				#region Random
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Random") == 0)
				{
					cvlib.CvReleaseImage(ref image);
					Rand rnd = new Rand(this);
					WriteLine((rnd.GetDouble()).ToString(), true, false);
					return;
				}
				#endregion

				#region Moments
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Moments") == 0)
				{
					Features f = new Features(this);
					f.Moments(image);
					return;
				}
				#endregion

				#region Flood Fill
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Flood Fill") == 0)
				{
					Drawings d = new Drawings(this);
					image = d.FloodFill(image);
					SetCurrentImage("flood&fill", image, true);
				}
				#endregion

				#region Template Matching
				if (((string)comboBoxOperations.SelectedItem).CompareTo("Template Matching") == 0)
				{
					Matching m = new Matching(this);
					image = m.TemplateMatching(image);
					SetCurrentImage("Templates", image, true);
					return;
				}
				#endregion
				#endregion
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		private void MenuItemProcess_Click(object sender, EventArgs e)
		{
			IplImage image;

			if (timerGrab.Enabled)
			{
				if (executeToolStripMenuItem.Text.CompareTo("Run") == 0)
				{
					executeToolStripMenuItem.Text = "Stop";
				}
				else
				{
					executeToolStripMenuItem.Text = "Run";
				}
			}
			else
			{

				#region Get Image
				if (tabControlPictures.SelectedTab == null) return;

				try
				{
					image = GetCurrentImage(false);
					cvlib.CvSetErrMode(cvlib.CV_ErrModeParent);
				}
				catch { return; }
				#endregion

				try
				{
					Process(image);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Process: " + ex.Message, "Anwendungsfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
		#endregion

		#region Dialogbox Operations
		/// <summary>
		/// Within this method call a parameter dialog for the several
		/// image processing funtions will be dynamic created. Also the
		/// controls are added dynamically. If you interested hoe to
		/// build such a dialog look at Dialogs/DialogParams.cs
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboBoxOperations_SelectedIndexChanged(object sender, EventArgs e)
		{
			#region Defaults
			// After a user changed the the expected image processing
			// operation in the combox the event will arrive here.
			// some defauts are restored and position of the dialog determind.

			// clear watershed points and set draw to false
			drawWatershedPoints = false;
			watershedPoints.Clear();

			// initial Location of the dialog
			this.Location = new Point(0, 0);
			if (dlg != null) dlg.Close();
			dlg = new DlgParams();
			dlg.BackColor = Color.FromArgb(244, 239, 240);
			dlg.Icon = this.Icon;
			dlg.StartPosition = FormStartPosition.Manual;

			// set right the main window
			dlg.Location = new Point(this.Location.X + this.Size.Width, this.Location.Y);

			// clear text output window
			textBox.Clear();
			#endregion

			#region Create Dialog's
			#region Perspective Transform
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Perspective Transform") == 0)
			{
				WriteLine("Example outputs point coordinates before and after transformation. Additional the transformation matrix will be shown.", true, false);
				WriteLine("Push the red 'Run' button to start the image processing.", true, false);
				return;
			}
			#endregion

			#region Dilate Filter
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Dilate") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				return;
			}
			#endregion

			#region Erode Filter
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Erode") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				return;
			}
			#endregion

			#region Sobel Filter
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Sobel") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				dlg.AddTrackbar("X-Derivative", 0, 0, 2, 1, 1);
				dlg.AddTrackbar("Y-Derivative", 1, 0, 2, 1, 1);
				dlg.AddComboBox("Mask Size", 2, new string[] { "1", "3", "5", "7" });
				dlg.Show();
				return;
			}
			#endregion

			#region Laplace Filter
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Laplace") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				dlg.AddComboBox("Mask Size", 0, new string[] { "1", "3", "5", "7" });
				dlg.Show();
				return;
			}
			#endregion

			#region Canny Filter
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Canny") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				dlg.AddTrackbar("Lower Treshold", 0, 0, 255, 10, 10);
				dlg.AddTrackbar("High Treshold", 1, 0, 255, 10, 40);
				dlg.AddComboBox("Mask Size", 2, new string[] { "3", "5", "7" });
				dlg.Show();
				return;
			}
			#endregion

			#region Corner Harris
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Corner Harris") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				WriteLine("- There is no explicit maximum search for corners. Only a threshold with c = 1 for response > 0.001 is used.", true, false);
				WriteLine("- The neighborhood size is choosen to 3. Mask size for sobel operator is 3 and harris parameter is 0.04.", true, false);
			}
			#endregion

			#region HAAR Classifier
			if (((string)comboBoxOperations.SelectedItem).CompareTo("HAAR Classifier (Faces)") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to find faces.", true, false);
				return;
			}
			#endregion

			#region Convert to Gray
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Convert2Gray") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				return;
			}
			#endregion

			#region Calibration
			#region Calibration (Teach)
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Calibration (Teach)") == 0)
			{
				WriteLine("- OpenCV Camera calibration Example. (Teaching).", true, false);
				WriteLine("- For the calibration you should prepare some chess board images in a separate folder.", true, false);
				WriteLine("1. Push the red 'Run' button to start the image processing.", true, false);
				WriteLine("2. In the folder dialog select the folder with the teach-images.", true, false);
				WriteLine("3. The camera params will now be written in a file in the exe-folder.", true, false);
				dlg.AddTrackbar("Inner Corn. x", 0, 1, 10, 1, 8);
				dlg.AddTrackbar("Inner Corn. y", 1, 1, 10, 1, 6);
				dlg.Show();
				return;
			}
			#endregion

			#region Calibration (Correct)
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Calibration (Correct)") == 0)
			{
				WriteLine("- Open Cv Camera calibration. (Undistort).", true, false);
				WriteLine("1. Load any Image you want to undistort. (ofcourse it should be taken with the same camera that has been used for teaching)", true, false);
				WriteLine("2. The previously generated file from teaching will be used and must be available in program-exe-folder.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				return;
			}
			#endregion
			#endregion

			#region Hough
			#region Hough Lines
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Hough Lines") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				dlg.AddTrackbar("Lower Treshold", 0, 0, 255, 10, 100);
				dlg.AddTrackbar("High Treshold", 1, 0, 255, 10, 100);
				dlg.AddComboBox("Method", 2, new string[] { "Probabilistic", "Standard" });
				dlg.Show();
				return;
			}
			#endregion

			#region Hough Circles
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Hough Circles") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				dlg.AddTrackbar("Accu Resolution", 0, 1, 10, 1, 4);
				dlg.AddTrackbar("MinDist btw circles", 1, 1, 500, 10, 1);
				dlg.AddTrackbar("High Treshold for Canny (lower twice smaller)", 2, 1, 255, 10, 36);
				dlg.AddTrackbar("Accu Treshold for Center", 3, 1, 255, 10, 115);
				dlg.AddTrackbar("Min Radius", 4, 1, 500, 10, 25);
				dlg.AddTrackbar("Max Radius", 5, 1, 1000, 10, 790);
				dlg.Show();
				return;
			}
			#endregion
			#endregion

			#region Segmentation
			#region Pyramid Segmentation
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Pyramid Segmentation") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				dlg.AddTrackbar("Treshold 1", 0, 0, 255, 10, 100);
				dlg.AddTrackbar("Treshold 2", 1, 0, 255, 10, 100);
				dlg.AddTrackbar("Level", 2, 1, 10, 1, 5);
				dlg.Show();
				return;
			}
			#endregion

			#region Watershed Transformation
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Watershed Transformation") == 0)
			{
				drawWatershedPoints = true;
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("1. Click in the image to set some seed points.", true, false);
				WriteLine("2. Right mouse button clears a point.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				WriteLine("4. The Number of segmented regions are equal to the number of your choosen seed points.", true, false);
			}
			#endregion
			#endregion

			#region Find Contours
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Find Contours") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				dlg.AddTrackbar("Lower Treshold", 0, 0, 255, 10, 10);
				dlg.AddTrackbar("High Treshold", 1, 0, 255, 10, 40);
				dlg.Show();
				return;
			}
			#endregion

			#region Singular Value Transformation

			if (((string)comboBoxOperations.SelectedItem).CompareTo("Singular Value Transformation") == 0)
			{
				WriteLine("Example demonstrates the usage of Arrays and CvMath. Output is a SVD", true, false);
				WriteLine("Push the red 'Run' button to start the image processing.", true, false);
				return;
			}
			#endregion

			#region Matrix Multiplication
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Matrix Multiplication") == 0)
			{
				WriteLine("Example demonstrates the usage of Arrays and CvMath. Out is the result of the Matrix mul", true, false);
				WriteLine("Push the red 'Run' button to start the image processing.", true, false);
				return;
			}
			#endregion

			#region Random
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Random") == 0)
			{

				WriteLine("Example generates some random values.", true, false);
				WriteLine("Push the red 'Run' button to start.", true, false);
				return;
			}
			#endregion

			#region Moments
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Moments") == 0)
			{
				WriteLine("Calculation of several Image Moments.", true, false);
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				return;
			}
			#endregion

			#region Flood Fill
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Flood Fill") == 0)
			{
				WriteLine("Flood Fill Algorithm. Seed Point is fixed x=100, y=100. Take care image size is bigger than the seed point.", true, false);
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
			}
			#endregion

			#region Approximation
			#region FitEllipse
			if (((string)comboBoxOperations.SelectedItem).CompareTo("FitEllipse") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				dlg.AddComboBox("Points are", 0, new string[] { "black", "white" });
				dlg.Show();
				return;
			}
			#endregion

			#region FitLine
			if (((string)comboBoxOperations.SelectedItem).CompareTo("FitLine") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				dlg.AddComboBox("Points are", 0, new string[] { "black", "white" });
				dlg.Show();
				return;
			}
			#endregion
			#endregion

			#region Good Features to Track
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Good Features to Track") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start the image processing.", true, false);
				return;
			}
			#endregion

			#region Histogram
			#region OpenCv Histogram Example
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Hue-Sat Histogram") == 0)
			{
				WriteLine("Demonstration of external openCV window by creating a hue - saturation histogram (see openCV docu for futher details).", true, false);
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start processing.", true, false);
				return;
			}
			#endregion

			#region OpenCv Histogram Example
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Equalize Histogram") == 0)
			{
				WriteLine("The image will be converted to gray and then the gray values expanded for optimal filling the histogram.", true, false);
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start processing.", true, false);
				return;
			}
			#endregion

			#region Nice Histogram
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Scanline Histograms") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start processing.", true, false);
				dlg.AddTrackbar("Position of Scanline in percent of image height (bottom up)", 0, 0, 100, 10, 50);
				dlg.AddCheckBox("Show Red Channel (Red Line)", 1, true);
				dlg.AddCheckBox("Show Green Channel (Green Line)", 2, true);
				dlg.AddCheckBox("Show Blue Channel (Blue Line)", 3, true);
				dlg.AddCheckBox("Show Hue (Cyan)", 4, true);
				dlg.AddCheckBox("Show moving Hue Mean (Yellow)", 5, true);
				dlg.AddCheckBox("Euclidian distance between color channels (Magneta)", 6, true);
				dlg.AddCheckBox("Show ScanLine (White dotted)", 7, true);
				dlg.Show();
				return;
			}
			#endregion
			#endregion

			#region Template Matching
			if (((string)comboBoxOperations.SelectedItem).CompareTo("Template Matching") == 0)
			{
				WriteLine("1. Load a Image if not already done.", true, false);
				WriteLine("2. Push the red 'Run' button to start processing.", true, false);
				WriteLine("3. In the file selection dialog select the template. This must be smaller than 1/4 width and height.", true, false);
				WriteLine("4. With the ruler select the number of expected objects in the image.", true, false);
				dlg.AddTrackbar("Number of Objects expected", 0, 1, 40, 1, 10);
				dlg.Show();
				return;
			}
			#endregion
			#endregion
		}
		#endregion

		//////////////////////
		
		#region Video Start / Stop / Timer
		/// <summary>
		/// The video Start Button event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void VideoStartStopToolStripMenuItem_Click(object sender, EventArgs e)
		{
			double w, h;

			// There are to modes with one button
			// video start / video stop
			// selected by Tool strip text
			if (videoToolStripMenuItem1.Text.CompareTo("Start Video") == 0)
			{
				// only in the case that ... somthing is wrong
				if (tabControlPictures.SelectedTab == null) return;

				// set new text
				videoToolStripMenuItem1.Text = "Stop Video";

				// select video modus by video type
				switch (vidoType)
				{
					case typeOfVideo.inlineVFW:
						#region Inline Video
						// create the video capture
						videoCapture = cvlib.CvCreateCameraCapture(0);

						// check for valid
						if (videoCapture.ptr == IntPtr.Zero)
						{
							MessageBox.Show("Creation of Video Capture failed.");
							return;
						}

						// a little thing good to know
						// We need to grab a frame first to get the info !!
						cvlib.CvQueryFrame(ref videoCapture);
						w = cvlib.cvGetCaptureProperty(videoCapture, cvlib.CV_CAP_PROP_FRAME_WIDTH);
						h = cvlib.cvGetCaptureProperty(videoCapture, cvlib.CV_CAP_PROP_FRAME_HEIGHT);
						SetApplicationSize((int)w, (int)h);
						break;
						#endregion
					case typeOfVideo.externVFW:
						#region Extern Window
						videoCapture = cvlib.CvCreateCameraCapture(0);
						extWindowHandle = cvlib.CvNamedWindow("Capture", 1);
						if (videoCapture.ptr == IntPtr.Zero)
						{
							MessageBox.Show("Creation of Video Capture failed.");
							return;
						}
						break;
						#endregion
					case typeOfVideo.file:
						#region File Capture
						// in this case we create a file capture
						videoCapture = cvlib.CvCreateFileCapture(videoFile);

						// check if video file is valid e.g. format
						if (videoCapture.ptr == IntPtr.Zero)
						{
							MessageBox.Show("Creation of File Capture failed.");
							return;
						}

						// how many frames consit that video?
						nFrames = (int)cvlib.cvGetCaptureProperty(videoCapture, cvlib.CV_CAP_PROP_FRAME_COUNT);
						fcnt = 0;

						// We need to grab a frame first to get right info
						cvlib.CvQueryFrame(ref videoCapture);
						w = cvlib.cvGetCaptureProperty(videoCapture, cvlib.CV_CAP_PROP_FRAME_WIDTH);
						h = cvlib.cvGetCaptureProperty(videoCapture, cvlib.CV_CAP_PROP_FRAME_HEIGHT);
						SetApplicationSize((int)w, (int)h);
						break;
						#endregion
					default:
						break;
				}

				// make sure that video will be shown in current tab
				openResultInNewTabpageToolStripMenuItem.Checked = false;
				// preventation
				openResultInNewTabpageToolStripMenuItem.Enabled = false;

				if (dlg != null)
				{
					dlg.StartPosition = FormStartPosition.Manual;
					dlg.Location = new Point(this.Location.X + this.Size.Width, this.Location.Y);
				}

				// start video grabbing by timer interval
				timerGrab.Interval = 42;
				timerFPS.Interval = 1100;
				timerGrab.Enabled = true;
				timerFPS.Enabled = true;
			}
			else
			{
				videoToolStripMenuItem1.Text = "Start Video";

				timerGrab.Enabled = false;
				timerFPS.Enabled = false;
				openResultInNewTabpageToolStripMenuItem.Checked = true;
				openResultInNewTabpageToolStripMenuItem.Enabled = true;

				if (videoCapture.ptr != IntPtr.Zero)
				{
					cvlib.CvReleaseCapture(ref videoCapture);
					videoCapture.ptr = IntPtr.Zero;
				}
				if (extWindowHandle != 0)
				{
					cvlib.CvDestroyWindow("Capture");
					extWindowHandle = 0;
				}
			}
		}
	
		/// <summary>
		/// Timer event ocured
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TimerGrab_Tick(object sender, EventArgs e)
		{
			IplImage imgIpl;

			// when somthing went wrong...
			if (tabControlPictures.SelectedTab == null) return;

			// select what do do by video type
			switch (vidoType)
			{
				case typeOfVideo.inlineVFW:
					#region Inline Video
					// release Picture Box Image
					((PictureBox)tabControlPictures.SelectedTab.Controls[0]).Image.Dispose();
					
					// Query new image from cam and flip
					imgIpl = cvlib.CvQueryFrame(ref videoCapture);
					cvlib.CvFlip(ref imgIpl, ref imgIpl, 0);
					SetCurrentImage("Live Video", imgIpl, false);
					break;
					#endregion
				case typeOfVideo.file:
					#region File
					// we need to count the frames or we recive error after last frame
					// so we set pos back and run video in a loop
					if (fcnt == nFrames - 1)
					{
						cvlib.CvSetCaptureProperty(ref videoCapture, cvlib.CV_CAP_PROP_POS_FRAMES, 0);
						fcnt = 0;
					}
					else fcnt++;

					// Query new image from cam and flip
					imgIpl = cvlib.CvQueryFrame(ref videoCapture);
					cvlib.CvFlip(ref imgIpl, ref imgIpl, 0);
					SetCurrentImage("Live Video", imgIpl, false);
					break;
					#endregion
				case typeOfVideo.externVFW:
					#region External Window
					imgIpl = cvlib.CvQueryFrame(ref videoCapture);
					cvlib.CvShowImage("Capture", ref imgIpl);
					break;
					#endregion
				default:
					break;
			}
			fps++;
		}

		private void TimerFPS_Tick(object sender, EventArgs e)
		{
			toolStripStatusLabelFrameRate.ForeColor = Color.Red;
			toolStripStatusLabelPos.Text = "Frame Rate: " + fps.ToString() + " Fps";
			fps = 0;
		}
		#endregion

		//////////////////////

		#region Calib-Eventhandler
		/// <summary>
		/// Example Callback used for camera calibration
		/// </summary>
		/// <param name="success">calibration of actual image successful</param>
		/// <param name="n">Number of image</param>
		private void OnCalibrationCalibrated(bool sucess, int n)
		{
			WriteLine("Image " + n + " calibrated " + (sucess ? "sucessfull" : "not sucessfull"), true, false);
		}
		#endregion

		#region Text Area output
		/// <summary>
		/// Write a single string to the text output control
		/// </summary>
		/// <param name="s">input string</param>
		/// <param name="crlf">insert carrige return line feed</param>
		/// <param name="date">prints date and time at begin of line</param>
		public void WriteLine(string s, bool crlf, bool date)
		{
			if ((s.Length + textBox.TextLength) > textBox.MaxLength)
			{
				textBox.Clear();
			}
			if (!crlf && !date)
				textBox.AppendText(s);
			if (!crlf && date)
				textBox.AppendText(DateTime.Now.ToString() + ">> " + s);
			if (crlf && !date)
				textBox.AppendText(s + "\r\n");
			if (crlf && date)
				textBox.AppendText(DateTime.Now.ToString() + ">> " + s + "\r\n");
		}

		private void clearOutputToolStripMenuItem_Click(object sender, EventArgs e)
		{
			textBox.Clear();
		}
		#endregion

		#region Set Application Pos and Size
		/// <summary>
		/// adjust applications width and height by consideration screen size
		/// </summary>
		/// <param name="w">image width</param>
		/// <param name="h">image height</param>
		private void SetApplicationSize(int w, int h)
		{
			PictureBox pb = (PictureBox)(tabControlPictures.SelectedTab.Controls[0]);
			Size ms = SystemInformation.PrimaryMonitorSize;
			Size maxBounds = new Size(ms.Width - dlg.Width, ms.Height);
			pb.Width = w;
			pb.Height = h;

			int bH = this.Width - this.ClientSize.Width + tabControlPictures.Width - 
				tabControlPictures.SelectedTab.Width;
			if (pb.Width + bH > maxBounds.Width) this.Width = maxBounds.Width;
			else 
				if (pb.Width + bH < 650) this.Width = 650;
				else this.Width = pb.Width + bH;

			int bV = this.Height - this.ClientSize.Height + tabControlPictures.Height -
				tabControlPictures.SelectedTab.Height;
			if (pb.Height + bV + statusStrip.Height + menuStrip.Height +
				textBox.Height > maxBounds.Height) this.Height = maxBounds.Height;
			else this.Height = pb.Height + bV + statusStrip.Height + 
				menuStrip.Height + textBox.Height;
		}
		#endregion
	}
}
