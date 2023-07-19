namespace SpeedDial;


public partial class GameLoop : Entity
{
	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
	}

	[Net] public IList<IClient> Clients { get; set; }

	public void SetFromCurrent()
	{
		Game.AssertServer();
		Clients.Clear();
		foreach ( var client in Game.Clients )
		{
			Clients.Add( client );
		}
	}

	public void AddClient( IClient client )
	{
		Game.AssertServer();
		Clients.Add( client );
	}

	public void RemoveClient( IClient client )
	{
		Game.AssertServer();
		Clients.Remove( client );
	}

	public void ValidateClients( Func<IClient, bool> predicate )
	{
		Game.AssertServer();
		foreach ( var client in Clients.Reverse() )
		{
			if ( !predicate( client ) )
			{
				RemoveClient( client );
			}
		}
	}
}
