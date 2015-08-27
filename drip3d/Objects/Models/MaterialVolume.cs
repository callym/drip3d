using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using drip3d.Materials;

namespace drip3d.Objects.Models
{
	interface IHasMaterial
	{
		Material Material { get; set; }
	}

	abstract class MaterialVolume : Volume, IHasMaterial
	{
		Material _material = new Material();
		public virtual Material Material
		{
			get { return _material; }
			set { _material = value; }
		}

		public override void Start()
		{
			foreach (string filename in Material.Textures)
			{
				Game.LoadImage(filename);
				Console.WriteLine("Loaded image {0}, value: {1}", filename, ObjectManager.Instance.Textures[filename]);
			}

			base.Start();
		}
	}
}
