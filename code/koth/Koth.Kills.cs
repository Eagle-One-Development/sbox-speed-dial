using SpeedDial.Classic.UI;
using SpeedDial.Classic.Player;
using SpeedDial.Classic;

namespace SpeedDial.Koth;

public partial class KothGamemode : ClassicGamemode
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
				ScreenHints.FireEvent( To.Single( player.Client ), killtext, killtextextra, true, player.LastAttacker.Client, killevent is KillEvent.Domination or KillEvent.Revenge );
			}
		}
	}

	public override void HandleKillFeedEntry( ClassicPlayer victim, bool highlight = false )
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

	/// <summary>
	/// Handle current domination and return the dimination event for the passed killed pawn
	/// </summary>
	/// <param name="pawn">killed pawn</param>
	/// <returns>returns KillEvent.Domination if the pawn is dominated by its last attacker. returns KillEvent.Revenge if the last attacker has taken revenge against pawn. returns KillEvent.None on a normal kill</returns>
	public override KillEvent HandleDomination( ClassicPlayer pawn )
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
}
