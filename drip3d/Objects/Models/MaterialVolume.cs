using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using drip3d.Materials;
using drip3d.Textures;

namespace drip3d.Objects.Models
{
	interface IHasMaterial
	{
		Material Material { get; set; }
	}

	abstract class MaterialVolume : Volume, IHasMaterial
	{
		Material _material = new Material();
		public virtual Material Material { get; set; } = new Material();

		public override void Start()
		{
			Material.Start();

			base.Start();
		}
	}
}
