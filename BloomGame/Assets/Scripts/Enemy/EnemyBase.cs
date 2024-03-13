using SmoothShakePro;
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

    public AudioSource[] hit, death;

    public bool randomLayer = true;
    public string[] layers;
    public GameObject[] objectsToRandomlyLayer;
    public GameObject[] objectsToRandomlyLayerWithChildren;

    public SmoothShakeStarter enemyIsHitShake;

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

            if(objectsToRandomlyLayer.Length > 0)
            {
                foreach(GameObject obj in objectsToRandomlyLayer)
                {
                    obj.layer = LayerMask.NameToLayer(selectedLayer);
                }

                if(objectsToRandomlyLayerWithChildren.Length > 0)
                {
                    foreach (GameObject obj in objectsToRandomlyLayerWithChildren)
                    {
                        foreach (Transform child in obj.transform)
                        {
                            child.gameObject.layer = LayerMask.NameToLayer(selectedLayer);
                            SetLayerRecursively(child.gameObject, LayerMask.NameToLayer(selectedLayer));
                        }
                    }
                }
            }
            else
            {
                foreach (Transform child in gameObject.transform)
                {
                    child.gameObject.layer = LayerMask.NameToLayer(selectedLayer);
                    SetLayerRecursively(child.gameObject, LayerMask.NameToLayer(selectedLayer));
                }
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
        if (CombatManager.instance.navMeshIsGenerated)
        {
            if (activeAttack == null)
            {
                agent.isStopped = false;
                agent.destination = player.position;
            }
            else
            {
                agent.isStopped = true;
            }
        }

        if (Vector3.Distance(player.position, transform.position) <= attackDistance)
        {
            activeAttack ??= StartCoroutine(Attack());
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
            FeedbackManager.instance.hitShake.StartShake(FeedbackManager.instance.hitShakePlayer);
            Player.instance.health--;
            Debug.Log("Player health is " + Player.instance.health);
        }
    }

    public void Damage()
    {
        if (hit.Length > 0)
        {
            int randomIndex = Random.Range(0, hit.Length);
            Utility.RandomizePitch(hit, randomIndex, 0.8f, 1.2f);
            hit[randomIndex].Play();
        }
        if (enemyIsHitShake != null) enemyIsHitShake.StartShake();
        health--;
        if(health == 0) Die();
        FeedbackManager.instance.hitShake.StartShake(FeedbackManager.instance.hitShakeEnemy);
    }

    public void Die()
    {
        if (Random.value > 0.5f)
        {
            Utility.RandomizePitch(FeedbackManager.instance.enemyDeath1, 0.8f, 1.2f);
            FeedbackManager.instance.enemyDeath1.Play();
        }
        else
        {
            Utility.RandomizePitch(FeedbackManager.instance.enemyDeath2, 0.8f, 1.2f);
            FeedbackManager.instance.enemyDeath2.Play();
        }
        FeedbackManager.instance.hitShake.StartShake(FeedbackManager.instance.EnemyDieShake);
        //Debug.Log("I died");
        WaveManager.instance.enemies.Remove(gameObject);
        Destroy(gameObject);
    }
}
