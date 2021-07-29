using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Sandbox;
using SpeedDial.Player;

namespace SpeedDial {
	public class PostRound : BaseRound {
		public override string RoundName => "Post Round";
		public override int RoundDuration => 1;//10

		private static WebSocket client = new();

		protected override void OnFinish() {
			Log.Info("Finished Game Round");

		}

		protected override void OnStart() {
			Log.Info("POST ROUND START");
			var players = Client.All;
			foreach(var p in players.ToArray()) {

				if(p.Pawn is SpeedDialPlayer jp) {
					jp.Freeze();
					//jp.StopSoundtrack(To.Single(jp));
				}
			}

			if(Host.IsServer) {
				SendGameInfo();
			}


		}

		protected override void OnTimeUp() {
			Log.Info("Post Round Time Up");

			SpeedDialGame.Instance.ChangeRound(new VoteRound());

			base.OnTimeUp();
		}

		private static async void SendGameInfo() {
			Log.Info("Sending Game Info");
			ScoreMessage sm = new();
			string[] splitMessage = Global.MapName.Split('.');
			sm.mapName = splitMessage[0];
			if(splitMessage.Length > 1)
				sm.mapOrg = Global.MapName[(Global.MapName.IndexOf('.') + 1)..];
			else {
				Log.Warning("Invalid Map. Won't send to backend. No Organization found for map");
				return;
			}
			sm.type = "round_results";
			sm.scores = new();
			foreach(var item in Client.All.Where((e) => e.Pawn is SpeedDialPlayer).ToList()) {
				sm.scores.Add(new() {
					name = item.Name,
					score = (item.Pawn as SpeedDialPlayer).KillScore,
					steamid = item.SteamId.ToString()
				});
			}
			Log.Info(sm.ToString());

			//return;
			await client.Connect($"ws://34.69.127.70:6969");

			await client.Send(JsonSerializer.Serialize(sm, new() {
				IncludeFields = true
			}));

			Log.Info("Sent Round Info");


		}

		struct ScoreMessage {
			public string type;
			public string mapName;
			public string mapOrg;
			public List<PlayerScore> scores;

			public override string ToString() {

				string end = $"{type} : {mapName} : {mapOrg} : [";
				foreach(var item in scores) {
					end += item.ToString();
				}
				return end + "\n]";
			}
		}

		struct PlayerScore {
			public string name;
			public string steamid;
			public int score;

			public override string ToString() {
				return $"\n{name} : {steamid} : {score}";
			}
		}
	}
}
