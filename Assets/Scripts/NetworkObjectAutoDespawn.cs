using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class NetworkObjectAutoDespawn : MonoBehaviour
{
    [SerializeField] private float delayTime;
    private NetworkObject no;

    private void Awake()
    {
        no = GetComponent<NetworkObject>();
        Invoke(nameof(DespawnServerRpc), delayTime);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnServerRpc()
    {
        no.Despawn(true);
    }
}
