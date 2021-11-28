using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using SpeedDial.Classic.Player;
using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.UI {
	public partial class WorldHints : Panel {
		public static WorldHints Current;
		private Panel PickupPanel;
		private Label PickupLabel;
		private ClassicBaseWeapon _lastPickup;
		private float _pickupScale = 0;

		public WorldHints() {
			Current = this;

			StyleSheet.Load("/classic/ui/worldhints/WorldHints.scss");
			AddClass("worldhints");

			PickupPanel = Add.Panel("pickup");
			PickupLabel = PickupPanel.Add.Label($"{Input.GetKeyWithBinding("+iv_attack2").ToUpper()} TO PICK UP", "pickuplabel");
		}

		public override void Tick() {
			var pawn = Local.Client.GetPawn<ClassicPlayer>();
			if(pawn is null) return;

			// pickup panel stuff
			{
				PanelTransform transform = new();
				// workaround to not have the hint blip in when picking up a gun or throwing your held gun
				if(_lastPickup.IsValid() && (_lastPickup.TimeSinceAlive < 0.5f || _lastPickup == pawn.ActiveChild)) _pickupScale = 0;
				transform.AddScale(_pickupScale);
				PickupPanel.Style.Transform = transform;

				if(_lastPickup.IsValid()) {
					var pos = _lastPickup.Position.ToScreen();
					PickupPanel.Style.Left = Length.Fraction(pos.x);
					PickupPanel.Style.Top = Length.Fraction(pos.y);
				}

				if(!pawn.Pickup) {
					_pickupScale = _pickupScale.LerpTo(0, Time.Delta * 5f);
					return;
				}

				_lastPickup = pawn.PickupWeapon;
				_pickupScale = _pickupScale.LerpTo(1, Time.Delta * 5f);
			}
		}
	}
}
