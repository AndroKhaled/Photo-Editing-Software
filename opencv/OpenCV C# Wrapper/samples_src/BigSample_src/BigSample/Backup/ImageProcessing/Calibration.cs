//**********************************************************
// File name: $ Calibration.cs $
// Author:		$ Heiko Kieﬂling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Example code shows how to implement camera calibration
//						by using OpenCV lib.
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
using System.Windows.Forms;
using System.Runtime.InteropServices;
using openCV;
using System.IO;

namespace CvExample
{
	public class Calibration
	{
		private MainForm form;

		public Calibration(MainForm form)
		{
			this.form = form;
		}

		public delegate void CalibratedHandler(bool success, int n);
		public event CalibratedHandler Calibrated;

		/// <summary>
		/// Calculation of camera params for undistortion
		/// </summary>
		/// <param name="image">input image (not used)</param>
		/// <param name="found">file has been created</param>
		/// <returns>last image from folder with drawn corners</returns>
		public IplImage FindParams(IplImage image, ref int found)
		{
			int i, j;
			int nImages = 0;
			int found1 = 1;
			int corner_count = 0;
			int processedImages = 0;
			string path = "";
			DirectoryInfo di = null;
			FileInfo[] files = null;
			IplImage image_flip, image_gray;
			CvPoint2D32f[] corners;


			// setting up number of chess bord rectangles (x,y) 
			CvSize board_size = new CvSize(form.dlg.GetP(0).i, form.dlg.GetP(1).i);
			
			// Open Folder browser dialog to
			// select the place from where the images will be loaded
			FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
			folderBrowserDialog1.SelectedPath = Application.StartupPath;
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
				path = folderBrowserDialog1.SelectedPath;
			else return image;

			di = new DirectoryInfo(path);
			files = di.GetFiles();
			nImages = files.Length;
			
			// prepare place for chess board corners
			corners = new CvPoint2D32f[board_size.width * board_size.height * nImages];

			// process each image
			for (i = 0; i < nImages; i++)
			{
				// later copied to corner array
				CvPoint2D32f[] tmpCorners = new CvPoint2D32f[board_size.width * board_size.height];
				
				// check if we have a supported image format
				if (files[i].Extension != ".jpg" && files[i].Extension != ".bmp" && files[i].Extension != ".png")
					continue;

				// if we have at least one valid extension
				if (i == 0) cvlib.CvReleaseImage(ref image);

				// get the actual image
				image = cvlib.CvLoadImage(files[i].FullName, cvlib.CV_LOAD_IMAGE_COLOR);

				// in case when image is upside down
				if (image.origin == cvlib.CV_ORIGIN_BL)
				{
					image_flip = cvlib.CvCloneImage(ref image);
					cvlib.CvFlip(ref image, ref image_flip, 0);
					cvlib.CvReleaseImage(ref image);
					image = image_flip;
				}

				/// create gray scale image
				image_gray = cvlib.CvCreateImage(new CvSize(image.width, image.height), (int)cvlib.IPL_DEPTH_8U, 1);
				
				/// do color conversion
				cvlib.CvCvtColor(ref image, ref image_gray, cvlib.CV_BGR2GRAY);
				
				// get pointer to corner array
				GCHandle h;
				IntPtr ptr = cvtools.Convert1DArrToPtr(tmpCorners, out h);

				// get chess board corners
				found = cvlib.CvFindChessboardCorners(ref image_gray, board_size, ptr, ref corner_count, 0);
				
				if (found == 0)
				{
					found1 = 0;
					cvlib.CvReleaseImage(ref image_gray);
					if (i < (nImages - 1)) cvlib.CvReleaseImage(ref image);
					cvtools.ReleaseHandel(h);
					Calibrated(false, i);
					continue;
				}

				// calculate sub pix accuracy
				cvlib.CvFindCornerSubPix(ref image_gray, ptr, corner_count, new CvSize(11, 11), new CvSize(-1, -1), new CvTermCriteria(cvlib.CV_TERMCRIT_ITER, 30, 0));

				// draw the result
				if (i == (nImages - 1)) cvlib.CvDrawChessboardCorners(ref image, board_size, ptr, corner_count, found);

				// clean up
				cvlib.CvReleaseImage(ref image_gray);
				if (i < (nImages - 1)) cvlib.CvReleaseImage(ref image);

				// if we found corners add them to corner array
				if (found > 0)
				{
					for (j = 0; j < board_size.height * board_size.width; j++)
					{
						corners[i * board_size.height * board_size.width + j] = tmpCorners[j];
					}
					Calibrated(true, i);
				}
				cvtools.ReleaseHandel(h);
				processedImages++;
			}

			// in case we missed corner on one image return
			if (found1 == 0 || processedImages == 0) return image;

			// calculation flags
			int flags = 0 | cvlib.CV_CALIB_FIX_PRINCIPAL_POINT;
			
			// the output matrices
			CvMat camera;
			CvMat dist_coeffs;
			CvMat extr_params;
			CvMat reproj_errs;
			double avg_reproj_err;

			// run the calibration
			bool code = RunCalibration(
				corners,
				new CvSize(image.width, image.height), 
				board_size,
				1, 
				1, 
				flags, 
				out camera, 
				out dist_coeffs, 
				out extr_params,
				out reproj_errs, 
				out avg_reproj_err,
				nImages);

			 //save camera parameters in any case, to catch Inf's/NaN's
			SaveCameraParams(
				Application.StartupPath + "\\CamCalibParams.txt", 
				1,
				new CvSize(image.width, image.height),
			  board_size, 
				1, 
				1, 
				flags,
			  camera, 
				dist_coeffs, 
				extr_params,
				corners,
				reproj_errs, 
				avg_reproj_err);
			
			// release ressources
			cvlib.CvReleaseMat(ref camera);
			cvlib.CvReleaseMat(ref dist_coeffs);
			cvlib.CvReleaseMat(ref extr_params);
			cvlib.CvReleaseMat(ref reproj_errs);

			// return
			return image;
		}

