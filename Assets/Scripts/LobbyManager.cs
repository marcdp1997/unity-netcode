
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    private Lobby hostLobby;
    private float heartbeatTimer;
    private float updateLobbiesTimer;

    private const float UpdateLobbiesTime = 1;
    private const int LobbyMaxPlayers = 2;
    private const float HeartbeatTime = 15;
    private const string KeyJoinCode = "RelayCode";

    public static LobbyManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        if ((updateLobbiesTimer -= Time.deltaTime) < 0)
        {
            updateLobbiesTimer = UpdateLobbiesTime;
            CheckLobbies();
        }

        // If a lobby does not receive data in 30 sec it became inactive, which means
        // new players can't find it. However, players that are already in the lobby
        // can get data. To keep it alive while players try to join, we send heartbeats:
        HandleHostLobbyHeartbeat();
    }

    private async void HandleHostLobbyHeartbeat()
    {
        if (hostLobby == null) return;

        if ((heartbeatTimer -= Time.deltaTime) < 0)
        {
            heartbeatTimer = HeartbeatTime;
            await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
        }
    }

    public async void CreateLobby()
    {
        try
        {
            // Create relay and create join code
            string joinCode = await RelayManager.Instance.CreateRelay();

            // Create lobby
            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KeyJoinCode, new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                }
            };

            hostLobby = await LobbyService.Instance.CreateLobbyAsync(name, LobbyMaxPlayers, options);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void CheckLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            GameUIManager.Instance.ShowLobbies(queryResponse.Results);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobby(string id)
    {
        try
        {
            // Join lobby, get code and join relay
            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(id);
            RelayManager.Instance.JoinRelay(joinedLobby.Data[KeyJoinCode].Value);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
