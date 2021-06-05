using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.Meds {
	[Library("meds_ritindi")]
	public class Ritindi : BaseMedication {
		public override string WorldModel => "models/abilities/sm_candy.vmdl";
		public override float RotationSpeed => 75f;
		public override string DrugName => "Ritindi";
		public override string DrugFlavor => "steady hands"; // keep steady // true aim // recoil control
		public override float DrugDuration => 4f;
		public override DrugType Drug => DrugType.Ritindi;
		public override string icon => "materials/ui/pill.png";
	}
}
