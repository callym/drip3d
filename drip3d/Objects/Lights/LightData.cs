using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace drip3d.Objects.Lights
{
	public struct LightData
	{
		public float Intensity;
		public Vector3 Color;
		public Vector4 Position;
		public float Attenuation;
		public Vector3 Ambient;
		public float ConeAngle;
		public Vector3 ConeDirection;
	}
}
