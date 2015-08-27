using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace drip3d
{
	public static partial class Utils
	{
		public static void CheckError()
		{
			ErrorCode ec = GL.GetError();
			if (ec != 0)
			{
				Console.WriteLine(ec.ToString());
				throw new System.Exception(ec.ToString());
			}
		}

		public static string GetExecutableName()
		{
			string path = Process.GetCurrentProcess().MainModule.FileName;
			int lastPos = path.LastIndexOf('\\');

			return path.Substring(lastPos + 1, path.Length - lastPos - 1);
		}

		public static void Populate<T>(this T[] arr, T value)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = value;
			}
		}
	}
}
