//**********************************************************
// File name: $ Calibration.cs $
// Author:		$ Heiko Kieﬂling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Example code shows how to implement the FloodFill
//						algorithm by request.
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
	public class Drawings
	{
		private MainForm form;

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="form">main form</param>
		public Drawings(MainForm form)
		{
			this.form = form;
		}

		/// <summary>
		/// Demontration of using the FloodFill operation
		/// </summary>
		/// <param name="image"></param>
		public IplImage FloodFill(IplImage image)
		{
			IplImage maskImg;
			int lo_diff, up_diff;
			CvConnectedComp comp;
			CvPoint floodSeed;
			CvScalar floodColor;
			lo_diff = 8;
			up_diff = 8;
			floodSeed = new CvPoint(100, 100);
			floodColor = cvlib.CV_RGB(255, 0, 0); //set the flood color to red
			comp = new CvConnectedComp();
			maskImg = cvlib.CvCreateImage(new CvSize(image.width + 2, image.height + 2), 8, 1);
			cvlib.CvSetZero(ref maskImg);

			//Flood and Fill from pixel(100, 100) with color red and the flood range of (-8, +8)
			cvlib.CvFloodFill(ref image, floodSeed, floodColor, cvlib.CV_RGB(lo_diff, lo_diff, lo_diff),
			cvlib.CV_RGB(up_diff, up_diff, up_diff), ref comp, 8, ref maskImg);
			cvlib.CvReleaseImage(ref maskImg);
			return image;
		}
	}
}
