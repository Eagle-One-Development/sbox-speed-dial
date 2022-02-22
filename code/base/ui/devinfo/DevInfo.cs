namespace SpeedDial;

[UseTemplate]
public partial class DevInfo : Panel {
	private string Info { get; set; }
	public override void Tick() {
		SetClass("visible", Debug.Playtest || Debug.Enabled);
		Info = $"{(Debug.Playtest ? "PLAYTEST" : "IN-DEV")} {DateTime.Now.ToString("dd.MM.yyyy")}";
	}
}
