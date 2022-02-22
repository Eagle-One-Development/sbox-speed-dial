using SpeedDial.Classic.Drugs;

namespace SpeedDial.Classic.Entities;

public partial class ClassicDrugSpawn : GamemodeEntity {
	public virtual string DrugClass { get; }
	[Property]
	public virtual float RespawnTime { get; set; } = 10;
	[Net] protected bool Taken { get; set; }
	[Net] private TimeSince TimeSinceTaken { get; set; }
	protected ClassicBaseDrug SpawnedDrug { get; set; }

	public override void Spawn() {
		base.Spawn();
		SpawnDrug();
	}

	[SpeedDialEvent.Gamemode.Reset]
	public void GamemodeReset() {
		// respawn drug on gamemode reset
		SpawnedDrug?.Delete();
		SpawnedDrug = null;

		SpawnDrug();
	}

	public virtual void DrugTaken() {
		TimeSinceTaken = 0;
		Taken = true;
		SpawnedDrug = null;
	}

	[Event.Tick.Server]
	public void Tick() {
		if(Taken && TimeSinceTaken > RespawnTime) {
			SpawnDrug();
		}
	}

	public virtual void SpawnDrug() {
		Host.AssertServer();
		if(!Enabled) return;

		var ent = Library.Create<ClassicBaseDrug>(DrugClass);
		ent.Transform = Transform;
		ent.DrugSpawn = this;
		ent.ResetInterpolation();

		SpawnedDrug = ent;
		Taken = false;
	}
}
