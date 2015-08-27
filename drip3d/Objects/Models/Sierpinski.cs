using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace drip3d.Objects.Models
{
	class Sierpinski : Volume
	{
		private List<Vector3> vertices = new List<Vector3>();
		private List<int> indices = new List<int>();
		private List<Vector3> colors = new List<Vector3>();

		public Sierpinski(int subdivisions = 1) : base()
		{
			int NumTris = (int)Math.Pow(4, subdivisions + 1);

			VerticesCount = NumTris;
			ColorDataCount = NumTris;
			IndiceCount = 3 * NumTris;

			Tetrahedron twhole = new Tetrahedron(
							new Vector3(0.0f, 0.0f, 1.0f),  // Apex center 
							new Vector3(0.943f, 0.0f, -0.333f),  // Base center top
							new Vector3(-0.471f, 0.816f, -0.333f),  // Base left bottom
							new Vector3(-0.471f, -0.816f, -0.333f));

			List<Tetrahedron> allTets = twhole.Divide(subdivisions);

			int offset = 0;
			foreach (Tetrahedron t in allTets)
			{
				vertices.AddRange(t.GetVertices());
				indices.AddRange(t.GetIndices(offset * 4));
				colors.AddRange(t.GetColorData());
				offset++;
			}
		}

		public override Vector3[] GetVertices()
		{
			return vertices.ToArray();
		}

		public override Vector3[] GetColorData()
		{
			return colors.ToArray();
		}

		public override int[] GetIndices(int offset = 0)
		{
			int[] inds = indices.ToArray();

			if (offset != 0)
			{
				for (int i = 0; i < inds.Length; i++)
				{
					inds[i] += offset;
				}
			}

			return inds;
		}
	}
}
