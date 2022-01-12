using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.UI {
	[UseTemplate]
	public partial class DrugPanel : Panel {
		public Panel ProgressBar { get; set; }
		public Label DrugName { get; set; }

		public DrugPanel() {
			DrugName.BindClass("show", () => (Local.Pawn as ClassicPlayer).ActiveDrug && (Local.Pawn as ClassicPlayer).TimeSinceDrugTaken >= 1.5f);
		}

		public override void Tick() {
			if(Local.Pawn is ClassicPlayer player) {
				
				DrugName.Text = $"{player.DrugType.ToString().ToUpper()}";
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
