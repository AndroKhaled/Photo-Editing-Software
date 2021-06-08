//**********************************************************
// File name: $ Hough.cs $
// Author:		$ Heiko Kieﬂling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Example code shows how to use cvlib dll.
//						Hough Transformation for lines and circles.
//						Mentioned i wasnt verry sucessfull with finding good params for circles:-)
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
using System.Collections.Generic;
using System.Text;
using openCV;
using System.Runtime.InteropServices;

namespace CvExample
{
	public class HoughTransform
	{
		private MainForm form;

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="form">main form</param>
		public HoughTransform(MainForm form)
		{
			this.form = form;
		}

		/// <summary>
		/// Finds all hough lines in a image
		/// </summary>
		/// <param name="image">input image</param>
		/// <returns>input image with drawn hough lines</returns>
		public IplImage HoughLines(IplImage image)
		{	
			/// create gray scale image
			IplImage gray = cvlib.CvCreateImage(cvlib.CvGetSize(ref image), (int)cvlib.IPL_DEPTH_8U, 1);
			
			/// color conversion
			cvlib.CvCvtColor(ref image, ref gray, cvlib.CV_BGR2GRAY);

			IplImage dst = cvlib.CvCreateImage(cvlib.CvGetSize(ref gray), 8, 1);
			IplImage color_dst = cvlib.CvCreateImage(cvlib.CvGetSize(ref gray), 8, 3);
			CvMemStorage storage = cvlib.CvCreateMemStorage(0);
			CvSeq lines;
			int i;
			cvlib.CvCanny(ref gray, ref dst, form.dlg.GetP(0).i, form.dlg.GetP(1).i, 3);
			cvlib.CvCvtColor(ref dst, ref color_dst, cvlib.CV_GRAY2BGR);

			if (form.dlg.GetP(2).s.CompareTo("Standard") == 0)
			{
				lines = cvlib.CvHoughLines2(ref dst, storage.ptr, cvlib.CV_HOUGH_STANDARD, 1, Math.PI / 180, 20, 0, 0);

				for (i = 0; i < Math.Min(lines.total, 100); i++)
				{
					float[] line = new float[2];
					Marshal.Copy(cvlib.CvGetSeqElem(ref lines, i), line, 0, 2);
					float rho = line[0];
					float theta = line[1];
					CvPoint pt1, pt2;
					double a = Math.Cos(theta), b = Math.Sin(theta);
					double x0 = a * rho, y0 = b * rho;
					pt1.x = (int)Math.Round(x0 + 1000 * (-b));
					pt1.y = (int)Math.Round(y0 + 1000 * (a));
					pt2.x = (int)Math.Round(x0 - 1000 * (-b));
					pt2.y = (int)Math.Round(y0 - 1000 * (a));
					cvlib.CvLine(ref color_dst, pt1, pt2, cvlib.CV_RGB(255, 0, 0), 1, 8, 0);
				}
			}
			else
			{
				lines = cvlib.CvHoughLines2(ref dst, storage.ptr, cvlib.CV_HOUGH_PROBABILISTIC, 1, Math.PI / 180, 20, 10, 10);
				CvPoint line0, line1;
				IntPtr p0, p1;
				for (i = 0; i < lines.total; i++)
				{
					p0 = cvlib.CvGetSeqElem(ref lines, i);
					line0 = (CvPoint)Marshal.PtrToStructure(p0, typeof(CvPoint));
					int adress = p0.ToInt32();
					adress += 8;
					p1 = new IntPtr(adress);
					line1 = (CvPoint)Marshal.PtrToStructure(p1, typeof(CvPoint));
					cvlib.CvLine(ref color_dst, line0, line1, cvlib.CV_RGB(255, 0, 0), 1, 8, 0);
				}
			}
			cvlib.CvReleaseImage(ref dst);
			cvlib.CvReleaseImage(ref gray);
			return color_dst;
		}

		/// <summary>
		/// Finds all hough circels in image
		/// </summary>
		/// <param name="image">input image</param>
		/// <returns>input image with drawn hough circels</returns>
		public IplImage HoughCircles(IplImage image)
		{
			IplImage gray = cvlib.CvCreateImage(cvlib.CvGetSize(ref image), 8, 1);;
			cvlib.CvCvtColor(ref image, ref gray, cvlib.CV_BGR2GRAY);
			CvMemStorage storage = cvlib.CvCreateMemStorage(0);

			cvlib.CvSmooth(ref gray, ref gray, cvlib.CV_GAUSSIAN, 9, 9, 0, 0); // smooth it, otherwise a lot of false circles may be detected
			CvSeq circles = cvlib.CvHoughCircles(ref gray, storage.ptr, cvlib.CV_HOUGH_GRADIENT, 
				form.dlg.GetP(0).i, form.dlg.GetP(1).i, form.dlg.GetP(2).i, form.dlg.GetP(3).i, form.dlg.GetP(4).i, form.dlg.GetP(5).i);
			
			for (int i = 0; i < circles.total; i++)
			{
				float[] p = new float[3];
				Marshal.Copy(cvlib.CvGetSeqElem(ref circles, i), p, 0, 3);
				cvlib.CvCircle(ref image, cvlib.CvPoint((int)Math.Round(p[0]), (int)Math.Round(p[1])), 3, cvlib.CV_RGB(0, 255, 0), -1, 8, 0);
				cvlib.CvCircle(ref image, cvlib.CvPoint((int)Math.Round(p[0]), (int)Math.Round(p[1])), (int)Math.Round(p[2]), cvlib.CV_RGB(255, 0, 0), 3, 8, 0);
			}
			
			cvlib.CvReleaseImage(ref gray);
			return image;
		}
	}
}
