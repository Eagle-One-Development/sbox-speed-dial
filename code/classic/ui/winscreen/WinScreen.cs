using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {

	[UseTemplate]
	public partial class WinScreen : Panel {
		private WinPanel FirstPanel { get; set; }
		private WinPanel SecondPanel { get; set; }
		private WinPanel ThirdPanel { get; set; }
		public static WinScreen Current { get; private set; }
		private bool Open { get; set; }

		public WinScreen() {
			Current = this;
		}

		[ClientRpc]
		public static void UpdatePanels() {
			if(Current is null) return;

			var clients = Client.All.ToList();
			clients.Sort((x, y) => x.GetValue("score", 0) < y.GetValue("score", 0) ? 1 : -1);
			Debug.Log("Win Panel Update clients");

			var client1 = clients.ElementAtOrDefault(0);
			Debug.Log($"1st {client1?.Name} - {client1?.GetValue("score", 0)}");
			Current.FirstPanel.UpdateFrom(client1, 1);

			var client2 = clients.ElementAtOrDefault(1);
			Debug.Log($"2nd {client2?.Name} - {client2?.GetValue("score", 0)}");
			Current.SecondPanel.UpdateFrom(client2, 2);

			var client3 = clients.ElementAtOrDefault(2);
			Debug.Log($"3rd {client3?.Name} - {client3?.GetValue("score", 0)}");
			Current.ThirdPanel.UpdateFrom(client3, 3);
		}

		[ClientRpc]
		public static void SetState(bool state) {
			if(Current is null) return;
			Current.Open = state;
		}

		public override void Tick() {
			base.Tick();
			SetClass("open", Open);
			FirstPanel?.SetClass("open", Open && FirstPanel.Client.IsValid());
			SecondPanel?.SetClass("open", Open && SecondPanel.Client.IsValid());
			ThirdPanel?.SetClass("open", Open && ThirdPanel.Client.IsValid());
		}
	}
}
