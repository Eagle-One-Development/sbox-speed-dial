using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.UI {
	public partial class AmmoPanel : Panel {
		private static AmmoPanel Current;
		private readonly Label AmmoLabel;
		private readonly Panel Panel;
		private float AmmoScale = 1;


		public AmmoPanel() {
			Current = this;

			StyleSheet.Load("/classic/ui/ammo/AmmoPanel.scss");
			AddClass("ammopanel");
			Panel = Add.Panel("panel");
			AmmoLabel = Panel.Add.Label("0", "ammo");
		}

		[ClientRpc]
		public static void Fire() {
			Current.AmmoScale += 0.2f;
		}

		public override void Tick() {
			// clamp scale for Fire effect
			AmmoScale = AmmoScale.Clamp(0, 1.5f);

			// text scaling
			PanelTransform transform = new();
			transform.AddScale(AmmoScale);
			AmmoLabel.Style.Transform = transform;

			// update ammo label number or scale down if no weapon
			if(Local.Pawn.ActiveChild is null) {
				AmmoScale = AmmoScale.LerpTo(0, Time.Delta * 7f);
				return;
			}
			AmmoLabel.Text = $"{(Local.Pawn.ActiveChild as ClassicBaseWeapon).AmmoClip}";

			// lerp to normal scale
			AmmoScale = AmmoScale.LerpTo(1, Time.Delta * 7f);
		}
	}
}
