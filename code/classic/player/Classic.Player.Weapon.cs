using System;
using System.Linq;

using Sandbox;

using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.Player {
	public partial class ClassicPlayer {
		[Net] public ClassicBaseWeapon PickupWeapon { get; set; }
		[Net] public TimeSince TimeSincePickup { get; set; }
		[Net] public TimeSince TimeSinceWeaponCarried { get; set; }
		[Net] public bool Pickup { get; set; }

		public virtual void ThrowWeapon() {
			if(IsServer) {
				if(!DropWeapon(out ClassicBaseWeapon dropped)) return;

				dropped.Position = EyePos;
				dropped.ResetInterpolation();
				if(dropped.PhysicsGroup != null && dropped is ClassicBaseWeapon weapon) {
					weapon.PhysicsBody.Velocity += EyeRot.Forward * 700;
					weapon.PhysicsBody.AngularVelocity = new Vector3(0, 0, 100f);
					using(Prediction.Off()) {
						weapon.PlaySound("weaponspin");
					}
				}
			}
		}

		public void GiveWeapon<T>(string weaponclass, bool drop = false) where T : ClassicBaseWeapon {
			var weapon = Library.Create<T>(weaponclass);
			if(weapon is null) return;

			if(drop) {
				DropWeapon();
			} else {
				if(ActiveChild is not null) {
					var oldwep = ActiveChild;
					oldwep.Delete();
					ActiveChild = null;
				}
			}

			weapon.Parent = this;
			weapon.OnCarryStart(this);
			ActiveChild = weapon;
		}

		public void GiveWeapon<T>(bool drop = false) where T : ClassicBaseWeapon, new() {
			if(drop) {
				DropWeapon();
			} else {
				if(ActiveChild is not null) {
					var oldwep = ActiveChild;
					oldwep.Delete();
					ActiveChild = null;
				}
			}

			T weapon = new();
			weapon.Parent = this;
			weapon.OnCarryStart(this);
			ActiveChild = weapon;
		}

		public bool DropWeapon(out ClassicBaseWeapon weapon) {
			weapon = DropWeapon();
			if(weapon is null || !weapon.IsValid()) return false;
			return true;
		}

		public ClassicBaseWeapon DropWeapon() {
			if(ActiveChild is null) {
				return null;
			}
			var weapon = ActiveChild as ClassicBaseWeapon;
			weapon.Parent = null;
			weapon.OnCarryDrop(this);

			if(IsServer) {
				weapon.Position = EyePos;
				weapon.Velocity = Velocity * 0.75f;
				weapon.ResetInterpolation();
			}
			ActiveChild = null;

			return weapon;
		}
	}
}
