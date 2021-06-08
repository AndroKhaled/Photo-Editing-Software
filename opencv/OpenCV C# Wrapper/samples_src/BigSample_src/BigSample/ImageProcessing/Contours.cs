//**********************************************************
// File name: $ Contours.cs $
// Author:		$ Heiko Kieﬂling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Example code shows how to implement contour processing
//						by using OpenCV lib.
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
using System.Drawing;
using System.Windows.Forms;

namespace CvExample
{
	public class Conturfinder
	{
		private MainForm form;

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="form">main form</param>
		public Conturfinder(MainForm form)
		{
			this.form = form;
		}

		/// <summary>
		/// Find and approximate contours
		/// </summary>
		/// <param name="image">input image</param>
		/// <returns>result image</returns>
		public IplImage FindContoursCvLib(IplImage image)
		{
			// contours works only with gray/binary images
			IplImage gray;

			/// create gray scale image
			gray = cvlib.CvCreateImage(cvlib.CvGetSize(ref image), (int)cvlib.IPL_DEPTH_8U, 1);
			/// do color conversion
			cvlib.CvCvtColor(ref image, ref gray, cvlib.CV_BGR2GRAY);
			cvlib.CvReleaseImage(ref image);
			image = gray;


			// destination image and mem storage
			IplImage dst = cvlib.CvCreateImage(cvlib.CvGetSize(ref image), 8, 3);
			CvMemStorage storage = cvlib.CvCreateMemStorage(0);

			// perform canny
			cvlib.CvCanny(ref image, ref image, form.dlg.GetP(0).i, form.dlg.GetP(1).i, 3);


			// Perform contour finder
			GCHandle h;
			CvSeq s = new CvSeq();
			IntPtr p = cvtools.ConvertStructureToPtr(s, out h);
			cvlib.CvFindContours(ref image, ref storage, ref p, Marshal.SizeOf(typeof(CvContour)),
											cvlib.CV_RETR_TREE, cvlib.CV_CHAIN_APPROX_SIMPLE, new CvPoint(0, 0));

			CvSeq contours = cvlib.CvApproxPoly(p, Marshal.SizeOf(typeof(CvContour)), ref storage, cvlib.CV_POLY_APPROX_DP, 1, 1);


			// draw black background
			cvlib.CvSetZero(ref dst);

			// draw the countours
			cvlib.CvDrawContours(ref dst, ref contours, new CvScalar(255, 0, 0, 0), new CvScalar(0, 255, 0, 0), 5, 1, 8, new CvPoint(0, 0));

			cvtools.ReleaseHandel(h);

			cvlib.CvReleaseImage(ref image);

			return dst;
		}
	}
}
