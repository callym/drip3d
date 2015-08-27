using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace drip3d.Shaders
{
	public class AttributeInfo
	{
		public String Name = "";
		public int Address = -1;
		public int Size = 0;
		public ActiveAttribType Type;
	}

	public abstract class AttributeVariable : ShaderVariable
	{
		public bool Normalised = true;
	}

	public class AttributeVector2 : AttributeVariable
	{
		public Vector2[] Value;
		public override void Update(ShaderProgram shader, int i = -1)
		{
			if (shader.GetAttribute(Name) != -1)
			{
				GL.BindBuffer(BufferTarget.ArrayBuffer, shader.GetBuffer(Name));
				GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(Value.Length * Vector2.SizeInBytes), Value, BufferUsageHint.StaticDraw);
				GL.VertexAttribPointer(shader.GetAttribute(Name), 2, VertexAttribPointerType.Float, Normalised, 0, 0);
			}
		}
	}

	public class AttributeVector3 : AttributeVariable
	{
		public Vector3[] Value;
		public override void Update(ShaderProgram shader, int i = -1)
		{
			if (shader.GetAttribute(Name) != -1)
			{
				GL.BindBuffer(BufferTarget.ArrayBuffer, shader.GetBuffer(Name));
				GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(Value.Length * Vector3.SizeInBytes), Value, BufferUsageHint.StaticDraw);
				GL.VertexAttribPointer(shader.GetAttribute(Name), 3, VertexAttribPointerType.Float, Normalised, 0, 0);
			}
		}
	}
}
