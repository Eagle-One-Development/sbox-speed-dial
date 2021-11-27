using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Sandbox;

namespace SpeedDial {
	public partial class ItemRespawn : Entity {

		/// <summary>
		/// A record of an entity and its position
		/// </summary>
		public class Record {
			public Transform Transform;
			public string ClassName;
			public float RespawnTime;
		}

		/// <summary>
		/// a list of entity records
		/// </summary>
		static Dictionary<Entity, Record> Records = new();

		/// <summary>
		/// Create a bunch of records from the existing entities. This should be called before
		/// any players are spawned, but right after the level is loaded.
		/// </summary>
		public static void Init() {
			Records = new();

			foreach(var entity in All.Where(x => x.GetType().GetInterfaces().Contains(typeof(IRespawnable)))) {
				AddRecordFromEntity(entity);
			}
		}

		/// <summary>
		/// Respawn this entity if it gets deleted (and Pickup is called before!)
		/// </summary>
		/// <param name="ent"></param>
		public static void AddRecordFromEntity(Entity ent) {
			if(ent is not IRespawnable re) return;
			var record = new Record {
				Transform = ent.Transform,
				ClassName = ent.ClassInfo.Name,
				RespawnTime = re.RespawnTime
			};

			Records[ent] = record;
		}

		/// <summary>
		/// Entity has been picked up, or deleted or something.
		/// If it was in our records, start a respawn timer
		/// </summary>
		public static void Pickup(Entity ent) {
			if(Records.Remove(ent, out var record)) {
				_ = RespawnAsync(record);
			}
		}

		static async Task RespawnAsync(Record record) {
			// TODO - Take.Delay In Game Time 
			await GameTask.DelaySeconds(record.RespawnTime);

			var ent = Library.Create<Entity>(record.ClassName);
			ent.Transform = record.Transform;

			Records[ent] = record;
		}
	}
}
