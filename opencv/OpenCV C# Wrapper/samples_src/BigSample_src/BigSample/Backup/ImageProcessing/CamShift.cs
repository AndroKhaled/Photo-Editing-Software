using System;
using System.Collections.Generic;
using System.Text;
using openCV;
using System.Runtime.InteropServices;

namespace CvExample
{
	/// <summary>
	/// Under construction
	/// </summary>
	public class Tracker
	{
		private IplImage image, hsv, hue, mask, backproject, histimg;
		private CvHistogram hist;
		private int[] hdims;
		private float[] hranges;
		private int backproject_mode = 0;
		private int select_object = 0;
		private int track_object = 0;
		private int show_hist = 1;
		private CvPoint origin;
		private CvRect selection;
		private CvRect track_window;
		private CvBox2D track_box;
		private CvConnectedComp track_comp;
		private int vmin = 10, vmax = 256, smin = 30;

		CvScalar hsv2rgb( float hue )
		{
			int[] rgb = new int[3];
			int p, sector;
			int[][] sector_data = new int[6][];
			sector_data[0] = new int[] {0,2,1};
			sector_data[1] = new int[] {1,2,0};
			sector_data[2] = new int[] {1,0,2};
			sector_data[3] = new int[] {2,0,1};
			sector_data[4] = new int[] {2,1,0};
			sector_data[5] = new int[] {0,1,2};
			hue *= 0.033333333333333333333333333333333F;
			sector = (int)Math.Floor(hue);
			p = cvlib.CvRound(255 * (hue - (float)sector));
			p ^= (sector & (int)1) > 0 ? 255 : 0;

			rgb[sector_data[sector][0]] = 255;
			rgb[sector_data[sector][1]] = 0;
			rgb[sector_data[sector][2]] = p;

			return new CvScalar(rgb[2], rgb[1], rgb[0], 0);
		}

		//private IplImage Run(IplImage frame)
		//{
		//  int i, bin_w, c;

		//  if( frame.ptr == IntPtr.Zero )
		//    throw new Exception("Invalid Frame");

		//    if( image.ptr == IntPtr.Zero )
		//    {
		//      /* allocate all the buffers */
		//      image = cvlib.CvCreateImage( new CvSize(frame.width, frame.height), 8, 3 );
		//      image.origin = frame.origin;
		//      hsv = cvlib.CvCreateImage( cvlib.CvGetSize(ref frame), 8, 3 );
		//      hue = cvlib.CvCreateImage( cvlib.CvGetSize(ref frame), 8, 1 );
		//      mask = cvlib.CvCreateImage( cvlib.CvGetSize(ref frame), 8, 1 );
		//      backproject = cvlib.CvCreateImage( cvlib.CvGetSize(ref frame), 8, 1 );
		//      hdims = new int[1] {16};
		//      hranges = new float[2] {0, 180};
		//      GCHandle h;
		//      hist = cvlib.CvCreateHist( 1, hdims, cvlib.CV_HIST_ARRAY, cvtools.Convert1DArrToPtr(hranges, out h), 1 );
		//      cvtools.ReleaseHandel(h);
		//      histimg = cvlib.CvCreateImage( new CvSize(320, 200), 8, 3 );
		//      cvlib.CvSetZero( ref histimg );
		//    }

		//    cvlib.CvCopy(ref frame, ref image);
		//    cvlib.CvCvtColor(ref image, ref hsv, cvlib.CV_BGR2HSV );

		//    if( track_object == 1)
		//    {
		//        int _vmin = vmin, _vmax = vmax;

		//        cvlib.CvInRangeS( ref hsv, new CvScalar(0, (double)smin, Math.Min(_vmin, _vmax), 0),
		//                    new CvScalar(180, 256, Math.Max(_vmin, _vmax) ,0), ref mask );
		//        cvlib.CvSplit(ref hsv, ref hue);

		//        if( track_object < 0 )
		//        {
		//            float max_val = 0;
		//            cvlib.CvSetImageROI( ref hue, selection );
		//            cvlib.CvSetImageROI( ref mask, selection );
		//            cvlib.CvCalcHist(new IntPtr[] {hue.ptr}, ref hist, 0, ref mask );
		//            int min_val, min_idx, max_idx;
		//            cvlib.CvGetMinMaxHistValue(ref hist, ref min_val, ref max_val, ref min_idx, ref max_idx );
		//            cvlib.CvConvertScale( hist.bins, hist.bins, max_val ? 255.0 / (double)max_val : 0, 0 );
		//            cvlib.CvResetImageROI( ref hue );
		//            cvlib.CvResetImageROI( ref mask );
		//            track_window = selection;
		//            track_object = 1;

		//            cvlib.CvSetZero( ref histimg );
		//            bin_w = histimg.width / hdims[0];
		//            for( i = 0; i < hdims; i++ )
		//            {
		//              int val = cvlib.CvRound( cvlib.CvGetReal1D(hist.bins, i) * (double)histimg.height / (double)255 );
		//              CvScalar color = hsv2rgb((float)i * (float)180 / (float)hdims[0]);
		//              cvlib.CvRectangle( ref histimg, new CvPoint(i * bin_w, histimg.height),
		//                             new CvPoint((i + 1) * bin_w, histimg.height - val),
		//                             color, -1, 8, 0 );
		//            }
		//        }

		//        cvlib.CvCalcBackProject( ref hue, ref backproject, ref hist );
		//        cvlib.CvAnd( ref backproject, ref mask, ref backproject, 0 );
		//        cvCamShift( backproject, track_window,
		//                    cvTermCriteria( CV_TERMCRIT_EPS | CV_TERMCRIT_ITER, 10, 1 ),
		//                    &track_comp, &track_box );
		//        track_window = track_comp.rect;
            
		//        if( backproject_mode )
		//            cvCvtColor( backproject, image, CV_GRAY2BGR );
		//        if( image->origin )
		//            track_box.angle = -track_box.angle;
		//        cvEllipseBox( image, track_box, CV_RGB(255,0,0), 3, CV_AA, 0 );
		//    }
        
		//    if( select_object && selection.width > 0 && selection.height > 0 )
		//    {
		//        cvSetImageROI( image, selection );
		//        cvXorS( image, cvScalarAll(255), image, 0 );
		//        cvResetImageROI( image );
		//    }

		//    cvShowImage( "CamShiftDemo", image );
		//    cvShowImage( "Histogram", histimg );

		//    c = cvWaitKey(10);
		//    if( (char) c == 27 )
		//        break;
		//    switch( (char) c )
		//    {
		//    case 'b':
		//        backproject_mode ^= 1;
		//        break;
		//    case 'c':
		//        track_object = 0;
		//        cvZero( histimg );
		//        break;
		//    case 'h':
		//        show_hist ^= 1;
		//        if( !show_hist )
		//            cvDestroyWindow( "Histogram" );
		//        else
		//            cvNamedWindow( "Histogram", 1 );
		//        break;
		//    default:
		//        ;
		//    }
		//}
		//}
	}
}
