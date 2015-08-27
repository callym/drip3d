using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using System.Drawing;

namespace drip3d
{
	public static partial class Utils
	{
		public static class Colors
		{
			public static Vector3 ToVector(Color color)
			{
				return new Vector3
				(
					color.R / 255.0f,
					color.G / 255.0f,
					color.B / 255.0f
				);
			}

			public static Color ToColor(Vector3 v)
			{
				return Color.FromArgb
				(
					(int)v.X * 255,
					(int)v.Y * 255,
					(int)v.Z * 255
				);
			}
		}
	}
}
