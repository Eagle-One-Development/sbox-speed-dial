using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.Meds {
	[Library("meds_polvo")]
	public class Polvo : BaseMedication {
		public override string WorldModel => "models/abilities/sm_potion1.vmdl";
		public override float RotationSpeed => 75f;
		public override string DrugName => "POLVO";
		public override string DrugFlavor => "you are speed"; // legs so fast // gotta move // dashing!
		public override float DrugDuration => 4f;
		public override DrugType Drug => DrugType.Polvo;
		public override string icon => "materials/ui/polvo.png";
		public override string PickupSound => "sd_polvo_take";
	}
}
