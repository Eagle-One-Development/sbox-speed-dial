using System;
using System.Linq;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.UI {
	public partial class VoteItem : Panel {

		public int voteItemID;

		public Package MapInfo;

		public Panel Center;
		public Image MapThumb;
		public Label VoteCount;
		public int votes = 0;

		public VoteItem(int id) {
			voteItemID = id;
			Center = AddChild<Panel>("Center");
			MapThumb = Center.Add.Image(classname: "MapName");
			VoteCount = Center.Add.Label("0", "VoteCount");
		}
		public async void initwithOffset(int ms) {
			await GameTask.DelayRealtime(ms);
			MapThumb.SetTexture(MapInfo.Thumb);
			SetClass("Active", true);
		}
		public override void Tick() {
			base.Tick();
			VoteCount.Text = votes.ToString();
		}
		protected override void OnClick(MousePanelEvent e) {
			if(!VoteItemCollection.Voted) {
				AddVotesForItem(voteItemID);
				VoteItemCollection.Voted = true;
			}
		}

		[ServerCmd]
		public static void AddVotesForItem(int voteItemID) {
			SetVotesForItem(voteItemID, 1);
		}
		[ClientRpc]
		public static void SetVotesForItem(int voteItemID, int Votes) {
			VoteItemCollection.items[voteItemID].votes += Votes;
		}


	}

	public class MapItem {
		public string id { get; set; }
	}
}
