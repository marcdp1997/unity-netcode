using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils;
using Unity.Netcode;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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
    [SerializeField] private List<Color> playerColors;

    private NetworkVariable<ushort> currHearts;
    private float shootCd;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    private bool usingGamepad;
    private Vector3 prevAimDirection;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        currHearts = new NetworkVariable<ushort>((ushort)heartsFill.Count, NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server);

        currHearts.OnValueChanged += UpdateHearts;

        playerInput = GetComponent<PlayerInput>();
        playerInput.onControlsChanged += ChangeControls;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Shoot.performed += Shoot;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = lineSight.startColor = lineSight.endColor = playerColors[(int)OwnerClientId];
    }

    private void Update()
    {
        if (!IsOwner) return;

        UpdateTimers();
        Move();
        Aim();
        UpdateSight();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Not only the owner has to check if a bullet collides with himself.
        // Only server can write to currHearts variable. When server changes its value,
        // everyone will call OnValueChanged and update the hearts.
        if (!IsServer) return;

        if (col.gameObject.layer == LayerMask.NameToLayer("Bullet"))
            LoseHeart();
    }

    private void UpdateTimers()
    {
        shootCd -= Time.deltaTime;
    }

    private void Move()
    {
        Vector3 movement = playerInputActions.Player.Movement.ReadValue<Vector2>();
        movement = speed * Time.deltaTime * movement.normalized;
        transform.position += movement;
    }

    private void Aim()
    {
        Vector3 direction;

        if (!usingGamepad)
        {
            direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        }
        else
        {
            direction = playerInputActions.Player.Aim.ReadValue<Vector2>();
            if (direction == Vector3.zero) direction = prevAimDirection;
        }

        float angle = Utils.GetAngleFromVector(direction);
        gunRoot.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        gunRoot.localScale = new Vector3(1, angle > 90 && angle < 270 ? -1 : 1, 1);

        prevAimDirection = direction;
    }

    public void LoseHeart()
    {
        if (currHearts.Value == 0) return;

        currHearts.Value -= 1;

        if (currHearts.Value == 0)
        {
            PlayerDeadServerRpc();
        }
    }

    private void UpdateHearts(ushort prevValue, ushort newValue)
    {
        heartsFill[prevValue - 1].SetActive(false);
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed && shootCd <= 0)
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

    private void ChangeControls(PlayerInput pi)
    {
        usingGamepad = pi.currentControlScheme == "Gamepad";

        if (usingGamepad) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.Confined;

        Cursor.visible = !usingGamepad;
    }

    // RPCs are reliable by default. This means they're guaranteed to be received and
    // executed on the remote side. However, sometimes developers might want to opt-out
    // reliability, which is often the case for non-critical events such as particle
    // effects, sounds effects etc.
    [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
    private void SpawnBulletServerRpc(Vector3 position, Vector3 direction, float speed)
    {
        if (!IsServer) return;

        Bullet bullet = Instantiate(bulletPrefab, position, Quaternion.identity).GetComponent<Bullet>();
        bullet.gameObject.GetComponent<NetworkObject>().Spawn(true);
        bullet.Shoot(direction, speed);
        Physics2D.IgnoreCollision(bullet.GetCollider(), hitbox);
    }

    [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
    private void PlayerDeadServerRpc()
    {
        // Only server check the win or lose condition and sends to all clients.
        // If the one who send this Rpc is the owner (aka your player) that means
        // you died. If not, the other player in the server died, so you won.
        CheckWinOrLose();
        PlayerDeadClientRpc();
    }

    [ClientRpc(Delivery = RpcDelivery.Reliable)]
    private void PlayerDeadClientRpc()
    {
        CheckWinOrLose();
    }

    private void CheckWinOrLose()
    {
        if (!IsOwner) GameUIManager.Instance.EnableWinScreen();
        else GameUIManager.Instance.EnableLoseScreen();

        playerInputActions.Player.Disable();
    }
}
