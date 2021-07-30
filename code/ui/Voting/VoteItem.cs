using System;
using System.Linq;
using System.Text.Json.Serialization;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.UI {
	public partial class VoteItem : Image {

		public Package MapInfo;

		public MapItem MapItem;

		public Image Center;
		public Image MapThumb;
		public Label VoteCount;

		public Image Sticker;
		public Label MapTitle;
		public Label MapPlayCount;

		public Panel Move;
		public Image Cassette;
		public Image CassetteMapThumb;

		public Texture MapThumbnail;

		public Panel HoverItem;

		public int votes = 0;

		public VoteItem() {

			Cassette = Add.Image("/UI/MapSelection/Cassetteblank.png", "Cassette");
			CassetteMapThumb = Cassette.Add.Image(classname: "CassetteMapThumb");


			Center = Add.Image("/UI/MapSelection/Cassetteblankwithoutback.png", "Center");
			MapThumb = Center.Add.Image(classname: "MapName");

			Sticker = Center.Add.Image("/UI/MapSelection/Cassettesticker.png", "Sticker");
			VoteCount = Sticker.Add.Label("0", "VoteCount");
			MapTitle = Center.Add.Label("Map - Org", "MapTitle");
			MapPlayCount = Center.Add.Label("P-0", "PlayCount");

			HoverItem = Add.Panel("HoverItem");

		}
		public async void initwithOffset(int ms) {
			//Log.Info("initing Vote Item");
			MapTitle.Text = MapInfo.Title + " - " + MapInfo.Org.Ident;
			MapThumbnail = Texture.Load(MapInfo.Thumb);
			MapThumb.Texture = MapThumbnail;
			CassetteMapThumb.Texture = MapThumbnail;

			MapPlayCount.Text = "P-" + MapItem.roundsPlayed.ToString();
			await GameTask.DelayRealtime(ms);
			SetClass("Active", true);
			//Log.Info("Finished Vote Item");

		}

		public void InitReturnButton() {

			DeleteChildren(true);

			VoteCount = Add.Label("0", "VoteCount DefaultLabel");

			Add.Label("STAY", "Stay DefaultLabel");
			Add.Icon("fast_rewind", "ReturnIcon DefaultLabel");
			Texture = null;


		}
		public override void Tick() {
			base.Tick();
			VoteCount.Text = votes.ToString();
		}
		protected override void OnClick(MousePanelEvent e) {
			if(!VoteItemCollection.Voted) {
				AddVotesForItem(MapInfo.FullIdent);
				VoteItemCollection.Voted = true;
			}
		}

		[ServerCmd]
		public static void AddVotesForItem(string voteItemID) {
			SetVotesForItem(voteItemID);
		}
		[ClientRpc]
		public static void SetVotesForItem(string voteItemID) {
			var item = VoteItemCollection.items.Find((e) => e.MapInfo.FullIdent.Equals(voteItemID));
			item.votes += 1;
		}


	}

	public class MapItem {
		public string organization { get; set; }
		public string name { get; set; }
		public int roundsPlayed { get; set; }
	}
}
