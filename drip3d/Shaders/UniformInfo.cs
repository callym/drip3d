using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace drip3d.Shaders
{
	public class UniformInfo
	{
		public String Name = "";
		public int Address = -1;
		public int Size = 0;
		public ActiveUniformType Type;
	}

	public abstract class UniformVariable : ShaderVariable
	{

	}

	public abstract class UniformMatrix : ShaderVariable
	{
		public bool Transpose = false;
	}

	public class UniformMatrix4 : UniformMatrix
	{
		public Matrix4 Value;
		public override void Update(ShaderProgram shader, int i = -1)
		{
			string s = String.Format(Name, i);
			if (shader.GetUniform(s) != -1)
			{
				GL.UniformMatrix4(shader.GetUniform(s), Transpose, ref Value);
			}
		}
	}

	public class UniformTexture2D : UniformVariable
	{
		public int Value;

		public override void Update(ShaderProgram shader, int i = -1)
		{
			string s = String.Format(Name, i);
			if (shader.GetUniform(s) != -1)
			{
				GL.BindTexture(TextureTarget.Texture2D, Value);
				GL.Uniform1(shader.GetUniform(s), Value);
			}
		}
	}

	public class UniformInt : UniformVariable
	{
		public int Value;
		public override void Update(ShaderProgram shader, int i = -1)
		{
			string s = String.Format(Name, i);
			if (shader.GetUniform(s) != -1)
			{
				GL.Uniform1(shader.GetUniform(s), Value);
			}
		}
	}

	public class UniformFloat : UniformVariable
	{
		public float Value;
		public override void Update(ShaderProgram shader, int i = -1)
		{
			string s = String.Format(Name, i);
			if (shader.GetUniform(s) != -1)
			{
				GL.Uniform1(shader.GetUniform(s), Value);
			}
		}
	}

	public class UniformVector3 : UniformVariable
	{
		public Vector3 Value;
		public override void Update(ShaderProgram shader, int i = -1)
		{
			string s = String.Format(Name, i);
			if (shader.GetUniform(s) != -1)
			{
				GL.Uniform3(shader.GetUniform(s), Value);
			}
		}
	}

	public class UniformVector4 : UniformVariable
	{
		public Vector4 Value;
		public override void Update(ShaderProgram shader, int i = -1)
		{
			string s = String.Format(Name, i);
			if (shader.GetUniform(s) != -1)
			{
				GL.Uniform4(shader.GetUniform(s), Value);
			}
		}
	}
}
