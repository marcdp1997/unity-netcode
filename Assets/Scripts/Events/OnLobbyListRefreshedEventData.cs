using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Arrowfist.Managers
{
    public class OnLobbyListRefreshedEventData : EventData
    {
        public readonly List<Lobby> Lobbies;

        public OnLobbyListRefreshedEventData(List<Lobby> lobbies) : base(EventIds.OnLobbyListUpdated)
        {
            Lobbies = lobbies;
        }
    }
}


