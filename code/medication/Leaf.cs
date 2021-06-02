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
		public override float RotationSpeed => 75f;
		public override string DrugName => "LEAF";
		// these are kinda lame, not sure how to get the armor with weed into context lol
		public override string DrugFlavor => "you feel less pain, bro"; // lizard skin (?) // tough as steel // armadillo/turtle mode // 
		public override float DrugDuration => 4f;
		public override DrugType Drug => DrugType.Leaf;
	}
}
