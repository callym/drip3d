using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using drip3d.Shaders;

namespace drip3d
{
	public sealed class ShaderManager
	{
		public List<ShaderProgram> Shaders = new List<ShaderProgram>();

		public int CurrentShaderIndex { get; private set; } = -1;
		public string CurrentShaderName { get { return Shaders[CurrentShaderIndex].Name; } }
		public ShaderProgram CurrentShader { get { return Shaders[CurrentShaderIndex]; } }

		public int NextShaderIndex { get; private set; } = -1;
		public string NextShaderName
		{
			get
			{
				if (NextShaderIndex != -1)
				{
					return Shaders[NextShaderIndex].Name;
				}
				return null;
			}
		}
		public ShaderProgram NextShader
		{
			get
			{
				if (NextShaderIndex != -1)
				{
					return Shaders[NextShaderIndex];
				}
				return null;
			}
		}

		ShaderManager() { }

		public void Init()
		{
			CurrentShaderIndex = 0;
		}

		public int GetShaderIndex(string name)
		{
			for (int i = 0; i < Shaders.Count; i++)
			{
				if (Shaders[i].Name == name)
				{
					return i;
				}
			}
			return -1;
		}

		public ShaderProgram GetShader(string name)
		{
			int i = GetShaderIndex(name);

			if (i != -1)
			{
				return Shaders[i];
			}

			return null;
		}

		public void SetShader(string name)
		{
			int i = GetShaderIndex(name);
			if (i != -1)
			{
				NextShaderIndex = i;
			}
		}
		public void SetShader(int index)
		{
			if (index < Shaders.Count && index >= 0)
			{
				NextShaderIndex = index;
			}
		}

		public void ChangeToNextShader()
		{
			int i = CurrentShaderIndex + 1;
			if (i >= Shaders.Count)
			{
				i = 0;
			}
			SetShader(i);
		}

		public void ChangeToPreviousShader()
		{
			int i = CurrentShaderIndex - 1;
			if (i < 0)
			{
				i = Shaders.Count - 1;
			}
			SetShader(i);
		}

		public void SwitchShaders()
		{
			if (NextShaderIndex != -1)
			{
				Console.WriteLine($"*** SWITCHING FROM {Shaders[CurrentShaderIndex].Name} TO {Shaders[NextShaderIndex].Name} ***");
				CurrentShaderIndex = NextShaderIndex;
				NextShaderIndex = -1;
			}
		}

		public static ShaderManager Instance { get { return Nested.instance; } }
		class Nested
		{
			// Explicit static constructor to tell C# compiler
			// not to mark type as beforefieldinit
			static Nested() { }
			internal static readonly ShaderManager instance = new ShaderManager();
		}
	}
}
