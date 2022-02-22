namespace SpeedDial.Classic.Drugs;

[Library("sd_ollie")]
[EditorModel("models/drugs/ollie/ollie.vmdl")]
[EntityTool("Ollie", "Speed-Dial Drugs", "Spawns one Ollie.")]
public class Ollie : ClassicBaseDrug {
	public override string WorldModel => "models/drugs/ollie/ollie.vmdl";
	public override string DrugName => "OLLIE";
	public override string DrugDescription => "penetrating, richocheting"; // super bullets // uranium slugs // shootin' lazers
	public override DrugType DrugType => DrugType.Ollie;
	public override string Icon => "materials/ui/drugs/ollie.png";
	public override string PickupSound => "sd_ollie_take";
	public override Color HighlightColor => new(0.1f, 0.1f, 1, 1);
	public override string ParticleName => "particles/drug_fx/sd_ollie/sd_ollie.vpcf";
}
