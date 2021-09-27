using System.Numerics;

using System;
using Sandbox;
using SpeedDial.Weapons;
using SpeedDial.UI;
using SpeedDial.Meds;
using System.Threading.Tasks;
using SpeedDial.GameSound;
using System.Linq;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer : Sandbox.Player {

		[Net, Local] public TimeSince TimeSinceDied { get; set; } = 0;
		[Net, Local] public float RespawnTime { get; set; } = 1f;
		[Net] public bool pickup { get; set; }
		private Entity pickUpEntity;
		TimeSince timeSinceDropped;
		[Net, Local, Predicted] public TimeSince TimeSinceMelee { get; set; }
		[Net, Local] public bool ResetTimeSinceMelee { get; set; } = false;
		[Net, Local, Predicted] public TimeSince TimeSinceMedTaken { get; set; }
		[Net, Local] public bool ResetTimeSinceMedTaken { get; set; }
		[Net] public bool MedTaken { get; set; }
		[Net] public float MedDuration { get; set; }
		[Net] public DrugType CurrentDrug { get; set; }
		public Particles DrugParticles { get; set; }
		[Net] public BaseSpeedDialCharacter character { get; set; }
		[Net, Local, Predicted] public bool Frozen { get; set; } = false; // sorry for naming differences
		public SoundTrack SoundTrack { get; set; }
		public bool SoundtrackPlaying { get; set; }
		private bool screenOpen = false;
		[ClientVar] public static bool sd_soundtrack { get; set; } = true;

		public SpeedDialPlayer() {
			Inventory = new SpeedDialInventory(this);
		}

		public void InitialSpawn() {

			Controller = new SpeedDialController();
			Camera = new SpeedDialCamera();

			//Set a default character
			MedTaken = false;
			character = SpeedDialGame.Instance.characters[0];

			SpeedDialGame.Instance.Round?.OnPlayerSpawn(this);

			if(SpeedDialGame.Instance.Round is PreRound) {
				(Controller as SpeedDialController).Freeze = true;
				Frozen = true;
				StopSoundtrack(To.Single(this), true);
				PlaySoundtrack(To.Single(this));
			}

			Respawn();
		}

		private bool GetMusicBool() {
			if(Settings.SettingsManager.GetSetting("Music On").TryGetBool(out bool? res)) {
				return res.Value;
			}
			return false;
		}

		public void onSettingChange() {
			if(!IsClient) return;
			if(Global.IsListenServer)
				if(Settings.SettingsManager.GetSetting("Sniper Wallbang").TryGetBool(out bool? res)) {
					SetSetting(res.Value);
				}
			if(!GetMusicBool()) {
				SoundTrack?.Stop();
				SoundtrackPlaying = false;
				return;
			}
			if(!SoundtrackPlaying && SpeedDialGame.Instance is not null)
				_ = PlaySoundtrackAsync(SpeedDialGame.Instance.CurrentSoundtrack, 2.5f);
		}

		[ServerCmd]
		public static void SetSetting(bool val) {
			SpeedDialGame.Instance.SniperCanPenetrate = val;
		}

		[ClientRpc]
		public void PlayRoundendClimax() {
			if(!GetMusicBool()) return;
			SoundTrack.FromScreen("climax");
			_ = StopSoundtrackAsync();
		}

		private async Task StopSoundtrackAsync(int delay = 5) {
			await GameTask.DelaySeconds(delay);
			_ = SoundTrack.Stop(5, 500);
			SoundtrackPlaying = false;
		}


		[ClientRpc]
		public void PlaySoundtrack() {
			if(!GetMusicBool()) return;
			_ = PlaySoundtrackAsync(SpeedDialGame.Instance.CurrentSoundtrack, 2.5f);
		}

		private async Task PlaySoundtrackAsync(string track, float delay) {
			Log.Info($"NEW TRACK {track}");
			await GameTask.DelaySeconds(delay);
			if(!SoundtrackPlaying) {
				SoundTrack = SoundTrack.FromScreen(track);
				SoundtrackPlaying = true;
			}
		}

		[ClientRpc]
		public void StopSoundtrack(bool instant = false) {
			if(!GetMusicBool()) return;
			if(instant) {
				SoundTrack?.Stop();
				SoundtrackPlaying = false;
			} else {
				SoundTrack?.Stop(1);
				SoundtrackPlaying = false;
			}
		}

		[ClientRpc]
		public void FadeSoundtrack(float volumeTo) {
			if(!GetMusicBool()) return;
			SoundTrack?.FadeVolumeTo(volumeTo);
		}

		[ClientRpc]
		public void PlayUISound(string sound) {
			Sound.FromScreen(sound);
		}

		public override void Respawn() {
			if(character == null) {
				SetModel("models/playermodels/character_fallback.vmdl");
			} else {
				SetModel(character.Model);
			}

			(Camera as SpeedDialCamera).Freeze = false;
			(Controller as SpeedDialController).Freeze = false;
			Animator = new PlayerAnimator();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			CauseOfDeath = COD.Gunshot;

			Host.AssertServer();

			KillCombo = 0;
			MedTaken = false;

			// reset weapon/ammo on spawn
			ResetWeapon();

			LifeState = LifeState.Alive;
			Health = 100;
			Velocity = Vector3.Zero;
			CreateHull();
			ResetInterpolation();
			SpeedDialGame.MoveToSpawn(this);
		}

		/// <summary>
		/// Reloads weapons to give them their selected character's weapons.
		/// </summary>
		public void ResetWeapon() {
			if(Inventory == null) {
				return;
			}
			Inventory.DeleteContents();
			BaseSpeedDialWeapon weapon = Library.Create<BaseSpeedDialWeapon>(character.Weapon);
			Inventory.Add(weapon, true);
			if(character == null) {
				SetModel("models/playermodels/character_fallback.vmdl");
			} else {
				SetModel(character.Model);
			}
		}

		[ClientRpc]
		public void SetPlayerBodyGroup(int group, int value) {
			//Log.Info("Set Bodygroup Client");
			SetBodyGroup(group, value);
		}

		[ClientRpc]
		public void DrugBump(string s, string f, bool b) {
			AmmoPanel.Current.DrugBump(s, f, b);
		}

		[ClientRpc]
		public void Dominate(Entity e) {
			AmmoPanel.Current.AddDominator(e);
		}

		[ClientRpc]
		public void Revenge(Entity e) {
			AmmoPanel.Current.RemoveDominator(e);
		}

		/// <summary>
		/// Completely freezes the player. Essentially stops camera, controller and entity from simulating.
		/// </summary>
		public void Freeze() {
			(Controller as SpeedDialController).Freeze = true;
			(Camera as SpeedDialCamera).Freeze = true;
			Frozen = true;
		}

		/// <summary>
		/// Completely unfreezes the player. Resumes camera, controller and entity simulating.
		/// </summary>
		public void Unfreeze() {
			(Controller as SpeedDialController).Freeze = false;
			(Camera as SpeedDialCamera).Freeze = false;
			Frozen = false;
		}

		/// <summary>
		/// Completely freezes or unfreezes the player. Essentially stops/resumes camera, controller and entity from simulating.
		/// </summary>
		public void Freeze(bool freeze) {
			(Controller as SpeedDialController).Freeze = freeze;
			(Camera as SpeedDialCamera).Freeze = freeze;
			Frozen = freeze;
		}

		/// <summary>
		/// Handles Punching
		/// </summary>
		async Task HandleMelee() {
			if(Input.Pressed(InputButton.Attack1)) {
				if(TimeSinceMelee > 0.5f) {
					ResetTimeSinceMelee = true;
					await GameTask.DelaySeconds(0.1f);
					var forward = EyeRot.Forward;
					Vector3 pos = EyePos + Vector3.Down * 20f;
					var tr = Trace.Ray(pos, pos + forward * 40f)
					.UseHitboxes()
					.Ignore(this)
					.Size(20f)
					.Run();

					using(Prediction.Off()) {
						if(IsServer) {
							PlaySound("woosh");
						}
					}

					SetAnimBool("b_attack", true);

					if(!IsServer) return;
					if(!tr.Entity.IsValid()) return;
					if(!(LifeState == LifeState.Alive)) return;

					// We turn predictiuon off for this, so any exploding effects don't get culled etc
					using(Prediction.Off()) {

						var damage = DamageInfo.FromBullet(tr.EndPos, Owner.EyeRot.Forward * 100, 200)
							.UsingTraceResult(tr)
							.WithAttacker(Owner)
							.WithWeapon(this);

						damage.Attacker = this;
						damage.Position = Position;
						if(IsServer) {
							PlaySound("smack");
						}
						await GameTask.DelaySeconds(0.2f);
						if(!(LifeState == LifeState.Alive)) return;

						if(tr.Entity is SpeedDialPlayer player) {
							player.CauseOfDeath = COD.Melee;
						}

						tr.Entity.TakeDamage(damage);
					}
				}
			}
		}

		public override void Simulate(Client cl) {

			if(SpeedDialGame.Instance.Round is PreRound) {
				screenOpen = true;
			}

			if(Frozen) return;

			if(LifeState == LifeState.Dead) {
				DrugParticles?.Destroy(false);
				if(TimeSinceDied > RespawnTime && IsServer) {
					Respawn();
				}
				return;
			}

			if(!screenOpen && SpeedDialGame.Instance.Round is GameRound) {
				CharacterSelect.Current?.ToggleOpen();

				screenOpen = true;
			}

			if(ResetTimeSinceMelee) {
				TimeSinceMelee = 0;
				ResetTimeSinceMelee = false;
			}

			if(ResetTimeSinceMedTaken) {
				TimeSinceMedTaken = 0;
				ResetTimeSinceMedTaken = false;
			}

			if(KillCombo > maxCombo) {
				maxCombo = KillCombo;
				cl.SetScore("maxcombo", maxCombo);
			}

			var controller = GetActiveController();
			controller?.Simulate(cl, this, GetActiveAnimator());

			if(Input.ActiveChild != null) {
				ActiveChild = Input.ActiveChild;
			}

			if(ActiveChild == null) {
				_ = HandleMelee();
			}

			if(TimeSinceMedTaken > MedDuration) {
				MedTaken = false;

				DrugParticles?.Destroy(false);

				//Basically remove our extra health after the drug duration if we're high on leaf
				if(CurrentDrug == DrugType.Leaf) {
					if(Health > 100) {
						Health = 100;
					}
				}
			}

			SetAnimBool("b_polvo", MedTaken && CurrentDrug == DrugType.Polvo);

			if(Input.Pressed(InputButton.Attack2)) {
				var dropped = Inventory.DropActive();
				if(dropped != null) {
					ResetInterpolation();
					dropped.Position = EyePos;
					if(dropped.PhysicsGroup != null) {
						if(dropped is BaseSpeedDialWeapon wep) {
							wep.ApplyThrowVelocity(EyeRot.Forward);
							wep.DespawnAfterTime = true;
							wep.GlowState = GlowStates.GlowStateOn;
							wep.GlowDistanceStart = 0;
							wep.GlowDistanceEnd = 1000;
							if(wep.AmmoClip > 0)
								wep.GlowColor = new Color(0.2f, 1, 0.2f, 1);
							else {
								if(wep.AmmoClip == -1)
									wep.GlowColor = new Color(1, 1, 1, 1);
								else
									wep.GlowColor = new Color(1, 0.2f, 0.2f, 1);
							}
							wep.GlowActive = true;
							PlaySound("weaponspin");
						}
					}

					timeSinceDropped = 0;
				}
				if(IsClient && ActiveChild != null) {
					PlaySound("weaponspin");
				}
			}

			if(Input.Pressed(InputButton.Attack2) && pickup && pickUpEntity != null && Input.ActiveChild == null) {
				Inventory?.Add(pickUpEntity, Inventory.Active == null);
				(pickUpEntity as BaseSpeedDialWeapon).GlowState = GlowStates.GlowStateOff;
				(pickUpEntity as BaseSpeedDialWeapon).GlowActive = false;
				pickup = false;
				pickUpEntity = null;
			}

			if(IsClient) {
				AmmoPanel.Current.pickedup = 0f;
			}

			if(IsClient && pickup && Input.ActiveChild == null) {
				AmmoPanel.Current.pickedup = 1f;
			}

			SimulateActiveChild(cl, ActiveChild);
		}
	}
}
