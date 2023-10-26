using Unity.Services.Lobbies.Models;

namespace Arrowfist.Managers
{
    public class OnLobbyJoinedUpdatedEventData : EventData
    {
        public readonly Lobby Lobby;

        public OnLobbyJoinedUpdatedEventData(Lobby lobby) : base(EventIds.OnLobbyJoinedUpdated)
        {
            Lobby = lobby;
        }
    }
}


