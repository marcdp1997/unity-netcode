using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Unity.Services.Core;

#if UNITY_EDITOR
using ParrelSync;
#endif

namespace Arrowfist.Managers
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] private bool autoUpdateLobbyList;

        private float heartbeatTimer;
        private float refreshLobbiesTimer;
        private float lobbyPollTimer;
        private Lobby joinedLobby;
        private string playerName;

        private const int MaxLobbiesToShow = 5;

        public static string KeyJoinCode { get { return "JoinCode"; } }
        public static string KeyPlayerName { get { return "PlayerName"; } }
        public static LobbyManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (autoUpdateLobbyList)
                HandleRefreshLobbyList();

            HandleLobbyPollForUpdates();

            // If a lobby does not receive data in 30 sec it became inactive, which means
            // new players can't find it. However, players that are already in the lobby
            // can get data. To keep it alive while players try to join, we send heartbeats:
            HandleHostLobbyHeartbeat();
        }

        public async void Authenticate(string playerName)
        {
            this.playerName = playerName;
            InitializationOptions options = new InitializationOptions();
            options.SetProfile(this.playerName);

            await UnityServices.InitializeAsync(options);

#if UNITY_EDITOR
            if (ClonesManager.IsClone())
            {
                // When using a ParrelSync clone, switch to a different authentication profile to force the clone
                // to sign in as a different anonymous user account.
                string customArgument = ClonesManager.GetArgument();
                AuthenticationService.Instance.SwitchProfile($"Clone_{customArgument}_Profile");
            }
#endif

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private async void HandleHostLobbyHeartbeat()
        {
            if (!IsLobbyHost()) return;

            if ((heartbeatTimer -= Time.deltaTime) <= 0)
            {
                heartbeatTimer = 15;
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }

        public async void CreateLobby(string name, int numPlayers, bool isPrivate)
        {
            try
            {
                CreateLobbyOptions options = new CreateLobbyOptions()
                {
                    IsPrivate = isPrivate,
                    Player = GetPlayer(),
                    Data = new Dictionary<string, DataObject>
                {
                    { KeyJoinCode, new DataObject(DataObject.VisibilityOptions.Member, "0") }
                }
                };

                joinedLobby = await LobbyService.Instance.CreateLobbyAsync(name, numPlayers, options);
                EventManager.Instance.Publish(new EventData(EventIds.OnLobbyCreated));
                lobbyPollTimer = 0;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void StartGame()
        {
            if (!IsLobbyHost()) return;

            // Join relay, get the code and update lobby info for clients to start game
            string joinCode = await RelayManager.Instance.CreateRelay();

            UpdateLobbyOptions options = new UpdateLobbyOptions()
            {
                Data = new Dictionary<string, DataObject>
            {
                { KeyJoinCode, new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
            }
            };

            joinedLobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, options);
            SceneLoaderManager.Instance.LoadSceneAsync(ProjectScenes.Game);
            EventManager.Instance.Publish(new EventData(EventIds.OnGameStarted));

            joinedLobby = null;
        }

        private async void HandleLobbyPollForUpdates()
        {
            if (joinedLobby == null) return;

            if ((lobbyPollTimer -= Time.deltaTime) <= 0)
            {
                lobbyPollTimer = 1.1f;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                if (IsPlayerInLobby())
                {
                    if (!IsLobbyHost() && joinedLobby.Data[KeyJoinCode].Value != "0")
                    {
                        // Game started for clients
                        RelayManager.Instance.JoinRelay(joinedLobby.Data[KeyJoinCode].Value);
                        SceneLoaderManager.Instance.LoadSceneAsync(ProjectScenes.Game);
                        EventManager.Instance.Publish(new EventData(EventIds.OnGameStarted));

                        joinedLobby = null;
                    }
                    else EventManager.Instance.Publish(new OnLobbyJoinedUpdatedEventData(joinedLobby));
                }
                else
                {
                    // Client has been kicked out
                    joinedLobby = null;
                    EventManager.Instance.Publish(new EventData(EventIds.OnPlayerKickedFromLobby));
                }
            }
        }

        private void HandleRefreshLobbyList()
        {
            if ((refreshLobbiesTimer -= Time.deltaTime) < 0)
            {
                refreshLobbiesTimer = 5f;
                RefreshLobbyList();
            }
        }

        public async void RefreshLobbyList()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized || !AuthenticationService.Instance.IsSignedIn ||
                        joinedLobby != null) return;

            try
            {
                QueryLobbiesOptions options = new QueryLobbiesOptions()
                {
                    Count = MaxLobbiesToShow,
                    Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                    Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.AvailableSlots),
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
                };

                QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(options);
                EventManager.Instance.Publish(new OnLobbyListRefreshedEventData(response.Results));
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void JoinLobbyById(string lobbyId)
        {
            try
            {
                JoinLobbyByIdOptions options = new JoinLobbyByIdOptions()
                {
                    Player = GetPlayer()
                };

                joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);
                EventManager.Instance.Publish(new EventData(EventIds.OnLobbyJoined));
                lobbyPollTimer = 0;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void LeaveLobby()
        {
            if (joinedLobby == null) return;

            try
            {
                await Lobbies.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                EventManager.Instance.Publish(new EventData(EventIds.OnLobbyLeft));
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public Player GetPlayer()
        {
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
            {
                { KeyPlayerName, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
            };
        }

        public async void KickPlayer(string playerId)
        {
            if (!IsLobbyHost()) return;

            try
            {
                await Lobbies.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private bool IsPlayerInLobby()
        {
            if (joinedLobby != null && joinedLobby.Players != null)
            {
                foreach (Player player in joinedLobby.Players)
                {
                    if (player.Id == AuthenticationService.Instance.PlayerId)
                        return true;
                }
            }

            return false;
        }

        public bool IsLobbyHost() { return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId; }

        public Lobby GetJoinedLobby() { return joinedLobby; }
    }
}

