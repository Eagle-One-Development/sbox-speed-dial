namespace SpeedDial.Classic.Drugs;

[Library( "sd_polvo" )]
[EditorModel( "models/drugs/polvo/polvo.vmdl" )]
[Category( "Speed-Dial Drugs" )]
public class Polvo : ClassicBaseDrug
{
	public override string WorldModel => "models/drugs/polvo/polvo.vmdl";
	public override string DrugName => "POLVO";
	public override string DrugDescription => "you are speed"; // legs so fast // gotta move // dashing!
	public override DrugType DrugType => DrugType.Polvo;
	public override string Icon => "materials/ui/drugs/polvo.png";
	public override string PickupSound => "sd_polvo_take";
	public override string ParticleName => "particles/drug_fx/sd_polvo/sd_polvo.vpcf";
}
