using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.Meds {
	[Library("meds_ollie")]
	public class Ollie : BaseMedication {
		public override string WorldModel => "models/drugs/ollie/ollie.vmdl";
		public override float RotationSpeed => 75f;
		public override string DrugName => "OLLIE";
		public override string DrugFlavor => "penetrating, richocheting"; // super bullets // uranium slugs // shootin' lazers
		public override float DrugDuration => 4f;
		public override DrugType Drug => DrugType.Ollie;
		public override string icon => "materials/ui/ollie.png";
		public override string PickupSound => "sd_ollie_take";

		public override Color OutlineColor => new Color(0.1f, 0.1f, 1, 1);
	}
}
