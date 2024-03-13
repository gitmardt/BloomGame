using System.Collections.Generic;
using UnityEngine;

public class ParticleProjectile : MonoBehaviour
{
    public AudioSource shotSound;
    public Vector2 shotAudioPitchRange;
    public ParticleSystem particles;
    public Transform trailParent;
    public string tagName = "Enemy";

    public List<ParticleCollisionEvent> collisionEvents;
    [HideInInspector] public float speed = 75f;
    [HideInInspector] public float spread = 6f;
    private float randomSpread = 0f;

    bool collided = false;

    void Start()
    {
        Utility.RandomizePitch(shotSound, shotAudioPitchRange.x, shotAudioPitchRange.y);

        collided = false;

        collisionEvents = new List<ParticleCollisionEvent>();

        var main = particles.main;
        main.startSpeed = speed;
        randomSpread = Random.Range(-spread, spread);
        transform.eulerAngles += new Vector3(0, randomSpread , 0);
    }

    private void Update()
    {
        if(!collided) trailParent.position += transform.forward * (speed * Time.deltaTime);
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(particles, other, collisionEvents);

        if (other.CompareTag(tagName))
        {
            //Debug.Log("Test");
            if (other.GetComponent<EnemyBase>() != null)
            {
                EnemyBase enemy = other.GetComponent<EnemyBase>();
                enemy.Damage();
            }
        }

       collided = true;
    }
}
