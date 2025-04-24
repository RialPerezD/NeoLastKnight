using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float tiempoDeVida = 0.25f; // segundos

    void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }
}