		/// <summary>
		/// internal function (see opencv book)
		/// </summary>
		/// <param name="image_points_seq">image_points_seq</param>
		/// <param name="img_size">img_size</param>
		/// <param name="board_size">board_size</param>
		/// <param name="square_size">square_size</param>
		/// <param name="aspect_ratio">aspect_ratio</param>
		/// <param name="flags">flags</param>
		/// <param name="camera_matrix">camera_matrix</param>
		/// <param name="dist_coeffs">dist_coeffs</param>
		/// <param name="extr_params">extr_params</param>
		/// <param name="reproj_errs">reproj_errs</param>
		/// <param name="avg_reproj_err">avg_reproj_err</param>
		/// <param name="image_count">image_count</param>
		/// <returns>success</returns>
		private bool RunCalibration(CvPoint2D32f[] image_points_seq, CvSize img_size, CvSize board_size,
											 float square_size, float aspect_ratio, int flags,
											 out CvMat camera_matrix, out CvMat dist_coeffs, out CvMat extr_params,
											 out CvMat reproj_errs, out double avg_reproj_err, int image_count)
		{
			bool code;
			int point_count = board_size.width * board_size.height;
			camera_matrix = cvlib.CvCreateMat(3, 3, cvlib.CV_64F);
			dist_coeffs = cvlib.CvCreateMat(1, 4, cvlib.CV_64F);
			CvMat image_points = cvlib.CvCreateMat(1, image_count * point_count, cvlib.CV_32FC2);
			CvMat object_points = cvlib.CvCreateMat(1, image_count * point_count, cvlib.CV_32FC2);
			CvMat point_counts = cvlib.CvCreateMat(1, image_count, cvlib.CV_32SC1);
			CvMat rot_vects = new CvMat();
			CvMat trans_vects = new CvMat();
			int i, j, k;

			
			cvlib.CvSet(ref point_counts, new CvScalar(point_count, 0, 0, 0));

			for (i = 0; i < image_count; i++)
			{
				for (j = 0; j < board_size.height; j++)
					for (k = 0; k < board_size.width; k++)
					{
						cvlib.CvSet2D(ref image_points, 0, i * board_size.width * board_size.height + j * board_size.width + k,
							new CvScalar(image_points_seq[i * board_size.width * board_size.height + j * board_size.width + k].x,
							image_points_seq[i * board_size.width * board_size.height + j * board_size.width + k].y, 0, 0));
						cvlib.CvSet2D(ref object_points, 0, i * board_size.width * board_size.height + j * board_size.width + k,
							new CvScalar(j * square_size, k * square_size, 0, 0));
					}
			}

			extr_params = cvlib.CvCreateMat(image_count, 6, cvlib.CV_32FC1);
			cvlib.CvGetCols(ref extr_params, ref rot_vects, 0, 3);
			cvlib.CvGetCols(ref extr_params, ref trans_vects, 3, 6);

			cvlib.CvSetZero(ref camera_matrix);
			cvlib.CvSetZero(ref dist_coeffs);

			if ((flags & cvlib.CV_CALIB_FIX_ASPECT_RATIO) > 0)
			{
				cvlib.CvSetReal2D(ref camera_matrix, 0, 0, aspect_ratio);
				cvlib.CvSetReal2D(ref camera_matrix, 1, 1, 1);
				cvlib.CvSetReal2D(ref camera_matrix, 2, 2, 1);
			}

			cvlib.CvCalibrateCamera2(ref object_points, ref image_points, ref point_counts,
													img_size, ref camera_matrix, ref dist_coeffs,
													ref rot_vects, ref trans_vects, flags);

			code = (cvlib.CvCheckArr(ref camera_matrix, 0, 0, 0)>0?true:false) &&
				(cvlib.CvCheckArr(ref dist_coeffs, 0, 0, 0)>0?true:false) &&
				(cvlib.CvCheckArr(ref extr_params, 0, 0, 0)>0?true:false);

			reproj_errs = cvlib.CvCreateMat(1, image_count, cvlib.CV_64FC1);
			avg_reproj_err = ComputeReprojectionError(ref object_points, ref rot_vects, ref trans_vects,
							ref camera_matrix, ref dist_coeffs, ref image_points, ref point_counts, ref reproj_errs);

			cvlib.CvReleaseMat(ref object_points);
			cvlib.CvReleaseMat(ref image_points);
			cvlib.CvReleaseMat(ref point_counts);
			
			return code;
		}


