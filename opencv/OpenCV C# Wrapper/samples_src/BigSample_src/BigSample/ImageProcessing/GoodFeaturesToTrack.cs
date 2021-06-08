//**********************************************************
// File name: $ GoodFeaturesToTrack.cs $
// Author:		$ Heiko Kieﬂling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Example code shows how to use cvlib dll.
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
	public class FeatureDetector
	{
		private MainForm form;

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="form">Main form</param>
		public FeatureDetector(MainForm form)
		{
			this.form = form;
		}

		/// <summary>
		/// Find good Features to track
		/// </summary>
		/// <param name="image">input image</param>
		/// <returns>feature image</returns>
		public IplImage GoodFeaturesToTrack(IplImage image)
		{
			int corner_count = 100;

			/// create gray scale image
			IplImage gray = cvlib.CvCreateImage(new CvSize(image.width, image.height), (int)cvlib.IPL_DEPTH_8U, 1);
			IplImage eig_image = cvlib.CvCreateImage(new CvSize(image.width, image.height), (int)cvlib.IPL_DEPTH_32F, 1);
			IplImage tmp_image = cvlib.CvCreateImage(new CvSize(image.width, image.height), (int)cvlib.IPL_DEPTH_32F, 1);
			CvPoint2D32f[] pts = new CvPoint2D32f[corner_count];

			/// do color conversion
			cvlib.CvCvtColor(ref image, ref gray, cvlib.CV_BGR2GRAY);

			GCHandle h;
			cvlib.CvGoodFeaturesToTrack(ref gray, ref eig_image, ref tmp_image, cvtools.Convert1DArrToPtr(pts, out h),
				ref corner_count, 0.01, 1, IntPtr.Zero, 3, 1, 0.04);
			
			foreach (CvPoint2D32f p in pts)
			{
				cvlib.CvCircle(ref image, new CvPoint((int)p.x, (int)p.y), 2, new CvScalar(0, 255, 0, 0), 2, 8, 0);
			}
			
			cvlib.CvReleaseImage(ref eig_image);
			cvlib.CvReleaseImage(ref tmp_image);
			cvlib.CvReleaseImage(ref gray);
			cvtools.ReleaseHandel(h);

			return image;
		}
	}
}
