//**********************************************************
// File name: $ Math.cs $
// Author:		$ Heiko Kieﬂling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Example code shows how to use cvlib dll.
//						Usage of some math stuff of CV lib.
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
	public class MathExamples
	{
		private MainForm form;

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="form">main form</param>
		public MathExamples(MainForm form)
		{
			this.form = form;
		}

		/// <summary>
		/// Perform singular value transform
		/// </summary>
		public void SVD()
		{
			// Crate
			CvMat m, evec, evec1, eval;
			m = cvlib.CvCreateMat(3, 3, cvlib.CV_64FC1);
			evec = cvlib.CvCreateMat(3, 3, cvlib.CV_64FC1);
			evec1 = cvlib.CvCreateMat(3, 3, cvlib.CV_64FC1);
			eval = cvlib.CvCreateMat(3, 1, cvlib.CV_64FC1);

			// GetDims
			int[] sizes = new int[2];
			int d = cvlib.CvGetDims(ref m, sizes);

			// Set Values
			cvlib.CvSetReal2D(ref m, 0, 0, 12.4);
			cvlib.CvSetReal2D(ref m, 0, 1, 2.4);
			cvlib.CvSetReal2D(ref m, 0, 2, 22.4);
			cvlib.CvSetReal2D(ref m, 1, 0, 11.4);
			cvlib.CvSetReal2D(ref m, 1, 1, 15.4);
			cvlib.CvSetReal2D(ref m, 1, 2, 17.4);
			cvlib.CvSetReal2D(ref m, 2, 0, 25.4);
			cvlib.CvSetReal2D(ref m, 2, 1, 26.4);
			cvlib.CvSetReal2D(ref m, 2, 2, 1.4);
			cvlib.cvmSet(ref m, 1, 2, 12.4);

			// Get a value
			double f = cvlib.CvGetReal2D(ref m, 1, 2);

			// perform operation
			//cvlib.CvEigenVV(ref m, ref evec, ref eval, 0);
			cvlib.CvSVD(ref m, ref eval, ref evec, ref evec1, 3);
			form.WriteLine(cvlib.CvGetReal2D(ref evec, 0, 0).ToString() + " ", false, false);
			form.WriteLine(cvlib.CvGetReal2D(ref evec, 0, 1).ToString() + " ", false, false);
			form.WriteLine(cvlib.CvGetReal2D(ref evec, 0, 2).ToString() + " ", true, false);
			form.WriteLine(cvlib.CvGetReal2D(ref evec, 1, 0).ToString() + " ", false, false);
			form.WriteLine(cvlib.CvGetReal2D(ref evec, 1, 1).ToString() + " ", false, false);
			form.WriteLine(cvlib.CvGetReal2D(ref evec, 1, 2).ToString() + " ", true, false);
			form.WriteLine(cvlib.CvGetReal2D(ref evec, 2, 0).ToString() + " ", false, false);
			form.WriteLine(cvlib.CvGetReal2D(ref evec, 2, 1).ToString() + " ", false, false);
			form.WriteLine(cvlib.CvGetReal2D(ref evec, 2, 2).ToString() + " ", true, false);

			form.WriteLine(cvlib.CvGetReal2D(ref eval, 0, 0).ToString(), true, false);
			form.WriteLine(cvlib.CvGetReal2D(ref eval, 1, 0).ToString(), true, false);
			form.WriteLine(cvlib.CvGetReal2D(ref eval, 2, 0).ToString(), true, false);
		}

		/// <summary>
		/// Show how to multiply matrices
		/// </summary>
		public void Mul()
		{
			double[] a = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
			double[] b = { 1, 5, 9, 2, 6, 10, 3, 7, 11, 4, 8, 12 };
			double[] e = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
			double[] c = new double[9];
			CvMat Ma = cvlib.CvCreateMat(3, 4, cvlib.CV_64FC1);
			CvMat Mb = cvlib.CvCreateMat(4, 3, cvlib.CV_64FC1);
			CvMat Mc = cvlib.CvCreateMat(3, 4, cvlib.CV_64FC1);
			CvMat Md = cvlib.CvCreateMat(3, 4, cvlib.CV_64FC1);
			
			/// as long as the handels not released the garbage collector
			/// will not touch the arrays. This means it is only necessary to
			/// save a handels reference if you expect an output from unmanaged
			/// function that is visible through a change in values of an array.
			/// This is not the case here.
			GCHandle ha, hb, hc, hd;

			cvlib.CvInitMatHeader(ref Ma, 3, 4, cvlib.CV_64FC1, cvtools.Convert1DArrToPtr(a, out ha), cvlib.CV_AUTO_STEP);
			cvlib.CvInitMatHeader(ref Mb, 4, 3, cvlib.CV_64FC1, cvtools.Convert1DArrToPtr(b, out hb), cvlib.CV_AUTO_STEP);
			cvlib.CvInitMatHeader(ref Mc, 3, 3, cvlib.CV_64FC1, cvtools.Convert1DArrToPtr(e, out hc), cvlib.CV_AUTO_STEP);
			cvlib.CvInitMatHeader(ref Md, 3, 3, cvlib.CV_64FC1, cvtools.Convert1DArrToPtr(c, out hd), cvlib.CV_AUTO_STEP);

			cvtools.ReleaseHandel(ha);
			cvtools.ReleaseHandel(hb);
			cvtools.ReleaseHandel(hc);
			cvtools.ReleaseHandel(hd);

			cvlib.CvMatMulAdd(ref Ma, ref Mb, ref Md, ref Mc);

			/// output
			form.WriteLine(cvlib.CvGetReal2D(ref Mc, 0, 0).ToString() + " ", false, false);
			form.WriteLine(cvlib.CvGetReal2D(ref Mc, 0, 1).ToString() + " ", false, false);
			form.WriteLine(cvlib.CvGetReal2D(ref Mc, 0, 2).ToString() + " ", true, false);
			form.WriteLine(cvlib.CvGetReal2D(ref Mc, 1, 0).ToString() + " ", false, false);
			form.WriteLine(cvlib.CvGetReal2D(ref Mc, 1, 1).ToString() + " ", false, false);
			form.WriteLine(cvlib.CvGetReal2D(ref Mc, 1, 2).ToString() + " ", true, false);
			form.WriteLine(cvlib.CvGetReal2D(ref Mc, 2, 0).ToString() + " ", false, false);
			form.WriteLine(cvlib.CvGetReal2D(ref Mc, 2, 1).ToString() + " ", false, false);
			form.WriteLine(cvlib.CvGetReal2D(ref Mc, 2, 2).ToString() + " ", true, false);

			// release unmanaged memory
			cvlib.CvReleaseMat(ref Ma);
			cvlib.CvReleaseMat(ref Mb);
			cvlib.CvReleaseMat(ref Mc);
			cvlib.CvReleaseMat(ref Md);
		}

		/// <summary>
		/// Fit a line into a set of points (image)
		/// </summary>
		/// <param name="image">input image (point cloud)</param>
		/// <returns>result image</returns>
		public IplImage FitLine(IplImage image)
		{
			int nPoints = 0;
			IplImage gray = cvlib.CvCreateImage(cvlib.CvGetSize(ref image), 8, 1);
			cvlib.CvCvtColor(ref image, ref gray, cvlib.CV_RGB2GRAY);

			// count nPoints
			for (int y = 0; y < gray.height; y++)
				for (int x = 0; x < gray.width; x++)
				{
					unsafe
					{
						byte val = ((byte*)(gray.imageData.ToInt32() + gray.widthStep * y))[x];
						if (form.dlg.GetP(0).s.CompareTo("black") == 0)
						{
							if (val < 200) nPoints++;
						}
						else
						{
							if (val > 50) nPoints++;
						}
					}
				}

			CvMat arrOfPoints = cvlib.CvCreateMat(nPoints, 1, cvlib.CV_32FC2);
			int i = 0;
			for (int y = 0; y < gray.height; y++)
				for (int x = 0; x < gray.width; x++)
				{
					unsafe
					{
						byte val = ((byte*)(gray.imageData.ToInt32() + gray.widthStep * y))[x];
						if (form.dlg.GetP(0).s.CompareTo("black") == 0)
						{
							if (val < 200)
							{
								cvlib.CvSet2D(ref arrOfPoints, i, 0, new CvScalar(x, y, 0, 0));
								i++;
							}
						}
						else
						{
							if (val > 50)
							{
								cvlib.CvSet2D(ref arrOfPoints, i, 0, new CvScalar(x, y, 0, 0));
								i++;
							}
						}
					}
				}
			float[] line = new float[4];

			cvlib.CvFitLine(ref arrOfPoints, cvlib.CV_DIST_L2, 0, 0.01, 0.01, line);
			float vx = line[0]; float vy = line[1];
			float x0 = line[2]; float y0 = line[3];
			float xp = 0, yp, t;
			t = (xp - x0) / vx;
			yp = y0 + t * vy;
			int ymax = image.height;
			int xmax = image.width;
			while (xp < xmax)
			{
				if (yp >= 0 && yp < ymax) break;
				xp++;
				t = (xp - x0) / vx;
				yp = y0 + t * vy;
			}
			CvPoint pt1 = new CvPoint((int)xp, (int)yp);

			while (xp < xmax && yp < ymax)
			{
				xp++;
				t = (xp - x0) / vx;
				yp = y0 + t * vy;
			}
			CvPoint pt2 = new CvPoint((int)xp, (int)yp);
			cvlib.CvLine(ref image, pt1, pt2, new CvScalar(255, 0, 0, 0), 1, 8, 0);
			cvlib.CvReleaseImage(ref gray);
			return image;
		}

		/// <summary>
		/// Fit a ellipse into a set of points (image)
		/// </summary>
		/// <param name="image">input image</param>
		/// <returns>result image</returns>
		public IplImage FitEllipse(IplImage image)
		{
			int nPoints = 0;
			IplImage gray = cvlib.CvCreateImage(cvlib.CvGetSize(ref image), 8, 1);
			cvlib.CvCvtColor(ref image, ref gray, cvlib.CV_RGB2GRAY);

			// count nPoints
			for (int y = 0; y < gray.height; y++)
				for (int x = 0; x < gray.width; x++)
				{
					unsafe
					{
						byte val = ((byte*)(gray.imageData.ToInt32() + gray.widthStep * y))[x];
						if (form.dlg.GetP(0).s.CompareTo("black") == 0)
						{
							if (val < 200) nPoints++;
						}
						else
						{
							if (val > 50) nPoints++;
						}
					}
				}

			CvMat arrOfPoints = cvlib.CvCreateMat(nPoints, 1, cvlib.CV_32FC2);
			int i = 0;
			for (int y = 0; y < gray.height; y++)
				for (int x = 0; x < gray.width; x++)
				{
					unsafe
					{
						byte val = ((byte*)(gray.imageData.ToInt32() + gray.widthStep * y))[x];
						if (form.dlg.GetP(0).s.CompareTo("black") == 0)
						{
							if (val < 200)
							{
								cvlib.CvSet2D(ref arrOfPoints, i, 0, new CvScalar(x, y, 0, 0));
								i++;
							}
						}
						else
						{
							if (val > 50)
							{
								cvlib.CvSet2D(ref arrOfPoints, i, 0, new CvScalar(x, y, 0, 0));
								i++;
							}
						}
					}
				}
			float[] line = new float[4];

			CvBox2D box = cvlib.CvFitEllipse2(ref arrOfPoints);

			cvlib.CvEllipse(ref image, new CvPoint((int)box.center.x, (int)box.center.y),
				new CvSize((int)box.size.width / 2, (int)box.size.height / 2), box.angle, 0, 360, new CvScalar(255, 0, 0, 0), 1, 8, 0);
			
			cvlib.CvReleaseImage(ref gray);

			return image;
		}
	}
}
