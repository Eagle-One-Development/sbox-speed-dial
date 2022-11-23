using SpeedDial.Classic.Drugs;

namespace SpeedDial.Classic.Entities;

[Library( "sd_drugspawn_random", Title = "Random Drug Spawn" )]
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

[Library( "sd_drugspawn_leaf" )]
[EditorModel( "models/drugs/leaf/leaf.vmdl" )]
[Category( "Speed-Dial Drugspawns" )]
public class LeafDrugSpawn : ClassicDrugSpawn
{
	public override string DrugClass => "sd_leaf";
}

[Library( "sd_drugspawn_ollie" )]
[EditorModel( "models/drugs/ollie/ollie.vmdl" )]
[Category( "Speed-Dial Drugspawns" )]
public class OllieDrugSpawn : ClassicDrugSpawn
{
	public override string DrugClass => "sd_ollie";
}

[Library( "sd_drugspawn_polvo" )]
[EditorModel( "models/drugs/polvo/polvo.vmdl" )]
[Category( "Speed-Dial Drugspawns" )]
public class PolvoDrugSpawn : ClassicDrugSpawn
{
	public override string DrugClass => "sd_polvo";
}

[Library( "sd_drugspawn_ritindi" )]
[EditorModel( "models/drugs/ritindi/ritindi.vmdl" )]
[Category( "Speed-Dial Drugspawns" )]
public class RitindiDrugSpawn : ClassicDrugSpawn
{
	public override string DrugClass => "sd_ritindi";
}
