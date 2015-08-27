using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using SOP;
using OpenTK.Graphics.OpenGL;

namespace drip3d
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			SOP.SOP.SOP_SetProfile("Sharp OpenGL", Utils.GetExecutableName());
			Console.WriteLine("Nvidia Optimus profile set!");
			
			using (Game game = new Game())
			{
				game.Run(30.0, 30.0);
			}
		}
	}
}
