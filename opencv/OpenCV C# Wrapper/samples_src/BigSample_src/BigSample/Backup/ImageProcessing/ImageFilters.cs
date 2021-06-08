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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace CvExample
{
	public class ImageFilters
	{
		private MainForm form;

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="form">main form</param>
		public ImageFilters(MainForm form)
		{
			this.form = form;
		}

		/// <summary>
		/// perform sobel filtering
		/// </summary>
		/// <param name="image">input image</param>
		/// <returns>sobel image</returns>
		public IplImage Sobel(IplImage image)
		{
			if (form.dlg.GetP(0).i == 0 && form.dlg.GetP(1).i == 0)
			{
				return image;
			}

			IplImage sobel = cvlib.CvCloneImage(ref image);
			
			// process the image
			cvlib.CvSobel(ref image, ref sobel, form.dlg.GetP(0).i, form.dlg.GetP(1).i, Convert.ToInt32(form.dlg.GetP(2).s));
			cvlib.CvReleaseImage(ref image);

			return sobel;
		}

		/// <summary>
		/// perform laplacian filtering
		/// </summary>
		/// <param name="image">input image</param>
		/// <returns>laplacian image</returns>
		public IplImage Laplace(IplImage image)
		{
			int i;
			IplImage[] planes = new IplImage[3];
			IplImage laplace;

			for (i = 0; i < 3; i++)
				planes[i] = cvlib.CvCreateImage(new CvSize(image.width, image.height), 8, 1);

			unchecked // (int)cvlib.IPL_DEPTH_16S
			{
				laplace = cvlib.CvCreateImage(new CvSize(image.width, image.height), (int)cvlib.IPL_DEPTH_16S, 1);
			}
			IplImage colorlaplace = cvlib.CvCreateImage(new CvSize(image.width, image.height), 8, 3);

			cvlib.CvSplit(ref image, ref planes[0], ref planes[1], ref planes[2]);
			
			for (i = 0; i < 3; i++)
			{
				cvlib.CvLaplace(ref planes[i], ref laplace, Convert.ToInt32(form.dlg.GetP(0).s));
				cvlib.CvConvertScaleAbs(ref laplace, ref planes[i], 1, 0);
			}
			
			cvlib.CvMerge(ref planes[0], ref planes[1], ref planes[2], ref colorlaplace);

			// release unmanaged ressources
			cvlib.CvReleaseImage(ref image);
			cvlib.CvReleaseImage(ref laplace);
			for (i = 0; i < 3; i++) cvlib.CvReleaseImage(ref planes[i]);

			return colorlaplace;
		}

		/// <summary>
		/// Perform Canny Edge detector
		/// </summary>
		/// <param name="image">input image</param>
		/// <returns>canny image</returns>
		public IplImage Canny(IplImage image)
		{
			/// create gray scale image
			IplImage gray = cvlib.CvCreateImage(new CvSize(image.width, image.height), (int)cvlib.IPL_DEPTH_8U, 1);
			
			/// color conversion
			cvlib.CvCvtColor(ref image, ref gray, cvlib.CV_BGR2GRAY);

			/// edge image
			IplImage edges = cvlib.CvCloneImage(ref gray);

			/// canny filter
			cvlib.CvCanny(ref gray, ref edges, form.dlg.GetP(0).i, form.dlg.GetP(1).i, Convert.ToInt32(form.dlg.GetP(2).s));

			/// release temp image
			cvlib.CvReleaseImage(ref gray);

			// crate color image back
			cvlib.CvCvtColor(ref edges, ref image, cvlib.CV_GRAY2BGR);

			// release temp image
			cvlib.CvReleaseImage(ref edges);

			return image;
		}

		/// <summary>
		/// Harris corner detector
		/// </summary>
		/// <param name="image">input image</param>
		/// <returns>image with marked corners</returns>
		public IplImage CornerHarris(IplImage image)
		{
			IplImage harris, gray;

			gray = cvlib.CvCreateImage(cvlib.CvGetSize(ref image), 8, 1);
			harris = cvlib.CvCreateImage(cvlib.CvGetSize(ref image), (int)cvlib.IPL_DEPTH_32F, 1);
			cvlib.CvCvtColor(ref image, ref gray, cvlib.CV_RGB2GRAY);
			cvlib.CvConvertScale(ref gray, ref harris, 1, 0);
			cvlib.CvCornerHarris(ref gray, ref harris, 3, 3, 0.04);

			for (int y = 0; y < harris.height; y++)
				for (int x = 0; x < harris.width; x++)
				{
					float val = (float)Marshal.PtrToStructure(new IntPtr(harris.imageData.ToInt32() + harris.widthStep * y + 4 * x), typeof(float));
					if (val > 0.001)
						cvlib.CvCircle(ref image, new CvPoint(x, y), 1, new CvScalar(0, 255, 0, 0), 2, 8, 0);
				}

			cvlib.CvReleaseImage(ref gray);
			cvlib.CvReleaseImage(ref harris);
			return image;
		}

		/// <summary>
		/// Expand Edges
		/// </summary>
		/// <param name="image">Input image</param>
		/// <returns>dilated image</returns>
		public IplImage Dilate(IplImage image)
		{
			// create result image
			IplImage dilate = cvlib.CvCloneImage(ref image);
			// create mask
			int[] arr = new int[9] { 1, 1, 1, 1, 0, 1, 1, 1, 1 };
			// create kernel
			IplConvKernel k = cvlib.CvCreateStructuringElementEx(3, 3, 1, 1, cvlib.CV_SHAPE_CUSTOM, arr);
			// process the image
			cvlib.CvDilate(ref image, ref dilate, ref k, 1);

			return dilate;
		}

		/// <summary>
		/// Image erosion
		/// </summary>
		/// <param name="image">Input Image</param>
		/// <returns>erosion image</returns>
		public IplImage Erode(IplImage image)
		{
			// create result image
			IplImage erode = cvlib.CvCloneImage(ref image);
			// create mask
			int[] arr = new int[9] { 1, 1, 1, 1, 0, 1, 1, 1, 1 };
			// create kernel
			IplConvKernel k = cvlib.CvCreateStructuringElementEx(3, 3, 1, 1, cvlib.CV_SHAPE_CUSTOM, arr);
			// process the image
			cvlib.CvErode(ref image, ref erode, ref k, 1);

			return erode;
		}
	}
}
