using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	public partial class WorldText : Panel {
		private Vector3 _pos;

		public WorldText(string text, Vector3 position, float duration, Panel parent) {
			Parent = parent;
			TimeSince _time = 0;
			_pos = position;
			StyleSheet.Load("/classic/ui/worldhints/WorldText.scss");
			var canvas = Add.Panel("canvas");
			canvas.BindClass("visible", () => !(_time > duration));
			canvas.Add.Label($"{text}", "text");
		}

		public WorldText() {
			_pos = Vector3.Zero;
		}

		public override void Tick() {
			if(!IsVisible) {
				((WorldHints)Parent).WorldTexts.Remove(this);
				Delete();
				return;
			}

			var pos = _pos.ToScreen();
			Style.Left = Length.Fraction(pos.x);
			Style.Top = Length.Fraction(pos.y);
		}
	}
}
