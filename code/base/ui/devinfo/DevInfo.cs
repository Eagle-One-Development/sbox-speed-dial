namespace SpeedDial;

/// <summary>
/// IN-DEV/PLAYTEST watermark with timestamp in bottom right of the screen.
/// </summary>
public partial class DevInfo
{
	public string Info { get; set; }
	public override void Tick()
	{
		SetClass( "visible", Debug.Playtest || Debug.Enabled );

		var old = Info;
		Info = $"{(Debug.Playtest ? "PLAYTEST" : "IN-DEV")} {DateTime.Now:dd.MM.yyyy}";

		if ( Info != old )
			StateHasChanged();
	}
}
