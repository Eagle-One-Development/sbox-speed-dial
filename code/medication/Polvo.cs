using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.Meds {
	[Library("meds_polvo")]
	public class Polvo : BaseMedication {
		public override string WorldModel => "models/abilities/sm_candy.vmdl";
		public override float rotationSpeed => 75f;
		public override string drugName => "POLVO";
		public override string drugFlavor => "you are speed";
		public override float drugDuration => 4f;
		public override DrugType drug => DrugType.Polvo;
	}
}
