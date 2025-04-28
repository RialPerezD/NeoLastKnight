using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Scroll : MonoBehaviour
{
    private RectTransform scoretomove;

    void Start()
    {
        scoretomove = gameObject.GetComponent<RectTransform>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(scoretomove.transform.position, new Vector3(scoretomove.transform.position.x, 6800, scoretomove.transform.position.z)) < 0.6f)
        {
            UnityEngine.Debug.Log("OUT");
            SceneManager.LoadScene(sceneBuildIndex: 0);
        }
        scoretomove.transform.position = Vector3.MoveTowards(scoretomove.transform.position, new Vector3(scoretomove.transform.position.x, 6800, scoretomove.transform.position.z), 200.0f * Time.deltaTime);
    }

   
}
