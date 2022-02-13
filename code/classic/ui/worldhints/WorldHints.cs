using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using SpeedDial.Classic.Player;
using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.UI {
	[UseTemplate]
	public partial class WorldHints : Panel {
		public static WorldHints Current { get; private set; }

		public List<Panel> WorldTexts = new();

		private Weapon _lastPickup;
		private float _pickupScale = 0;

		public Panel PickupPanel { get; set; }
		public string PickupText => $" TO PICK UP";

		public WorldHints() {
			Current = this;
		}

		public override void Tick() {
			var pawn = Local.Client.GetPawn<ClassicPlayer>();
			if(pawn is null) return;

			// pickup panel
			{
				PanelTransform transform = new();
				// workaround to not have the hint blip in when picking up a gun or throwing your held gun
				if(_lastPickup.IsValid() && (_lastPickup.TimeSinceAlive < 0.5f || _lastPickup == pawn.ActiveChild)) _pickupScale = 0;
				transform.AddScale(_pickupScale);
				PickupPanel.Style.Transform = transform;

				if(_lastPickup.IsValid()) {
					var pos = _lastPickup.Position.ToScreen();
					PickupPanel.Style.Left = Length.Fraction(pos.x - 0.05f); // this is dumb and doesn't work well on weird aspect ratios (4:3)
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

		[ClientRpc]
		public static void AddHint(string text, Vector3 position, float duration) {
			if(Current is null) return;
			WorldText worldtext = new(text, position, duration, Current);
			Current.WorldTexts.Add(worldtext);
		}
	}
}
