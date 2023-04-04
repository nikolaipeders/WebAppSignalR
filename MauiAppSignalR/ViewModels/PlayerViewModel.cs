using MauiAppSignalR.Models;
using Realms;

namespace MauiAppSignalR.ViewModels;

public partial class PlayerViewModel : BaseViewModel
{

    [ObservableProperty]
    public string message = "Save";

    [ObservableProperty]
    public string nameInput = string.Empty;

    public PlayerViewModel()
    {
        var realm = Realm.GetInstance();

        var player = realm.All<Player>().FirstOrDefault();

        if (player != null)
        {
            nameInput = player.Name;
        }
    }

    [RelayCommand]
    private void Save()
    {
        var realm = Realm.GetInstance();

        var player = realm.All<Player>().FirstOrDefault();

        if (player != null)
        {
            player = new Player { Name = nameInput };
            realm.Write(() =>
            {
                realm.Add(player);
            });
        }

        realm.Write(() =>
        {
            player.Name = nameInput;
        });

        // Trying to hide the keyboard 
        Application.Current.MainPage.Focus();
    }

}

