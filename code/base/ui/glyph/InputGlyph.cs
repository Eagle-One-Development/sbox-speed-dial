using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	[UseTemplate]
	public partial class InputGlyph : Panel {
		public Image Glyph { get; set; }
		public InputButton Button { get; set; }

		protected bool IsSet = false;

		public override void SetProperty(string name, string value) {
			base.SetProperty(name, value);

			if(name == "btn") {
				SetButton(Enum.Parse<InputButton>(value, true));
			}
		}

		public void SetButton(InputButton button) {
			Button = button;
			IsSet = true;
		}

		public override void Tick() {
			base.Tick();

			if(IsSet) {
				Texture glyphTexture = Input.GetGlyph(Button, InputGlyphSize.Medium, GlyphStyle.Knockout.WithNeutralColorABXY());

				Glyph.Texture = glyphTexture;
				Glyph.Style.AspectRatio = (float) glyphTexture.Width / glyphTexture.Height;
			}
		}
	}
}
