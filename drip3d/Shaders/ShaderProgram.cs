using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace drip3d.Shaders
{
	public class ShaderProgram
	{
		public int ProgramID { get; private set; }
		public int VertexShaderID { get; private set; }
		public int FragmentShaderID { get; private set; }
		public int AttributeCount { get; private set; }
		public int UniformCount { get; private set; }

		public Dictionary<String, AttributeInfo> Attributes = new Dictionary<string, AttributeInfo>();
		public Dictionary<String, UniformInfo> Uniforms = new Dictionary<string, UniformInfo>();
		public Dictionary<String, uint> Buffers = new Dictionary<string, uint>();

		public string Name;

		public ShaderProgram()
		{
			ProgramID = -1;
			VertexShaderID = -1;
			FragmentShaderID = -1;
			AttributeCount = 0;
			UniformCount = 0;

			ProgramID = GL.CreateProgram();
		}

		public ShaderProgram(string name, string fragmentShader, string vertexShader, bool fromFile = true) : this()
		{
			Name = name;
			if (fromFile)
			{
				LoadShaderFromFile(vertexShader + "Vertex", ShaderType.VertexShader);
				LoadShaderFromFile(fragmentShader + "Fragment", ShaderType.FragmentShader);
			}
			else
			{
				LoadShaderFromString(vertexShader, ShaderType.VertexShader);
				LoadShaderFromString(fragmentShader, ShaderType.FragmentShader);
			}

			Link();
			GenerateBuffers();
		}

		public ShaderProgram(string name, string filename) : this()
		{
			Name = name;
			LoadShaderFromFile(filename + "Vertex", ShaderType.VertexShader);
			LoadShaderFromFile(filename + "Fragment", ShaderType.FragmentShader);

			Link();
			GenerateBuffers();
		}

		public void UpdateFrame()
		{

		}

		private void LoadShader(String code, ShaderType type, out int address, string filename = null)
		{
			address = GL.CreateShader(type);
			GL.ShaderSource(address, code);
			GL.CompileShader(address);
			GL.AttachShader(ProgramID, address);
			GL.DeleteShader(address);
			string log = GL.GetShaderInfoLog(address);
			if (log.Length != 0)
			{
				if (filename == null)
				{
					Console.WriteLine("!!! ERROR: shader threw following errors (from string) !!!");
				}
				else
				{
					Console.WriteLine($"!!! ERROR: shader threw following errors ({filename}.glsl) !!!");
				}
				Console.WriteLine(log);
			}
		}

		public void LoadShaderFromString(String code, ShaderType type)
		{
			int id = -1;
			LoadShader(code, type, out id);
			if (type == ShaderType.VertexShader)
			{
				VertexShaderID = id;
			}
			else if (type == ShaderType.FragmentShader)
			{
				FragmentShaderID = id;
			}
			Console.WriteLine("*** loaded shader from string ***");
		}

		public void LoadShaderFromFile(String filename, ShaderType type)
		{
			string path = Path.Combine("Assets", "Shaders", filename + ".glsl");
			using (StreamReader sr = new StreamReader(path))
			{
				int id = -1;
				LoadShader(sr.ReadToEnd(), type, out id, filename);
				if (type == ShaderType.VertexShader)
				{
					VertexShaderID = id;
				}
				else if (type == ShaderType.FragmentShader)
				{
					FragmentShaderID = id;
				}
			}
			Console.WriteLine($"*** loaded shader from file ({filename}.glsl) ***");
		}

		public void Link()
		{
			GL.LinkProgram(ProgramID);

			string log = GL.GetProgramInfoLog(ProgramID);
			if (log.Length != 0)
			{
				Console.WriteLine(log);
			}

			int attributeCount, uniformCount;
			GL.GetProgram(ProgramID, GetProgramParameterName.ActiveAttributes, out attributeCount);
			GL.GetProgram(ProgramID, GetProgramParameterName.ActiveUniforms, out uniformCount);

			AttributeCount = attributeCount;
			UniformCount = uniformCount;

			for (int i = 0; i < AttributeCount; i++)
			{
				AttributeInfo info = new AttributeInfo();
				int length = 0;

				StringBuilder name = new StringBuilder(64);

				GL.GetActiveAttrib(ProgramID, i, 512, out length, out info.Size, out info.Type, name);

				info.Name = name.ToString();
				Attributes.Add(name.ToString(), info);
				info.Address = GL.GetAttribLocation(ProgramID, info.Name);
			}

			for (int i = 0; i < UniformCount; i++)
			{
				UniformInfo info = new UniformInfo();
				int length = 0;

				StringBuilder name = new StringBuilder(64);

				GL.GetActiveUniform(ProgramID, i, 512, out length, out info.Size, out info.Type, name);

				info.Name = name.ToString();
				Uniforms.Add(name.ToString(), info);
				info.Address = GL.GetUniformLocation(ProgramID, info.Name);
			}
		}

		public void GenerateBuffers()
		{
			for (int i = 0; i < Attributes.Count; i++)
			{
				uint buffer = 0;
				GL.GenBuffers(1, out buffer);

				Buffers.Add(Attributes.Values.ElementAt(i).Name, buffer);
			}

			for (int i = 0; i < Uniforms.Count; i++)
			{
				uint buffer = 0;
				GL.GenBuffers(1, out buffer);

				Buffers.Add(Uniforms.Values.ElementAt(i).Name, buffer);
			}
		}

		public void EnableVertexAttribArrays()
		{
			for (int i = 0; i < Attributes.Count; i++)
			{
				GL.EnableVertexAttribArray(Attributes.Values.ElementAt(i).Address);
			}
		}

		public void DisableVertexAttribArrays()
		{
			for (int i = 0; i < Attributes.Count; i++)
			{
				GL.DisableVertexAttribArray(Attributes.Values.ElementAt(i).Address);
			}
		}

		public int GetAttribute(string name)
		{
			if (Attributes.ContainsKey(name))
			{
				return Attributes[name].Address;
			}
			else
			{
				return -1;
			}
		}

		public int GetUniform(string name)
		{
			if (Uniforms.ContainsKey(name))
			{
				return Uniforms[name].Address;
			}
			else
			{
				return -1;
			}
		}

		public uint GetBuffer(string name)
		{
			if (Buffers.ContainsKey(name))
			{
				return Buffers[name];
			}
			else
			{
				return 0;
			}
		}
	}
}
