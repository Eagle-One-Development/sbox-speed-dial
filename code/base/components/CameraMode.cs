namespace SpeedDial;

public class CameraMode : EntityComponent
{
	public Vector3 Position
	{
		get
		{
			return Camera.Position;
		}
		set
		{
			Camera.Position = value;
		}
	}
	public Rotation Rotation
	{
		get
		{
			return Camera.Rotation;
		}
		set
		{
			Camera.Rotation = value;
		}
	}

	public float FieldOfView
	{
		get
		{
			return Camera.FieldOfView;
		}
		set
		{
			Camera.FieldOfView = value;
		}
	}

	public Entity Viewer
	{
		get
		{
			return Camera.FirstPersonViewer as Entity;
		}
		set
		{
			Camera.FirstPersonViewer = value;
		}
	}
	public virtual void Deactivated() { }
	public virtual void Activated() { }

	public virtual void Build()
	{
		Update();
	}

	public virtual void BuildInput() { }

	public virtual void Update()
	{

	}
}
