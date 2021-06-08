//**********************************************************
// File name: $ Calibration.cs $
// Author:		$ Heiko Kieﬂling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Example code shows how to implement simple rgb2grey color
//						conversion by using OpenCV lib.
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
using System.Drawing;
using System.Drawing.Imaging;

namespace CvExample
{
	public enum GrayscalMode { Mean, EyeSensitivity, UserDefined };

	public class ColorConvert
	{
		private MainForm form;

		public ColorConvert(MainForm form)
		{
			this.form = form;
		}
		
		/// <summary>
		/// Example Image Conversion
		/// </summary>
		/// <param name="image">input image</param>
		/// <returns>3 channel gray image</returns>
		public IplImage Convert2Gray(IplImage image) 
		{
			/// create gray scale image
			IplImage gray = cvlib.CvCreateImage(new CvSize(image.width, image.height), (int)cvlib.IPL_DEPTH_8U, 1);
			
			/// do color conversion
			cvlib.CvCvtColor(ref image, ref gray, cvlib.CV_BGR2GRAY);
			cvlib.CvCvtColor(ref gray, ref image, cvlib.CV_GRAY2BGR);
			cvlib.CvReleaseImage(ref gray);
			return image;
		}
	}
}
