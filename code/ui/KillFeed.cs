using Sandbox;
using Sandbox.UI;
using SpeedDial.Player;
using SpeedDial.Settings;

namespace SpeedDial.UI {
	public partial class KillFeed : Panel {

		public static KillFeed Instance;

		public KillFeed() {
			StyleSheet.Load("/ui/KillFeed.scss");
			Instance = this;

			SettingsManager.SettingsChanged += onSettingChange;
		}

		public void onSettingChange() {
			//Log.Info("Kill feed CHanged");
			if(SettingsManager.GetSetting("Kill Feed").TryGetBool(out bool? res)) {
				SetClass("enabled", res.Value);
			}
			//Can't find a better playe to put it in. if put in the constructor of player it calls it multiple times. for each player joined.
			if(Local.Pawn is SpeedDialPlayer sdp) {
				sdp.onSettingChange();
			}

		}

		public Panel AddEntry(ulong lsteamid, string left, ulong rsteamid, string right, string method, bool Dom, bool Mult, bool Rev, COD cod) {

			//Log.Info($"{left} killed {right} using {method}");

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
