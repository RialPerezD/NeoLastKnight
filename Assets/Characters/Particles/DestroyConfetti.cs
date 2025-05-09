using UnityEngine;
using System.Collections;

public class DestroyConfetti : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Destroy()
    {
        Destroy(gameObject);
    }

}
