namespace SpeedDial;

public static class ClientExtensions
{
	/// <summary>
	/// Assigns a pawn to a client, deletes any existing previous Pawn.
	/// </summary>
	/// <typeparam name="T">Type of the new Pawn</typeparam>
	/// <param name="cl">Client extension</param>
	/// <param name="respawn">Whether to call InitialSpawn on the new Pawn or not.</param>
	/// <returns>A reference to the new Pawn.</returns>
	public static T AssignPawn<T>( this Client cl, bool respawn = true ) where T : BasePlayer, new()
	{
		cl.Pawn?.Delete();

		var player = new T();
		cl.Pawn = player;

		if ( respawn )
			player.InitialRespawn();

		return player;
	}

	/// <summary>
	/// Swaps out the pawn of a client, maintaining Transforms between pawns. Calls AssignPawn instead of the client has no Pawn.
	/// </summary>
	/// <typeparam name="T">Type of the new Pawn</typeparam>
	/// <param name="cl">Client extension</param>
	/// /// <param name="respawn">Whether to call InitialSpawn on the new Pawn or not.</param>
	/// <returns>A reference to the new Pawn.</returns>
	public static T SwapPawn<T>( this Client cl, bool respawn = true ) where T : BasePlayer, new()
	{
		// null check to assign instead
		if ( cl.Pawn is null )
		{
			return cl.AssignPawn<T>( respawn );
		}

		// swap out pawn for spectator pawn
		var oldpawn = cl.Pawn;
		var newpawn = new T();
		newpawn.Transform = oldpawn.Transform;
		cl.Pawn = newpawn;

		if ( respawn )
			newpawn.InitialRespawn();

		// get rid of old pawn
		oldpawn.Delete();

		return newpawn;
	}
}
