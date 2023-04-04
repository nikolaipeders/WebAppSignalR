using MauiAppSignalR.Models;
using MauiAppSignalR.Services;
using Realms;

namespace MauiAppSignalR.ViewModels;

public partial class HistoryViewModel : BaseViewModel
{
    private Realm _realm;
    private Player _player;

    public HistoryViewModel()
    {
        _realm = Realm.GetInstance();
        _player = _realm.All<Player>().FirstOrDefault();
        Refresh();

        // Subscribe to the PlayerScoreUpdated event
        SaveScoreEventHandler.Instance.PlayerScoreUpdated += OnPlayerScoreUpdated;
    }

    // ...

    private void OnPlayerScoreUpdated(object sender, EventArgs e)
    {
        // Call the Refresh method to update the UI
        Refresh();
    }


    [ObservableProperty]
	public string refreshButtonText = "Refresh";

	[ObservableProperty]
	public string wins = string.Empty;

	[ObservableProperty]
	public string losses = string.Empty;

    [RelayCommand]
	private void Refresh()
	{
        var realm = Realm.GetInstance();

        var player = realm.All<Player>().FirstOrDefault();

        if (player != null)
        {
			Wins = $"Wins: {player.Wins}";
            Losses = $"Wins: {player.Losses}";
        }
    }
}
