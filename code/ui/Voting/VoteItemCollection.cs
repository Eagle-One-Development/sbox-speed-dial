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

		public static bool Voted = false;
		public static List<VoteItem> items = new();

		public async void FetchItems() {
			if(items.Count != 0) {
				foreach(var item in items) {
					item.Delete(true);
				}
			}
			items = new();
			var mapItems = JsonSerializer.Deserialize<MapItem[]>(FileSystem.Mounted.ReadAllText("/data/localMapList.json"));

			for(int i = 0; i < mapItems.Length; i++) {
				MapItem item = mapItems[i];
				var vitem = new VoteItem(i);
				AddChild(vitem);
				var idk = Package.Fetch(item.id, false);
				await idk;
				if(idk.Result == null) continue;
				vitem.MapInfo = idk.Result;
				vitem.initwithOffset((i + 1) * 500);
				items.Add(vitem);
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
				Log.Error("Current Map not Found.... This should not happen.");
				//backItem.Delete(true);
				backItem.MapInfo = items[0].MapInfo;
				backItem.initwithOffset(500);
				items.Add(backItem);
				return;
			}
			backItem.MapInfo = pakfetch.Result;
			backItem.initwithOffset(500);
			items.Add(backItem);

			/* for(int i = 0; i < 5; i++) {
				var item = AddChild<VoteItem>();
				item.initwithOffset((i + 1) * 500);
				items.Add(item);
			} */
		}

	}
}
