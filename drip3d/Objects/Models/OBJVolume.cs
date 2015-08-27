using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using System.IO;

namespace drip3d.Objects.Models
{
	[Obsolete("OBJVolume is deprecated, use AssimpVolume instead.", true)]
	class OBJVolume : MaterialVolume
	{
		Vector3[] vertices;
		Vector3[] colors;
		Vector2[] textureCoords;

		List<Tuple<FaceVertex, FaceVertex, FaceVertex>> faces = new List<Tuple<FaceVertex, FaceVertex, FaceVertex>>();

		OBJVolume() : base()
		{

		}

		public override void Start()
		{
			vertices = GetVertices();
			colors = GetColorData();
			textureCoords = GetTextureCoords();

			base.Start();
		}

		public override int VerticesCount { get { return vertices.Length; } }
		public override int IndiceCount { get { return faces.Count * 3; } }
		public override int ColorDataCount { get { return colors.Length; } }

		public override Vector3[] GetVertices()
		{
			List<Vector3> vertices = new List<Vector3>();

			foreach (var f in faces)
			{
				vertices.Add(f.Item1.Position);
				vertices.Add(f.Item2.Position);
				vertices.Add(f.Item3.Position);
			}

			return vertices.ToArray();
		}

		public override int[] GetIndices(int offset = 0)
		{
			return Enumerable.Range(offset, IndiceCount).ToArray();
		}

		public override Vector3[] GetColorData()
		{
			Vector3[] colors = new Vector3[VerticesCount];
			Vector3 color = Utils.Colors.ToVector(System.Drawing.Color.Gray);
			Utils.Populate(colors, color);
			return colors;
		}

		public override Vector2[] GetTextureCoords()
		{
			List<Vector2> textureCoords = new List<Vector2>();

			foreach (var f in faces)
			{
				textureCoords.Add(f.Item1.TextureCoord);
				textureCoords.Add(f.Item2.TextureCoord);
				textureCoords.Add(f.Item3.TextureCoord);
			}

			return textureCoords.ToArray();
		}

		public static OBJVolume LoadFromFile(string filename)
		{
			OBJVolume obj = new OBJVolume();
			string path = Path.Combine("Assets", "Models", filename);

			try 
			{
				using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
				{
					obj = LoadFromString(reader.ReadToEnd(), filename);
				}
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine("!!! ERROR: obj file not found ({0}) !!!", filename);
			}
			catch (Exception e)
			{
				Console.WriteLine("!!! ERROR: cannot load obj ({0}) !!!", filename);
			}

			return obj;
		}

