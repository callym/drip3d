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

		int _currentShaderIndex = -1;
		public int CurrentShaderIndex { get { return _currentShaderIndex; } }
		public string CurrentShaderName { get { return Shaders[_currentShaderIndex].Name; } }
		public ShaderProgram CurrentShader { get { return Shaders[_currentShaderIndex]; } }

		int _nextShaderIndex = -1;
		public int NextShaderIndex { get { return _nextShaderIndex; } }
		public string NextShaderName
		{
			get
			{
				if (_nextShaderIndex != -1)
				{
					return Shaders[_nextShaderIndex].Name;
				}
				return null;
			}
		}
		public ShaderProgram NextShader
		{
			get
			{
				if (_nextShaderIndex != -1)
				{
					return Shaders[_nextShaderIndex];
				}
				return null;
			}
		}

		ShaderManager() { }

		public void Init()
		{
			_currentShaderIndex = 0;
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
				_nextShaderIndex = i;
			}
		}
		public void SetShader(int index)
		{
			if (index < Shaders.Count && index >= 0)
			{
				_nextShaderIndex = index;
			}
		}

		public void ChangeToNextShader()
		{
			int i = _currentShaderIndex + 1;
			if (i >= Shaders.Count)
			{
				i = 0;
			}
			SetShader(i);
		}

		public void ChangeToPreviousShader()
		{
			int i = _currentShaderIndex - 1;
			if (i < 0)
			{
				i = Shaders.Count - 1;
			}
			SetShader(i);
		}

		public void SwitchShaders()
		{
			if (_nextShaderIndex != -1)
			{
				Console.WriteLine($"*** SWITCHING FROM {Shaders[_currentShaderIndex].Name} TO {Shaders[_nextShaderIndex].Name} ***");
				_currentShaderIndex = _nextShaderIndex;
				_nextShaderIndex = -1;
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
