using Sandbox;
using Sandbox.UI;
using SpeedDial.Player;

namespace SpeedDial.UI {
	public partial class KillFeed : Panel {

		public static KillFeed Instance;

		public KillFeed() {
			StyleSheet.Load("/ui/KillFeed.scss");
			Instance = this;
		}

		public Panel AddEntry(ulong lsteamid, string left, ulong rsteamid, string right, string method, bool Dom, bool Mult, bool Rev, COD cod) {
			Log.Info($"{left} killed {right} using {method}");

			var e = AddChild<KillFeedEntry>();

			e.AddClass(method);

			e.Left.Text = left;
			e.Left.SetClass("me", lsteamid == (Local.SteamId));

			e.Right.Text = right;
			e.Right.SetClass("me", rsteamid == (Local.SteamId));

			e.IsRevenge = Rev;
			e.IsMultiKill = Mult;
			e.IsDominating = Dom;
			
			if(cod == COD.Melee) {
				e.Icon.Texture = Texture.Load("materials/ui/killfeed_melee.png");
			}

			if(Rev || Dom) {
				e.Icon.Texture = Texture.Load("materials/ui/skull.png");
			}

			return e;
		}
	}
}
