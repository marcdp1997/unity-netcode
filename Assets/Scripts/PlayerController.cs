using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private Transform gunRoot;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private LineRenderer lineSight;
    [SerializeField] private List<GameObject> heartsFill;

    private int currHearts;
    private float shootCd;

    private void Awake()
    {
        currHearts = heartsFill.Count;
    }

    private void Update()
    {
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
        if (currHearts == 0) return;

        heartsFill[currHearts - 1].SetActive(false);
        currHearts--;

        if (currHearts == 0)
        {
            //lose
        }
    }

    private void AttemptToShoot()
    {
        if (Input.GetMouseButtonDown(0) && shootCd <= 0)
        {
            Shoot();
            shootCd = fireRate;
        }
    }

    private void Shoot()
    {
        Bullet bullet = Instantiate(bulletPrefab).GetComponent<Bullet>();
        bullet.transform.position = firePoint.position;
        bullet.Shoot(firePoint.right, bulletSpeed);
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
}
