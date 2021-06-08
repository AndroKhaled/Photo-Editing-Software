//**********************************************************
// File name: $ Calibration.cs $
// Author:		$ Heiko Kieﬂling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Example code shows how to calculate Moments by
//						using openCv lib.
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

namespace CvExample
{
	public class Features
	{
		private MainForm form;

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="form">main form</param>
		public Features(MainForm form)
		{
			this.form = form;
		}

		/// <summary>
		/// Calculation Example for Moments and Hu-Moments
		/// </summary>
		/// <param name="image">input image</param>
		public void Moments(IplImage image)
		{
			// color conversion if image hasnt one channel
			IplImage gray = cvlib.CvCreateImage(new CvSize(image.width, image.height), (int)cvlib.IPL_DEPTH_8U, 1);
			cvlib.CvCvtColor(ref image, ref gray, cvlib.CV_BGR2GRAY);

			// calculate all possible moments
			// take care that order follows the rule:
			// ox + oy <= 3, ox | oy >=0, ox <=3, oy <=3
			CvMoments moments = new CvMoments();
			CvHuMoments humoments = new CvHuMoments();
			cvlib.CvMoments(ref gray, ref moments, 0);
			double sm = cvlib.CvGetSpatialMoment(ref moments, 1, 1);
			double cm = cvlib.CvGetCentralMoment(ref moments, 1, 1);
			double ncm = cvlib.CvGetNormalizedCentralMoment(ref moments, 1, 1);
			cvlib.CvGetHuMoments(ref moments, ref humoments);

			// print moments
			form.WriteLine("m00 = " + moments.m00, true, false);
			form.WriteLine("m01 = " + moments.m01, true, false);
			form.WriteLine("m02 = " + moments.m02, true, false);
			form.WriteLine("m03 = " + moments.m03, true, false);
			form.WriteLine("m10 = " + moments.m10, true, false);
			form.WriteLine("m11 = " + moments.m11, true, false);
			form.WriteLine("m12 = " + moments.m12, true, false);
			form.WriteLine("m20 = " + moments.m20, true, false);
			form.WriteLine("m21 = " + moments.m21, true, false);
			form.WriteLine("m30 = " + moments.m30, true, false);
			form.WriteLine("mu02 = " + moments.mu02, true, false);
			form.WriteLine("mu03 = " + moments.mu03, true, false);
			form.WriteLine("mu11 = " + moments.mu11, true, false);
			form.WriteLine("mu12 = " + moments.mu12, true, false);
			form.WriteLine("mu20 = " + moments.mu20, true, false);
			form.WriteLine("mu21 = " + moments.mu21, true, false);
			form.WriteLine("mu30 = " + moments.mu30, true, false);
			form.WriteLine("sm = " + sm, true, false);
			form.WriteLine("cm = " + cm, true, false);
			form.WriteLine("ncm = " + ncm, true, false);
			form.WriteLine("hu1 = " + humoments.hu1, true, false);
			form.WriteLine("hu2 = " + humoments.hu2, true, false);
			form.WriteLine("hu3 = " + humoments.hu3, true, false);
			cvlib.CvReleaseImage(ref image);
			cvlib.CvReleaseImage(ref gray);
		}
	}
}
