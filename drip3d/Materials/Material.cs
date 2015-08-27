using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using System.Drawing;
using System.IO;

namespace drip3d.Materials
{
	public class Material
	{
		public Vector3 AmbientColor = new Vector3();
		public Vector3 DiffuseColor = new Vector3();
		public Vector3 SpecularColor = new Vector3();

		public float SpecularExponent = 1;
		public float Opacity = 1.0f;

		public string DiffuseTextureFile = "blank.png";
		public string SpecularTextureFile = "blank.png";
		public string OpacityTextureFile = "blank.png";
		public string NormalTextureFile = "blank.png";
		public List<string> Textures
		{
			get
			{
				return new List<string>()
				{
					DiffuseTextureFile,
					SpecularTextureFile,
					OpacityTextureFile,
					NormalTextureFile
				};
			}
		}

		public Material()
		{
			AmbientColor = Utils.Colors.ToVector(Color.Black);
			DiffuseColor = Utils.Colors.ToVector(Color.DeepPink);
			SpecularColor = Utils.Colors.ToVector(Color.White);
		}

		public static Dictionary<string, Material> LoadFromFile(string filename)
		{
			var materials = new Dictionary<string, Material>();

			try
			{
				string currentMaterial = "";
				string path = Path.Combine("Assets", "Materials", filename);
				using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
				{
					string currentLine;

					while (!reader.EndOfStream)
					{
						currentLine = reader.ReadLine();

						if (!currentLine.StartsWith("newmtl"))
						{
							if (currentMaterial.StartsWith("newmtl"))
							{
								currentMaterial += currentLine + "\n";
							}
						}
						else
						{
							if (currentMaterial.Length > 0)
							{
								Material newMaterial = new Material();
								string newMaterialName = "";

								newMaterial = LoadFromString(currentMaterial, out newMaterialName);

								materials.Add(newMaterialName, newMaterial);
							}
							currentMaterial = currentLine + "\n";
						}
					}
				}

				if (currentMaterial.Count((char c) => c == '\n') > 0)
				{
					Material newMaterial = new Material();
					string newMaterialName = "";

					newMaterial = LoadFromString(currentMaterial, out newMaterialName, filename);

					materials.Add(newMaterialName, newMaterial);
				}
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine("!!! ERROR: mtl file not found ({0}) !!!", filename);
			}
			catch (Exception e)
			{
				Console.WriteLine("!!! ERROR: cannot load mtl ({0}) !!!", filename);
			}

			return materials;
		}

		public static Material LoadFromString(string material, out string name, string filename = null)
		{
			Material output = new Material();
			name = "";

			List<string> lines = material.Split('\n').ToList();

			lines = lines.SkipWhile(s => !s.StartsWith("newmtl ")).ToList();

			if (lines.Count != 0)
			{
				name = lines[0].Substring("newmtl ".Length);
			}

			lines = lines.Select((string s) => s.Trim()).ToList();

			foreach (string l in lines)
			{
				if (l.Length < 3 || l.StartsWith("//") || l.StartsWith("#"))
				{
					continue;
				}

				// ambient color
				if (l.StartsWith("Ka"))
				{
					output.AmbientColor = ParseColor(l, filename);
				}

				// diffuse color
				if (l.StartsWith("Kd"))
				{
					output.DiffuseColor = ParseColor(l, filename);
				}

				// specular color
				if (l.StartsWith("Ks"))
				{
					output.SpecularColor = ParseColor(l, filename);
				}

				// specular exponent
				if (l.StartsWith("Ns"))
				{
					float exponent = 0.0f;
					bool success = float.TryParse(l.Substring(3), out exponent);

					output.SpecularExponent = exponent;

					if (!success)
					{
						if (filename != null)
						{
							Console.WriteLine("!!! ERROR: cannot parse specular exponent (line: {0}, file: {1}) !!!",
											l, filename);
						}
						else
						{
							Console.WriteLine("!!! ERROR: cannot parse specular exponent (line: {0}, from string) !!!",
											l);
						}
					}
				}

				// diffuse map
				if (l.StartsWith("map_Kd"))
				{
					// Check that file name is present
					if (l.Length > "map_Kd".Length + 6)
					{
						output.DiffuseTextureFile = l.Substring("map_Kd".Length + 1);
					}
				}

				// specular map
				if (l.StartsWith("map_Ks"))
				{
					// Check that file name is present
					if (l.Length > "map_Ks".Length + 6)
					{
						output.SpecularTextureFile = l.Substring("map_Ks".Length + 1);
					}
				}

				// normal map
				if (l.StartsWith("map_normal"))
				{
					// Check that file name is present
					if (l.Length > "map_normal".Length + 6)
					{
						output.NormalTextureFile = l.Substring("map_normal".Length + 1);
					}
				}

				// opacity map
				if (l.StartsWith("map_opacity"))
				{
					// Check that file name is present
					if (l.Length > "map_opacity".Length + 6)
					{
						output.OpacityTextureFile = l.Substring("map_opacity".Length + 1);
					}
				}
			}

			return output;
		}

		static Vector3 ParseColor(string line, string filename)
		{
			string[] colorParts = line.Substring(3).Split(' ');

			if (colorParts.Length < 3)
			{
				InvalidColorData(line, filename);
			}

			Vector3 v = new Vector3();

			bool success = float.TryParse(colorParts[0], out v.X);
			success |= float.TryParse(colorParts[1], out v.Y);
			success |= float.TryParse(colorParts[2], out v.Z);

			if (!success)
			{
				InvalidColorData(line, filename);
			}

			return v;
		}

		static void InvalidColorData(string line, string filename = null)
		{
			string error = String.Format("!!! ERROR: invalid color data (line: {0}, from string) !!!",
									line);
			if (filename != null)
			{
				error = String.Format("!!! ERROR: invalid color data (line: {0}, file: {1}) !!!",
									line, filename);
			}
			Console.WriteLine(error);
			throw new ArgumentException(error);
		}
	}
}
