using Sandbox;

namespace SpeedDial.Classic.Drugs {
	[Library("sd_polvo")]
	[Hammer.EditorModel("models/drugs/polvo/polvo.vmdl")]
	[Hammer.EntityTool("Polvo", "Speed-Dial Drugs", "Spawns one Polvo.")]
	public class Polvo : ClassicBaseDrug {
		public override string WorldModel => "models/drugs/polvo/polvo.vmdl";
		public override string DrugName => "POLVO";
		public override string DrugDescription => "you are speed"; // legs so fast // gotta move // dashing!
		public override DrugType DrugType => DrugType.Polvo;
		public override string Icon => "materials/ui/drugs/polvo.png";
		public override string PickupSound => "sd_polvo_take";
		public override string ParticleName => "particles/drug_fx/sd_polvo/sd_polvo.vpcf";
	}
}
