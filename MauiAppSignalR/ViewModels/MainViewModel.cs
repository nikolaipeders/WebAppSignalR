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
            .WithUrl("https://10.0.0.19:5176/gamehub/")
            .Build();

        _connection.On<string>("Logic", (message) =>
        {
            MakeMove(message);
        });

        Task.Run(async () =>
        {
            await _connection.StartAsync();
        });
    }


    [RelayCommand]
    private void GetPlayer()
    {
        // Get the Realm instance
        var realm = Realm.GetInstance();

        // Find the player with the given name (if any)
        var player = realm.All<Player>().FirstOrDefault();

        // If a player was found, set the nameInput property to the player's name
        if (player != null)
        {
            Winner = $"Playing as {player.Name}";
        }
    }

    [RelayCommand]
    private async void SendMove(string parameter)
    {
        await _connection.InvokeAsync("SendMove", parameter);
    }


    [RelayCommand]
    private void MakeMove(string parameter)
    {
        string[] indices = parameter.Split(',');
        int row = int.Parse(indices[0]);
        int col = int.Parse(indices[1]);

        if (board[row, col] != null) // check if the cell is already occupied
            return;

        // Update the board with the player's move
        board[row, col] = IsXTurn ? "X" : "O";

        // Check for a winner
        bool hasWinner = false;
        string winningSymbol = null;

        // Check rows
        for (int i = 0; i < 3; i++)
        {
            if (board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2] && board[i, 0] != null)
            {
                hasWinner = true;
                winningSymbol = board[i, 0];
                break;
            }
        }

        // Check columns
        if (!hasWinner)
        {
            for (int i = 0; i < 3; i++)
            {
                if (board[0, i] == board[1, i] && board[1, i] == board[2, i] && board[0, i] != null)
                {
                    hasWinner = true;
                    winningSymbol = board[0, i];
                    break;
                }
            }
        }

        // Check diagonals
        if (!hasWinner)
        {
            if (board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2] && board[0, 0] != null)
            {
                hasWinner = true;
                winningSymbol = board[0, 0];
            }
            else if (board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0] && board[0, 2] != null)
            {
                hasWinner = true;
                winningSymbol = board[0, 2];
            }
        }

        if (hasWinner)
        {
            Winner = $"{winningSymbol} wins!";
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
                    if (board[i, j] == null)
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

        // Raise PropertyChanged event for board
        OnPropertyChanged(nameof(board));

        // Raise PropertyChanged event for the specific cell that was updated
        string propertyName = $"board[{row},{col}]";
        OnPropertyChanged(propertyName);

        // Toggle the turn to the other player
        IsXTurn = !IsXTurn;
    }

    [RelayCommand]
    private void PlayAgain()
    {
        // Reset the game
        board = new string[3, 3];
        IsGameOver = false;
        IsXTurn = true;

        GetPlayer();

        // Hide the "Play again" button
        PlayAgainButtonVisibility = false;
        OnPropertyChanged(nameof(PlayAgainButtonVisibility));

        // Raise PropertyChanged event for board to update UI
        OnPropertyChanged(nameof(board));
    }

}
