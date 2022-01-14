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

		/// <summary>
		/// Call Finish() to finish a round
		/// </summary>
		[Net] public bool Finished { get; private set; }
		/// <summary>
		/// Call Start() to finish a round
		/// </summary>
		[Net] public bool Started { get; private set; }

		[Net] public float StartTime { get; private set; }
		public virtual string RoundText => "Round";

		/// <summary>
		/// Formatted version of the time elapsed in the round in seconds
		/// </summary>
		[Net]
		public string TimeElapsedFormatted { get; set; } = "";

		/// <summary> [Server Assert] Start the round </summary>
		public virtual void Start() {
			Host.AssertServer();

			if(Started)
				return;

			MapSettings.Current?.OnRoundStarted.Fire(null, ClassInfo.Name);

			Debug.Log($"Round start {ClassInfo.Name}");

			Started = true;
			StartTime = Time.Now;

			_ = ThinkTimer();

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

			if(Finished || !Started)
				return;

			MapSettings.Current?.OnRoundFinished.Fire(null, ClassInfo.Name);

			Debug.Log($"Round finish {ClassInfo.Name}");

			Finished = true;
			Started = false;
			OnFinish();
		}

		private async Task ThinkTimer() {
			while(!Finished && Started) {
				OnThink();
				await GameTask.DelaySeconds(ThinkTime);
			}
		}

		/// <summary> [Server] Will invoke when the round has started </summary>
		protected virtual void OnStart() { Debug.Log($"Round on start {ClassInfo.Name}"); }

		/// <summary> [Server] Will invoke every think tick, which can be defined by overriding "ThinkTime" </summary>
		protected virtual void OnThink() {
			TimeElapsedFormatted = GetElapsedTime().ToString(@"mm\:ss");
		}

		/// <summary> [Server] On Server Tick </summary>
		protected virtual void OnTick() { }

		/// <summary> [Server] Will invoke when the round has finished </summary>
		protected virtual void OnFinish() { Debug.Log($"Round on finish {ClassInfo.Name}"); }
	}
}
