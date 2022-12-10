using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.UI;

[UseTemplate]
public partial class DrugPanel : Panel
{
	public Panel ProgressBar { get; set; }
	public Label DrugName { get; set; }

	public DrugPanel()
	{
		DrugName.BindClass( "show", () => (Game.LocalPawn as ClassicPlayer).ActiveDrug && (Game.LocalPawn as ClassicPlayer).TimeSinceDrugTaken >= 1.5f );
	}

	public override void Tick()
	{
		if ( Game.LocalPawn is ClassicPlayer player )
		{

			DrugName.Text = $"{player.DrugType.ToString().ToUpper()}";
			if ( player.ActiveDrug )
			{
				var progress = 1 - (player.TimeSinceDrugTaken / player.DrugDuration);
				ProgressBar.Style.Width = Length.Percent( progress * 100 );
			}
			else
			{
				ProgressBar.Style.Width = Length.Percent( 0 );
			}
		}
	}
}
