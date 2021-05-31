using System.Numerics;
using System.Diagnostics;
using System;
using Sandbox;
using SpeedDial.Weapons;
using SpeedDial.UI;


namespace SpeedDial.Player {
	public partial class SpeedDialPlayer : Sandbox.Player {

		[Net, Local]
		private TimeSince TimeSinceDied { get; set; } = 0;

		[Net, Local]
		public float RespawnTime { get; set; } = 1f;

		[Net]
		public Color32 PlayerColor { get; set; }

		[Net]
		public bool pickup { get; set; }
		private Entity pickUpEntity;

		TimeSince timeSinceDropped;

		public SpeedDialPlayer() {
			Inventory = new SpeedDialInventory(this);
		}

		public BaseSpeedDialCharacter character;

		public void InitialSpawn() {

			if(GetClientOwner().SteamId == 76561198000823482) { // bak
				PlayerColor = new Color32(250, 176, 3);
			} else if(GetClientOwner().SteamId == 76561198203314521) { // gurke
				PlayerColor = new Color32(70, 0, 70);
			} else if(GetClientOwner().SteamId == 76561198095231052) { // generic
				PlayerColor = new Color32(27, 49, 63);
			} else {
				PlayerColor = Color.Random;
			}

			//Set a default character
			character = SpeedDialGame.Instance.characters[0];

			Respawn();
		}

		public override void StartTouch(Entity other) {
			if(timeSinceDropped < 1) return;

			if(IsClient) return;

			if(other is PickupTrigger pt) {
				if(other.Parent is BaseSpeedDialWeapon wep1) {
					StartTouch(other.Parent);

					float magnitude = wep1.PhysicsBody.Velocity.Length;
					Log.Info($"Velocity: {magnitude}");
					if(magnitude > 450f) {
						wep1.PhysicsBody.EnableAutoSleeping = false;
						Sound.FromEntity("weaponhit", this);
						KillMyself(wep1.previousOwner);
						wep1.Velocity *= -0.5f;
					}
				}
				return;
			}
		}

		public override void Touch(Entity other) {

			if(timeSinceDropped < 1f) return;

			if(IsClient) return;

			if(other is PickupTrigger) {
				if(other.Parent is BaseSpeedDialWeapon) {
					Touch(other.Parent);
					pickup = true;
				}
				return;
			}
			pickUpEntity = other;
		}

		public override void EndTouch(Entity other) {
			base.EndTouch(other);
			if(other is PickupTrigger) {
				if(other.Parent is BaseSpeedDialWeapon) {
					Touch(other.Parent);
					pickUpEntity = null;
					pickup = false;
				}
				return;
			}
		}

		public override void Respawn() {
			SetModel("models/biped_standard/biped_standard.vmdl");

			RenderColor = PlayerColor;

			Camera = new SpeedDialCamera();
			Controller = new SpeedDialController();
			Animator = new PlayerAnimator();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Host.AssertServer();

			KillCombo = 0;

			BaseSpeedDialWeapon weapon = Library.Create<BaseSpeedDialWeapon>(character.Weapon);
			Inventory.Add(weapon, true);

			LifeState = LifeState.Alive;
			Health = 100;
			Velocity = Vector3.Zero;
			CreateHull();
			ResetInterpolation();
			SpeedDialGame.MoveToSpawn(this);
		}

		[ClientRpc]
		public void IncreaseWeaponClip() {
			if(ActiveChild is BaseSpeedDialWeapon weapon) {
				if(IsClient)
					Log.Info("Updated clip on client in rpc");
				if(IsServer)
					Log.Info("Updated clip on server in rpc");
				weapon.AwardAmmo();
			}
		}

		[ClientRpc]
		public void GiveLoadout() {

		}

		public override void Simulate(Client cl) {
			if(LifeState == LifeState.Dead) {
				if(TimeSinceDied > RespawnTime && IsServer) {

					Respawn();
				}
				return;
			}

			var controller = GetActiveController();
			controller?.Simulate(cl, this, GetActiveAnimator());

			if(Input.ActiveChild != null) {
				ActiveChild = Input.ActiveChild;
			}

			if(Input.Pressed(InputButton.Attack2)) {
				var dropped = Inventory.DropActive();
				if(dropped != null) {
					if(dropped.PhysicsGroup != null) {
						(dropped as BaseSpeedDialWeapon).ApplyThrowVelocity(EyeRot.Forward);
						PlaySound("weaponspin");
					}

					timeSinceDropped = 0;
				}
				if(IsClient && ActiveChild != null) {
					PlaySound("weaponspin");
				}
			}

			if(IsClient && pickUpEntity != null) {
				Log.Info(pickUpEntity.ToString());
			}
			if(Input.Pressed(InputButton.Attack2) && pickup && pickUpEntity != null && Input.ActiveChild == null) {
				Inventory?.Add(pickUpEntity, Inventory.Active == null);
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
