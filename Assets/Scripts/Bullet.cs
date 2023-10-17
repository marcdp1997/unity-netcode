using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            CreateExplosion();
        }
    }

    public void Shoot(Vector3 direction, float speed)
    {
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Utils.GetAngleFromVector(direction)));
    }

    private void CreateExplosion()
    {
        GameObject explosion = Instantiate(explosionPrefab);
        explosion.transform.position = transform.position;
        Destroy(gameObject);
        Destroy(explosion, 1f);
    }
}
