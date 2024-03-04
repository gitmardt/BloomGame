using System.Collections.Generic;
using UnityEngine;

public class ParticleProjectile : MonoBehaviour
{
    public ParticleSystem particles;
    public List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }


    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(particles, other, collisionEvents);

        for (int i = 0; i < collisionEvents.Count; i++)
        {
            Debug.Log("Particle " + particles.name + " collided with " + other.name);
        }
    }
}
