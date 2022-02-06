using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace Sandbox.koth.entities {
	public class HillSpot : ModelEntity {
		public override void Spawn() {
			base.Spawn();
			SetModel("models/koth/ring.vmdl");
			Transmit = TransmitType.Always;
		}
	}
}
