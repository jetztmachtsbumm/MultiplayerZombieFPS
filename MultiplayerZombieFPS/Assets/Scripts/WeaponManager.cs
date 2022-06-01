using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{

    [SerializeField] private int damage = 15;
    [SerializeField] private float range = 100;
    [SerializeField] private Animator anim;

    private void Update()
    {
        anim.SetBool("isShooting", false);

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }


    private void Shoot()
    {
        anim.SetBool("isShooting", true);

        if(Physics.Raycast(Camera.main.transform.position, transform.forward, out RaycastHit hit, range))
        {
            Debug.Log(hit.transform.gameObject.name);
            if (hit.transform.gameObject.CompareTag("Zombie"))
            {
                hit.transform.gameObject.GetComponent<EnemyManager>().TakeDamage(damage);
            }
        }
    }

}
