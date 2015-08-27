using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace drip3d.Objects.Models
{
	class Cube : MaterialVolume
	{
		public Cube() : base()
		{
			VerticesCount = 8;
			IndiceCount = 36;
			ColorDataCount = 8;

			smooth = false;
		}

		public override Vector3[] GetVertices()
		{
			return new Vector3[]
			{
				new Vector3(-0.5f, -0.5f,  -0.5f),
                new Vector3(0.5f, -0.5f,  -0.5f),
                new Vector3(0.5f, 0.5f,  -0.5f),
                new Vector3(-0.5f, 0.5f,  -0.5f),
                new Vector3(-0.5f, -0.5f,  0.5f),
                new Vector3(0.5f, -0.5f,  0.5f),
                new Vector3(0.5f, 0.5f,  0.5f),
                new Vector3(-0.5f, 0.5f,  0.5f),
            };
		}

		public override int[] GetIndices(int offset = 0)
		{
			int[] indices = new int[]
			{
                //left
                0, 2, 1,
                0, 3, 2,
                //back
                1, 2, 6,
                6, 5, 1,
                //right
                4, 5, 6,
                6, 7, 4,
                //top
                2, 3, 6,
                6, 3, 7,
                //front
                0, 7, 3,
                0, 4, 7,
                //bottom
                0, 1, 5,
                0, 5, 4
            };

			if (offset != 0)
			{
				for (int i = 0; i < indices.Length; i++)
				{
					indices[i] += offset;
				}
			}

			return indices;
		}

		public override Vector3[] GetColorData()
		{
			return new Vector3[]
			{
                new Vector3( 1f, 0f, 0f),
                new Vector3( 0f, 0f, 1f),
                new Vector3( 0f, 1f, 0f),
                new Vector3( 1f, 0f, 0f),
                new Vector3( 0f, 0f, 1f),
                new Vector3( 0f, 1f, 0f),
                new Vector3( 1f, 0f, 0f),
                new Vector3( 0f, 0f, 1f)
            };
		}
	}
}