		private double ComputeReprojectionError(ref CvMat object_points,
				ref CvMat rot_vects, ref CvMat trans_vects, ref CvMat camera_matrix,
				ref CvMat dist_coeffs, ref CvMat image_points, ref CvMat point_counts,
				ref CvMat per_view_errors)
		{
			CvMat image_points2 = cvlib.CvCreateMat(image_points.rows,
				image_points.cols, image_points.type);
			int i, image_count = rot_vects.rows, points_so_far = 0;
			double total_err = 0, err;

			for (i = 0; i < image_count; i++)
			{
				CvMat object_points_i = new CvMat(), image_points_i = new CvMat(), image_points2_i = new CvMat();
				int point_count = (int)cvlib.CvGet2D(ref point_counts, 0, i).val1;
				CvMat rot_vect = new CvMat(), trans_vect = new CvMat();

				cvlib.CvGetCols(ref object_points, ref object_points_i,
						points_so_far, points_so_far + point_count);
				cvlib.CvGetCols(ref image_points, ref image_points_i,
						points_so_far, points_so_far + point_count);
				cvlib.CvGetCols(ref image_points2, ref image_points2_i,
						points_so_far, points_so_far + point_count);
				points_so_far += point_count;

				cvlib.CvGetRow(ref rot_vects, ref rot_vect, i);
				cvlib.CvGetRow(ref trans_vects, ref trans_vect, i);

				cvlib.CvProjectPoints2(ref object_points_i, ref rot_vect, ref trans_vect,
													ref camera_matrix, ref dist_coeffs, ref image_points2_i);
				
				err = cvlib.CvNorm(ref image_points_i, ref image_points2_i, cvlib.CV_L1, IntPtr.Zero);
				if (per_view_errors.data != IntPtr.Zero)
					cvlib.CvSetReal2D(ref per_view_errors, 0, i, err / (double)point_count);
				total_err += err;
			}

			cvlib.CvReleaseMat(ref image_points2);
			return total_err / (double)points_so_far;
		}

