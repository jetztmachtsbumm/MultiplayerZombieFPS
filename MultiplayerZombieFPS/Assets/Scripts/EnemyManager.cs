using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public GameObject player;
    public Animator enemyAnimator;
    public float damage = 20f;
    public float health = 100;
    public GameManager gameManager;
    public Slider slider;
    public bool playerInReach;
    private float attackDelayTimer;
    public float howMuchEarlierStartAttackAnim;
    public float delayBetweenAttacks;
    public AudioClip[] zombieSounds;
    public AudioSource audioSource;
    public int points = 20;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");

        slider.maxValue = health;
        slider.value = health;
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            Debug.Log("Is not playing");
            audioSource.clip = zombieSounds[Random.Range(0, zombieSounds.Length)];
            audioSource.Play();
        }

        slider.gameObject.transform.LookAt(player.transform.position);
        
        GetComponent<NavMeshAgent>().destination = player.transform.position;

        if (GetComponent<NavMeshAgent>().velocity.magnitude > 1)
        {
            enemyAnimator.SetBool("isRunning", true);
        }
        else
        {
            enemyAnimator.SetBool("isRunning", false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {       
            playerInReach = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (playerInReach)
        {
            attackDelayTimer += Time.deltaTime;
        }

        if (attackDelayTimer >= delayBetweenAttacks - howMuchEarlierStartAttackAnim && attackDelayTimer <= delayBetweenAttacks && playerInReach)
        {
            enemyAnimator.SetTrigger("isAttacking");
        }
        
        if (attackDelayTimer >= delayBetweenAttacks && playerInReach)
        {
            player.GetComponent<PlayerManager>().Hit(damage);
            attackDelayTimer = 0;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == player)
        {
            playerInReach = false;
            attackDelayTimer = 0;
        }
    }



    public void Hit(float damage)
    {
        health -= damage;
        slider.value = health;
        if (health <= 0)
        {
            enemyAnimator.SetTrigger("isDead");            
            Destroy(gameObject, 10f);        
            gameManager.enemiesAlive--;
            Destroy(GetComponent<NavMeshAgent>());
            Destroy(GetComponent<EnemyManager>());
            Destroy(GetComponent<CapsuleCollider>());
        }
    }

}
