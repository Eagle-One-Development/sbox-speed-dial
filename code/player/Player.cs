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

		[Net]
		public TimeSince TimeSinceDied { get; set; }

		[Net]
		public float RespawnTime { get; set; } = 1f;

		[Net]
		public int BodyGroup { get; set; }

		[Net]
		public bool pickup { get; set; }
		protected Entity pickUpEntity;

		[Net, Predicted]
		public TimeSince TimeSinceMedTaken { get; set; }

		[Net]
		public bool ResetTimeSinceMedTaken { get; set; }

		[Net]
		public bool MedTaken { get; set; }

		[Net]
		public float MedDuration { get; set; }

		[Net]
		public DrugType CurrentDrug { get; set; }

		public Particles DrugParticles { get; set; }
		[Net] public Character character { get; set; }
		[Net] public bool Freeze { get; set; } = false; // sorry for naming differences

		protected bool screenOpen = false;

		public SpeedDialPlayer() {
			Inventory = new SpeedDialInventory(this);
		}

		public virtual void InitialSpawn() {

			Controller = new SpeedDialController();
			Camera = new SpeedDialCamera();

			//Set a default character
			MedTaken = false;
			character = Character.All.ElementAtOrDefault(Rand.Int(0, Character.All.Count - 1));

			SpeedDialGame.Instance.Round?.OnPlayerSpawn(this);

			if(SpeedDialGame.Instance.Round is PreRound) {
				Freeze = true;
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

		public override void Respawn() {
			if(character == null) {
				SetModel("models/playermodels/character_fallback.vmdl");
			} else {
				SetModel(character.Model);
			}

			Freeze = false;
			Animator = new PlayerAnimator();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			LagCompensation = true;

			CauseOfDeath = COD.Gunshot;

			Host.AssertServer();

			Client.SetValue("killcombo", 0);
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
			BaseSpeedDialWeapon weapon = Library.Create<BaseSpeedDialWeapon>(character.WeaponClass);
			Inventory.Add(weapon, true);
			if(character == null) {
				SetModel("models/playermodels/character_fallback.vmdl");
			} else {
				SetModel(character.Model);
			}
		}

		public void Throw() {
			var dropped = Inventory.DropActive();
			if(dropped != null) {
				dropped.Position = EyePos;
				dropped.ResetInterpolation();
				if(dropped.PhysicsGroup != null && dropped is BaseSpeedDialWeapon wep) {
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
			if(IsClient) {
				PlaySound("weaponspin");
			}
		}

		public override void Simulate(Client cl) {

			if(SpeedDialGame.Instance.Round is PreRound) {
				screenOpen = true;
			}

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

			if(ResetTimeSinceMedTaken) {
				TimeSinceMedTaken = 0;
				ResetTimeSinceMedTaken = false;
			}

			if(Client.GetValue("killcombo", 0) > Client.GetValue("maxcombo", 0)) {
				cl.SetValue("maxcombo", Client.GetValue("killcombo", 0));
			}

			var controller = GetActiveController();
			controller?.Simulate(cl, this, GetActiveAnimator());

			if(Input.ActiveChild != null) {
				ActiveChild = Input.ActiveChild;
			}

			if(ActiveChild == null && Input.Pressed(InputButton.Attack1) && TimeSinceMeleeStarted >= 0.7f) {
				StartMelee();
			}

			if(ActiveMelee) {
				SimulateMelee();
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

			// TODO: refactor drug stuff and move this to the animator
			SetAnimBool("b_polvo", MedTaken && CurrentDrug == DrugType.Polvo);

			if(Input.Pressed(InputButton.Attack2) && ActiveChild != null && !Freeze) {
				Throw();
			}

			// TODO: hold input for pickup too?
			if(Input.Pressed(InputButton.Attack2) && pickup && pickUpEntity != null && Input.ActiveChild == null) {
				Inventory?.Add(pickUpEntity, Inventory.Active == null);
				(pickUpEntity as BaseSpeedDialWeapon).GlowState = GlowStates.GlowStateOff;
				(pickUpEntity as BaseSpeedDialWeapon).GlowActive = false;
				pickup = false;
				pickUpEntity = null;
			}

			if(IsClient) {
				GamePanel.Current.pickedup = 0f;
			}

			if(IsClient && pickup && Input.ActiveChild == null) {
				GamePanel.Current.pickedup = 1f;
			}

			SimulateActiveChild(cl, ActiveChild);
		}
	}
}
