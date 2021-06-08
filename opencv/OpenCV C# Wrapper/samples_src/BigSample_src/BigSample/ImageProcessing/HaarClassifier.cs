//**********************************************************
// File name: $ HaarClassifier.cs $
// Author:		$ Heiko Kieﬂling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Example code shows how to use cvlib dll.
//						Be sure you put haarcascade_frontalface_alt2.xml
//						in your executable folder.
//						You must copy cvlib.dll and all OpenCV dll's in
//						your release and debug folder or the appl.
//						will not execute.
// 
// License:		There is no explicit license attached. Feel free
//						to use the code how you like but without any warranty.
//						If you include the code in your own projects and/or
//						redistribute pls. include this header.
//
// History:		Rev. 1.0 (beta), hki - initial revision
// 						Rev. 1.1, hki
//**********************************************************


using System;
using System.Collections.Generic;
using System.Text;
using openCV;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CvExample
{
	public class Classifier
	{
		private MainForm form;

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="form">Main form</param>
		public Classifier(MainForm form)
		{
			this.form = form;
		}

		/// <summary>
		/// Find Faces in a image
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		public IplImage HaarClassifier(IplImage image)
		{
			//////////////////////////////////////////////////////
			// Important! :
			// There is a bug in cvlib:
			// you must do at least one call to the cvlib lib
			// so that CvType is initializid, so that when calling
			// cvLoad the haar-type can be registered and found.
			// same for histogram!
			//////////////////////////////////////////////////////

			/// create gray scale image
			IplImage gray = cvlib.CvCreateImage(new CvSize(image.width, image.height), (int)cvlib.IPL_DEPTH_8U, 1);
				
			/// do color conversion
			cvlib.CvCvtColor(ref image, ref gray, cvlib.CV_BGR2GRAY);

			IplImage small_image = image;
			CvMemStorage storage = cvlib.CvCreateMemStorage(0);
			CvSeq faces;
			int i, scale = 1;
			bool do_pyramids = true;

			if (!System.IO.File.Exists(Application.StartupPath + "\\haarcascade_frontalface_alt2.xml"))
			{
				form.WriteLine("File haarcascade_frontalface_alt2.xml not found.", true, false);
				cvlib.CvReleaseMemStorage(ref storage);
				cvlib.CvReleaseImage(ref gray);
				return image;
			}
			IntPtr p = cvlib.CvLoad(Application.StartupPath + "\\haarcascade_frontalface_alt2.xml");
			CvHaarClassifierCascade cascade = (CvHaarClassifierCascade)cvtools.ConvertPtrToStructure(p, typeof(CvHaarClassifierCascade));
			cascade.ptr = p;

			/* if the flag is specified, down-scale the input image to get a
				 performance boost w/o loosing quality (perhaps) */
			if (do_pyramids)
			{
				small_image = cvlib.CvCreateImage(new CvSize(image.width / 2, image.height / 2), (int)cvlib.IPL_DEPTH_8U, 3);
				cvlib.CvPyrDown(ref image, ref small_image, (int)CvFilter.CV_GAUSSIAN_5x5);
				scale = 2;
			}

			/* use the fastest variant */
			faces = cvlib.CvHaarDetectObjects(ref small_image, ref cascade, ref storage, 1.2, 2, cvlib.CV_HAAR_DO_CANNY_PRUNING, new CvSize(0, 0));

			if (faces.total == 0)
			{
				form.WriteLine("No Faces could be found", true, true);
			}

			/* draw all the rectangles */
			for (i = 0; i < faces.total; i++)
			{
				/* extract the rectanlges only */
				CvRect face_rect = (CvRect)cvtools.ConvertPtrToStructure(cvlib.CvGetSeqElem(ref faces, i), typeof(CvRect));
				cvlib.CvRectangle(ref image, new CvPoint(face_rect.x * scale, face_rect.y * scale),
													 new CvPoint((face_rect.x + face_rect.width) * scale, (face_rect.y + face_rect.height) * scale),
													 cvlib.CV_RGB(255, 0, 0), 3, 8, 0);
			}

			cvlib.CvReleaseMemStorage(ref storage);
			cvlib.CvReleaseHaarClassifierCascade(ref cascade);
			cvlib.CvReleaseImage(ref gray);
			return image;
		}
	}
}
