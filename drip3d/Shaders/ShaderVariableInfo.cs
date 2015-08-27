using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;

namespace drip3d.Shaders
{
	public abstract class ShaderVariable
	{
		public String Name = "";

		public abstract void Update(ShaderProgram shader, int i = -1);
	}
}
