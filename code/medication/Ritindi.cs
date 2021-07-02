using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.Meds {
	[Library("meds_ritindi")]
	public class Ritindi : BaseMedication {
		public override string WorldModel => "models/drugs/ritindi/ritindi.vmdl";
		public override float RotationSpeed => 75f;
		public override string DrugName => "Ritindi";
		public override string DrugFlavor => "steady hands"; // keep steady // true aim // recoil control
		public override float DrugDuration => 5f;
		public override DrugType Drug => DrugType.Ritindi;
		public override string icon => "materials/ui/pill.png";
		public override string PickupSound => "sd_ritindi_take";
		public override Color OutlineColor => new Color(1, 0.3f, 0, 1);
		public override string ParticleName => "particles/drug_fx/sd_ritindi/sd_ritindi.vpcf";
	}
}
