using UnityEngine;


public class GameManager : MonoBehaviour
{

    public static float grid_x_scale = 1.825f; // Escala en x de la cuadricula
    public static float grid_y_scale = 1.905f; // Escala en y de la cuadricula

    public int actualLevel;
    WorldController worldController;


    void Start()
    {
        UpdateReferences();

        actualLevel = 1;
        worldController.LoadLevel(actualLevel);
    }


    void UpdateReferences()
    {
        if(worldController == null) worldController = GameObject.Find("WorldController").GetComponent<WorldController>();

    }

    public void LanzaBeat()
    {
        worldController.GeneraMovimientosObjetos();
        worldController.AplicaMovimientos();
        worldController.DestruyeBasura();
    }
}
