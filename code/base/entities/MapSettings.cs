using System;

using Sandbox;

//CREDIT: Taken from Espionage.Engine by Jake Wooshito
namespace SpeedDial {
	/// <summary> An sd_map_settings entity is the central point of any Speed-Dial Map.
	/// It has output callbacks for a wide variety of functions in game
	/// as well as it stores meta data about a map. 
	/// To define which gamemodes this map supports, use the tags, each tag symbolizing the gamemode's name. 
	/// </summary>
	[Library("sd_map_settings", Title = "Speed-Dial Map")]
	[Hammer.EntityTool("Map Settings", "Speed-Dial", "Defines information about a map, and its supported gamemodes.")]
	public partial class MapSettings : Entity {
		public static MapSettings Current { get; private set; }

		public MapSettings() {
			if(Current is not null)
				throw new Exception("Only one sd_map_settings is allowed on a map");

			Current = this;
		}

		//
		// Event Callbacks
		//

		/// <summary> Gamemode has started, string is the name of the gamemode </summary>
		public Output<string> GamemodeStart { get; private set; }

		/// <summary> Gamemode has finished, string is the name of the gamemode </summary>
		public Output<string> GamemodeFinish { get; private set; }

		/// <summary> Round has finished, string is the name of the round </summary>
		public Output<string> OnRoundFinished { get; private set; }

		/// <summary> Round has started, string is the name of the round </summary>
		public Output<string> OnRoundStarted { get; private set; }

		/// <summary> Pawn has died, string is the name of the pawn </summary>
		public Output<string> OnPawnKilled { get; private set; }

		/// <summary> Pawn has respawned, string is the name of the pawn </summary>
		public Output<string> OnPawnRespawned { get; private set; }

		/// <summary> Client is ready, string is the username of the client </summary>
		public Output<string> OnClientReady { get; private set; }

		/// <summary> Client has joined, string is the username of the client </summary>
		public Output<string> OnClientJoined { get; private set; }

		/// <summary> Client has disconnected, string is the username of the client </summary>
		public Output<string> OnClientDisconnected { get; private set; }
	}
}
