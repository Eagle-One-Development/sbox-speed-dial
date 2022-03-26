using SpeedDial.Classic.UI;
using SpeedDial.Classic.Player;

namespace SpeedDial.Classic;

public partial class ClassicGamemode
{

	protected override void OnPawnKilled( BasePlayer pawn )
	{
		// killfeed population
		if ( pawn is ClassicPlayer player )
		{

			// handle domination/revenge stuff
			var killevent = HandleDomination( player );

			HandleKillFeedEntry( player, killevent == KillEvent.Domination );

			string killtextextra;
			if ( killevent == KillEvent.Domination )
			{
				// last attacker dominates pawn, highlight them for pawn
				ClassicPlayer.UpdateGlow( To.Single( player.Client ), player.LastAttacker as ModelEntity, true, Color.Red );
				killtextextra = "+DOMINATION";
			}
			else if ( killevent == KillEvent.Revenge )
			{
				// last attacker has taken revenge against pawn; pawn no longer dominates last attacker
				ClassicPlayer.UpdateGlow( To.Single( player.LastAttacker.Client ), player, false, Color.Black );
				killtextextra = "+REVENGE";
			}
			else
			{
				killtextextra = "";
			}

			// get random awesome kill message
			int index = Rand.Int( 0, KillMessages.Length - 1 );
			string killtext = KillMessages[index];

			// only show killer if we got killed by a player
			if ( player.DeathCause == ClassicPlayer.CauseOfDeath.Suicide || player.LastAttacker is null )
			{
				ScreenHints.FireEvent( To.Single( player.Client ), killtext, killtextextra );
			}
			else
			{
				ScreenHints.FireEvent( To.Single( player.Client ), killtext, killtextextra, true, player.LastAttacker.Client, killevent == KillEvent.Domination || killevent == KillEvent.Revenge );
			}
		}
	}

	public virtual void HandleKillFeedEntry( ClassicPlayer victim, bool highlight = false )
	{
		// handle killfeed entry accordingly
		if ( victim.LastAttacker != null )
		{
			if ( victim.LastAttacker.Client != null )
			{
				// tell everyone but the two clients involved about the kill like usual
				ClassicKillFeed.AddDeath( To.Multiple( Client.All.Except( new Client[] { victim.Client, victim.LastAttacker.Client } ) ), victim.LastAttacker.Client.PlayerId, victim.LastAttacker.Client.Name, victim.Client.PlayerId, victim.Client.Name, victim.DeathCause.ToString() );
				// only tell the people involved about domination
				ClassicKillFeed.AddDeath( To.Multiple( new Client[] { victim.Client, victim.LastAttacker.Client } ), victim.LastAttacker.Client.PlayerId, victim.LastAttacker.Client.Name, victim.Client.PlayerId, victim.Client.Name, victim.DeathCause.ToString(), highlight );
			}
			else
			{
				ClassicKillFeed.AddDeath( victim.LastAttacker.NetworkIdent, victim.LastAttacker.ToString(), victim.Client.PlayerId, victim.Client.Name, victim.DeathCause.ToString() );
			}
		}
		else
		{
			ClassicKillFeed.AddDeath( victim.Client.PlayerId, victim.Client.Name, 0, "", victim.DeathCause.ToString() );
		}
	}

	public enum KillEvent
	{
		Domination,
		Revenge,
		None
	}

	/// <summary>
	/// List to track all previous kills, serialized with the attacker and victim of the kill. This is automatically handled and cleared up for the domination system.
	/// </summary>
	public virtual List<Kill> Kills { get; set; } = new();

	/// <summary>
	/// Handle current domination and return the dimination event for the passed killed pawn
	/// </summary>
	/// <param name="pawn">killed pawn</param>
	/// <returns>returns KillEvent.Domination if the pawn is dominated by its last attacker. returns KillEvent.Revenge if the last attacker has taken revenge against pawn. returns KillEvent.None on a normal kill</returns>
	public virtual KillEvent HandleDomination( ClassicPlayer pawn )
	{

		// add new kill to list
		var kill = new Kill( pawn.LastRecievedDamage.Attacker, pawn );
		Kills.Add( kill );

		// clear out kills with disconnected/invalid attacker or victim entity
		Kills.RemoveAll( x => !x.Attacker.IsValid() || !x.Victim.IsValid );

		// amount of times last attacker has killed us recently
		var attackerkills = Kills.Where( x => x.Attacker == pawn.LastRecievedDamage.Attacker && x.Victim == pawn ).Count();
		// amount of times we killed the attacker recently
		var victimkills = Kills.Where( x => x.Attacker == pawn && x.Victim == pawn.LastRecievedDamage.Attacker ).Count();

		// remove all kill entries where the pawn was the attacker and the current attacker was the victim, ends possible domination streaks
		Kills.RemoveAll( x => x.Attacker == pawn && x.Victim == pawn.LastRecievedDamage.Attacker );

		if ( victimkills >= 3 )
		{
			Log.Debug( $"REVENGE FROM {pawn.LastRecievedDamage.Attacker.Client.Name} AGAINST {pawn.Client.Name}" );
			return KillEvent.Revenge;
		}

		// domination on three or more kills
		if ( attackerkills >= 3 )
		{
			Log.Debug( $"DOMINATION FROM {pawn.LastRecievedDamage.Attacker.Client.Name} AGAINST {pawn.Client.Name}" );
			return KillEvent.Domination;
		}

		return KillEvent.None;
	}

