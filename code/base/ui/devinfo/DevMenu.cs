
namespace SpeedDial;
public partial class DevMenu : Panel
{

	[ConCmd.Server]
	public static void RunCommandServer( string command )
	{
		ConsoleSystem.Run( command );
	}

}
