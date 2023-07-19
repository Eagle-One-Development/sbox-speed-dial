using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.UI;

public partial class DrugPanel
{
	public Panel ProgressBar { get; set; }
	public Label DrugName { get; set; }

	public override void Tick()
	{
		if ( Game.LocalPawn is ClassicPlayer player )
		{
			var old = DrugName.Text;
			DrugName.Text = $"{player.DrugType.ToString().ToUpper()}";
			DrugName.SetClass( "show", (Game.LocalPawn as ClassicPlayer).ActiveDrug && (Game.LocalPawn as ClassicPlayer).TimeSinceDrugTaken >= 1.5f );

			if ( DrugName.Text != old )
				StateHasChanged();

			if ( player.ActiveDrug )
			{
				var progress = 1 - (player.TimeSinceDrugTaken / player.DrugDuration);
				ProgressBar.Style.Width = Length.Fraction( progress );
				ProgressBar.Style.Dirty();
			}
			else
			{
				ProgressBar.Style.Width = Length.Percent( 0 );
			}
		}
	}
}
