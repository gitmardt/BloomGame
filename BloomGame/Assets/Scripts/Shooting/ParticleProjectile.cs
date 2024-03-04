using System.Collections.Generic;
using UnityEngine;

public class ParticleProjectile : MonoBehaviour
{
    public ParticleSystem particles;
    public Transform trailParent;
    public List<ParticleCollisionEvent> collisionEvents;
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public float speed = 75f;
    [HideInInspector] public float spread = 6f;
    private float randomSpread = 0f;

    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();

        var main = particles.main;
        main.startSpeed = speed;
        randomSpread = Random.Range(-spread, spread);
        transform.rotation = Quaternion.Euler(0, randomSpread , 0);
        //direction.y += randomSpread;
    }

    private void Update()
    {
        trailParent.position += direction.normalized * (speed * Time.deltaTime);
    }


    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(particles, other, collisionEvents);

        Debug.Log("Particle " + particles.name + " collided with " + other.name);

        for (int i = 0; i < collisionEvents.Count; i++)
        {

        }
    }
}
