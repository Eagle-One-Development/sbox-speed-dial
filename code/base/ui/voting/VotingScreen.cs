namespace SpeedDial;

[UseTemplate]
public partial class VotingScreen : Panel
{
	private Label Title { get; set; }
	private Label Description { get; set; }
	private Panel Items { get; set; }
	public Panel ProgressBar { get; set; }

	// used to cache the number of vote items
	private int _count;
	private TimeSince _updateDelta;
	private VoteEntity Vote => VoteEntity.Current;
	public override void Tick()
	{
		SetClass( "open", (Vote.IsValid() && !Vote.Concluded) || (Vote.IsValid() && Vote.Concluded && Vote.TimeSinceConcluded < 1.0f) );
		SetClass( "active", Vote.IsValid() && !Vote.Concluded );
		if ( !Vote.IsValid() )
		{
			return;
		}

		Title.Text = Vote.VoteTitle;
		Description.Text = Vote.VoteDescription;

		if ( _updateDelta > 0.1f )
		{
			_updateDelta = 0;
			// number of items has changed, re init the ui
			if ( _count != Vote.VoteItems.Count )
			{
				_count = Vote.VoteItems.Count;
				InitializeItems();
			}
		}

		if ( !Vote.Concluded )
		{
			var progress = 1 - Vote.TimeSinceStarted / Vote.VoteDuration;
			ProgressBar.Style.Width = Length.Percent( progress * 100 );
		}
		else
		{
			ProgressBar.Style.Width = Length.Percent( 0 );
		}
	}

	private void InitializeItems()
	{
		Items.DeleteChildren();

		for ( var i = 0; i < Vote.VoteItems.Count; i++ )
		{
			Items.AddChild( new VoteItemPanel( i ) );
		}
	}
}
