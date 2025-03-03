using System.Diagnostics;
using Steamworks;

namespace WpfCardGame.network;

public class SteamLobby
{
    protected Callback<LobbyCreated_t> lobbyCreatedCallback;
    protected Callback<GameLobbyJoinRequested_t> joinRequestCallback;
    protected Callback<LobbyEnter_t> lobbyEnteredCallback;
    protected Callback<LobbyMatchList_t> lobbyMatchListCallback;

    public ulong CurrentLobbyID;
    private const int MaxPlayers = 2;
    private const string HostAddressKey = "HostAddress";
    private NetworkManager manager;


    public SteamLobby()
    {
        lobbyCreatedCallback = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyEnteredCallback = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void CreateLobby()
    {
        if (!SteamManager.Initialized)
        {
            Trace.WriteLine("[SteamLobby.cs] Steam API is not initialized.");
            return;
        }

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, MaxPlayers);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Trace.WriteLine("[SteamLobby.cs] Failed to create lobby.");
            return;
        }

        Trace.WriteLine("[SteamLobby.cs] Lobby created successfully!");
        SteamMatchmaking.SetLobbyData((CSteamID)callback.m_ulSteamIDLobby, "name", SteamFriends.GetPersonaName() + "'s Lobby");
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        Trace.WriteLine("[SteamLobby.cs] Joined lobby: " + callback.m_ulSteamIDLobby);
    }

    public void FindLobby()
    {
        if (!SteamManager.Initialized)
        {
            Trace.WriteLine("[SteamLobby.cs] Steam API is not initialized.");
            return;
        }

        lobbyMatchListCallback = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
        SteamMatchmaking.RequestLobbyList();
    }

    private void OnLobbyMatchList(LobbyMatchList_t callback)
    {
        Trace.WriteLine($"[SteamLobby.cs]Found {callback.m_nLobbiesMatching} lobbies.");

        if (callback.m_nLobbiesMatching > 0)
        {
            CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(0);
            SteamMatchmaking.JoinLobby(lobbyId);
        }
    }
}