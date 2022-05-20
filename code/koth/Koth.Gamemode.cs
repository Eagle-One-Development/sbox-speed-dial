using SpeedDial.Classic;
using SpeedDial.Classic.Bots;
using SpeedDial.Koth.Bots;
using SpeedDial.Koth.Player;
using SpeedDial.Koth.Rounds;
using SpeedDial.Koth.UI;

namespace SpeedDial.Koth;

[Library( "koth" )]
public partial class KothGamemode : ClassicGamemode
{
	public override GamemodeIdentity Identity => GamemodeIdentity.Koth;

	public override void OnBotAdded( ClassicBot bot )
	{
		bot.ApplyBehaviour<KothBotBehaviour>();
	}

	protected override void OnClientReady( Client client )
	{
		client.AssignPawn<KothPlayer>( true );
	}

	public override void CreateGamemodeUI()
	{
		Hud.SetGamemodeUI( new KothHud() );
	}

	protected override void OnStart()
	{
		ChangeRound( new KothWarmupRound() );
	}
}
