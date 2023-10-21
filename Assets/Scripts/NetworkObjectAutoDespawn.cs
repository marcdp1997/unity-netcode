using System.Collections;
using System.Collections.Generic;
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
        Invoke(nameof(Despawn), delayTime);
    }

    private void Despawn()
    {
        if (IsServer) DespawnServerRpc();
    }

    [ServerRpc]
    private void DespawnServerRpc()
    {
        no.Despawn(true);
    }
}
