namespace SpeedDial.Classic.Player;

[GameResource( "Speed-Dial Character", "sdchar", "Speed-Dial Character Definition", Icon = "person" )]
public partial class Character : GameResource
{
	public string CharacterName { get; set; } = "name";
	public string Description { get; set; } = "desc";
	public string WeaponClass { get; set; } = "sd_pistol";
	[ResourceType( "png" )] public string Portrait { get; set; } = "materials/ui/portraits/jack_hd.png";
	[ResourceType( "vmdl" )] public string Model { get; set; } = "models/playermodels/character_jack.vmdl";

	[EditorBrowsable( EditorBrowsableState.Never )]
	public static List<Character> All = new();

	[EditorBrowsable( EditorBrowsableState.Never )]
	public Texture PortraitTexture { get; private set; }

	[EditorBrowsable( EditorBrowsableState.Never )]
	public Model CharacterModel { get; private set; }

	protected override void PostLoad()
	{
		if ( Game.IsClient )
		{
			PortraitTexture = Texture.Load( FileSystem.Mounted, Portrait );
		}

		CharacterModel = Sandbox.Model.Load( Model );

		All.Add( this );
		Log.Debug( $"loaded character {CharacterName}" );
	}
}
