using Sandbox;
using Sandbox.UI;

using SpeedDial.Classic.Player;
using SpeedDial.Classic.Settings;

namespace SpeedDial.Classic.UI {
	public partial class KillFeed : Panel {

		public static KillFeed Instance;

		public KillFeed() {
			StyleSheet.Load("/classic/ui/killfeed/SD_KillFeed.scss");
			Instance = this;

		}
		[Event("SDEvents.Settings.Changed")]
		public void onSettingChange() {
			if(SettingsManager.GetSetting("Kill Feed").TryGetBool(out bool? res)) {
				SetClass("enabled", res.Value);
			}
			//Can't find a better playe to put it in. if put in the constructor of player it calls it multiple times. for each player joined.
			if(Local.Pawn is SpeedDialPlayer sdp) {
				sdp.onSettingChange();
			}

		}

		public Panel AddEntry(long lsteamid, string left, long rsteamid, string right, string method, bool Dom, bool Mult, bool Rev, COD cod) {

			var e = AddChild<KillFeedEntry>();

			e.AddClass(method);

			e.Left.Text = left;
			e.Left.SetClass("me", lsteamid == (Local.PlayerId));

			e.Right.Text = right;
			e.Right.SetClass("me", rsteamid == (Local.PlayerId));

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
