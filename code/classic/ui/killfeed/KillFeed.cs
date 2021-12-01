using System;
using System.Threading.Tasks;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	public partial class KillFeed : Panel {
		public static KillFeed Current;

		public KillFeed() {
			Current = this;

			StyleSheet.Load("/classic/ui/killfeed/KillFeed.scss");
		}

		public virtual Panel AddEntry(long lsteamid, string left, long rsteamid, string right, string method) {
			var e = Current.AddChild<KillFeedEntry>();

			e.Left.Text = left;
			if(string.IsNullOrWhiteSpace(e.Left.Text)) {
				e.Left.AddClass("noname");
			}
			e.Left.SetClass("me", lsteamid == (Local.Client?.PlayerId));

			Log.Info($"death with {method.ToLower()}");

			// set kill icon based on Cause Of Death
			e.Method.SetTexture($"materials/ui/killicons/{method.ToLower()}.png");
			if(e.Method.Texture is null) {
				e.Method.SetTexture("materials/ui/killicons/generic.png");
			}



			e.Right.Text = right;
			if(string.IsNullOrWhiteSpace(e.Right.Text)) {
				e.Right.AddClass("noname");
			}
			e.Right.SetClass("me", rsteamid == (Local.Client?.PlayerId));

			return e;
		}

		[ClientRpc]
		public static void AddDeath(long lsteamid, string left, long rsteamid, string right, string method) {
			Current.AddEntry(lsteamid, left, rsteamid, right, method);
		}
	}
}
