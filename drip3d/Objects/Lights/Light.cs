using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using System.Drawing;

namespace drip3d.Objects.Lights
{
	public class Light : GameObject
	{
		public LightData Data = new LightData();

		public Light() : base()
		{
			Data.Position.W = 1f;
			Data.Color = Vector3.One;
			Data.Intensity = 1f;
			Data.Attenuation = 0.1f;
			Data.Ambient = Vector3.Zero;
			Data.ConeAngle = 180f;
			Data.ConeDirection = Vector3.Zero;
		}

		public virtual void ChangePosition(Vector3 position)
		{
			Data.Position = new Vector4(position, Data.Position.W);
		}
	}
}
