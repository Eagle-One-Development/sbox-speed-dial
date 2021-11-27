using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Sandbox;

//CREDIT: Taken from Espionage.Engine by Jake Wooshito
namespace SpeedDial {
	public abstract partial class Round : BaseNetworkable {
		/// <summary>
		/// Duration of a round
		/// </summary>
		public virtual int RoundDuration => 0;
		/// <summary>
		/// Name of The round
		/// </summary>
		public virtual string RoundName => "";

		//When the end of the round occurs relative to the Time.Now of the start of the round
		[Net, Predicted]
		public float RoundEndTime { get; set; }

		[Net] public bool Finished { get; private set; }

		/// <summary>
		/// Time left in the round
		/// </summary>
		[Net, Predicted]
		public float TimeLeft {
			get {
				return RoundEndTime - Time.Now;
			}
		}

		/// <summary>
		/// Formatted version of the time left in the round in seconds
		/// </summary>
		[Net]
		public string TimeLeftFormatted { get; set; } = "";

		/// <summary> [Server Assert] Start the round </summary>
		public void Start() {
			Host.AssertServer();
			MapSettings.Current?.OnRoundStarted.Fire(null, ClassInfo.Name);

			Log.Info($"Round started {GetType()}");

			if(Host.IsServer && RoundDuration > 0) {
				RoundEndTime = Time.Now + RoundDuration;
			}

			_ = SecondTimer();

			OnStart();
		}

		/// <summary> [Server Assert] Finish the round </summary>
		public void Finish() {
			Host.AssertServer();

			if(Finished)
				return;

			Log.Info($"Round ended {GetType()}");

			RoundEndTime = 0f;

			Finished = true;
			OnFinish();
		}

		/// <summary> [Server] On Server Tick </summary>
		[Event.Tick.Server]
		public virtual void OnTick() { }

		public virtual void OnSecond() {
			if(Host.IsServer) {
				if(RoundEndTime > 0 && Time.Now >= RoundEndTime) {
					RoundEndTime = 0f;
					OnFinish();
				} else {
					TimeLeftFormatted = TimeSpan.FromSeconds(TimeLeft).ToString(@"mm\:ss");
				}
			}
		}

		private async Task SecondTimer() {
			while(!Finished) {
				OnSecond();
				await GameTask.DelaySeconds(1);
			}
		}

		/// <summary> [Server] Will invoke when the round has started </summary>
		protected virtual void OnStart() { }

		/// <summary> [Server] Will invoke when the round has finished </summary>
		protected virtual void OnFinish() { }

		/// <summary> [Server] Will invoke when a pawn has been killed </summary>
		public virtual void OnPlayerKilled(BasePlayer pawn) { }

		/// <summary> [Server] Willinvoke when a pawn has respawned </summary>
		public virtual void OnPlayerRespawned(BasePlayer newPawn) { }
	}
}
