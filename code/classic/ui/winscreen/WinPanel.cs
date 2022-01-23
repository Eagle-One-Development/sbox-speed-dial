using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.UI {
	[UseTemplate]
	public partial class WinPanel : Panel {
		public Client Client { get; set; }
		public Image Portrait { get; set; }
		public Label Score { get; set; }
		public Image Avatar { get; set; }
		public Image Icon { get; set; }
		public Label Position { get; set; }
		public Label Name { get; set; }


		public void UpdateFrom(Client client, int position) {
			if(!client.IsValid()) {
				Client = null;
				return;
			}
			Client = client;
			var pawn = client.GetPawn<ClassicPlayer>();
			Portrait.Texture = pawn.Character.PortraitTexture;
			Score.Text = $"{client.GetValue("score", 0)} PTS";
			Avatar.SetTexture($"avatar:{client.PlayerId}");
			//TODO: icon
			Position.Text = $"{(position == 1 ? "WINNER" : position == 2 ? "2ND" : "3RD")}";
			Name.Text = $"{client.Name.ToUpper()}";
		}

		public override void Tick() {
			base.Tick();
			SetClass("hidden", !Client.IsValid());
		}
	}
}
