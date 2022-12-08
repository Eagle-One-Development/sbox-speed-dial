using SpeedDial.Classic.Drugs;

namespace SpeedDial.Classic.Entities;

[Library( "sd_drugspawn_random" ), Title( "Random Drug Spawn" ), HammerEntity]
[EditorModel( "models/drugs/leaf/leaf.vmdl" )]
[Category( "Speed-Dial Drugspawns" )]
public partial class ClassicRandomDrugSpawn : ClassicDrugSpawn
{
	public override void SpawnDrug()
	{
		Host.AssertServer();
		if ( !Enabled ) return;

		var ent = TypeLibrary.Create<ClassicBaseDrug>( ClassicBaseDrug.GetRandomSpawnableType().Name );
		ent.Transform = Transform;
		ent.DrugSpawn = this;
		ent.ResetInterpolation();

		SpawnedDrug = ent;
		Taken = false;
	}
}

[Library( "sd_drugspawn_leaf" ), HammerEntity]
[EditorModel( "models/drugs/leaf/leaf.vmdl" )]
[Category( "Speed-Dial Drugspawns" )]
public class LeafDrugSpawn : ClassicDrugSpawn
{
	public override string DrugClass => "sd_leaf";
}

[Library( "sd_drugspawn_ollie" ), HammerEntity]
[EditorModel( "models/drugs/ollie/ollie.vmdl" )]
[Category( "Speed-Dial Drugspawns" )]
public class OllieDrugSpawn : ClassicDrugSpawn
{
	public override string DrugClass => "sd_ollie";
}

[Library( "sd_drugspawn_polvo" ), HammerEntity]
[EditorModel( "models/drugs/polvo/polvo.vmdl" )]
[Category( "Speed-Dial Drugspawns" )]
public class PolvoDrugSpawn : ClassicDrugSpawn
{
	public override string DrugClass => "sd_polvo";
}

[Library( "sd_drugspawn_ritindi" ), HammerEntity]
[EditorModel( "models/drugs/ritindi/ritindi.vmdl" )]
[Category( "Speed-Dial Drugspawns" )]
public class RitindiDrugSpawn : ClassicDrugSpawn
{
	public override string DrugClass => "sd_ritindi";
}
