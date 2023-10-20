using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils;
using Unity.Netcode;
using UnityEngine.EventSystems;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private Transform gunRoot;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Collider2D hitbox;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private LineRenderer lineSight;
    [SerializeField] private List<GameObject> heartsFill;

    private NetworkVariable<ushort> currHearts;
    private float shootCd;

    private void Awake()
    {
        currHearts = new NetworkVariable<ushort>((ushort)heartsFill.Count, NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server);

        if (!IsServer) currHearts.OnValueChanged += UpdateHearts;
    }

    private void Update()
    {
        if (!IsOwner) return;

        UpdateTimers();
        AttemptToShoot();
        CheckMovement();
        AimGun();
        UpdateSight();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            LoseHeart();
        }
    }

    private void UpdateTimers()
    {
        shootCd -= Time.deltaTime;
    }

    private void CheckMovement()
    {
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized * speed * Time.deltaTime;
        transform.position += movement;
    }

    private void AimGun()
    {
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Utils.GetAngleFromVector(direction);
        gunRoot.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        gunRoot.localScale = new Vector3(1, angle > 90 && angle < 270 ? -1 : 1, 1);
    }

    public void LoseHeart()
    {
        if (currHearts.Value == 0) return;

        currHearts.Value -= 1;

        if (currHearts.Value == 0)
        {
            //lose
        }
    }

    private void UpdateHearts(ushort prevValue, ushort newValue)
    {
        heartsFill[prevValue - 1].SetActive(false);
    }

    private void AttemptToShoot()
    {
        if (Input.GetMouseButtonDown(0) && shootCd <= 0)
        {
            SpawnBulletServerRpc(firePoint.position, firePoint.right, bulletSpeed);
            shootCd = fireRate;
        }
    }

    private void UpdateSight()
    {
        const float LineLength = 1000;

        lineSight.SetPosition(0, firePoint.position);
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, LineLength, 1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Player"));

        Vector3 hitPos = hit ? hit.point : firePoint.right * LineLength;
        hitPos.z = 0;

        lineSight.SetPosition(1, hitPos);
    }

    // RPCs are reliable by default. This means they're guaranteed to be received and
    // executed on the remote side. However, sometimes developers might want to opt-out
    // reliability, which is often the case for non-critical events such as particle
    // effects, sounds effects etc.
    [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
    private void SpawnBulletServerRpc(Vector3 position, Vector3 direction, float speed)
    {
        Bullet bullet = Instantiate(bulletPrefab, position, Quaternion.identity).GetComponent<Bullet>();
        bullet.gameObject.GetComponent<NetworkObject>().Spawn(true);
        bullet.Shoot(direction, speed);
        Physics2D.IgnoreCollision(bullet.GetCollider(), hitbox);
    }
}
