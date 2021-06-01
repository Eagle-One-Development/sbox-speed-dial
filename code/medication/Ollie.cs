using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.Meds {
	[Library("meds_ollie")]
	public class Ollie : BaseMedication {
		public override string WorldModel => "models/abilities/sm_candy.vmdl";
		public override float rotationSpeed => 75f;
		public override string drugName => "OLLIE";
		public override float drugDuration => 4f;
		public override DrugType drug => DrugType.Ollie;
	}
}
