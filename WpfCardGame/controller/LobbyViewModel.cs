using System.Collections.ObjectModel;
using WpfCardGame.domain;

namespace WpfCardGame.controller;

public class LobbyViewModel
{
    public ObservableCollection<Lobby> Lobbies { get; set; }

    public LobbyViewModel()
    {
        Lobbies = new ObservableCollection<Lobby>();
    }

    public void UpdateLobbies(List<Lobby> newLobbies)
    {
        Lobbies.Clear();
        foreach (var lobby in newLobbies)
        {
            Lobbies.Add(lobby);
        }
    }
}