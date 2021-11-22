using Sandbox;

using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.WeaponSpawns {

	public partial class BaseWeaponSpawn : Entity {

		public virtual string WeaponClass => "sd_pistol";

		[Property]
		public virtual float RespawnTime { get; set; }

		[Net]
		public bool ItemTaken { get; set; } = false;

		[Net]
		public TimeSince TimeSinceTaken { get; set; }


		public override void Spawn() {
			base.Spawn();
			SpawnWeapon();
		}

		[Event("server.tick")]
		public void Simulate() {
			if(ItemTaken && TimeSinceTaken > RespawnTime) {
				SpawnWeapon();
				ItemTaken = false;
			}
		}

		public virtual void SpawnWeapon() {
			var ent = Library.Create<Entity>(WeaponClass) as BaseSpeedDialWeapon;
			ent.Transform = Transform;
			ent.WeaponSpawn = this;
			ent.DespawnAfterTime = false;
			ent.GlowState = GlowStates.GlowStateOn;
			ent.GlowDistanceStart = 0;
			ent.GlowDistanceEnd = 1000;
			ent.GlowColor = new Color(1, 1, 1, 1);
			ent.GlowActive = true;
		}
	}
}
