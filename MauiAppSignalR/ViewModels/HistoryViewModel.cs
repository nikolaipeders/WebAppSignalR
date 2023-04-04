using MauiAppSignalR.Models;
using Realms;

namespace MauiAppSignalR.ViewModels;

public partial class HistoryViewModel : BaseViewModel
{
	public HistoryViewModel() 
	{
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
