using Sandbox;
using SpeedDial.Weapons;


namespace SpeedDial.Player {
	public partial class SpeedDialPlayer : Sandbox.Player {

		[Net, Local]
		private TimeSince TimeSinceDied { get; set; } = 0;

		[Net]
		public float RespawnTime { get; set; } = 1f;

		[Net]
		public Color32 PlayerColor { get; set; }

		public SpeedDialPlayer() {
			Inventory = new SpeedDialInventory(this);
		}

		[Net]
		public bool giveclip { get; set; }
		public BaseSpeedDialCharacter character;

		public void InitialSpawn() {

			if(GetClientOwner().SteamId == 76561198000823482) {
				PlayerColor = new Color32(250, 176, 3); // bak
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

			//Inventory.Add(new Pistol(), true);
			//string[] s = {character.Weapon};
			//Log.Info(s[0].ToString());
			//ConsoleSystem.Run("give_weapon",s);
			BaseSpeedDialWeapon weapon = Library.Create<BaseSpeedDialWeapon>(character.Weapon);
			Inventory.Add(weapon, true);

			//GiveAmmo(AmmoType.Pistol, 1000);

			LifeState = LifeState.Alive;
			Health = 100;
			Velocity = Vector3.Zero;
			CreateHull();
			ResetInterpolation();
			SpeedDialGame.MoveToSpawn(this);
		}

		[ClientRpc]
		public void BloodSplatter(Vector3 dir) {
			Vector3 pos = Position + Vector3.Up * 50f;

			// splatters around and behind the target, mostly from impact
			for(int i = 0; i < 15; i++) {

				// var forward = Owner.EyeRot.Forward;
				// forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
				// forward = forward.Normal;

				// TODO
				// proper distribution of blood behind and around the target
				var trSplatter = Trace.Ray(pos, pos + dir.Normal * 85f + Vector3.Random)
						.UseHitboxes()
						.Ignore(this)
						.Size(1)
						.Run();

				// FIXME
				// oops stupid path
				var decalPathSplatter = "materials/decals/blood/blood_splatter.decal";
				if(decalPathSplatter != null) {
					if(DecalDefinition.ByPath.TryGetValue(decalPathSplatter, out var decal)) {
						decal.PlaceUsingTrace(trSplatter);
					}
				}
			}

			//For blood splatter on the ground, pool of blood essentially

			// UPCOMING
			// Better and more decals for ground splatter
			var tr = Trace.Ray(pos, pos + Vector3.Down * 85f + Vector3.Random * 0.2f)
					.UseHitboxes()
					.Ignore(this)
					.Size(1)
					.Run();

			//DebugOverlay.Line(pos, tr.EndPos, Color.Red, 3f ,false);
			var decalPath = "decals/blood_test.decal";
			//var decalPath = Rand.FromArray(tr.Surface.ImpactEffects.BulletDecal);
			if(decalPath != null) {
				if(DecalDefinition.ByPath.TryGetValue(decalPath, out var decal)) {

					decal.PlaceUsingTrace(tr);
				}
			}

			// TODO
			// particles
			// (water particle dyed red? blood impact particle looks lame)

		}

		public override void OnKilled() {
			Game.Current?.OnKilled(this);

			//Create the combo score on the client
			if(LastDamage.Attacker is SpeedDialPlayer attacker && attacker != this) {
				//attacker.ComboEvents(EyePos,(SpeedDialGame.ScoreBase * attacker.KillCombo));
				BloodSplatter(Position - attacker.Position);
			}

			BecomeRagdollOnClient(new Vector3(Velocity.x / 2, Velocity.y / 2, 300), GetHitboxBone(0));

			Inventory.DeleteContents();

			TimeSinceDied = 0;
			LifeState = LifeState.Dead;

			Controller = null;

			EnableAllCollisions = false;
			EnableDrawing = false;
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

			if(Input.ActiveChild != null) {
				ActiveChild = Input.ActiveChild;
			}

			if(giveclip) {
				(ActiveChild as BaseSpeedDialWeapon).OnReloadFinish();
				giveclip = false;
			}

			SimulateActiveChild(cl, ActiveChild);

			var controller = GetActiveController();
			controller?.Simulate(cl, this, GetActiveAnimator());
		}

		DamageInfo LastDamage;

		public override void TakeDamage(DamageInfo info) {
			LastDamage = info;


			base.TakeDamage(info);

			if(info.Attacker is SpeedDialPlayer attacker && attacker != this) {
				// Note - sending this only to the attacker!
				attacker.DidDamage(To.Single(attacker), info.Position, info.Damage, Health);
				attacker.giveclip = true;

				TookDamage(To.Single(this), info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.Position);
			}
		}


		/// <summary>
		/// A client side function for client side effects when the player has done damage
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="amount"></param>
		/// <param name="healthinv"></param>
		[ClientRpc]
		public void DidDamage(Vector3 pos, float amount, float healthinv) {
			//Sound.FromScreen( "dm.ui_attacker" )
			//	.SetPitch( 1 + healthinv * 1 );
			//	
			//HitIndicator.Current?.OnHit( pos, amount );
			if(healthinv <= 0) {

				int ScoreBase = SpeedDialGame.ScoreBase;
				ComboEvents(pos, ScoreBase * KillCombo);


			}
		}


		[ClientRpc]
		public void TookDamage(Vector3 pos) {
			//DebugOverlay.Sphere( pos, 5.0f, Color.Red, false, 50.0f );

			//DamageIndicator.Current?.OnHit( pos );
		}
	}
}
