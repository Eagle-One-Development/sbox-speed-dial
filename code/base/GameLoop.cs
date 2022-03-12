namespace SpeedDial;


public partial class GameLoop : Entity {
	public override void Spawn() {
		base.Spawn();

		Transmit = TransmitType.Always;
	}

	[Net] public IList<Client> Clients { get; set; }

	public void SetFromCurrent() {
		Host.AssertServer();
		Clients.Clear();
		foreach(var client in Client.All) {
			Clients.Add(client);
		}
	}

	public void AddClient(Client client) {
		Host.AssertServer();
		Clients.Add(client);
	}

	public void RemoveClient(Client client) {
		Host.AssertServer();
		Clients.Remove(client);
	}

	public void ValidateClients(Func<Client, bool> predicate) {
		Host.AssertServer();
		foreach(var client in Clients.Reverse()) {
			if(!predicate(client)) {
				RemoveClient(client);
			}
		}
	}
}
