using SpeedDial.Classic.Drugs;

namespace SpeedDial.Classic.Entities;

[Library( "sd_drugspawn_random", Title = "Random Drug Spawn" )]
[EditorModel( "models/drugs/leaf/leaf.vmdl" )]
[EntityTool( "Random Drug", "Speed-Dial Drugspawns", "Spawns random Drugs." )]
public partial class ClassicRandomDrugSpawn : ClassicDrugSpawn
{
	public override void SpawnDrug()
	{
		Host.AssertServer();
		if ( !Enabled ) return;

		var ent = Library.Create<ClassicBaseDrug>( ClassicBaseDrug.GetRandomSpawnableType() );
		ent.Transform = Transform;
		ent.DrugSpawn = this;
		ent.ResetInterpolation();

		SpawnedDrug = ent;
		Taken = false;
	}
}

[Library( "sd_drugspawn_leaf" )]
[EditorModel( "models/drugs/leaf/leaf.vmdl" )]
[EntityTool( "Leaf", "Speed-Dial Drugspawns", "Spawns Leaf." )]
public class LeafDrugSpawn : ClassicDrugSpawn
{
	public override string DrugClass => "sd_leaf";
}

[Library( "sd_drugspawn_ollie" )]
[EditorModel( "models/drugs/ollie/ollie.vmdl" )]
[EntityTool( "Ollie", "Speed-Dial Drugspawns", "Spawns Ollie." )]
public class OllieDrugSpawn : ClassicDrugSpawn
{
	public override string DrugClass => "sd_ollie";
}

[Library( "sd_drugspawn_polvo" )]
[EditorModel( "models/drugs/polvo/polvo.vmdl" )]
[EntityTool( "Polvo", "Speed-Dial Drugspawns", "Spawns Polvo." )]
public class PolvoDrugSpawn : ClassicDrugSpawn
{
	public override string DrugClass => "sd_polvo";
}

[Library( "sd_drugspawn_ritindi" )]
[EditorModel( "models/drugs/ritindi/ritindi.vmdl" )]
[EntityTool( "Ritindi", "Speed-Dial Drugspawns", "Spawns Ritindi." )]
public class RitindiDrugSpawn : ClassicDrugSpawn
{
	public override string DrugClass => "sd_ritindi";
}
