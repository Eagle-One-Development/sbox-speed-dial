using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using SpeedDial.Classic.Player;
using SpeedDial.Classic.Weapons;
using SpeedDial.Classic.Drugs;

namespace SpeedDial.Classic.Bot {
	public partial class ClassicBotBehaviour {
		public ClassicBot Bot { get; set; }

		public NavSteer Steer;
		public Sandbox.Debug.Draw Draw => Sandbox.Debug.Draw.Once;

		#region Randomized Variables
		private float turnSpeed = Rand.Float(10f, 25f); // lower is slower
		private float sinAimSpeed = Rand.Float(10.0f, 15.0f);
		private float accuracy = Rand.Float(0.0f, 10.0f); // lower is more accurate

		#endregion

		#region Inputs
		public bool Attack1 { get; set; }
		public bool Attack2 { get; set; }
		public Angles ViewAngles { get; set; }
		#endregion

		public Entity CurrentTarget { get; private set; }
		public Entity CurrentPlayer { get; private set; }
		public Vector3 InputVelocity { get; private set; }

		#region Static Variables
		public float UpdateInterval => 1.0f;
		public float SearchRadius => 400.0f;
		public float MinWanderRadius => 200;
		public float MaxWanderRadius => 500;
		public float PlayerOrbitDistance => 200;
		#endregion

		private TimeSince timeSinceUpdate;

		public virtual void Tick() {
			if(Bot.Client.Pawn.LifeState == LifeState.Dead) return;

			Debug.Sphere(Bot.Client.Pawn.Position, SearchRadius, Color.Magenta);

			SetInputs();

			// Reevaulate our target every interval
			if (timeSinceUpdate > UpdateInterval) {
				CurrentTarget = EvaulateTarget();
				timeSinceUpdate = 0f;
			}

			if(Steer != null) {
				if(CurrentTarget != null && CurrentTarget.IsValid) {
					Steer.Target = EvaulatePositon(CurrentTarget);
				} else if (Steer.Path.IsEmpty) {
					// Wander
					var t = NavMesh.GetPointWithinRadius(Bot.Client.Pawn.Position, MinWanderRadius, MaxWanderRadius);
					if(t.HasValue) Steer.Target = t.Value;
					else Steer.Target = Bot.Client.Pawn.Position;
				}

				Steer.Tick(Bot.Client.Pawn.Position);

				if(!Steer.Output.Finished) {
					InputVelocity = Steer.Output.Direction.Normal;
				}

				if(Debug.Enabled) {
					Steer.DebugDrawPath();
				}
			} else {
				Steer = new NavSteer();
			}
		}
		
		/// <summary>
		/// Decide inputs
		/// </summary>
		public virtual void SetInputs() {
			var pawn = Bot.Client.Pawn;
			var weapon = pawn.ActiveChild as ClassicBaseWeapon;

			Attack1 = CurrentTarget is ClassicPlayer;
			Attack2 = (weapon == null) || (weapon != null) && (weapon.AmmoClip <= 0) || CurrentTarget != null && CurrentTarget.IsValid && (CurrentTarget is ClassicBaseWeapon target && weapon.AmmoClip < weapon.ClipSize && Vector3.DistanceBetween(pawn.Position, target.Position) <= 10);

			var targetView = CurrentPlayer != null && CurrentPlayer.IsValid ? Rotation.LookAt((CurrentPlayer.Position - Bot.Client.Pawn.Position).Normal, Vector3.Up).Angles() :
					Rotation.LookAt(InputVelocity, Vector3.Up).Angles();
			targetView += new Angles(0, MathF.Sin(Time.Now * sinAimSpeed) * accuracy, 0);
			ViewAngles = Angles.Lerp(ViewAngles, targetView, Time.Delta * turnSpeed);
		}

		/// <summary>
		/// Decide where we want to go based on our target
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public virtual Vector3 EvaulatePositon (Entity target) {
			var pawn = Bot.Client.Pawn;

			// Don't go right up to the player if we have a gun
			if (target is ClassicPlayer player && pawn.ActiveChild != null) {
				return target.Position + (pawn.Position - target.Position).Normal * PlayerOrbitDistance;
			}

			return target.Position;
		}

		/// <summary>
		/// Choose what the bot should move to; the main decision making process. Override this for different gamemodes ande write your own logic
		/// </summary>
		/// <returns></returns>
		public virtual Entity EvaulateTarget() {
			Entity target = null;

			// get those entities
			var closestPlayer = GetClosestEntityInSphere<ClassicPlayer>(Bot.Client.Pawn.Position, SearchRadius, Bot.Client.Pawn);
			var closestWeapon = GetClosestEntityInSphere<ClassicBaseWeapon>(Bot.Client.Pawn.Position, SearchRadius);
			var closestDrug = GetClosestEntityInSphere<ClassicBaseDrug>(Bot.Client.Pawn.Position, SearchRadius);

			CurrentPlayer = closestPlayer;

			// random variables
			var pawn = Bot.Client.Pawn as ClassicPlayer;

			bool weapon = (pawn.ActiveChild as ClassicBaseWeapon) != null;
			int ammo = 0;
			int clip = 0;

			if(weapon) {
				ammo = (pawn.ActiveChild as ClassicBaseWeapon).AmmoClip;
				clip = (pawn.ActiveChild as ClassicBaseWeapon).ClipSize;
			}

			bool drug = pawn.ActiveDrug;

			float playerDist = closestPlayer != null ? Vector3.DistanceBetween(pawn.Position, closestPlayer.Position) : float.MaxValue;
			float weaponDist = closestWeapon != null ? Vector3.DistanceBetween(pawn.Position, closestWeapon.Position) : float.MaxValue;
			float drugDist = closestDrug != null ? Vector3.DistanceBetween(pawn.Position, closestDrug.Position) : float.MaxValue;

			// dumb logic
			// precedence is player/weapon/drug
			// choose player if weapon and drug or no weapon and closest or weapon and closer than drug
			if(closestPlayer != null && ((weapon && drug) || (!weapon && playerDist < weaponDist && playerDist < drugDist) || (weapon && (playerDist < drugDist)))) {
				target = closestPlayer;
			}
			// choose weapon if no weapon or no ammo or closest and not full on ammo and closest has more relative ammo
			else if(closestWeapon != null && (!weapon || ammo <= 0 || (ammo < clip && (ammo / clip < closestWeapon.AmmoClip / closestWeapon.ClipSize) && weaponDist < playerDist && weaponDist < drugDist))) {
				target = closestWeapon;
			}
			// choose drug if no drug
			else if(closestDrug != null && (!drug)) {
				target = closestDrug;
			} 
			// if target is null the bot will patrol/wander
			else {
				target = null;
			}

			return target;
		}

		/// <summary>
		/// Finds the nearest entity within a sphere radius
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="position">Position of the sphere</param>
		/// <param name="radius">Radius of the sphere</param>
		/// <param name="ignore">Entities to ignore in the search</param>
		public static T GetClosestEntityInSphere<T>(Vector3 position, float radius, params Entity[] ignore) where T : Entity {
			List<Entity> ents = Physics.GetEntitiesInSphere(position, radius).Where(x => x is T && !ignore.Contains(x)).ToList();
			Entity closestEnt = null;

			float smallestDist = 999999;
			foreach(var ent in ents) {
				var dist = Vector3.DistanceBetween(position, ent.Position);
				if(dist < smallestDist) {
					smallestDist = dist;
					closestEnt = ent;
				}
			}

			return closestEnt as T;
		}
	}
}
