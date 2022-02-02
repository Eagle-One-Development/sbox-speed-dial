using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.UI {
	[UseTemplate]
	public partial class WeaponPanel : Panel {
		public static WeaponPanel Current { get; private set; }

		public Label AmmoLabel { get; set; }
		public Label WeaponLabel { get; set; }
		public Panel Panel { get; set; }

		private float AmmoScale = 1;

		public WeaponPanel() {
			Current = this;
		}

		[ClientRpc]
		public static void Fire(float scale) {
			if(Current is null) return;
			Current.AmmoScale += 0.2f * scale;
		}

		[ClientRpc]
		public static void Fire() {
			if(Current is null) return;
			Current.AmmoScale += 0.2f;
		}

		public override void Tick() {
			var weapon = (ClassicBaseWeapon)Local.Pawn.ActiveChild;

			// ammo
			{
				// clamp scale for Fire effect
				AmmoScale = AmmoScale.Clamp(0, 1.5f);

				// text scaling
				PanelTransform transform = new();
				transform.AddScale(AmmoScale);
				AmmoLabel.Style.Transform = transform;

				// update ammo label number or scale down if no weapon
				if(weapon is null) {
					AmmoScale = AmmoScale.LerpTo(0, Time.Delta * 7f);
				} else {
					if(weapon.ClipSize < 0)
						AmmoLabel.Text = $"";
					else
						AmmoLabel.Text = $"{weapon.AmmoClip}";

					// lerp to normal scale
					AmmoScale = AmmoScale.LerpTo(1, Time.Delta * 7f);
				}
			}

			// weapon name
			{
				if(weapon is null) {
					WeaponLabel.Text = $"FISTS";
				} else {
					WeaponLabel.Text = $"{weapon.ClassInfo.Title}";
				}
			}
		}
	}
}
