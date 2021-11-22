using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using SpeedDial.Classic.Player;
using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.WeaponSpawns {
	[Library("sd_random_weaponspawn", Title = "Weapon Spawn")]
	[Hammer.EditorModel("models/weapons/pistol/prop_pistol.vmdl")]
	[Hammer.EntityTool("Random Weapon", "Speed Dial Weapons", "Random Weapon spawn point")]
	public partial class RandomWeaponSpawn : BaseWeaponSpawn {

		public string[] WeaponClasses = { // will implement a weighted randomness later
			"sd_pistol",
			"sd_shotgun",
			"sd_smg",
			"sd_rifle",
			"sd_sniper",
			"sd_roomclearer",
			"sd_bat",
			//"sd_pipe",
			//"sd_pan"
		};

		public override void SpawnWeapon() {
			Random random = new();
			int index = random.Next(0, WeaponClasses.Length);
			var ent = Library.Create<Entity>(WeaponClasses[index]) as BaseSpeedDialWeapon;
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
