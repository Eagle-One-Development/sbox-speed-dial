using System;

using Sandbox;

using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.Entities {
	public partial class ClassicWeaponSpawn : GamemodeEntity {
		public virtual string WeaponClass { get; }
		[Property]
		public virtual float RespawnTime { get; set; } = 10;
		[Net] private bool Taken { get; set; }
		[Net] private TimeSince TimeSinceTaken { get; set; }
		protected Weapon SpawnedWeapon { get; set; }

		public override void Spawn() {
			base.Spawn();
			SpawnWeapon();
		}

		[SpeedDialEvent.Gamemode.Reset]
		public void GamemodeReset() {
			if(!Enabled) {
				SpawnedWeapon?.Delete();
				SpawnedWeapon = null;
				return;
			}
			// respawn gun on gamemode reset
			if(Taken) {
				SpawnWeapon();
				Taken = false;
			}
		}

		public virtual void WeaponTaken() {
			TimeSinceTaken = 0;
			Taken = true;
			SpawnedWeapon = null;
		}

		[Event.Tick.Server]
		public void Tick() {
			if(Taken && TimeSinceTaken > RespawnTime) {
				SpawnWeapon();
				Taken = false;
			}
		}

		public virtual void SpawnWeapon() {
			Host.AssertServer();
			if(!Enabled) return;

			var ent = WeaponTemplate.Create(WeaponClass);
			ent.Transform = Transform;
			ent.WeaponSpawn = this;
			ent.ResetInterpolation();

			SpawnedWeapon = ent;
		}
	}
}
