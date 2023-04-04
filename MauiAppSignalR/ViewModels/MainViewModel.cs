using MauiAppSignalR.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Realms;
using System.Net;

namespace MauiAppSignalR.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly HubConnection _connection;

    [ObservableProperty]
    public string winner = string.Empty;

    [ObservableProperty]
    public string player = string.Empty;

    [ObservableProperty]
    private bool isGameOver = false;

    [ObservableProperty]
    private bool playAgainButtonVisibility = false;

    [ObservableProperty]
    private bool isXTurn = true;

    [ObservableProperty]
    private string[,] board = new string[3, 3];

    public MainViewModel()
    {
        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        _connection = new HubConnectionBuilder()
            .WithUrl("http://10.0.0.19:5242/gamehub")
            .Build();

        _connection.On<string>("Logic", (Message) =>
        {
            MakeMove(Message);
        });

        Task.Run(async () =>
        {
            Application.Current.Dispatcher.Dispatch(async () =>
            await _connection.StartAsync());
        });
    }

    [RelayCommand]
    private async Task SendMove(string parameter)
    {
        System.Diagnostics.Debug.WriteLine("NOT Connected");
        if (_connection.State == HubConnectionState.Connected)
        {
            System.Diagnostics.Debug.WriteLine("Connected");
            await _connection.InvokeAsync("SendMove", parameter);
        }
    }

    [RelayCommand]
    private void GetPlayer()
    {
        var realm = Realm.GetInstance();

        var player = realm.All<Player>().FirstOrDefault();

        if (player != null)
        {
            Player = player.Name;
            Winner = $"Playing as {Player}";
        }
    }

    private void SaveScore()
    {
        var realm = Realm.GetInstance();

        var player = realm.All<Player>().FirstOrDefault();

        if (player != null)
        {
            if (Winner.Contains(player.Name))
            {
                realm.Write(() =>
                {
                    player.Wins++;
                });
            }
            else if (!Winner.Contains(player.Name))
            {
                realm.Write(() =>
                {
                    player.Losses++;
                });
            }

        }
    }

    [RelayCommand]
    private void MakeMove(string parameter)
    {
        System.Diagnostics.Debug.WriteLine("MakeMove called with these parameters: " + parameter);
        string[] indices = parameter.Split(',');
        int row = int.Parse(indices[0]);
        int col = int.Parse(indices[1]);

        if (Board[row, col] != null) // check if the cell is already occupied
            return;

        // Update the Board with the player's move
        Board[row, col] = IsXTurn ? "X" : "O";

        // Check for a winner
        bool hasWinner = false;
        string winningSymbol = null;

        // Check rows
        for (int i = 0; i < 3; i++)
        {
            if (Board[i, 0] == Board[i, 1] && Board[i, 1] == Board[i, 2] && Board[i, 0] != null)
            {
                hasWinner = true;
                winningSymbol = Board[i, 0];
                break;
            }
        }

        // Check columns
        if (!hasWinner)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Board[0, i] == Board[1, i] && Board[1, i] == Board[2, i] && Board[0, i] != null)
                {
                    hasWinner = true;
                    winningSymbol = Board[0, i];
                    break;
                }
            }
        }

        // Check diagonals
        if (!hasWinner)
        {
            if (Board[0, 0] == Board[1, 1] && Board[1, 1] == Board[2, 2] && Board[0, 0] != null)
            {
                hasWinner = true;
                winningSymbol = Board[0, 0];
            }
            else if (Board[0, 2] == Board[1, 1] && Board[1, 1] == Board[2, 0] && Board[0, 2] != null)
            {
                hasWinner = true;
                winningSymbol = Board[0, 2];
            }
        }

        if (hasWinner)
        {
            Winner = $"{Player}/{winningSymbol} wins!";
            SaveScore();
            IsGameOver = true;
            PlayAgainButtonVisibility = true;
            OnPropertyChanged(nameof(PlayAgainButtonVisibility));
        }
        else
        {
            // Check for a draw
            bool isDraw = true;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Board[i, j] == null)
                    {
                        isDraw = false;
                        break;
                    }
                }
                if (!isDraw)
                    break;
            }

            if (isDraw)
            {
                Winner = "Draw!";
                IsGameOver = true;
                PlayAgainButtonVisibility = true;
                OnPropertyChanged(nameof(PlayAgainButtonVisibility));
            }
        }

        // Raise PropertyChanged event for Board
        OnPropertyChanged(nameof(Board));

        // Raise PropertyChanged event for the specific cell that was updated
        string propertyName = $"Board[{row},{col}]";
        OnPropertyChanged(propertyName);

        // Toggle the turn to the other player
        IsXTurn = !IsXTurn;
    }

    [RelayCommand]
    private void PlayAgain()
    {
        // Reset the game
        Board = new string[3, 3];
        IsGameOver = false;
        IsXTurn = true;

        GetPlayer();

        // Hide the "Play again" button
        PlayAgainButtonVisibility = false;
        OnPropertyChanged(nameof(PlayAgainButtonVisibility));

        // Raise PropertyChanged event for Board to update UI
        OnPropertyChanged(nameof(Board));
    }

}
