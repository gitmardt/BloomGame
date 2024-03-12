using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour
{
    //References
    protected Transform player;
    public PlayableDirector attackTimeline;
    public float attackDistance = 5;
    public float attackDamage = 1;
    public bool destroyOnAttack = true;

    public bool randomLayer = true;
    public string[] layers;

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

        if (randomLayer)
        {
            string selectedLayer = layers[Random.Range(0, layers.Length)];

            //Debug.Log(selectedLayer + " " + LayerMask.NameToLayer(selectedLayer));

            gameObject.layer = LayerMask.NameToLayer(selectedLayer);

            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer(selectedLayer);
                SetLayerRecursively(child.gameObject, LayerMask.NameToLayer(selectedLayer));
            }
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer = newLayer;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public virtual float AttackDuration() => 5;

    // Update is called once per frame
    void Update()
    {
        if(activeAttack == null)
        {
            agent.isStopped = false;
            agent.destination = player.position;
        }
        else
        {
            agent.isStopped = true;
        }

        if (Vector3.Distance(player.position, transform.position) <= attackDistance)
        {
            if (activeAttack == null) activeAttack = StartCoroutine(Attack());
        }
    }

    public virtual IEnumerator Attack()
    {
        attackTimeline.Play();
        yield return new WaitForSeconds(AttackDuration());
        activeAttack = null;
        if (destroyOnAttack) Destroy(gameObject);
    }

    public virtual void DealDamage() 
    {
        if (Vector3.Distance(player.position, transform.position) < attackDistance)
        {
            Player.instance.health--;
            Debug.Log("Player health is " + Player.instance.health);
        }
    }

    public void Damage()
    {
        health--;
        if(health == 0) Die();
    }

    public void Die()
    {
        //Debug.Log("I died");
        WaveManager.instance.enemies.Remove(gameObject);
        Destroy(gameObject);
    }
}
