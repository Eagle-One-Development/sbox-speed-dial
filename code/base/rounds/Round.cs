using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Sandbox;

//CREDIT: Taken from Espionage.Engine by Jake Wooshito
namespace SpeedDial {
	/// <summary> Round </summary>
	public abstract partial class Round : BaseNetworkable {
		/// <summary> how long are think ticks in seconds? </summary>
		protected virtual float ThinkTime => 0.1f;

		/// <summary> Call Finish() to finish a round </summary>
		[Net] public bool Finished { get; private set; }

		[Net] public float StartTime { get; private set; }
		public virtual string RoundText => "Round";

		/// <summary>
		/// Formatted version of the time elapsed in the round in seconds
		/// </summary>
		[Net]
		public string TimeElapsedFormatted { get; set; } = "";

		public Round() {
			// ui gets update on tick, round thinks 10 times a second
			// set it before start runs so ui doesn't look whack
			StartTime = Time.Now;
		}

		/// <summary> [Server Assert] Start the round </summary>
		public virtual void Start() {
			Host.AssertServer();
			MapSettings.Current?.OnRoundStarted.Fire(null, ClassInfo.Name);

			Log.Info($"Round started {GetType()}");

			_ = ThinkTimer();

			StartTime = Time.Now;
			OnStart();
		}

		public TimeSpan GetElapsedTime() {
			if(!Finished)
				return TimeSpan.FromSeconds(Time.Now - StartTime);
			else
				return TimeSpan.Zero;
		}

		public virtual TimeSpan GetTime() {
			return GetElapsedTime();
		}

		/// <summary> [Server Assert] Finish the round </summary>
		public virtual void Finish() {
			Host.AssertServer();

			if(Finished)
				return;

			MapSettings.Current?.OnRoundFinished.Fire(null, ClassInfo.Name);

			Log.Info($"Round ended {GetType()}");

			Finished = true;
			OnFinish();
		}

		private async Task ThinkTimer() {
			while(!Finished) {
				OnThink();
				await GameTask.DelaySeconds(ThinkTime);
			}
		}

		/// <summary> [Server] Will invoke when the round has started </summary>
		protected virtual void OnStart() { }

		/// <summary> [Server] Will invoke every think tick, which can be defined by overriding "ThinkTime" </summary>
		protected virtual void OnThink() {
			TimeElapsedFormatted = GetElapsedTime().ToString(@"mm\:ss");
		}

		/// <summary> [Server] On Server Tick </summary>
		protected virtual void OnTick() { }

		/// <summary> [Server] Will invoke when the round has finished </summary>
		protected virtual void OnFinish() { }

		/// <summary> [Server] Will when a pawn has been killed </summary>
		public virtual void OnPlayerKilled(BasePlayer pawn) { }

		/// <summary> [Server] Will when a pawn has respawned </summary>
		public virtual void OnPlayerRespawned(BasePlayer newPawn) { }
	}
}
