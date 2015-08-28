using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Assimp;
using Assimp.Configs;
using OpenTK;

namespace drip3d.Objects.Models
{
	class AssimpVolume : MaterialVolume
	{
		Vector3[] vertices;
		Vector3[] colors;
		Vector2[] textureCoords;
		int[] indices;

		AssimpVolume() : base()
		{

		}

		public override void Start()
		{
			base.Start();
		}

		public override int VerticesCount => vertices.Length;
		public override int IndiceCount => indices.Length;
		public override int ColorDataCount => colors.Length;

		public override Vector3[] GetVertices() => vertices;

		public override int[] GetIndices(int offset = 0)
		{
			int[] inds = new int[IndiceCount];
			indices.CopyTo(inds, 0);

			if (offset != 0)
			{
				for (int i = 0; i < inds.Length; i++)
				{
					inds[i] += offset;
				}
			}

			return inds;
		}

		public override Vector3[] GetColorData()
		{
			if (colors == null)
			{
				Vector3[] newColors = new Vector3[VerticesCount];
				Vector3 color = Utils.Colors.ToVector(System.Drawing.Color.Gray);
				Utils.Populate(newColors, color);
				colors = newColors;
			}
			return colors;
		}

		public override Vector2[] GetTextureCoords()
		{
			if (textureCoords == null)
			{
				Vector2[] newTextureCoords = new Vector2[VerticesCount];
				Utils.Populate(newTextureCoords, new Vector2(0f, 0f));
				textureCoords = newTextureCoords;
			}
			return textureCoords;
		}

		static public AssimpVolume LoadFromFile(string filename)
		{
			string path = Path.Combine("Assets", "Models", filename);

			AssimpContext importer = new AssimpContext();

			NormalSmoothingAngleConfig normalSmoothing = new NormalSmoothingAngleConfig(66.0f);
			importer.SetConfig(normalSmoothing);

			LogStream logStream = new LogStream
			(
				delegate(string message, string userData)
				{
					Console.Write(message);
				}
			);
			logStream.Attach();

			Scene model = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);
			Mesh mesh = model.Meshes[0];

			AssimpVolume v = new AssimpVolume();

			List<Vector3> newVertices = new List<Vector3>();
			foreach (Assimp.Vector3D vert in mesh.Vertices)
			{
				newVertices.Add(new Vector3(vert.X, vert.Y, vert.Z));
			}
			v.vertices = newVertices.ToArray();

			v.indices = mesh.GetIndices();

			if (mesh.HasNormals)
			{
				v.generateNormals = false;
				List<Vector3> newNormals = new List<Vector3>();
				foreach (Assimp.Vector3D n in mesh.Normals)
				{
					newNormals.Add(new Vector3(n.X, n.Y, n.Z));
				}
				v.normals = newNormals.ToArray();
			}

			if (mesh.HasTextureCoords(0))
			{
				List<Vector2> newTextureCoords = new List<Vector2>();
				foreach (Assimp.Vector3D tc in mesh.TextureCoordinateChannels[0])
				{
					newTextureCoords.Add(new Vector2(tc.X, tc.Y));
				}
				v.textureCoords = newTextureCoords.ToArray();
			}

			if (mesh.HasVertexColors(0))
			{
				List<Vector3> newColors = new List<Vector3>();
				foreach (Assimp.Color4D c in mesh.VertexColorChannels[0])
				{
					newColors.Add(new Vector3(c.R, c.G, c.B));
				}
				v.colors = newColors.ToArray();
			}

			importer.Dispose();
			return v;
		}
	}
}
