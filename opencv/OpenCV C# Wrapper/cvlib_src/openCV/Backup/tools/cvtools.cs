//**********************************************************
// File name: $ cvtools.cs $
// Author:		$ Heiko Kieﬂling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Conversion between managed data types and 
// 						IntPtr and vice versa
// License:		There is no explicit license attached. Feel free
//						to use the code how you like but without any warranty.
//						If you include the code in your own projects and/or
//						redistribute pls. include this header.
// History:		Rev. 1.0.2 (beta), hki
//**********************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace openCV
{
	/// <summary>
	/// CvTools adds support for data type conversion
	/// between managed and unmanaged memory and vise versa
	/// </summary>
	public class cvtools
	{
		/// <summary>
		/// Converts a Jagged Array (array of arrays) to an IntPtr.
		/// The array can read and/or written in unmanaged dll.
		/// Internal memory managements expects to release the
		/// resourses after using. Simply make a variable GCHandel[] h
		/// without initalisation and pass to the function. After using 
		/// simply call ReleaseHandels(h).
		/// </summary>
		/// <param name="arr">2d Jagged Array</param>
		/// <param name="handles">Handle</param>
		/// <returns>IntPtr, like byte**</returns>
		public static IntPtr Convert2DArrToPtr(byte[][] arr, out GCHandle[] handles)
		{
			int i;
			IntPtr[] ptrarr;

			handles = new GCHandle[arr.GetLength(0) + 1];
			ptrarr = new IntPtr[arr.GetLength(0)];

			for (i = 0; i < arr.GetLength(0); i++)
			{
				handles[i] = GCHandle.Alloc(arr[i], GCHandleType.Pinned);
				ptrarr[i] = handles[i].AddrOfPinnedObject();
			}
			handles[i] = GCHandle.Alloc(ptrarr, GCHandleType.Pinned);
			return handles[i].AddrOfPinnedObject();
		}

		/// <summary>
		/// Converts a Jagged Array (array of arrays) to an IntPtr.
		/// The array can read and/or written in unmanaged dll.
		/// Internal memory managements expects to release the
		/// resourses after using. Simply make a variable GCHandel[] h
		/// without initalisation and pass to the function. After using 
		/// simply call ReleaseHandels(h).
		/// </summary>
		/// <param name="arr">2d Jagged Array</param>
		/// <param name="handles">Handle</param>
		/// <returns>IntPtr, like int**</returns>
		public static IntPtr Convert2DArrToPtr(int[][] arr, out GCHandle[] handles)
		{
			int i;
			IntPtr[] ptrarr;

			handles = new GCHandle[arr.GetLength(0) + 1];
			ptrarr = new IntPtr[arr.GetLength(0)];

			for (i = 0; i < arr.GetLength(0); i++)
			{
				handles[i] = GCHandle.Alloc(arr[i], GCHandleType.Pinned);
				ptrarr[i] = handles[i].AddrOfPinnedObject();
			}
			handles[i] = GCHandle.Alloc(ptrarr, GCHandleType.Pinned);
			return handles[i].AddrOfPinnedObject();
		}

		/// <summary>
		/// Converts a Jagged Array (array of arrays) to an IntPtr.
		/// The array can read and/or written in unmanaged dll.
		/// Internal memory managements expects to release the
		/// resourses after using. Simply make a variable GCHandel[] h
		/// without initalisation and pass to the function. After using 
		/// simply call ReleaseHandels(h).
		/// </summary>
		/// <param name="arr">2d Jagged Array</param>
		/// <param name="handles">Handle</param>
		/// <returns>IntPtr, like float**</returns>
		public static IntPtr Convert2DArrToPtr(float[][] arr, out GCHandle[] handles)
		{
			int i;
			IntPtr[] ptrarr;

			handles = new GCHandle[arr.GetLength(0) + 1];
			ptrarr = new IntPtr[arr.GetLength(0)];

			for (i = 0; i < arr.GetLength(0); i++)
			{
				handles[i] = GCHandle.Alloc(arr[i], GCHandleType.Pinned);
				ptrarr[i] = handles[i].AddrOfPinnedObject();
			}
			handles[i] = GCHandle.Alloc(ptrarr, GCHandleType.Pinned);
			return handles[i].AddrOfPinnedObject();
		}

		/// <summary>
		/// Converts a Jagged Array (array of arrays) to an IntPtr.
		/// The array can read and/or written in unmanaged dll.
		/// Internal memory managements expects to release the
		/// resourses after using. Simply make a variable GCHandel[] h
		/// without initalisation and pass to the function. After using 
		/// simply call ReleaseHandels(h).
		/// </summary>
		/// <param name="arr">2d Jagged Array</param>
		/// <param name="handles">Handle</param>
		/// <returns>IntPtr, like double**</returns>
		public static IntPtr Convert2DArrToPtr(double[][] arr, out GCHandle[] handles)
		{
			int i;
			IntPtr[] ptrarr;

			handles = new GCHandle[arr.GetLength(0) + 1];
			ptrarr = new IntPtr[arr.GetLength(0)];

			for (i = 0; i < arr.GetLength(0); i++)
			{
				handles[i] = GCHandle.Alloc(arr[i], GCHandleType.Pinned);
				ptrarr[i] = handles[i].AddrOfPinnedObject();
			}
			handles[i] = GCHandle.Alloc(ptrarr, GCHandleType.Pinned);
			return handles[i].AddrOfPinnedObject();
		}

		/// <summary>
		/// Converts a Jagged Array (array of arrays) to an IntPtr.
		/// The array can read and/or written in unmanaged dll.
		/// Internal memory managements expects to release the
		/// resourses after using. Simply make a variable GCHandel[] h
		/// without initalisation and pass to the function. After using 
		/// simply call ReleaseHandels(h).
		/// </summary>
		/// <param name="arr">2d Jagged Array</param>
		/// <param name="handles">Handle</param>
		/// <returns>IntPtr, like Point**</returns>
		public static IntPtr Convert2DArrToPtr(CvPoint[][] arr, out GCHandle[] handles)
		{
			int i;
			IntPtr[] ptrarr;

			handles = new GCHandle[arr.GetLength(0) + 1];
			ptrarr = new IntPtr[arr.GetLength(0)];

			for (i = 0; i < arr.GetLength(0); i++)
			{
				handles[i] = GCHandle.Alloc(arr[i], GCHandleType.Pinned);
				ptrarr[i] = handles[i].AddrOfPinnedObject();
			}
			handles[i] = GCHandle.Alloc(ptrarr, GCHandleType.Pinned);
			return handles[i].AddrOfPinnedObject();
		}

		/// <summary>
		/// Converts a one dimensional array to an IntPtr.
		/// The array can read and/or written in unmanaged dll.
		/// Internal memory management expects to release the
		/// resourses after using. Simply make a variable GCHandel[] h
		/// without initalisation and pass to the function. After using call
		/// the pointer simply call ReleaseHandels(h).
		/// </summary>
		/// <param name="arr">1d Array</param>
		/// <param name="handle">Handle</param>
		/// <returns>IntPtr, like byte*</returns>
		public static IntPtr Convert1DArrToPtr(byte[] arr, out GCHandle handle)
		{
			handle = new GCHandle();
			handle = GCHandle.Alloc(arr, GCHandleType.Pinned);
			return handle.AddrOfPinnedObject();
		}

		/// <summary>
		/// Converts a one dimensional array to an IntPtr.
		/// The array can read and/or written in unmanaged dll.
		/// Internal memory management expects to release the
		/// resourses after using. Simply make a variable GCHandel[] h
		/// without initalisation and pass to the function. After using call
		/// the pointer simply call ReleaseHandels(h).
		/// </summary>
		/// <param name="arr">1d Array</param>
		/// <param name="handle">Handle</param>
		/// <returns>IntPtr, like int*</returns>
		public static IntPtr Convert1DArrToPtr(int[] arr, out GCHandle handle)
		{
			handle = new GCHandle();
			handle = GCHandle.Alloc(arr, GCHandleType.Pinned);
			return handle.AddrOfPinnedObject();
		}

		/// <summary>
		/// Converts a one dimensional array to an IntPtr.
		/// The array can read and/or written in unmanaged dll.
		/// Internal memory management expects to release the
		/// resourses after using. Simply make a variable GCHandel[] h
		/// without initalisation and pass to the function. After using call
		/// the pointer simply call ReleaseHandels(h).
		/// </summary>
		/// <param name="arr">1d Array</param>
		/// <param name="handle">Handle</param>
		/// <returns>IntPtr, like float*</returns>
		public static IntPtr Convert1DArrToPtr(float[] arr, out GCHandle handle)
		{
			handle = new GCHandle();
			handle = GCHandle.Alloc(arr, GCHandleType.Pinned);
			return handle.AddrOfPinnedObject();
		}

		/// <summary>
		/// Converts a one dimensional array to an IntPtr.
		/// The array can read and/or written in unmanaged dll.
		/// Internal memory management expects to release the
		/// resourses after using. Simply make a variable GCHandel[] h
		/// without initalisation and pass to the function. After using call
		/// the pointer simply call ReleaseHandels(h).
		/// </summary>
		/// <param name="arr">1d Array</param>
		/// <param name="handle">Handle</param>
		/// <returns>IntPtr, like double*</returns>
		public static IntPtr Convert1DArrToPtr(double[] arr, out GCHandle handle)
		{
			handle = new GCHandle();
			handle = GCHandle.Alloc(arr, GCHandleType.Pinned);
			return handle.AddrOfPinnedObject();
		}

		/// <summary>
		/// Converts a one dimensional array to an IntPtr.
		/// The array can read and/or written in unmanaged dll.
		/// Internal memory management expects to release the
		/// resourses after using. Simply make a variable GCHandel[] h
		/// without initalisation and pass to the function. After using call
		/// the pointer simply call ReleaseHandels(h).
		/// </summary>
		/// <param name="arr">1d Array</param>
		/// <param name="handle">Handle</param>
		/// <returns>IntPtr, like double*</returns>
		public static IntPtr Convert1DArrToPtr(CvPoint2D32f[] arr, out GCHandle handle)
		{
			handle = new GCHandle();
			handle = GCHandle.Alloc(arr, GCHandleType.Pinned);
			return handle.AddrOfPinnedObject();
		}
		
		/// <summary>
		/// Converts a one dimensional array to an IntPtr.
		/// The array can read and/or written in unmanaged dll.
		/// Internal memory management expects to release the
		/// resourses after using. Simply make a variable GCHandel[] h
		/// without initalisation and pass to the function. After using call
		/// the pointer simply call ReleaseHandels(h).
		/// </summary>
		/// <param name="arr">1d Array</param>
		/// <param name="handle">Handle</param>
		/// <returns>IntPtr, like double*</returns>
		public static IntPtr Convert1DArrToPtr(CvPoint[] arr, out GCHandle handle)
		{
			handle = new GCHandle();
			handle = GCHandle.Alloc(arr, GCHandleType.Pinned);
			return handle.AddrOfPinnedObject();
		}

		/// <summary>
		/// Return a Pointer to an object eg. structure
		/// </summary>
		/// <param name="structure">typeof(object)</param>
		/// <param name="handle">handle</param>
		/// <returns>Pointer to object</returns>
		public static IntPtr ConvertStructureToPtr(object structure, out GCHandle handle)
		{
			handle = new GCHandle();
			handle = GCHandle.Alloc(structure, GCHandleType.Pinned);
			return handle.AddrOfPinnedObject();
		}

		/// <summary>
		/// Return the Object from Pointer (must be casted to Structure)
		/// </summary>
		/// <param name="p">The pointer</param>
		/// <param name="typeOfStructure">typeof(object)</param>
		/// <returns>the object</returns>
		public static object ConvertPtrToStructure(IntPtr p, Type typeOfStructure)
		{
			return Marshal.PtrToStructure(p, typeOfStructure);
		}

		/// <summary>
		/// Converts byte Pointer to Array.
		/// The Array must be preallocated by desired size.
		/// </summary>
		/// <param name="data">Pointer to data</param>
		/// <param name="arr">Resulting array</param>
		public static void ConvertPtrToArray(IntPtr data, byte[] arr)
		{
			int size = arr.Length;
			try
			{
				Marshal.Copy(data, arr, 0, size);
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		/// Converts float Pointer to Array.
		/// The Array must be preallocated by desired size.
		/// </summary>
		/// <param name="data">Pointer to data</param>
		/// <param name="arr">Resulting array</param>
		public static void ConvertPtrToArray(IntPtr data, float[] arr)
		{
			int size = arr.Length;
			try
			{
				Marshal.Copy(data, arr, 0, size);
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		/// Converts double Pointer to Array.
		/// The Array must be preallocated by desired size.
		/// </summary>
		/// <param name="data">Pointer to data</param>
		/// <param name="arr">Resulting array</param>
		public static void ConvertPtrToArray(IntPtr data, double[] arr)
		{
			int size = arr.Length;
			try
			{
				Marshal.Copy(data, arr, 0, size);
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		/// Converts int Pointer to Array.
		/// The Array must be preallocated by desired size.
		/// </summary>
		/// <param name="data">Pointer to data</param>
		/// <param name="arr">Resulting array</param>
		public static void ConvertPtrToArray(IntPtr data, int[] arr)
		{
			int size = arr.Length;
			try
			{
				Marshal.Copy(data, arr, 0, size);
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		/// Convert IntPtr to 2D Array
		/// Array must be allocated with desired size
		/// </summary>
		/// <param name="ptr">Pointer to Data</param>
		/// <param name="arr">Array</param>
		public static void ConvertPtrTo2DArray(IntPtr ptr, double[][] arr)
		{
			int i;
			IntPtr[] ptrarr = new IntPtr[arr.GetLength(0)];

			try
			{
				for (i = 0; i < arr.GetLength(0); i++)
				{
					ptrarr[i] = new IntPtr(ptr.ToInt32() + i * sizeof(double) * arr.GetLength(1));
				}
				for (i = 0; i < arr.GetLength(0); i++)
				{
					Marshal.Copy(ptrarr[i], arr[i], 0, arr.GetLength(1));
				}
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		/// Convert IntPtr to 2D Array
		/// Array must be allocated with desired size
		/// </summary>
		/// <param name="ptr">Pointer to Data</param>
		/// <param name="arr">Array</param>
		public static void ConvertPtrTo2DArray(IntPtr ptr, float[][] arr)
		{
			int i;
			IntPtr[] ptrarr = new IntPtr[arr.GetLength(0)];

			try
			{
				for (i = 0; i < arr.GetLength(0); i++)
				{
					ptrarr[i] = new IntPtr(ptr.ToInt32() + i * sizeof(float) * arr.GetLength(1));
				}
				for (i = 0; i < arr.GetLength(0); i++)
				{
					Marshal.Copy(ptrarr[i], arr[i], 0, arr.GetLength(1));
				}
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		/// Convert IntPtr to 2D Array
		/// Array must be allocated with desired size
		/// </summary>
		/// <param name="ptr">Pointer to Data</param>
		/// <param name="arr">Array</param>
		public static void ConvertPtrTo2DArray(IntPtr ptr, int[][] arr)
		{
			int i;
			IntPtr[] ptrarr = new IntPtr[arr.GetLength(0)];

			try
			{
				for (i = 0; i < arr.GetLength(0); i++)
				{
					ptrarr[i] = new IntPtr(ptr.ToInt32() + i * sizeof(int) * arr.GetLength(1));
				}
				for (i = 0; i < arr.GetLength(0); i++)
				{
					Marshal.Copy(ptrarr[i], arr[i], 0, arr.GetLength(1));
				}
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		/// Convert IntPtr to 2D Array
		/// Array must be allocated with desired size
		/// </summary>
		/// <param name="ptr">Pointer to Data</param>
		/// <param name="arr">Array</param>
		public static void ConvertPtrTo2DArray(IntPtr ptr, byte[][] arr)
		{
			int i;
			IntPtr[] ptrarr = new IntPtr[arr.GetLength(0)];

			try
			{
				for (i = 0; i < arr.GetLength(0); i++)
				{
					ptrarr[i] = new IntPtr(ptr.ToInt32() + i * sizeof(byte) * arr.GetLength(1));
				}
				for (i = 0; i < arr.GetLength(0); i++)
				{
					Marshal.Copy(ptrarr[i], arr[i], 0, arr.GetLength(1));
				}
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		/// Release the used handle(s)
		/// </summary>
		/// <param name="h">array of handle(s) given by Convert functions</param>
		public static void ReleaseHandels(GCHandle[] h)
		{
			for (int i = 0; i < h.GetLength(0); i++) h[i].Free();
		}

		/// <summary>
		/// Release the used handle
		/// </summary>
		/// <param name="h">Handle to be released</param>
		public static void ReleaseHandel(GCHandle h)
		{
			h.Free();
		}
	}
}
