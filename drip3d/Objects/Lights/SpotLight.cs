using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace drip3d.Objects.Lights
{
	class SpotLight : Light
	{
		public SpotLight() : base()
		{
			Data.ConeAngle = 20f;
			Data.ConeDirection = Vector3.Zero;
		}

		public void LookAt(Vector3 target)
		{
			Vector3 position = Data.Position.Xyz;
			Data.ConeDirection = (target - position).Normalized();
		}
	}
}
