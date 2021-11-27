using Sandbox;

namespace SpeedDial.Classic.Drugs {
	[Library("sdg_ritindi")]
	[Hammer.EditorModel("models/drugs/ritindi/ritindi.vmdl")]
	[Hammer.EntityTool("Polvo", "Speed-Dial Drugs", "Spawns one Ritindi.")]
	public class Ritindi : ClassicBaseDrug, ISpawnable {
		public override string WorldModel => "models/drugs/ritindi/ritindi.vmdl";
		public override string DrugName => "RITINDI";
		public override string DrugDescription => "you are speed"; // legs so fast // gotta move // dashing!
		public override DrugType DrugType => DrugType.Ritindi;
		public override string Icon => "materials/ui/pill.png";
		public override string PickupSound => "sd_ritindi_take";
		public override Color HighlightColor => new(1, 0.3f, 0, 1);
		public override string ParticleName => "particles/drug_fx/sd_ritindi/sd_ritindi.vpcf";
	}
}
