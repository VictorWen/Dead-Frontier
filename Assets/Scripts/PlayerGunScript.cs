using UnityEngine;

public class PlayerGunScript : MonoBehaviour
{
    public bool AimIsVisible { get; private set; }
    public float AimAngle { get { return angle; } set { angle = value; DrawAim(); } }
    public float AimRotation { get { return rotation; } set { rotation = value; DrawAim(); } }

    [Header("Gun Config")]
    [SerializeField] private int bullets;
    [SerializeField] private int maxBullets = 6;
    [SerializeField] private float bulletSpeed = 15;
    [SerializeField] private float aimSpeed = 3f;
    
    [SerializeField] private AudioClip gunShotSFX;
    [SerializeField] private AudioClip gunReloadSFX;

    [Header("Aim Rendering")]
    [SerializeField] private Projectile bulletPrefab;
    [SerializeField] private Projectile soulBulletPrefab;
    [SerializeField] private LineRenderer aimCone;
    [SerializeField] private LineRenderer aimLine;
    [SerializeField] private float length = 5;

    // Private fields
    private float angle = 0;
    private float rotation = 0;
    private bool isSoulBullet = false;

    private AudioSource audioSource;

    public void SetAimVisibility(bool visible)
    {
        AimIsVisible = visible;
        aimCone.gameObject.SetActive(visible);
        aimLine.gameObject.SetActive(visible);
    }

    // ===============
    //  UNITY METHODS
    // ===============

    private void Awake()
    {
        bullets = maxBullets;
    }

    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !AimIsVisible)
            TriggerAimGun(false);
        else if (Input.GetMouseButtonDown(1) && !AimIsVisible)
            TriggerAimGun(true);

        if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && AimIsVisible)
            UpdateAim();

        if
        (
            AimIsVisible &&
            (Input.GetMouseButtonUp(0) && !isSoulBullet) ||
            (Input.GetMouseButtonUp(1) && isSoulBullet)
        )
            TriggerShootGun();
    }

    // ================
    //  HELPER METHODS
    // ================

    private void TriggerAimGun(bool isSoulBullet)
    {
        if (bullets > 0)
            ActivateAimGun(isSoulBullet);
        else
            ActivateGunReload();
    }

    private void ActivateAimGun(bool isSoulBullet)
    {
        SetAimVisibility(true);
        AimAngle = 90 * Mathf.Deg2Rad;
        this.isSoulBullet = isSoulBullet;
    }

    private void ActivateGunReload()
    {
        bullets = maxBullets;
        audioSource.PlayOneShot(gunReloadSFX);
    }

    private void TriggerShootGun()
    {
        Shoot(bulletSpeed);
        SetAimVisibility(false);
        bullets--;
        audioSource.PlayOneShot(gunShotSFX);
    }

    private void Shoot(float speed)
    {
        // Calculate bullet direction towards mouse position
        Vector2 aim = RandomAim();

        Projectile.Data data = new Projectile.Data
        {
            canDamagePlayer = false,
            canDamageMobs = true
        };

        // Create bullet
        Projectile p;
        if (isSoulBullet)
            p = Instantiate(soulBulletPrefab);
        else
            p = Instantiate(bulletPrefab);
        p.transform.position = transform.position;
        p.Initialize(speed, aim, data);
    }

    private void UpdateAim()
    {
        if (AimAngle > 0)
            AimAngle = Mathf.Max(0, AimAngle - aimSpeed * Time.deltaTime);

        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        AimRotation = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
    }

    private void DrawAim()
    {
        float angle = this.angle / 2;
        float y = length * Mathf.Sin(angle);
        float x = length * Mathf.Cos(angle);

        // Rotate x and y
        float cos = Mathf.Cos(rotation);
        float sin = Mathf.Sin(rotation);

        float y0 = x * sin + y * cos;
        float x0 = x * cos - y * sin;

        float y1 = x * sin - y * cos;
        float x1 = x * cos + y * sin;

        aimCone.SetPosition(0, new Vector3(x0, y0, 0));
        aimCone.SetPosition(2, new Vector3(x1, y1, 0));

        aimLine.SetPosition(1, new Vector3(length * cos, length * sin, 0));
    }

    private Vector2 RandomAim()
    {
        float randAngle = Random.Range(0, angle);
        randAngle -= angle / 2;
        randAngle += rotation;

        return new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle));
    }
}
