using System;
using System.Collections.Generic;
using System.Text.Json;
using Sandbox.UI;
using Sandbox.DataModel;
using Sandbox;
using Sandbox.Utility;
using System.Threading.Tasks;
using Sandbox.UI.Construct;
using System.IO.Compression;
using System.Linq;

namespace SpeedDial.UI {
	public class VoteItemCollection : Panel {

		public static VoteItemCollection instance;

		public static bool Voted = false;
		public static List<VoteItem> items = new();

		public static bool FetchingItems = false;

		public VoteItemCollection() {
			instance = this;
			Event.Register(this);
		}


		[SDEvent.PostRound.Start]
		public static async void FetchItems() {
			if(items.Count != 0) {
				foreach(var item in items) {
					item.Delete(true);
				}
			}
			items = new();

			if(Global.IsListenServer && !FetchingItems) {
				//Log.Info("Fetching Map Selection");
				FetchingItems = true;

				/* VoteRound.RefreshMapSelection(FileSystem.Mounted.ReadAllText("/data/localMapList.json"));
				FetchingItems = false;
				return; //DEV ONLY  */
				try {
					Sandbox.Internal.Http client = new(new("https://us-central1-eagle-one-web.cloudfunctions.net/speeddial-maplist"));
					var tas = client.GetStringAsync();
					await tas;
					VoteRound.RefreshMapSelection(tas.Result);
					FetchingItems = false;
				} catch(System.Exception) {
					SpeedDialGame.Instance.ChangeRound(new WarmUpRound());
					FetchingItems = false;

					throw;
				}

			}

			/* for(int i = 0; i < 5; i++) {
				var item = AddChild<VoteItem>();
				item.initwithOffset((i + 1) * 500);
				items.Add(item);
			} */
		}


		public static void SetDataAndRecreate(string json) {
			var mapItems = JsonSerializer.Deserialize<MapItem[]>(json, new JsonSerializerOptions() {
				NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
			});

			for(int i = 0; i < mapItems.Length; i++) {
				MapItem item = mapItems[i];
				if(item.organization + "." + item.name == Global.MapName) continue;
				FetchAndCreate(item, i);
			}

			VoteItem backItem = new();
			backItem.AddClass("Back");
			backItem.Add.Label("Stay", "Stay");
			VoteItemCollection.instance.AddChild(backItem);

			backItem.MapInfo = new();
			backItem.MapInfo.Org = new();
			backItem.MapInfo.Org.Ident = "idc";
			backItem.MapInfo.Ident = "lmao";
			backItem.InitReturnButton();

			items.Add(backItem);

		}

		public void AddChildToFront(Panel p) {
			AddChild(p);

		}

		private static async void FetchAndCreate(MapItem mim, int i) {
			var idk = Package.Fetch(mim.organization + "." + mim.name, false);
			await idk;
			if(idk.Result == null) {
				Log.Warning(mim.name + "NOT FOUND ON BACKEND");
				return;
			}
			var vitem = new VoteItem();
			vitem.MapInfo = idk.Result;
			vitem.MapItem = mim;
			vitem.initwithOffset((i + 1) * 100);
			VoteItemCollection.instance.AddChild(vitem);
			items.Add(vitem);
		}

	}
}