		public static OBJVolume LoadFromString(string obj, string filename = null)
		{
			List<string> lines = new List<string>(obj.Split('\n'));

			List<Vector3> vertices = new List<Vector3>();
			List<Vector2> textureCoords = new List<Vector2>();
			var faces = new List<Tuple<TempVertex, TempVertex, TempVertex>>();

			vertices.Add(new Vector3());
			textureCoords.Add(new Vector2());

			foreach (string l in lines)
			{
				if (l.StartsWith("v "))
				{
					// remove 'v '
					string temp = l.Substring(2);

					Vector3 v = new Vector3();

					// Check if there's enough elements for a vertex (3)
					if (temp.Count((char c) => c == ' ') == 2)
					{
						string[] vertParts = temp.Split(' ');

						bool success = float.TryParse(vertParts[0], out v.X);
						success		|= float.TryParse(vertParts[1], out v.Y);
						success		|= float.TryParse(vertParts[2], out v.Z);

						if (!success)
						{
							if (filename != null)
							{
								Console.WriteLine("!!! ERROR: cannot parse vertex (line: {0}, file: {1}) !!!",
												l, filename);
							}
							else
							{
								Console.WriteLine("!!! ERROR: cannot parse vertex (line: {0}, from string) !!!",
												l);
							}
						}
					}
					vertices.Add(v);
				}
				else if (l.StartsWith("vt "))
				{
					// cut off 'vt '
					string temp = l.Substring(3);

					Vector2 v = new Vector2();

					// Check if there's enough elements for a vertex
					if (temp.Trim().Count((char c) => c == ' ') > 0)
					{
						string[] textureCoordParts = temp.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

						bool success = float.TryParse(textureCoordParts[0], out v.X);
						success		|= float.TryParse(textureCoordParts[1], out v.Y);

						if (filename != null)
						{
							Console.WriteLine("!!! ERROR: cannot parse texture coordinate (line: {0}, file: {1}) !!!",
											l, filename);
						}
						else
						{
							Console.WriteLine("!!! ERROR: cannot parse texture coordinate (line: {0}, from string) !!!",
											l);
						}
					}
					else
					{
						if (filename != null)
						{
							Console.WriteLine("!!! ERROR: cannot parse texture coordinate (line: {0}, file: {1}) !!!",
											l, filename);
						}
						else
						{
							Console.WriteLine("!!! ERROR: cannot parse texture coordinate (line: {0}, from string) !!!",
											l);
						}
					}

					textureCoords.Add(v);
				}
				else if (l.StartsWith("f "))
				{
					// remove 'f '
					string temp = l.Substring(2);

					var face = new Tuple<TempVertex, TempVertex, TempVertex>
					(
						new TempVertex(),
						new TempVertex(),
						new TempVertex()
					);

					// Check if there's enough elements for a face (3)
					if (temp.Trim().Count((char c) => c == ' ') == 2)
					{
						string[] faceParts = temp.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
						int i1, i2, i3;
						int t1, t2, t3;

						bool success = int.TryParse(faceParts[0].Split('/')[0], out i1);
						success		|= int.TryParse(faceParts[1].Split('/')[0], out i2);
						success		|= int.TryParse(faceParts[2].Split('/')[0], out i3);

						if (faceParts[0].Count((char c) => c == '/') == 2)
						{
							success |= int.TryParse(faceParts[0].Split('/')[1], out t1);
							success |= int.TryParse(faceParts[1].Split('/')[1], out t2);
							success |= int.TryParse(faceParts[2].Split('/')[1], out t3);
						}
						else
						{
							t1 = i1;
							t2 = i2;
							t3 = i3;
						}

						if (!success)
						{
							if (filename != null)
							{
								Console.WriteLine("!!! ERROR: cannot parse face (line: {0}, file: {1}) !!!",
												l, filename);
							}
							else
							{
								Console.WriteLine("!!! ERROR: cannot parse face (line: {0}, from string) !!!",
												l);
							}
						}
						else
						{
							TempVertex v1 = new TempVertex(i1, 0, t1);
							TempVertex v2 = new TempVertex(i2, 0, t2);
							TempVertex v3 = new TempVertex(i3, 0, t3);

							if (textureCoords.Count < t1)
							{
								textureCoords.Add(new Vector2());
							}
							if (textureCoords.Count < t2)
							{
								textureCoords.Add(new Vector2());
							}
							if (textureCoords.Count < t3)
							{
								textureCoords.Add(new Vector2());
							}
							face = new Tuple<TempVertex, TempVertex, TempVertex>(v1, v2, v3);
							faces.Add(face);
						}
					}
				}
			}

			OBJVolume volume = new OBJVolume();
			textureCoords.Add(new Vector2());
			textureCoords.Add(new Vector2());
			textureCoords.Add(new Vector2());

			foreach (var f in faces)
			{
				FaceVertex v1 = new FaceVertex
				(
					vertices[f.Item1.Vertex],
					new Vector3(),
					textureCoords[f.Item1.TextureCoord]
				);
				FaceVertex v2 = new FaceVertex
				(
					vertices[f.Item2.Vertex],
					new Vector3(),
					textureCoords[f.Item2.TextureCoord]
				);
				FaceVertex v3 = new FaceVertex
				(
					vertices[f.Item3.Vertex],
					new Vector3(),
					textureCoords[f.Item3.TextureCoord]
				);

				volume.faces.Add(new Tuple<FaceVertex, FaceVertex, FaceVertex>(v1, v2, v3));
			}

			return volume;
		}

		private struct TempVertex
		{
			public int Vertex;
			public int Normal;
			public int TextureCoord;

			public TempVertex(int vert = 0, int norm = 0, int tex = 0)
			{
				Vertex = vert;
				Normal = norm;
				TextureCoord = tex;
			}
		}
	}

	struct FaceVertex
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Vector2 TextureCoord;

		public FaceVertex(Vector3 position, Vector3 normal, Vector2 textureCoord)
		{
			Position = position;
			Normal = normal;
			TextureCoord = textureCoord;
		}
	}
}
