using UnityEngine;

public class ParticlesDamage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Death()
    {
        Destroy(gameObject);
    }
}
