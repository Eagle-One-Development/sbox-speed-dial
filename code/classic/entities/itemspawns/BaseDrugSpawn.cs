using System;

using Sandbox;

using SpeedDial.Classic.Drugs;

namespace SpeedDial.Classic.Entities {
	public partial class ClassicDrugSpawn : GamemodeEntity {
		public virtual string DrugClass { get; }
		[Property]
		public virtual float RespawnTime { get; set; } = 10;
		[Net] private bool Taken { get; set; }
		[Net] private TimeSince TimeSinceTaken { get; set; }

		public override void Spawn() {
			base.Spawn();
			SpawnDrug();
		}

		[SpeedDialEvent.Gamemode.Reset]
		public void GamemodeReset() {
			// respawn drug on gamemode reset
			if(Taken) {
				Taken = false;
				SpawnDrug();
			}
		}

		public virtual void DrugTaken() {
			TimeSinceTaken = 0;
			Taken = true;
		}

		[Event.Tick.Server]
		public void Tick() {
			if(Taken && TimeSinceTaken > RespawnTime) {
				SpawnDrug();
				Taken = false;
			}
		}

		public virtual void SpawnDrug() {
			var ent = Library.Create<ClassicBaseDrug>(DrugClass);
			ent.Transform = Transform;
			ent.DrugSpawn = this;

			//workaround since we don't actually parent the pickuptrigger right now
			ent.PickupTrigger.Position = Position;
			ent.PickupTrigger.ResetInterpolation();

			ent.ResetInterpolation();
		}
	}
}
