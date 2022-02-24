namespace SpeedDial;

public abstract class Team : EntityComponent {
	public virtual string Name { get; }
	public virtual string Description { get; }
	public virtual string Icon { get; }
	public virtual Color Color { get; }
}
