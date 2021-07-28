using System;
using System.Collections.Generic;
using System.Text.Json;
using Sandbox.UI;
using Sandbox.DataModel;
using Sandbox;
using Sandbox.Utility;
using System.Threading.Tasks;
using Sandbox.UI.Construct;

namespace SpeedDial.UI {
	public class VoteItemCollection : Panel {

		public static VoteItemCollection instance;

		public static bool Voted = false;
		public static List<VoteItem> items = new();

		public VoteItemCollection() {
			instance = this;
		}

		public async void FetchItems() {
			if(items.Count != 0) {
				foreach(var item in items) {
					item.Delete(true);
				}
			}
			items = new();

			if(Global.IsListenServer) {
				Sandbox.Internal.Http client = new(new("https://us-central1-eagle-one-web.cloudfunctions.net/speeddial-maplist"));
				var tas = client.GetStringAsync();
				await tas;
				VoteRound.RefreshMapSelection(tas.Result);
			}

			/* for(int i = 0; i < 5; i++) {
				var item = AddChild<VoteItem>();
				item.initwithOffset((i + 1) * 500);
				items.Add(item);
			} */
		}


		public async void SetDataAndRecreate(string json) {
			var mapItems = JsonSerializer.Deserialize<MapItem[]>(json, new() {
				NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
			});

			for(int i = 0; i < mapItems.Length; i++) {
				MapItem item = mapItems[i];
				FetchAndCreate(item, i);
			}

			VoteItem backItem = new(mapItems.Length);
			backItem.AddClass("Back");
			backItem.Add.Label("Stay", "Stay");
			AddChild(backItem);
			Log.Info(Global.MapName);
			string mapNameFixed = Global.MapName;
			if(!mapNameFixed.Contains(".")) mapNameFixed = "local." + mapNameFixed;
			Log.Info(mapNameFixed);
			var pakfetch = Package.Fetch(mapNameFixed, false);
			await pakfetch;
			if(pakfetch.Result == null) {
				Log.Warning("Current Map not Found.... This should not happen.");
				//backItem.Delete(true);
				backItem.MapInfo = new();
				backItem.initwithOffset(500);
				items.Add(backItem);
				return;
			}
			backItem.MapInfo = pakfetch.Result;
			backItem.initwithOffset(500);
			items.Add(backItem);
		}

		private async void FetchAndCreate(MapItem mim, int i) {
			var idk = Package.Fetch(mim.organization + "." + mim.name, false);
			await idk;
			if(idk.Result == null) return;
			var vitem = new VoteItem(i);
			AddChild(vitem);
			vitem.MapInfo = idk.Result;
			vitem.initwithOffset((i + 1) * 500);
			items.Add(vitem);
		}

	}
}
