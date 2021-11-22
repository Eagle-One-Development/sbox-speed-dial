using Sandbox;
using System;
using System.Linq;
using SpeedDial.Classic.Weapons;

using SpeedDial.Base.Player;

namespace SpeedDial.Classic.Player {
	public partial class SpeedDialInventory : BaseInventory {
		public SpeedDialInventory(BasePlayer player) : base(player) { }

		public override bool Add(Entity ent, bool makeActive = false) {
			var player = Owner as SpeedDialPlayer;
			if(ent is BaseSpeedDialWeapon weapon) {
				//
				// We don't want to pick up the same weapon twice
				// But we'll take the ammo from it Winky Face
				//
				if(weapon != null && IsCarryingType(ent.GetType())) {
					var ammo = weapon.AmmoClip;
					var ammoType = weapon.AmmoType;

					if(ammo > 0) {
						player.GiveAmmo(ammoType, ammo);
					}

					// Despawn it
					ent.Delete();
					return false;
				}
			}
			return base.Add(ent, makeActive);
		}

		public bool IsCarryingType(Type t) {
			return List.Any(x => x.GetType() == t);
		}
	}
}