	public virtual string[] KillMessages { get; } = {
			"BYPASSED",
			"WHACKED",
			"DESTROYED",
			"BROKEN",
			"ELIMINATED",
			"EXTERMINATED",
			"RUINED",
			"SLAPPED",
			"DECIMATED",
			"MINCED",
			"SMACKED",
			"VAPORIZED",
			"DRILLED",
			"404'D",
			"EMANCIPATED",
			"SMEARED",
			"TERMINATED",
			"OUTSOURCED",
			"CLAPPED",
			"CAPPED",
			"CARPE DIEM'D",
			"WASTED",
			"SHOT",
			"COMPLEXED",
			"BLASTED",
			"RDM'D",
			"ERADICATED",
			"BLASTED",
			"OBLITERATED",
			"DISCONNECTED",
			"RAZED",
			"SACKED",
			"TOTALLED",
			"SMOKED",
			"YOU ARE DEAD.",
			"MURDERED",
			"DESECRATED",
			"SPLITTED",
			"JACKED OUT",
			"RETROACTED",
			"ATE SHIT",
			"NAE NAE'D",
			"SHAT ON",
			"REDACTED",
			"BACK SCRATCHED",
			"DELETED",
			"REMOVED",
			"UNPLUGGED",
			"ZONKED",
			"BONKED",
			"CYBERSPACED",
			"SMIT",
			"YOU'RE ASS",
			"LAYED OUT",
			"LAID OUT",
			"ATOMIZED",
			"DUSTED",
			"BECAME A NEWMAN",
			"LAID LAD",
			"CRAYZED",
			"EVISCERATED",
			"CANCELLED",
			"GAME OVER",
			"TAKEN HOME",
			"BURST",
			"YOU SUCK",
			"AWFUL SKILL",
			"GIT GUD",
			"SKILL ISSUE",
			"SLIPSTREAM'D",
			"JETBALL'D",
			"BOUNTY'D",
			"QUAKED",
			"1984'D",
			"KILLED",
			"GET BENT",
			"DUNKED ON",
			"KILL MESSAGE",
			"SNAPSHOT'D",
			"WALKED THE PLANK",
			"SLICED",
			"DICED",
			"JACKASS'D",
			"DEFEATED",
			"FRIED",
			"MOTEL'D",
			"BELTALOWDA'D",
			"RAGE QUIT",
			"ALT+F4'D",
			"LOOPED",
			"SAM",
			"FACE PUNCHED",
			"LEFT 4 DEAD",
			"RED DEAD",
			"DMCA'D",
			"GREEN DAY'D",
			"GUMPED ON",
			"DODGE BALLED",
			"REFLECTED",
			"VSNAPPED",
			"DINNER TIME",
			"HAMSTER DANCED",
			"CLEANUP TIME",
			"SUCKER PUNCHED",
			"SLAIN",
			"ZOINKED",
			"GUMPED",
			"AVOOFO'D",
			"CBFLEXED",
			"MATRIXED",
			"INSANE'D",
			"DISHONOURED",
			"DETHRONED",
			"GODZILLA'D",
			"CLOCKED",
			"WALLED",
			"DOORED",
			"WINDOWED",
			"CHAIRED",
			"DESKED",
			"SHELFED",
			"EJECTED",
			"SPACED",
			"OMITTED",
			"MERGE CONFLICT",
			"STORAGE FULL",
			"OBJECT REFERENCE NOT SET TO AN INSTANCE OF AN OBJECT",
			"FRAGGED",
			"NULL",
			"PUSHED",
			"DEMOLISHED",
			"DEVASTATED",
			"RAVAGED",
			"SHATTERED",
			"SMASHED",
			"WRECKED",
			"CONSUMED",
			"DEVOURED",
			"GET FUCKED",
			"CONNECTION TERMINATED",
			"BUILD FAILED",
			"VRAD3 COMBINE'D",
			"HANG DETECTED",
		};
}

public class Kill
{
	public Kill( Entity attacker, Entity victim )
	{
		Attacker = attacker;
		Victim = victim;
	}

	public Entity Attacker;
	public Entity Victim;
}
