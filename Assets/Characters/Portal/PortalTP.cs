using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTP : MonoBehaviour
{

    public int leveldestination;

    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject other_go = other.gameObject;
        UnityEngine.Debug.Log("IN");
        if (other_go.tag == "Player")
        {
            SceneManager.LoadScene(sceneBuildIndex: leveldestination);

           

        }
    }
}
