using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.UI {
	public partial class WeaponPanel : Panel {
		private static WeaponPanel Current;
		private readonly Label AmmoLabel;
		private readonly Label WeaponLabel;
		private readonly Panel Panel;
		private float AmmoScale = 1;


		public WeaponPanel() {
			Current = this;

			StyleSheet.Load("/classic/ui/weapon/WeaponPanel.scss");
			Panel = Add.Panel("weapon");
			AmmoLabel = Panel.Add.Label("0", "clip");
			WeaponLabel = Panel.Add.Label("WEAPON", "weaponname");
		}

		[ClientRpc]
		public static void Fire() {
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
					WeaponLabel.Text = $"BEAR HANDS";
				} else {
					WeaponLabel.Text = $"{weapon.ClassInfo.Title}";
				}
			}
		}
	}
}
