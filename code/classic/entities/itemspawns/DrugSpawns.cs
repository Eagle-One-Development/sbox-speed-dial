using Sandbox;

using SpeedDial.Classic.Drugs;

namespace SpeedDial.Classic.Entities {
	[Library("sdg_drugspawn_random", Title = "Random Drug Spawn")]
	[Hammer.EditorModel("models/drugs/leaf/leaf.vmdl")]
	[Hammer.EntityTool("Random Drug", "Speed-Dial Drugspawns", "Spawns random Drugs.")]
	public partial class ClassicRandomDrugSpawn : ClassicDrugSpawn {
		public override void SpawnDrug() {
			var ent = Library.Create<ClassicBaseDrug>(ClassicBaseDrug.GetRandomSpawnableType());
			ent.Transform = Transform;
			ent.DrugSpawn = this;

			//workaround since we don't actually parent the pickuptrigger right now
			ent.PickupTrigger.Position = Position;
			ent.PickupTrigger.ResetInterpolation();

			ent.ResetInterpolation();
		}
	}

	[Library("sdg_drugspawn_leaf")]
	[Hammer.EditorModel("models/drugs/leaf/leaf.vmdl")]
	[Hammer.EntityTool("Leaf", "Speed-Dial Drugspawns", "Spawns Leaf.")]
	public class LeafDrugSpawn : ClassicDrugSpawn {
		public override string DrugClass => "sdg_leaf";
	}

	[Library("sdg_drugspawn_ollie")]
	[Hammer.EditorModel("models/drugs/ollie/ollie.vmdl")]
	[Hammer.EntityTool("Ollie", "Speed-Dial Drugspawns", "Spawns Ollie.")]
	public class OllieDrugSpawn : ClassicDrugSpawn {
		public override string DrugClass => "sdg_ollie";
	}

	[Library("sdg_drugspawn_polvo")]
	[Hammer.EditorModel("models/drugs/polvo/polvo.vmdl")]
	[Hammer.EntityTool("Polvo", "Speed-Dial Drugspawns", "Spawns Polvo.")]
	public class PolvoDrugSpawn : ClassicDrugSpawn {
		public override string DrugClass => "sdg_polvo";
	}

	[Library("sdg_drugspawn_ritindi")]
	[Hammer.EditorModel("models/drugs/ritindi/ritindi.vmdl")]
	[Hammer.EntityTool("Ritindi", "Speed-Dial Drugspawns", "Spawns Ritindi.")]
	public class RitindiDrugSpawn : ClassicDrugSpawn {
		public override string DrugClass => "sdg_ritindi";
	}
}
