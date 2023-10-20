using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils;
using Unity.Netcode;
using UnityEditor;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Collider2D col;
    [SerializeField] private GameObject explosionPrefab;

    private Rigidbody2D rb;
    private NetworkObject no;
    private NetworkObject explosion;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        no = GetComponent<NetworkObject>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Wall") ||
            col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            CreateExplosionServerRpc(transform.position);
        }
    }

    public void Shoot(Vector3 direction, float speed)
    {
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Utils.GetAngleFromVector(direction)));
    }

    [ServerRpc(Delivery = RpcDelivery.Unreliable, RequireOwnership = false)]
    private void CreateExplosionServerRpc(Vector3 position)
    {
        GameObject go = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion = go.GetComponent<NetworkObject>();
        explosion.Spawn(true);

        no.Despawn(true);
    }

    public Collider2D GetCollider() { return col; }
}
