using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace drip3d.Objects.Lights
{
	class DirectionalLight : Light
	{
		public DirectionalLight() : base()
		{
			Data.Position.W = 0f;
		}
	}
}
