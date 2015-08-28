using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace drip3d.Objects.Models
{
	class Tetrahedron : Volume
	{
		Vector3 PointApex;
		Vector3 PointA, PointB, PointC;

		public Tetrahedron(Vector3 apex, Vector3 a, Vector3 b, Vector3 c) : base()
		{
			PointApex = apex;
			PointA = a;
			PointB = b;
			PointC = c;

			VerticesCount = 4;
			IndiceCount = 12;
			ColorDataCount = 4;
		}

		public override Vector3[] GetVertices() => new Vector3[] { PointApex, PointA, PointB, PointC };

		public override int[] GetIndices(int offset = 0)
		{
			int[] inds = new int[]
			{	//bottom
				1,3,2,
				//other sides
				0,1,2,
				0,2,3,
				0,3,1
			};

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
			return new Vector3[]
			{
				new Vector3(1f, 0f, 0f),
				new Vector3(0f, 1f, 0f),
				new Vector3(0f, 0f, 1f),
				new Vector3(1f, 1f, 0f)
			};
		}

		public List<Tetrahedron> Divide(int n = 0)
		{
			if (n == 0)
			{
				return new List<Tetrahedron>(new Tetrahedron[] { this });
			}
			else
			{

				Vector3 halfa = (PointApex + PointA) / 2.0f;
				Vector3 halfb = (PointApex + PointB) / 2.0f;
				Vector3 halfc = (PointApex + PointC) / 2.0f;

				// Calculate points half way between base points
				Vector3 halfab = (PointA + PointB) / 2.0f;
				Vector3 halfbc = (PointB + PointC) / 2.0f;
				Vector3 halfac = (PointA + PointC) / 2.0f;

				Tetrahedron t1 = new Tetrahedron(PointApex, halfa, halfb, halfc);
				Tetrahedron t2 = new Tetrahedron(halfa, PointA, halfab, halfac);
				Tetrahedron t3 = new Tetrahedron(halfb, halfab, PointB, halfbc);
				Tetrahedron t4 = new Tetrahedron(halfc, halfac, halfbc, PointC);

				List<Tetrahedron> output = new List<Tetrahedron>();

				output.AddRange(t1.Divide(n - 1));
				output.AddRange(t2.Divide(n - 1));
				output.AddRange(t3.Divide(n - 1));
				output.AddRange(t4.Divide(n - 1));

				return output;
			}
		}
	}
}
