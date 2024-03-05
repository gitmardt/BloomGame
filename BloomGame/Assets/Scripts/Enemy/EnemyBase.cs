using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour
{
    //References
    Transform player;

    //Variables
    public float health = 5;

    //Private variables
    private NavMeshAgent agent;

    // Start is called before the first frame update
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        player = ThirdPersonMovement.instance.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = player.position;
    }

    public void Damage()
    {
        health--;
        Debug.Log("Health is " + health);
        if(health == 0) Die();
    }

    public void Die()
    {
        Debug.Log("I died");
        Destroy(gameObject);
    }
}
