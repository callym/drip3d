using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace drip3d.Objects.Lights
{
	class AmbientLight : Light
	{
		public AmbientLight() : base()
		{
			Data.Intensity = 0f;
			Data.Attenuation = 1f;
		}

		public AmbientLight(Vector3 color, float intensity) : this()
		{
			Data.Ambient = color * intensity;
		}

		public AmbientLight(System.Drawing.Color color, float intensity) : this(Utils.Colors.ToVector(color), intensity)
		{
			
		}
	}
}
