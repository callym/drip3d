using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace drip3d.Objects
{
	public abstract class GameObject
	{
		public string Name = "";

		public Action<GameObject> OnStart = null;
		public Action<GameObject, float> OnUpdate = null;

		public GameObject()
		{

		}

		public virtual void Start()
		{
			if (OnStart != null)
			{
				OnStart(this);
			}
		}

		public virtual void Update(float time)
		{
			if (OnUpdate != null)
			{
				OnUpdate(this, time);
			}
		}
	}
}
