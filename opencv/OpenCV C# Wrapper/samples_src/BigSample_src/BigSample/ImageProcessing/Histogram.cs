//**********************************************************
// File name: $ Histogram.cs $
// Author:		$ Heiko Kieﬂling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Example code shows how to use cvlib dll.
//						Plots some different histograms.
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
using Tools;
using System.Runtime.InteropServices;

namespace CvExample
{
	public class Histograms
	{
		private MainForm form;

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="form">main form</param>
		public Histograms(MainForm form)
		{
			this.form = form;
		}

		/// <summary>
		/// Plots a lot of different information for a scanline in the image
		/// </summary>
		/// <param name="image">input image</param>
		/// <returns>result image</returns>
		public IplImage HistLine(IplImage image)
		{
			IplImage res = cvlib.CvCreateImage(new CvSize(image.width, image.height), image.depth, 3);
			cvlib.CvSet(ref res, new CvScalar(0, 0, 0, 0));

			double[] pre = new double[3] { 0, 0, 0 };
			double d, dp = 0;
			int scanlineY = (image.height - 1)* (100 - form.dlg.GetP(0).i) / 100;
			int drawAxisX = (image.height - 1);
			int hr, hg, hb, hue, hrp = 0, hgp = 0, hbp = 0, huep = 0, meanp = 0, h = 0;
			long mean = 0, count = 0;
			bool first = true;
			for (int j = 0; j < image.width; j++)
			{
				unsafe
				{
					byte* src = &(((byte*)(image.imageData.ToInt32() + image.widthStep * scanlineY))[j * 3]);
					count++;
					hb = ((image.height - 1) * src[0] / 255);
					hg = ((image.height - 1) * src[1] / 255);
					hr = ((image.height - 1) * src[2] / 255);
					pre = Tables.ConvertColor(src[2], src[1], src[0], pre);
					h = (int)pre[0];
					hue = ((image.height - 1) * h / 360);
					mean += hue;
					if (first)
					{
						hbp = drawAxisX - hb;
						hgp = drawAxisX - hg;
						hrp = drawAxisX - hr;
						huep = drawAxisX - hue;
						meanp = drawAxisX - ((image.height - 1) * (int)(mean / count) / 360);
						dp = drawAxisX;
						first = false;
					}
					else
					{
						if (form.dlg.GetP(3).i > 0) cvlib.CvLine(ref res, new CvPoint(j, hbp), new CvPoint(j, drawAxisX - hb), new CvScalar(255, 0, 0, 0), 1, 8, 0);
						if (form.dlg.GetP(2).i > 0) cvlib.CvLine(ref res, new CvPoint(j, hgp), new CvPoint(j, drawAxisX - hg), new CvScalar(0, 255, 0, 0), 1, 8, 0);
						if (form.dlg.GetP(1).i > 0) cvlib.CvLine(ref res, new CvPoint(j, hrp), new CvPoint(j, drawAxisX - hr), new CvScalar(0, 0, 255, 0), 1, 8, 0);
						if (form.dlg.GetP(4).i > 0) cvlib.CvLine(ref res, new CvPoint(j, huep), new CvPoint(j, drawAxisX - hue), new CvScalar(255, 255, 0, 0), 1, 8, 0);
						if (form.dlg.GetP(5).i > 0) cvlib.CvLine(ref res, new CvPoint(j, meanp), new CvPoint(j, drawAxisX - (image.height * (int)(mean / count) / 360)), new CvScalar(0, 255, 255, 0), 1, 8, 0);
						//delta
						d = ((image.height - 1) * (Math.Sqrt(Math.Pow(hbp - drawAxisX + hb, 2) + Math.Pow(hgp - drawAxisX + hg, 2) + Math.Pow(hrp - drawAxisX + hr, 2) / 2)) / 360);
						if (form.dlg.GetP(6).i > 0) cvlib.CvLine(ref res, new CvPoint(j, (int)dp), new CvPoint(j, drawAxisX - (int)d), new CvScalar(255, 0, 255, 0), 1, 8, 0);
						hbp = drawAxisX - hb;
						hgp = drawAxisX - hg;
						hrp = drawAxisX - hr;
						huep = drawAxisX - hue;
						meanp = drawAxisX - ((image.height - 1) * (int)(mean / count) / 360);
						dp = drawAxisX - d;
					}
				}
			}
			if (form.dlg.GetP(7).i > 0)
			{
				for (int i = 0; i < image.width; i += 10)
					cvlib.CvLine(ref res, new CvPoint(i, scanlineY + 1), new CvPoint(i + 5, scanlineY + 1), new CvScalar(255, 255, 255, 0), 2, 1, 0);
			}

			cvlib.CvAddWeighted(ref image, 0.5, ref res, 0.5, 0, ref res);
			cvlib.CvReleaseImage(ref image);
			return res;
		}

