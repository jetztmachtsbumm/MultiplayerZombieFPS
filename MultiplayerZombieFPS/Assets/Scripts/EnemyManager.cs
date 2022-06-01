using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{

    [SerializeField] private float damage = 20;
    [SerializeField] private int health = 100;

    private GameObject player;
    private NavMeshAgent agent;
    private Animator anim;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        agent.SetDestination(player.transform.position);
        anim.SetBool("isRunning", agent.velocity.magnitude > 1);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == player)
        {
            player.GetComponent<PlayerManager>().GetDamage(damage);
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if(health <= 0)
        {
            //Die
            Destroy(gameObject);
        }
    }

}
