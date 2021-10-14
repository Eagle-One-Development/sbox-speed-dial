using Sandbox;
using System;
using SpeedDial.Player;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SpeedDial {

	public abstract partial class BaseRound : BaseNetworkable {
		/// <summary>
		/// Duration of a round
		/// </summary>
		public virtual int RoundDuration => 0;
		/// <summary>
		/// Name of The round
		/// </summary>
		public virtual string RoundName => "";

		/// <summary>
		/// List of players participating in the round
		/// </summary>
		public List<SpeedDialPlayer> Players = new();

		//When the end of the round occurs relative to the Time.Now of the start of the round
		[Net, Predicted]
		public float RoundEndTime { get; set; }

		/// <summary>
		/// Time left in the round
		/// </summary>
		[Net, Predicted]
		public float TimeLeft {
			get {
				return RoundEndTime - Sandbox.Time.Now;
			}
		}

		// TODO: This can be done better by using a shared timestamp.
		/// <summary>
		/// Formatted version of the time left in the round in seconds
		/// </summary>
		[Net]
		public string TimeLeftFormatted { get; set; } = "";


		public void Start() {
			if(Host.IsServer && RoundDuration > 0) {
				RoundEndTime = Sandbox.Time.Now + RoundDuration;
			}

			OnStart();
		}

		public void Finish() {
			if(Host.IsServer) {
				RoundEndTime = 0f;
				Players.Clear();
			}

			OnFinish();
		}

		public void AddPlayer(SpeedDialPlayer player) {
			Host.AssertServer();

			if(!Players.Contains(player)) {
				Players.Add(player);
			}
		}

		public virtual void OnPlayerSpawn(SpeedDialPlayer player) { }

		public virtual void OnPlayerKilled(SpeedDialPlayer player) { }

		public virtual void OnPlayerLeave(SpeedDialPlayer player) {
			Players.Remove(player);
		}

		public virtual void OnTick() { }

		public virtual void OnSecond() {
			if(Host.IsServer) {
				if(RoundEndTime > 0 && Sandbox.Time.Now >= RoundEndTime) {
					RoundEndTime = 0f;
					OnTimeUp();
				} else {
					TimeLeftFormatted = TimeSpan.FromSeconds(TimeLeft).ToString(@"mm\:ss");
					//NetworkDirty("TimeLeftFormatted", NetVarGroup.Net);
				}
			}
		}

		protected virtual void OnStart() { }

		protected virtual void OnFinish() { }

		protected virtual void OnTimeUp() { }

	}

}
