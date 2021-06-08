//**********************************************************
// File name: $ Math.cs $
// Author:		$ Heiko Kieﬂling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Example code shows how to use cvlib dll.
//						Calculation of affine Transformation Matrix and 
//						use this to transform some points.
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
using System.Runtime.InteropServices;

namespace CvExample
{
	public class PerspectiveTransform
	{
		private MainForm form;

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="form">main form</param>
		public PerspectiveTransform(MainForm form)
		{
			this.form = form;
		}

		/// <summary>
		/// Create affine transformation params from point sets
		/// and transform initial point set.
		/// </summary>
		/// <param name="image">image (not used here)</param>
		public IplImage TransformAffine(IplImage image)
		{
			// create transformation matrix and set all values zero
			CvMat tr_matr = cvlib.CvCreateMat(2, 3, cvlib.CV_64FC1);
			cvlib.CvSetZero(ref tr_matr);

			// point sets from / to
			CvPoint2D32f[] pts_from = new CvPoint2D32f[3];
			CvPoint2D32f[] pts_to = new CvPoint2D32f[3];

			// initialize point sets
			pts_from[0] = new CvPoint2D32f(0, 0);
			pts_from[1] = new CvPoint2D32f(640, 0);
			pts_from[2] = new CvPoint2D32f(640, 480);
			pts_to[0] = new CvPoint2D32f(0, 0);
			pts_to[1] = new CvPoint2D32f(700, 0);
			pts_to[2] = new CvPoint2D32f(700, 500);

			form.WriteLine("First Rectangle -from-: (0,0) / (640,480)", true, false);
			form.WriteLine("Second Rectangle -to-: (0,0) / (700,500)", true, false);

			// get transformation matrix -> tr_matr
			GCHandle h1, h2;
			IntPtr p1 = cvtools.ConvertStructureToPtr(pts_from, out h1);
			IntPtr p2 = cvtools.ConvertStructureToPtr(pts_to, out h2);
			cvlib.CvGetAffineTransform(p1, p2, ref tr_matr);

			// show this matrix
			form.WriteLine("Transformation Matrix:", true, false);
			form.WriteLine(cvlib.CvGetReal2D(ref tr_matr, 0, 0).ToString() + " ", false, false);
			form.WriteLine(cvlib.CvGetReal2D(ref tr_matr, 0, 1).ToString() + " ", false, false);
			form.WriteLine(cvlib.CvGetReal2D(ref tr_matr, 0, 2).ToString() + " ", true, false);
			form.WriteLine(cvlib.CvGetReal2D(ref tr_matr, 1, 0).ToString() + " ", false, false);
			form.WriteLine(cvlib.CvGetReal2D(ref tr_matr, 1, 1).ToString() + " ", false, false);
			form.WriteLine(cvlib.CvGetReal2D(ref tr_matr, 1, 2).ToString() + " ", true, false);

			// Transform now the initial points
			form.WriteLine("Now transform first Rectangle -from-: (0,0) / (640,480) with transformation Matrix:", true, false);
			CvMat srci = cvlib.CvCreateMat(1, 1, cvlib.CV_64FC2);
			CvMat trans = cvlib.CvCreateMat(2, 1, cvlib.CV_64FC1);
			cvlib.CvSetReal2D(ref trans, 0, 0, 0);
			cvlib.CvSetReal2D(ref trans, 1, 0, 0);
			CvMat dst = cvlib.CvCreateMat(1, 1, cvlib.CV_64FC2);
			for (int i = 0; i < 3; i++)
			{
				cvlib.CvSet2D(ref srci, 0, 0, new CvScalar(pts_from[i].x, pts_from[i].y, 0, 0));
				cvlib.CvTransform(ref srci, ref dst, ref tr_matr, ref trans);
				CvScalar res = cvlib.CvGet2D(ref dst, 0, 0);
				form.WriteLine("Point " + i.ToString() + ": (" + res.val1 + "," + res.val2 + ")", true, false);
			}

			// release ressources
			cvlib.CvReleaseMat(ref srci);
			cvlib.CvReleaseMat(ref tr_matr);
			cvlib.CvReleaseMat(ref trans);
			cvtools.ReleaseHandel(h1);
			cvtools.ReleaseHandel(h2);

			return image;
		}
	}
}
