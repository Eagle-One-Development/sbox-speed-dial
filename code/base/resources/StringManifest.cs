namespace SpeedDial;

[GameResource( "Speed-Dial String Manifest", "sdstrman", "desc" )]
public class StringManifest : GameResource
{
	[Property]
	public List<string> Manifest { get; set; }
}
