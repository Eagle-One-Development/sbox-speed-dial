namespace SpeedDial.Classic.Drugs;

[Library( "sd_ritindi" ), HammerEntity]
[EditorModel( "models/drugs/ritindi/ritindi.vmdl" )]
[Category( "Speed-Dial Drugs" )]
public class Ritindi : ClassicBaseDrug
{
	public override string WorldModel => "models/drugs/ritindi/ritindi.vmdl";
	public override string DrugName => "RITINDI";
	public override string DrugDescription => "steady hands"; // keep steady // true aim // recoil control
	public override DrugType DrugType => DrugType.Ritindi;
	public override string Icon => "materials/ui/drugs/pill.png";
	public override string PickupSound => "sd_ritindi_take";
	public override Color HighlightColor => new( 1, 0.3f, 0, 1 );
	public override string ParticleName => "particles/drug_fx/sd_ritindi/sd_ritindi.vpcf";
}
