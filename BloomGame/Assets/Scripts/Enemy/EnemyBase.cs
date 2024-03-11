using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour
{
    //References
    Transform player;
    public AnimationClip attackClip;
    public PlayableDirector attackTimeline;
    public string attackTrigger = "Attack";

    public float attackDistance = 5;
    public float attackDamage = 1;

    public Transform hitPosition;
    public float attackRange = 5;

    //Variables
    public float health = 5;

    private Coroutine activeAttack;

    //Private variables
    private NavMeshAgent agent;

    // Start is called before the first frame update
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        player = Player.instance.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(activeAttack == null)
        {
            agent.destination = player.position;
        }

        if (Vector3.Distance(player.position, transform.position) <= attackDistance)
        {
            if (activeAttack == null) activeAttack = StartCoroutine(Attack());
        }
    }

    public virtual IEnumerator Attack()
    {
        attackTimeline.Play();
        yield return new WaitForSeconds(attackClip.length);
        activeAttack = null;
    }

    public void DealDamage()
    {
        if(Vector3.Distance(player.position,hitPosition.position) < attackRange)
        {
            Player.instance.health--;
            Debug.Log("Player health is " + Player.instance.health);
        }
    }

    public void Damage()
    {
        health--;
        //Debug.Log("Health is " + health);
        if(health == 0) Die();
    }

    public void Die()
    {
        //Debug.Log("I died");
        Destroy(gameObject);
    }
}
