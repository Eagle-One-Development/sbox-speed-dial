using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.UI {
	public partial class DrugPanel : Panel {
		private readonly Panel ProgressBar;

		public DrugPanel() {
			StyleSheet.Load("/classic/ui/drug/DrugPanel.scss");

			ProgressBar = Add.Panel("progressbar");
			ProgressBar.Add.Panel("gradient");
		}

		public override void Tick() {
			if(Local.Pawn is ClassicPlayer player) {
				if(player.ActiveDrug) {
					var progress = 1 - player.TimeSinceDrugTaken / player.DrugDuration;
					ProgressBar.Style.Width = Length.Percent(progress * 100);
				} else {
					ProgressBar.Style.Width = Length.Percent(0);
				}
			}
		}
	}
}
