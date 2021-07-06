using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.Meds {
	[Library("meds_leaf")]
	[Hammer.EditorModel("models/drugs/leaf/leaf.vmdl")]
	[Hammer.EntityTool("Leaf", "Speed Dial Drugs", "Leaf spawn point")]
	public class Leaf : BaseMedication {
		public override string WorldModel => "models/drugs/leaf/leaf.vmdl";
		public override float RotationSpeed => 75f;
		public override string DrugName => "LEAF";
		// these are kinda lame, not sure how to get the armor with weed into context lol
		public override string DrugFlavor => "you feel less pain, bro"; // lizard skin (?) // tough as steel // armadillo/turtle mode // 
		public override float DrugDuration => 6f;
		public override DrugType Drug => DrugType.Leaf;
		public override string icon => "materials/ui/leaf.png";
		public override string PickupSound => "sd_leaf_take";
		public override Color OutlineColor => new Color(0.1f, 1, 0.1f, 1);
		public override string ParticleName => "particles/drug_fx/sd_leaf/sd_leaf.vpcf";

	}
}
