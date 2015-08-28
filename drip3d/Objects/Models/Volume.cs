using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using drip3d.Materials;

namespace drip3d.Objects.Models
{
	public abstract class Volume : GameObject
	{
		public Vector3 Position = Vector3.Zero;
		public Vector3 Rotation = Vector3.Zero;
		public Vector3 Scale = Vector3.One;

		public Matrix4 ModelMatrix = Matrix4.Identity;
		public Matrix4 ViewProjectionMatrix = Matrix4.Identity;
		public Matrix4 ModelViewProjectionMatrix = Matrix4.Identity;

		public bool IsTextured = false;
		public int TextureID;

		protected bool generateNormals = true;
		protected bool smooth = true;

		public virtual int VerticesCount { get; set; }
		public virtual int IndiceCount { get; set; }
		public virtual int ColorDataCount { get; set; }
		public virtual int TextureCoordsCount { get; set; }
		protected Vector3[] normals = new Vector3[0];
		public virtual int NormalCount => normals.Length;

		public Volume() : base()
		{

		}

		public override void Start()
		{
			if (generateNormals)
			{
				CalculateNormals();
			}

			base.Start();
		}

		public abstract Vector3[] GetVertices();
		public abstract int[] GetIndices(int offset = 0);
		public abstract Vector3[] GetColorData();
		public virtual Vector2[] GetTextureCoords() => new Vector2[] { };
		public virtual Vector3[] GetNormals() => normals;

		public virtual void CalculateModelMatrix()
		{
			ModelMatrix = Matrix4.CreateScale(Scale) *
							Matrix4.CreateRotationX(Rotation.X) *
							Matrix4.CreateRotationY(Rotation.Y) *
							Matrix4.CreateRotationZ(Rotation.Z) *
							Matrix4.CreateTranslation(Position);
		}

		public void CalculateNormals()
		{
			Vector3[] normals = new Vector3[VerticesCount];
			Vector3[] vertices = GetVertices();
			int[] indices = GetIndices();

			for (int i = 0; i < IndiceCount; i += 3)
			{
				Vector3 v1 = vertices[indices[i]];
				Vector3 v2 = vertices[indices[i + 1]];
				Vector3 v3 = vertices[indices[i + 2]];

				Vector3 cross = Vector3.Cross(v2 - v1, v3 - v1);
				normals[indices[i]] += cross;
				normals[indices[i + 1]] += cross;
				normals[indices[i + 2]] += cross;
			}

			if (smooth)
			{
				Vector3[] newNormals = new Vector3[VerticesCount];
				int length = vertices.Length;
				for (int i = 0; i < VerticesCount; i++)
				{
					Vector3 v = vertices[i];
					Vector3 normal = Vector3.Zero;
					float totalAngle = 0f;

					for (int j = 0; j < IndiceCount; j += 3)
					{
						Vector3 v1 = vertices[indices[j]];
						Vector3 v2 = vertices[indices[j + 1]];
						Vector3 v3 = vertices[indices[j + 2]];

						Vector3? jointVertice = null;
						int k = 0;

						if (v == v1)
						{
							jointVertice = v1;
							k = indices[j];
						}
						else if (v == v2)
						{
							jointVertice = v2;
							k = indices[j + 1];
						}
						else if (v == v3)
						{
							jointVertice = v3;
							k = indices[j + 2];
						}

						if (jointVertice != null)
						{
							if (normals[k] != normals[i])
							{
								Vector3 vv = (Vector3)jointVertice;
								float angle = (float)Math.Acos(Vector3.Dot(normals[k], normals[i]));
								totalAngle += angle;
								normal += (angle / totalAngle) * normals[k];
							}
						}
					}

					newNormals[i] += normal.Normalized();
				}
				normals = newNormals;
			}

			for (int i = 0; i < NormalCount; i++)
			{
				normals[i].Normalize();
			}

			this.normals = normals;
		}
	}
}
