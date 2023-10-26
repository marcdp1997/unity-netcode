using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class NetworkObjectAutoDespawn : NetworkBehaviour
{
    [SerializeField] private float delayTime;
    private NetworkObject no;

    private void Awake()
    {
        no = GetComponent<NetworkObject>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer) Invoke(nameof(DespawnServerRpc), delayTime);
    }

    [ServerRpc]
    private void DespawnServerRpc()
    {
        no.Despawn(true);
    }
}
