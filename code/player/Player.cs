using System.Numerics;

using System;
using Sandbox;
using SpeedDial.Weapons;
using SpeedDial.UI;
using SpeedDial.Meds;
using System.Threading.Tasks;

namespace SpeedDial.Player {
	public partial class SpeedDialPlayer : Sandbox.Player {

		[Net, Local]
		public TimeSince TimeSinceDied { get; set; } = 0;

		[Net, Local]
		public float RespawnTime { get; set; } = 1f;

		[Net]
		public Color32 PlayerColor { get; set; }

		[Net]
		public int BodyGroup { get; set; }

		[Net]
		public bool pickup { get; set; }
		private Entity pickUpEntity;

		TimeSince timeSinceDropped;

		[Net, Predicted]
		public TimeSince timeSinceMelee { get; set; }

		[Net, Predicted]
		public TimeSince timeSinceMedTaken { get; set; }

		[Net]
		public bool medTaken { get; set; }

		[Net]
		public float medDuration { get; set; }
		
		[Net]
		public DrugType currentDrug { get; set; }

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
			} else if(GetClientOwner().SteamId == 76561198257053769) { // whimsy
				PlayerColor = Color.Cyan;
			} else {
				PlayerColor = Color.Random;
			}

			BodyGroup = Rand.Int(0, 5);

			Controller = new SpeedDialController();
			Camera = new SpeedDialCamera();

			//Set a default character
			character = SpeedDialGame.Instance.characters[0];

			Respawn();
		}

		public override void Respawn() {
			SetModel("models/playermodels/playermodel_base.vmdl");

			SetPlayerBodyGroup(1, BodyGroup);

			RenderColor = PlayerColor;

			(Camera as SpeedDialCamera).Freeze = false;
			(Controller as SpeedDialController).Freeze = false;
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
		public void SetPlayerBodyGroup(int group, int value) {
			SetBodyGroup(group, value);
		}

		[ClientRpc]
		public void IncreaseWeaponClip()
		{
			if ( ActiveChild is BaseSpeedDialWeapon weapon )
			{
				if ( IsClient )
					Log.Info( "Updated clip on client in rpc" );
				if ( IsServer )
					Log.Info( "Updated clip on server in rpc" );
				weapon.AwardAmmo();
			}
		}

		[ClientRpc]
		public void DrugBump(string s )
		{
			AmmoPanel.Current.DrugBump(s);
		}

		public void Freeze() {
			(Controller as SpeedDialController).Freeze = true;
			(Camera as SpeedDialCamera).Freeze = true;
		}

		public void Unfreeze() {
			(Controller as SpeedDialController).Freeze = false;
			(Camera as SpeedDialCamera).Freeze = false;
		}

		public void Freeze(bool freeze) {
			(Controller as SpeedDialController).Freeze = freeze;
			(Camera as SpeedDialCamera).Freeze = freeze;
		}

		/// <summary>
		/// Handles Punching
		/// </summary>
		async Task HandleMelee() {
				if(Input.Pressed(InputButton.Attack1)) {
					if(timeSinceMelee > 0.33f) {
						await Task.DelaySeconds(0.1f);
						timeSinceMelee = 0;
						var forward = EyeRot.Forward;
						Vector3 pos = EyePos + Vector3.Down * 20f;
						var tr = Trace.Ray(pos, pos + forward * 40f)
						.UseHitboxes()
						.Ignore(this)
						.Size(20f)
						.Run();

						if ( IsClient )
						{

						PlaySwoosh();

						}
						SetAnimBool("b_attack", true);

						//DebugOverlay.Line(EyePos + Vector3.Down * 20, tr.EndPos, Color.White, 1, false);

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
							PlayClientSound("punch_connect_1");
							PlaySound("punch_connect_1");
							await Task.DelaySeconds(0.2f);
							if(!(LifeState == LifeState.Alive)) return;
							tr.Entity.TakeDamage(damage);
						}
					}
				}

		}

		[ClientRpc]
		public void PlaySwoosh() {
			float f = Rand.Float(1);
			if(f > 0.5f) {
				PlaySound("punch_woosh_1");

			} else {
				PlaySound("punch_woosh_2");
			}
			
		}


		public void PlayClientSound(string s) {
			PlaySound(s);
		}

		public override void Simulate(Client cl) {
			if(LifeState == LifeState.Dead) {
				if(TimeSinceDied > RespawnTime && IsServer) {

					Respawn();
				}
				return;
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

			

			if(timeSinceMedTaken > medDuration )
			{
				medTaken = false;
				//Basically remove our extra health after the drug duration if we're high on leaf
				if(currentDrug == DrugType.Leaf )
				{
					if(Health > 100 )
					{
						Health = 100;
					}
				}
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

		public override void StartTouch(Entity other) {
			if(timeSinceDropped < 1) return;

			if(IsClient) return;

			if(other is PickupTrigger pt) {
				if(other.Parent is BaseSpeedDialWeapon wep1) {
					StartTouch(other.Parent);

					float magnitude = wep1.PhysicsBody.Velocity.Length;
					//Log.Info($"Velocity: {magnitude}");
					if(magnitude > 450f) {
						wep1.PhysicsBody.EnableAutoSleeping = false;
						Sound.FromEntity("weaponhit", this);
						KillMyself(wep1.previousOwner);
						wep1.Velocity *= -0.5f;

					}
				}


				if(other.Parent is BaseMedication drug && !medTaken) {
					StartTouch(other.Parent);
					drug.PickUp();
					medTaken = true;
					if ( drug.drug != DrugType.Leaf )
					{
						
						
					}
					else
					{	
						//Since Leaf lets you take an extra hit we don't need to do any kind of effect over time so we can just set the health to 200
						Health = 200f;
						
					}
					currentDrug = drug.drug;
					timeSinceMedTaken = 0;
					medTaken = true;
					medDuration = drug.drugDuration;
					DrugBump( drug.drugName );

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
	}
}
