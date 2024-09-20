using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private int maxDistance;
    [SerializeField] private int fireForce;

    [Header("Reloading")]
    [SerializeField] private int currentAmmo;
    [SerializeField] private int maxAmmo;
    [SerializeField] private int fireRate;
    [SerializeField] private float reloadTime;
    private bool isReloading;

    [Header("Camera Shake")]
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeAmount;

    [Header("References")]
    [SerializeField] private Transform firePointTransform;
    [SerializeField] private AudioClip noAmmoAudio;
    [SerializeField] private ParticleSystem wallHitImpact;
    [SerializeField] private ParticleSystem enemyHitImpact;
    [SerializeField] private GameObject normalCrosshair;
    [SerializeField] private GameObject shotCrosshair;
    [SerializeField] private AudioClip hitmarkerAudio;

    public event Action<int> OnAmmoChanged;

    private Animator animator;
    private Camera cam;
    private AudioSource audioSource;

    private PlayerInputControls playerInputControls;
    private InputAction fireAction;
    private InputAction reloadAction;

    private float timeSinceLastShot = 0f;
    private int enemyLayer;
    private int wallLayer;

    private void Awake()
    {
        playerInputControls = new PlayerInputControls();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        cam = Camera.main;

        enemyLayer = LayerMask.NameToLayer("Enemy");
        wallLayer = LayerMask.NameToLayer("Wall");

        currentAmmo = maxAmmo;
        isReloading = false;
    }
    private void Start()
    {
        OnAmmoChanged?.Invoke(currentAmmo);
    }

    private void OnEnable()
    {
        fireAction = playerInputControls.Player.Fire;
        reloadAction = playerInputControls.Player.Reload;

        playerInputControls.Enable();
    }
    private void OnDisable()
    {
        playerInputControls.Disable();
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (fireAction.IsPressed())
        {
            Shoot();
        }
        if (reloadAction.triggered)
        {
            StartReloading();
        }
        
    }

    private void Shoot()
    {
        if (currentAmmo > 0 && CanShoot() && Time.timeScale == 1f)
        {
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f));
            RaycastHit hit;

            // Hit something
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                Debug.Log("Shot " + hit.collider.name);

                if(hit.collider.gameObject.layer == enemyLayer)
                {
                    Enemy enemy = hit.collider.gameObject.GetComponentInParent<Enemy>();

                    if (!enemy.GetIsDead())
                    {
                        StartCoroutine(ShowShotCrosshair());

                        int damage = UnityEngine.Random.Range(minDamage, maxDamage);
                        if (hit.collider.tag == "Head") damage *= 2;
                        enemy.TakeDamage(damage);
                    }

                    var hitImpact = Instantiate(enemyHitImpact, hit.point, Quaternion.LookRotation(hit.normal));

                    enemy.gameObject.GetComponent<RagdollEnabler>().EnableRagdoll();
                    hit.collider.GetComponent<Rigidbody>().AddForce(ray.direction * fireForce, ForceMode.Impulse);

                    if (!enemy.GetIsDead())
                    {
                        StartCoroutine(ReEnableAnimatorAfterDelay(enemy));
                    }
                }
                else if(hit.collider.gameObject.layer == wallLayer)
                {
                    var hitImpact = Instantiate(wallHitImpact, hit.point, Quaternion.LookRotation(hit.normal));
                }

                timeSinceLastShot = 0f;
            }
            // Hit nothing
            else
            {
                Debug.Log("Shot nothing");

                timeSinceLastShot = 0f;
            }

            Debug.DrawRay(cam.transform.position, cam.transform.forward * maxDistance, Color.red, 2f);
            currentAmmo--;
            OnAmmoChanged?.Invoke(currentAmmo);
            CameraShake.Instance.Shake(shakeDuration, shakeAmount);
            animator.SetTrigger("Fire");
        }
        else if(currentAmmo <= 0 && CanShoot() && Time.timeScale == 1f)
        {
            //Play no ammo sound
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            audioSource.clip = noAmmoAudio;
            audioSource.Play();

            timeSinceLastShot = 0f;
        }
    }
    private IEnumerator ShowShotCrosshair()
    {
        //normalCrosshair.SetActive(false);
        shotCrosshair.SetActive(true);

        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.clip = hitmarkerAudio;
        audioSource.Play();

        yield return new WaitForSeconds(0.2f);
        //normalCrosshair.SetActive(true);
        shotCrosshair.SetActive(false);
    }
    private IEnumerator ReEnableAnimatorAfterDelay(Enemy enemy)
    {
        yield return new WaitForSeconds(0.2f);

        enemy.gameObject.GetComponent<RagdollEnabler>().EnableAnimator();
    }
    private void StartReloading()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
            animator.SetTrigger("Reload");
        }
    }
    private IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        OnAmmoChanged?.Invoke(currentAmmo);
        isReloading = false;
    }
    private bool CanShoot() => !isReloading && timeSinceLastShot > 60f / fireRate; // timeSinceLastShot > 1f / (60f / fireRate);
    public int GetMaxAmmo() => maxAmmo;
}
