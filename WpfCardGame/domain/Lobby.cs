using Steamworks;

namespace WpfCardGame.domain;

public class Lobby
{
    public string Name { get; set; } = "[unnamed lobby]";
    public int Players { get; set; }
    public int MaxPlayers { get; set; }
    public CSteamID SteamId { get; }

    public Lobby(string name, int players, int maxPlayers, CSteamID steamId)
    {
        name = name;
        players = players;
        maxPlayers = maxPlayers;
        SteamId = steamId;
    }
}