		/// <summary>
		/// Plots HS Histogram in OpenCv Window
		/// </summary>
		/// <param name="image">input image</param>
		public void HSHistogram(IplImage image)
		{
			IplImage h_plane = cvlib.CvCreateImage(cvlib.CvGetSize(ref image), 8, 1);
			IplImage s_plane = cvlib.CvCreateImage(cvlib.CvGetSize(ref image), 8, 1);
			IplImage v_plane = cvlib.CvCreateImage(cvlib.CvGetSize(ref image), 8, 1);
			IntPtr[] planes = { h_plane.ptr, s_plane.ptr };
			IplImage hsv = cvlib.CvCreateImage(cvlib.CvGetSize(ref image), 8, 3);
			int h_bins = 30, s_bins = 32;
			int[] hist_size = { h_bins, s_bins };
			float[] h_ranges = { 0, 180 }; /* hue varies from 0 (~0∞red) to 180 (~360∞red again) */
			float[] s_ranges = { 0, 255 }; /* saturation varies from 0 (black-gray-white) to 255 (pure spectrum color) */
			float[][] ranges = new float[2][];
			ranges[0] = new float[2] { 0, 180 };
			ranges[1] = new float[2] { 0, 255 };
			int scale = 10;
			IplImage hist_img = cvlib.CvCreateImage(new CvSize(h_bins * scale, s_bins * scale), 8, 3);
			CvHistogram hist = new CvHistogram();
			float min_val = 0, max_value = 0;
			int max_idx = 0, min_idx = 0;
			int h, s;

			cvlib.CvCvtColor(ref image, ref hsv, cvlib.CV_BGR2HSV);
			cvlib.CvSplit(ref hsv, ref h_plane, ref s_plane, ref v_plane);
			//cvlib.CvNamedWindow("H-Plane", 1);
			//cvlib.CvShowImage("H-Plane", ref h_plane);
			//cvlib.CvNamedWindow("S-Plane", 1);
			//cvlib.CvShowImage("S-Plane", ref s_plane);
			//cvlib.CvNamedWindow("V-Plane", 1);
			//cvlib.CvShowImage("V-Plane", ref v_plane);
			GCHandle[] handel;
			IntPtr r = cvtools.Convert2DArrToPtr(ranges, out handel);
			hist = cvlib.CvCreateHist(2, hist_size, cvlib.CV_HIST_ARRAY, r, 1);
			cvlib.CvCalcHist(planes, ref hist, 0);
			cvlib.CvGetMinMaxHistValue(ref hist, ref min_val, ref max_value, ref min_idx, ref max_idx);
			cvlib.CvSetZero(ref hist_img);

			for (h = 0; h < h_bins; h++)
			{
				for (s = 0; s < s_bins; s++)
				{
					float bin_val = (float)cvlib.CvGetReal2D(hist.bins, h, s);
					int intensity = cvlib.CvRound(bin_val * 255.0 / max_value);
					cvlib.CvRectangle(ref hist_img, new CvPoint(h * scale, s * scale),
											 new CvPoint((h + 1) * scale - 1, (s + 1) * scale - 1),
											 cvlib.CV_RGB(intensity, intensity, intensity),
											 cvlib.CV_FILLED, 8, 0);
				}
			}

			cvlib.CvNamedWindow("H-S Histogram", 1);
			cvlib.CvShowImage("H-S Histogram", ref hist_img);
			cvtools.ReleaseHandels(handel);
			cvlib.CvReleaseImage(ref h_plane);
			cvlib.CvReleaseImage(ref s_plane);
			cvlib.CvReleaseImage(ref v_plane);
			cvlib.CvReleaseImage(ref hsv);
			cvlib.CvReleaseImage(ref hist_img);
			cvlib.CvReleaseImage(ref image);
		}

		/// <summary>
		/// Perform Histogram Equalisation
		/// </summary>
		/// <param name="image">input image</param>
		/// <returns>equalized histogram image</returns>
		public IplImage EqualizeHistogram(IplImage image)
		{
			int scale = 1;
			IplImage gray = cvlib.CvCreateImage(cvlib.CvGetSize(ref image), (int)cvlib.IPL_DEPTH_8U, 1);
			IplImage small_img = cvlib.CvCreateImage(cvlib.CvSize(cvlib.CvRound(image.width / scale), cvlib.CvRound(image.height / scale)), 8, 1);

			cvlib.CvCvtColor(ref image, ref gray, cvlib.CV_BGR2GRAY);
			cvlib.CvResize(ref gray, ref small_img, cvlib.CV_INTER_LINEAR);
			cvlib.CvEqualizeHist(ref small_img, ref small_img); // ? ERROR gets thrown on this line
			cvlib.CvCvtColor(ref small_img, ref image, cvlib.CV_GRAY2BGR); // convert to color (only color images supported in this appl)
			cvlib.CvReleaseImage(ref gray); // release memeory
			cvlib.CvReleaseImage(ref small_img); // release memory
			return image;
		}
	}
}
