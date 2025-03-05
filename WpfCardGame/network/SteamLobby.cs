using System.Diagnostics;
using Steamworks;
using WpfCardGame.controller;
using WpfCardGame.domain;

namespace WpfCardGame.network;

public class SteamLobby
{
    protected Callback<LobbyCreated_t> LobbyCreatedCallback;
    protected Callback<GameLobbyJoinRequested_t> JoinRequestCallback;
    protected Callback<LobbyEnter_t> LobbyEnteredCallback;
    protected Callback<LobbyMatchList_t> LobbyMatchListCallback;

    public ulong CurrentLobbyId;
    private const int MaxPlayers = 2;
    private const string HostAddressKey = "HostAddress";
    private NetworkManager manager;

    public List<Lobby> Lobbies { get; } = [];


    public SteamLobby()
    {
        LobbyCreatedCallback = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        LobbyEnteredCallback = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
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

        LobbyMatchListCallback = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
        SteamMatchmaking.RequestLobbyList();
    }

    private void OnLobbyMatchList(LobbyMatchList_t callback)
    {
        Trace.WriteLine($"[SteamLobby.cs]Found {callback.m_nLobbiesMatching} lobbies.");

        Lobbies.Clear();
        for (int i = 0; i < callback.m_nLobbiesMatching; ++i)
        {
            CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
            SteamMatchmaking.GetLobbyMemberLimit(lobbyId);

            Lobby l = new Lobby("Some name" + i, 1, 
                SteamMatchmaking.GetLobbyMemberLimit(lobbyId), lobbyId);

            Lobbies.Add(l);
        }



        //if (callback.m_nLobbiesMatching > 0)
        //{
        //    CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(0);
        //    SteamMatchmaking.JoinLobby(lobbyId);
        //}
    }
}