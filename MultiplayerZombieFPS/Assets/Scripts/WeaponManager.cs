using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class WeaponManager : MonoBehaviour
    {
        public GameObject playerCam;
        public int range = 100;
        public int damage = 25;
        public Animator playerAnimator;
        public ParticleSystem muzzleFlash;
        public GameObject hitParticles;
        public AudioClip gunshot;
        public AudioSource audioSource;
        public WeaponSway weaponSway;
        float swaySensitivity;
        public GameObject crosshair;
        public float currentAmmo;
        public float maxAmmo;
        public float reserveAmmo;
        public float reloadTime = 2f;
        bool isReloading;
        public Text currentAmmoText;
        public Text reserveAmmoText;
        public GameObject nonTargetHitParticles;
        public float firerate = 10;
        float firerateTimer = 0;
        public bool isAutomatic;
        public string weaponType;
        public PlayerManager playerManager;
        public float ammoCap;




    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        swaySensitivity = weaponSway.swaySensitivity;

        currentAmmoText.text = currentAmmo.ToString();
        reserveAmmoText.text = reserveAmmo.ToString();

        ammoCap = reserveAmmo;
    }

    private void OnEnable()
    {
        playerAnimator.SetTrigger(weaponType);
        currentAmmoText.text = currentAmmo.ToString();
        reserveAmmoText.text = reserveAmmo.ToString();
    }

    private void OnDisable()
    {
        playerAnimator.SetBool("isReloading", false);
        isReloading = false;
        Debug.Log("Reload Interupted");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerAnimator.GetBool("isShooting"))
        {
            playerAnimator.SetBool("isShooting", false);
        }

        if (reserveAmmo <= 0 && currentAmmo <= 0)
        {
            Debug.Log("No ammo for this weapon left");
            return;
        }

        if (currentAmmo <= 0 && isReloading == false)
        {
            Debug.Log("No ammo in current clip");
            StartCoroutine(Reload(reloadTime));
            return;
        }

        if (isReloading == true)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && reserveAmmo > 0)
        {
            Debug.Log("Manual Reload");
            StartCoroutine(Reload(reloadTime));
            return;
        }

        if (firerateTimer > 0)
        {
            firerateTimer = firerateTimer - Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && firerateTimer <= 0 && isAutomatic)
        {
            Shoot();
            firerateTimer = 1 / firerate;
        }

        if (Input.GetButtonDown("Fire1") && firerateTimer <= 0 && !isAutomatic)
        {
            Shoot();
            firerateTimer = 1 / firerate;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Aim();
            
        }

        if (Input.GetButtonUp("Fire2"))
        {
            if (playerAnimator.GetBool("isAiming"))
            {
                playerAnimator.SetBool("isAiming", false);
            }
            weaponSway.swaySensitivity = swaySensitivity;
            crosshair.SetActive(true);          
        }

    }

    void Shoot()
    {
        currentAmmo--;
        currentAmmoText.text = currentAmmo.ToString();
        muzzleFlash.Play();
        audioSource.PlayOneShot(gunshot, 1f);
        playerAnimator.SetBool("isShooting", true);
        RaycastHit hit;
        if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range))
        {
            EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
            if (enemyManager != null)
            {
                enemyManager.Hit(damage);
                if (enemyManager.health <= 0)
                {
                    playerManager.currentPoints += enemyManager.points;
                    Debug.Log("Enemy Down!");
                }
                GameObject InstParticles = Instantiate(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));
                InstParticles.transform.parent = hit.transform;
                Destroy(InstParticles, 2f);
            }
            else
            {
                GameObject InstParticles = Instantiate(nonTargetHitParticles, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(InstParticles, 20f);
            }
        }
    }

    void Aim()
    {
        playerAnimator.SetBool("isAiming", true);
        weaponSway.swaySensitivity = swaySensitivity / 3;
        crosshair.SetActive(false);
    }

    public IEnumerator Reload(float rt)
    {
        isReloading = true;
        playerAnimator.SetBool("isReloading", true);
        yield return new WaitForSeconds(rt);
        playerAnimator.SetBool("isReloading", false);
        float missingAmmo = maxAmmo - currentAmmo;

        if (reserveAmmo >= missingAmmo)
        {
            currentAmmo += missingAmmo;
            reserveAmmo -= missingAmmo;
            currentAmmoText.text = currentAmmo.ToString();
            reserveAmmoText.text = reserveAmmo.ToString();
        }
        else
        {
            currentAmmo += reserveAmmo;
            reserveAmmo = 0;
            currentAmmoText.text = currentAmmo.ToString();
            reserveAmmoText.text = reserveAmmo.ToString();
        }
        isReloading = false;
    }
}
