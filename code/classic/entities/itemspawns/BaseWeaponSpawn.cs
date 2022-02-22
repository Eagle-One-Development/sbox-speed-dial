using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.Entities;

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
		// respawn gun on gamemode reset
		SpawnedWeapon?.Delete();
		SpawnedWeapon = null;

		SpawnWeapon();
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
		}
	}

	public virtual void SpawnWeapon() {
		Host.AssertServer();
		if(!Enabled) return;

		var ent = WeaponBlueprint.Create(WeaponClass);
		ent.Transform = Transform;
		ent.WeaponSpawn = this;
		ent.ResetInterpolation();

		SpawnedWeapon = ent;
		Taken = false;
	}
}
