using System;
using System.Threading.Tasks;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	[UseTemplate]
	public partial class KillFeed : Panel {
		public static KillFeed Current { get; private set; }

		public KillFeed() {
			Current = this;
		}

		public virtual Panel AddEntry(long lsteamid, string left, long rsteamid, string right, string method, bool domination = false) {
			var e = Current.AddChild<KillFeedEntry>();

			e.Left.Text = left;
			if(string.IsNullOrWhiteSpace(e.Left.Text)) {
				e.Left.AddClass("noname");
			}
			e.Left.SetClass("me", lsteamid == (Local.Client?.PlayerId));

			// set kill icon based on Cause Of Death
			e.Method.SetClass("domination", domination);
			e.Method.SetTexture($"materials/ui/killicons/{method.ToLower()}.png");
			if(e.Method.Texture is null) {
				e.Method.SetTexture("materials/ui/killicons/generic.png");
			}

			e.Right.Text = right;
			if(string.IsNullOrWhiteSpace(e.Right.Text)) {
				e.Right.AddClass("noname");
			}
			e.Right.SetClass("me", rsteamid == (Local.Client?.PlayerId));

			e.Important = lsteamid == (Local.Client?.PlayerId) || rsteamid == (Local.Client?.PlayerId);

			return e;
		}

		[ClientRpc]
		public static void AddDeath(long lsteamid, string left, long rsteamid, string right, string method) {
			if(Current is null) return;
			Current.AddEntry(lsteamid, left, rsteamid, right, method);
		}

		[ClientRpc]
		public static void AddDeath(long lsteamid, string left, long rsteamid, string right, string method, bool domination) {
			if(Current is null) return;
			Current.AddEntry(lsteamid, left, rsteamid, right, method, domination);
		}
	}
}
