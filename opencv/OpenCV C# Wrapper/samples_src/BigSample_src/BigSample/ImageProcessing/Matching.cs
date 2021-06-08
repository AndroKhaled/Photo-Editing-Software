//**********************************************************
// File name: $ Math.cs $
// Author:		$ Heiko Kieﬂling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Example code shows how to use cvlib dll.
//						Template Matching. A template must be smaller than
//						image or program crash.
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
//**********************************************************

using System;
using System.Collections.Generic;
using System.Text;
using openCV;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CvExample
{
	public class MyRectangle
	{
		float count;
		public float Count
		{
			get { return count; }
			set { count = value; }
		}

		Rectangle rect;
		public Rectangle Rect
		{
			get { return rect; }
			set { rect = value; }
		}

		public MyRectangle(int x, int y, int w, int h, float count)
		{
			rect = new Rectangle(x, y, w, h);
			this.count = count;
		}
	}

	public class Matching
	{
		private MainForm form;

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="form">main form</param>
		public Matching(MainForm form)
		{
			this.form = form;
		}

		/// <summary>
		/// Example for template matching
		/// </summary>
		/// <param name="image">input image</param>
		/// <returns></returns>
		public IplImage TemplateMatching(IplImage image)
		{
			string path = null;
			float res;
			IplImage imggray;
			IplImage template_tmp;
			IplImage template;
			IplImage result;
			List<MyRectangle> objectsFound = new List<MyRectangle>(10);

			OpenFileDialog fd = new OpenFileDialog();
			fd.DefaultExt = "*.* (all Files)|*.*";
			fd.Title = "Select Template Image";
			if (fd.ShowDialog() == DialogResult.OK)
			{
				path = fd.FileName;
			}

			template_tmp = cvlib.CvLoadImage(path, cvlib.CV_LOAD_IMAGE_COLOR);
			Application.DoEvents();


			int w = template_tmp.width;
			int h = template_tmp.height;
			int ox = template_tmp.width >> 1;
			int oy = template_tmp.height >> 1;
			int tw = template_tmp.width;
			int th = template_tmp.height;

			imggray = cvlib.CvCreateImage(new CvSize(image.width, image.height), 8, 1);
			template = cvlib.CvCreateImage(new CvSize(template_tmp.width, template_tmp.height), 8, 1);
			result = cvlib.CvCreateImage(new CvSize(image.width - w + 1, image.height - h + 1), (int)cvlib.IPL_DEPTH_32F, 1);
			cvlib.CvCvtColor(ref image, ref imggray, cvlib.CV_BGR2GRAY);
			cvlib.CvCvtColor(ref template_tmp, ref template, cvlib.CV_BGR2GRAY);
			cvlib.CvMatchTemplate(ref imggray, ref template, ref result, (int)cvlib.CV_TM_SQDIFF);
			for (int y = 0; y < result.height; y++)
				for (int x = 0; x < result.width; x++)
				{
					res = (float)Marshal.PtrToStructure(new IntPtr(result.imageData.ToInt32() + result.widthStep * y + 4 * x), typeof(float));
					if (objectsFound.Count < 2)
					{
						MyRectangle rc = new MyRectangle(x, y, w, h, res);
						objectsFound.Add(rc);
						objectsFound.Sort(Compare);
					}
					else
					{
						if (res < objectsFound[objectsFound.Count - 1].Count)
						{
							MyRectangle rc = new MyRectangle(x, y, w, h, res);
							if (objectsFound.Count > form.dlg.GetP(0).i)
								objectsFound.RemoveAt(objectsFound.Count - 1);
							objectsFound.Add(rc);
							objectsFound.Sort(Compare);
						}
					}
				}

			foreach (MyRectangle rect in objectsFound)
				cvlib.CvRectangle(ref image, new CvPoint(rect.Rect.X, rect.Rect.Y), new CvPoint(rect.Rect.Right, rect.Rect.Bottom), new CvScalar(255, 0, 0, 0), 1, 8, 0);
	
			cvlib.CvReleaseImage(ref result);
			cvlib.CvReleaseImage(ref imggray);
			cvlib.CvReleaseImage(ref template);
			cvlib.CvReleaseImage(ref template_tmp);

			return image;
		}

		/// <summary>
		/// Comparer function
		/// </summary>
		/// <param name="a">value a to compare with</param>
		/// <param name="b">value b</param>
		/// <returns>-1 if a smaller b, 0 if a == b, 1 if a larger b</returns>
		private int Compare(MyRectangle a, MyRectangle b)
		{
			if (a.Count == b.Count) return 0;
			else return a.Count > b.Count ? 1 : -1;
		}
	}
}
