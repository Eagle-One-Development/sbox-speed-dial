using Sandbox;

namespace SpeedDial {
	public class Team : EntityComponent {
		public virtual string Name => ClassInfo.Title;
		public virtual string Description => ClassInfo.Description;
		public virtual string Icon => ClassInfo.Icon;
		public virtual Color Color => Color.White;
	}
}
