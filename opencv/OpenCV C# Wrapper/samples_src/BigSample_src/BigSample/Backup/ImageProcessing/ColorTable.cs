//**********************************************************
// File name: $ Colortable.cs $
// Author:		$ Heiko Kießling, (c) iib-chemnitz.de $
// Email:			hki@hrz.tu-chemnitz.de
// 
// Purpose:		Provides color conversion between r/g/b and hsv system
//						Note that the passed pre[] array can be used as to save
//						the last returned state for error recovering.
//						Additional, color tables can be generated. If -1 is
//						passed to the constructor a table with 255 randomly
//						filled values will be generated. If theres a argument
//						> 0 a blue to to red rainbow color table will be
//						generated. 
// 
// License:		There is no explicit license attached. Feel free
//						to use the code how you like but without any warranty.
//						If you include the code in your own projects and/or
//						redistribute pls. include this header.
//
// History:		Rev. 0.9 (beta), hki - rainbow table
//						0.91, hki - random table
//						0.92, hki - some math tables (square root, x^2)
//						1.0, hki - math tables released
//						(replaced later)
//**********************************************************


using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Tools
{
	class Tables
	{
		// the color table
		private Color[] table;

		/// <summary>
		/// return whole color table
		/// </summary>
		public Color[] Table
		{
			get { return table; }
		}

		// number of colors
		private int nColors;

		/// <summary>
		/// default constructor for 256 colors
		/// </summary>
		public Tables() 
		{
			table = new Color[256];
			nColors = -1;
			CalcColorTable();
		}

		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="nColors">Number of colors in the table</param>
		public Tables(int nColors)
		{
			table = new Color[nColors];
			this.nColors = nColors;
			CalcColorTable();
		}

		/// <summary>
		/// return color at specific position in the color table
		/// </summary>
		/// <param name="pos">position</param>
		/// <returns>Color</returns>
		public Color getColor(int pos)
		{
			if (pos >= 0 && pos < table.Length)
			{
				return table[pos];
			}
			else
			{
				throw new IndexOutOfRangeException("Get Color");
			}
		}

		/// <summary>
		/// Convert Color from r,g,b to hsv-system
		/// </summary>
		/// <param name="r">Red Part</param>
		/// <param name="g">Green Part</param>
		/// <param name="b">Blue Part</param>
		/// <param name="pre">Returned if undefined color</param>
		/// <returns>hsv value (h=index 0, s=index 1...</returns>
		public static double[] ConvertColor(byte r, byte g, byte b, double[] pre)
		{
			double[] res = new double[3];
			double min, max, delta;

			min = Math.Min(r, g);
			min = Math.Min(min, (double)b);
			max = Math.Max(r, g);
			max = Math.Max(max, (double)b);
			res[2] = max;				// v

			delta = max - min;

			if (max != 0)
				res[1] = (delta / max);		// s
			else
			{
				// r = g = b = 0		// s = 0, v is undefined
				res[1] = 0;
				res[0] = -1;
				return pre;
			}

			if (r == g && g == b && r == b)
			{
				res[0] = 0;
				return pre;
			}

			if (r == max)
				res[0] = (g - b) / delta;		// between yellow & magenta
			else if (g == max)
				res[0] = 2 + (b - r) / delta;	// between cyan & yellow
			else
				res[0] = 4 + (r - g) / delta;	// between magenta & cyan

			res[0] *= 60;				// degrees
			if (res[0] < 0)
				res[0] += 360;
			return res;
		}

		/// <summary>
		/// Set up color table with colors in a rainbow manner
		/// or if default constructor used with 255 random colors
		/// </summary>
		private void CalcColorTable()
		{
			double hi, f;
			int i;
			byte r, g, b, v, p, q, t;

			if (nColors == -1)
			{
				for (int h = 0; h < 256; h++)
				{
					hi = ((double)h / 60);
					i = (int)Math.Floor(hi);
					f = hi - (double)i;
					p = 0;
					q = (byte)((1 - f) * 255);
					t = (byte)(f * 255);
					v = 255;
					switch (i)
					{
						case 0:
							r = v; g = t; b = p;
							break;
						case 1:
							r = q; g = v; b = p;
							break;
						case 2:
							r = p; g = v; b = t;
							break;
						case 3:
							r = p; g = q; b = v;
							break;
						case 4:
							r = t; g = p; b = v;
							break;
						case 5:
							r = v; g = p; b = q;
							break;
						case 6:
							r = v; g = t; b = p;
							break;
						case -1:
							r = v; g = p; b = q;
							break;
						default: // should not go here
							throw new Exception("Value error in Pixel conversion");
					}
					table[255 - h] = Color.FromArgb(r, g, b);
				}
			}
			else
			{
				Random rng = new Random();
				for (int h = 0; h < nColors; h++)
				{
					r = (byte)rng.Next(0, 255);
					g = (byte)rng.Next(0, 255);
					b = (byte)rng.Next(0, 255);
					if ((r == 255 && g == 255 && b == 255) ||
						(r == 0 && g == 0 && b == 0)) // reserved colors
						h--;
					else
						table[h] = Color.FromArgb(r, g, b);
				}
			}
		}
	}
}
