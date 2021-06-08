//**********************************************************
// File name: $ Math.cs $
// Author:		$ Heiko Kieﬂling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Simple example how to generate random numbers.
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

namespace CvExample
{
	public class Rand
	{
		private MainForm form;

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="form">main form</param>
		public Rand(MainForm form)
		{
			this.form = form;
		}

		public double GetDouble()
		{
			ulong n = cvlib.CvRNG(12);
			return cvlib.CvRandReal(n);
		}
	}
}
