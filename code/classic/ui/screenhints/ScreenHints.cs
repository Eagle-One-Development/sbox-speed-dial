namespace SpeedDial.Classic.UI;

[UseTemplate]
public partial class ScreenHints : Panel
{
	public static ScreenHints Current { get; private set; }
	private bool Active;
	private TimeSince TimeSinceActive;
	public Panel Banner { get; set; }
	public Label Title { get; set; }
	public Label Extra { get; set; }
	private bool FireExtra;
	private bool FireBanner;

	// killer info
	public string KilledBy => $"KILLED BY:";
	public string KillerName { get; set; } = $"HANDSOME PERSON";
	public Panel KillerInfo { get; set; }
	public Image KillerAvatar { get; set; }
	private bool FireKiller;
	private Client Killer;
	private bool Domination;

	public ScreenHints()
	{
		Current = this;

		Banner.BindClass( "visible", () => Active && FireBanner );
		Title.BindClass( "visible", () => Active && TimeSinceActive > 0.05f - (FireBanner ? 0 : 0.05f) );
		Extra.BindClass( "visible", () => Active && FireExtra && TimeSinceActive > 0.5f - (FireBanner ? 0 : 0.05f) );

		KillerInfo.BindClass( "visible", () => (Active && FireKiller && (TimeSinceActive > 0.05f)) || (FireKiller && TimeSinceActive < 1.2f) );
		KillerInfo.BindClass( "dominating", () => Domination );
	}

	public override void Tick()
	{

		if ( TimeSinceActive > 1.5f )
		{
			Active = false;
			FireExtra = false;
		}

		KillerAvatar.SetTexture( $"avatar:{Killer?.PlayerId}" );
		KillerName = $"{Killer?.Name}";

		// if you're here to find a way to forcefully hide the current 
		// animation if a new one is played while it's still running... good luck
	}

	[ClientRpc]
	public static void FireEvent( string title, string extra, bool banner )
	{
		if ( Current is null ) return;
		Current.Title.Text = title;
		Current.Extra.Text = $"{extra}";
		Current.FireBanner = banner;
		Current.FireExtra = true;
		Current.FireKiller = false;
		Current.Domination = false;

		Current.Active = true;
		Current.TimeSinceActive = 0;
	}

	[ClientRpc]
	public static void FireEvent( string title, bool banner )
	{
		if ( Current is null ) return;
		Current.Title.Text = title;
		Current.FireExtra = false;
		Current.FireBanner = banner;
		Current.FireKiller = false;
		Current.Domination = false;

		Current.Active = true;
		Current.TimeSinceActive = 0;
	}

	[ClientRpc]
	public static void FireEvent( string title, string extra )
	{
		if ( Current is null ) return;
		Current.Title.Text = title;
		Current.Extra.Text = $"{extra}";
		Current.FireExtra = true;
		Current.FireBanner = true;
		Current.FireKiller = false;
		Current.Domination = false;

		Current.Active = true;
		Current.TimeSinceActive = 0;
	}

	[ClientRpc]
	public static void FireEvent( string title )
	{
		if ( Current is null ) return;
		Current.Title.Text = title;
		Current.FireExtra = false;
		Current.FireBanner = true;
		Current.FireKiller = false;
		Current.Domination = false;

		Current.Active = true;
		Current.TimeSinceActive = 0;
	}

	[ClientRpc]
	public static void FireEvent( string title, string extra, bool banner, Client killer )
	{
		if ( Current is null ) return;
		Current.Title.Text = title;
		Current.Extra.Text = $"{extra}";
		Current.FireBanner = banner;
		Current.FireExtra = true;
		Current.FireKiller = true;
		Current.Killer = killer;
		Current.Domination = false;

		Current.Active = true;
		Current.TimeSinceActive = 0;
	}

	[ClientRpc]
	public static void FireEvent( string title, string extra, bool banner, Client killer, bool domination )
	{
		if ( Current is null ) return;
		Current.Title.Text = title;
		Current.Extra.Text = $"{extra}";
		Current.FireBanner = banner;
		Current.FireExtra = true;
		Current.FireKiller = true;
		Current.Killer = killer;
		Current.Domination = domination;

		Current.Active = true;
		Current.TimeSinceActive = 0;
	}
}
