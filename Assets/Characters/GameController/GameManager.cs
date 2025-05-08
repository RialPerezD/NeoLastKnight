using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static float grid_x_scale = 1.825f;
    public static float grid_y_scale = 1.905f;
    public static float animDuration = 0.2f;

    public int actualLevel;
    WorldController worldController;

    int siguienteNivel = 0;

    void Awake()
    {
        // Implementación Singleton básica
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // No destruir este objeto al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Destruir duplicados
            return;
        }
    }

    void Start()
    {
        UpdateReferences();

        actualLevel = 0;
        worldController.LoadLevel(actualLevel);
        worldController.UpdateaEsNivel(false, 6);
        actualLevel = 1;
    }

    void UpdateReferences()
    {
        GameObject worldControllerObj = GameObject.Find("WorldController");
        if (worldControllerObj != null)
        {
            worldController = worldControllerObj.GetComponent<WorldController>();
        }
    }

    public void LanzaBeat()
    {
        if (worldController == null) UpdateReferences();

        worldController?.GeneraMovimientosObjetos();
        worldController?.AplicaMovimientos();
        worldController?.DestruyeBasura();
        worldController?.SpawneaObjetos();
    }

    public void CargarPueblo()
    {
        actualLevel++;
        siguienteNivel = 1;
        SceneManager.LoadScene(sceneBuildIndex: 2);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void CargarNivel()
    {
        siguienteNivel = actualLevel;
        SceneManager.LoadScene(sceneBuildIndex: 3);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateReferences();
        worldController?.LoadLevel(siguienteNivel);
        if(siguienteNivel == 1) worldController.UpdateaEsNivel(false, 2);
        SceneManager.sceneLoaded -= OnSceneLoaded; // Desuscribirse para evitar múltiples llamadas
    }
}
