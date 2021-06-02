using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.Meds {
	[Library("meds_leaf")]
	public class Leaf : BaseMedication {
		public override string WorldModel => "models/abilities/sm_candy.vmdl";
		public override float rotationSpeed => 75f;
		public override string drugName => "LEAF";
		public override string drugFlavor => "you feel less pain, bro";
		public override float drugDuration => 4f;
		public override DrugType drug => DrugType.Leaf;
	}
}