		private void SaveCameraParams(string out_filename,
			int image_count, CvSize img_size, CvSize board_size, float square_size,
			float aspect_ratio, int flags, CvMat camera_matrix, CvMat dist_coeffs,
			CvMat extr_params, CvPoint2D32f[] image_points_seq, CvMat reproj_errs,
			double avg_reproj_err)
		{
			CvFileStorage fs = cvlib.CvOpenFileStorage(out_filename, cvlib.CV_STORAGE_WRITE);
			GCHandle h = new GCHandle();
			CvAttrList al = new CvAttrList();
			al.attr = IntPtr.Zero;
			al.next = IntPtr.Zero;
			h = GCHandle.Alloc(al, GCHandleType.Pinned);
			al.ptr = h.AddrOfPinnedObject();
			h.Free();

			cvlib.CvWriteString(ref fs, "calibration_time", DateTime.Now.ToString(), 0);

			cvlib.CvWriteInt(ref fs, "image_count", image_count);
			cvlib.CvWriteInt(ref fs, "image_width", img_size.width);
			cvlib.CvWriteInt(ref fs, "image_height", img_size.height);
			cvlib.CvWriteInt(ref fs, "board_width", board_size.width);
			cvlib.CvWriteInt(ref fs, "board_height", board_size.height);
			cvlib.CvWriteReal(ref fs, "square_size", square_size);

			if ((flags & cvlib.CV_CALIB_FIX_ASPECT_RATIO) > 0)
				cvlib.CvWriteReal(ref fs, "aspect_ratio", aspect_ratio);

			if (flags != 0)
			{
				string buf = ("flags: ") +
						(((flags & cvlib.CV_CALIB_USE_INTRINSIC_GUESS) > 0) ? "+use_intrinsic_guess" : "") +
						(((flags & cvlib.CV_CALIB_FIX_ASPECT_RATIO) > 0) ? "+fix_aspect_ratio" : "") +
						(((flags & cvlib.CV_CALIB_FIX_PRINCIPAL_POINT) > 0) ? "+fix_principal_point" : "") +
						(((flags & cvlib.CV_CALIB_ZERO_TANGENT_DIST) > 0) ? "+zero_tangent_dist" : "");
				cvlib.CvWriteComment(ref fs, buf, 0);
			}

			cvlib.CvWriteInt(ref fs, "flags", flags);

			cvlib.CvWrite(ref fs, "camera_matrix", camera_matrix.ptr, al);
			cvlib.CvWrite(ref fs, "distortion_coefficients", dist_coeffs.ptr, al);

			cvlib.CvWriteReal(ref fs, "avg_reprojection_error", avg_reproj_err);
			if (reproj_errs.data != IntPtr.Zero)
				cvlib.CvWrite(ref fs, "per_view_reprojection_errors", reproj_errs.ptr, al);

			if (extr_params.data != IntPtr.Zero)
			{
				cvlib.CvWriteComment(ref fs, "a set of 6-tuples (rotation vector + translation vector) for each view", 0);
				cvlib.CvWrite(ref fs, "extrinsic_parameters", extr_params.ptr, al);
			}

			cvlib.CvWriteComment(ref fs, "End of File", 0);
			cvlib.CvReleaseFileStorage(ref fs);
		}

		/// <summary>
		/// Undistore Image
		/// </summary>
		/// <param name="image">Input image</param>
		/// <returns>Result image</returns>
		public IplImage Undistort(IplImage image)
		{
			CvFileNode node;

			if (!System.IO.File.Exists(Application.StartupPath + "\\CamCalibParams.txt"))
			{
				cvlib.CvReleaseImage(ref image);
				throw new Exception("File: " + Application.StartupPath + "\\CamCalibParams.txt does not exist.\n" +
					"Please run Calibration (Teach) first.");
			}
			CvFileStorage fs = cvlib.CvOpenFileStorage(Application.StartupPath + "\\CamCalibParams.txt", cvlib.CV_STORAGE_READ);
			
			node = cvlib.CvGetFileNodeByName(ref fs, "camera_matrix");
			CvTypeInfo ty = (CvTypeInfo)cvtools.ConvertPtrToStructure(node.info, typeof(CvTypeInfo));
			CvMat camera = (CvMat)cvtools.ConvertPtrToStructure(cvlib.CvRead(ref fs, ref node), typeof(CvMat));
			
			node = cvlib.CvGetFileNodeByName(ref fs, "distortion_coefficients");
			CvMat dist_coeffs = (CvMat)cvtools.ConvertPtrToStructure(cvlib.CvRead(ref fs, ref node), typeof(CvMat));
			
			IplImage t = cvlib.CvCloneImage(ref image);
			cvlib.CvUndistort2(ref t, ref image, ref camera, ref dist_coeffs);

			cvlib.CvReleaseImage(ref t);
			cvlib.CvReleaseFileStorage(ref fs);
			return image;
		}
	}
}
