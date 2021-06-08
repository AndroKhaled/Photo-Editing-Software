//**********************************************************
// File name: $ Segment.cs $
// Author:		$ Heiko Kieﬂling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Example code shows how to use cvlib dll.
//
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
using Tools;

namespace CvExample
{
	public class Segmentation
	{
		private MainForm form;

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="form">main form</param>
		public Segmentation(MainForm form)
		{
			this.form = form;
		}

		/// <summary>
		/// Watershed transformation
		/// </summary>
		/// <param name="image">input image</param>
		/// <returns>return image</returns>
		public IplImage Watershed(IplImage image)
		{
			int rgn, i = 1, j;
			if ((rgn = form.watershedPoints.Count) < 1) return image;
			IplImage edges, image1 = cvlib.CvCloneImage(ref image);
			Tables ct = new Tables(rgn);

			// create mask image
			unchecked
			{
				edges = cvlib.CvCreateImage(new CvSize(image.width, image.height), (int)cvlib.IPL_DEPTH_32S, 1);
			}
			cvlib.CvSetZero(ref edges);
			foreach (Point pt in form.watershedPoints)
			{
				cvlib.CvCircle(ref edges, new CvPoint(pt.X, pt.Y), 1, new CvScalar(i++, 0, 0, 0), 1, 8, 0);
			}

			// perform watershed
			cvlib.CvWatershed(ref image, ref edges);

			//paint the watershed image
			for (i = 0; i < image.height; i++)
				for ( j = 0; j < image.width; j++)
				{
					unsafe
					{
						int idx = ((int*)(edges.imageData.ToInt32() + edges.widthStep * i))[j];
						byte* dst = &(((byte*)(image.imageData.ToInt32() + image.widthStep * i))[j * 3]);
						if (idx == -1)
							dst[0] = dst[1] = dst[2] = (byte)255;
						else if (idx <= 0 || idx > 255)
							dst[0] = dst[1] = dst[2] = (byte)0; // should not get here
						else
						{
							dst[0] = ct.getColor(idx-1).B; dst[1] = ct.getColor(idx-1).G; dst[2] = ct.getColor(idx-1).R;
						}
					}
				}

			cvlib.CvAddWeighted(ref image, 0.5, ref image1, 0.5, 1, ref image);
			cvlib.CvReleaseImage(ref image1);
			cvlib.CvReleaseImage(ref edges);
			return image;
		}

		/// <summary>
		/// Pyramid segmentation
		/// </summary>
		/// <param name="image">input image</param>
		/// <returns>return image</returns>
		public IplImage PyrSegment(IplImage image)
		{
			IplImage image0, image1;
			int block_size = 1000;
			CvSeq comp = new CvSeq();
			CvMemStorage storage;

			storage = cvlib.CvCreateMemStorage(block_size);

			image.width &= -(1 << form.dlg.GetP(2).i);
			image.height &= -(1 << form.dlg.GetP(2).i);

			image0 = cvlib.CvCloneImage(ref image);
			image1 = cvlib.CvCloneImage(ref image);

			cvlib.CvPyrSegmentation(ref image0, ref image1, ref storage, ref comp,
								 form.dlg.GetP(2).i, form.dlg.GetP(0).i, form.dlg.GetP(1).i);

			cvlib.CvReleaseMemStorage(ref storage);
			cvlib.CvReleaseImage(ref image);
			cvlib.CvReleaseImage(ref image0);
			return image1;
		}
	}
}
