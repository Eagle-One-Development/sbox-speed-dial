using System;
using System.Linq;

using Sandbox;
using SpeedDial.Classic.Player;
using SpeedDial.Classic.UI;
using SpeedDial.Classic.Entities;

namespace SpeedDial.Classic.Drugs {
	public partial class ClassicBaseDrug : ModelEntity {
		public virtual string WorldModel { get; }
		public virtual string DrugName { get; }
		public virtual string DrugDescription { get; }
		public virtual DrugType DrugType { get; }
		public virtual string Icon { get; }
		public virtual string PickupSound { get; }
		public virtual string ParticleName { get; }
		public virtual Color HighlightColor => new(1, 1, 1, 1);
		public TimeSince TimeSinceSpawned { get; set; }
		public BasePickupTrigger PickupTrigger { get; protected set; }
		public ClassicDrugSpawn DrugSpawn { get; set; }

		public override void Spawn() {
			base.Spawn();

			CollisionGroup = CollisionGroup.Weapon; // so players touch it as a trigger but not as a solid
			SetInteractsAs(CollisionLayer.Debris); // so player movement doesn't walk into it

			SetModel(WorldModel);

			MoveType = MoveType.None;

			GlowState = GlowStates.GlowStateOn;
			GlowDistanceStart = 0;
			GlowDistanceEnd = 1000;
			GlowColor = HighlightColor;
			GlowActive = true;

			PickupTrigger = new();
			PickupTrigger.Position = Position;
			PickupTrigger.ParentEntity = this;
			PickupTrigger.ResetInterpolation();
			PickupTrigger.EnableTouchPersists = true;
			PickupTrigger.EnableTouch = true;
			PickupTrigger.EnableAllCollisions = true;
			PickupTrigger.SetTriggerSize(25);

			TimeSinceSpawned = 0;
		}

		[Event.Tick]
		public void Tick() {
			if(PickupTrigger is null) return;
			Debug.Sphere(Position, 5, Color.Green, 0.01f, false);
			Debug.Sphere(PickupTrigger.Position, 10, Color.Red, 0.01f, false);
			Debug.Line(Position, PickupTrigger.Position, Color.Yellow, 0.01f, false);
		}

		public void Taken(ClassicPlayer player) {
			if(DrugSpawn is not null) {
				DrugSpawn.DrugTaken();
				DrugSpawn = null;
			}

			player.ActiveDrug = true;
			player.DrugType = DrugType;
			player.TimeSinceDrugTaken = 0;

			var particle = Particles.Create(ParticleName);
			particle.SetForward(0, Vector3.Up);
			particle.SetEntityBone(0, player, player.GetBoneIndex("head"), Transform.Zero, true);
			player.DrugParticles = particle;

			BasePlayer.SoundFromScreen(To.Single(player.Client), PickupSound);

			Effect(player);

			// workaround since we don't actually parent the trigger right now
			PickupTrigger.EnableAllCollisions = false;
			PickupTrigger.Delete();

			Delete();
		}

		public virtual void Effect(ClassicPlayer player) {
			// call ui effects and shit
			ScreenHints.FireEvent(To.Single(player.Client), $"{DrugName}", $"{DrugDescription}", false);
		}

		[Event.Tick.Server]
		public void ServerTick() {
			Rotation = Rotation.RotateAroundAxis(Vector3.Up, Time.Delta * 20f);
			Position += Vector3.Up * MathF.Sin(Time.Now) * 0.15f;
		}

		[Event.Frame]
		public void Frame() {
			//SceneObject.Rotation = SceneObject.Rotation.RotateAroundAxis(Vector3.Up, Time.Delta * 20f);
			//SceneObject.Position += Vector3.Up * MathF.Sin(Time.Now) * 0.15f;
		}

		public static Type GetRandomSpawnableType() {
			// this shit is dumb
			// ideally I'd use LibraryAttribute's Spawnable for this but it doesn't work with run-time types like this so fuck it, interface it is
			var types = Library.GetAll<ClassicBaseDrug>().Where(x => x.GetInterfaces().Contains(typeof(ISpawnable)));
			return types.Random();
		}
	}

	public enum DrugType {
		Polvo,
		Leaf,
		Ollie,
		Ritindi
	}
}