using Sandbox;

namespace SpeedDial.Classic.Drugs {
	[Library("sd_leaf")]
	[Hammer.EditorModel("models/drugs/leaf/leaf.vmdl")]
	[Hammer.EntityTool("Leaf", "Speed-Dial Drugs", "Spawns one Leaf.")]
	public class Leaf : ClassicBaseDrug {
		public override string WorldModel => "models/drugs/leaf/leaf.vmdl";
		public override string DrugName => "LEAF";
		// these are kinda lame, not sure how to get the armor with weed into context lol
		public override string DrugDescription => "you feel less pain, bro"; // lizard skin (?) // tough as steel // armadillo/turtle mode // 
		public override DrugType DrugType => DrugType.Leaf;
		public override string Icon => "materials/ui/drugs/leaf.png";
		public override string PickupSound => "sd_leaf_take";
		public override Color HighlightColor => new(0.1f, 1, 0.1f, 1);
		public override string ParticleName => "particles/drug_fx/sd_leaf/sd_leaf.vpcf";
	}
}
