using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils;
using Unity.Netcode;
using UnityEditor;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private Collider2D col;
    [SerializeField] private GameObject explosionPrefab;

    private Rigidbody2D rb;
    private NetworkObject no;
    private NetworkObject explosion;
    private bool collided;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        no = GetComponent<NetworkObject>();
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Wall")) 
            CheckCollision();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player")) 
            CheckCollision();
    }

    private void CheckCollision()
    {
        if (!IsServer && !collided) return;

        collided = true;
        CreateExplosionServerRpc(transform.position);
    }

    public void Shoot(Vector3 direction, float speed)
    {
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Utils.GetAngleFromVector(direction)));
    }

    [ServerRpc(Delivery = RpcDelivery.Unreliable)]
    private void CreateExplosionServerRpc(Vector3 position)
    {
        GameObject go = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion = go.GetComponent<NetworkObject>();
        explosion.Spawn(true);

        no.Despawn(true);
    }

    public Collider2D GetCollider() { return col; }
}
