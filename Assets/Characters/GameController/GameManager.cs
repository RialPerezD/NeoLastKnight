using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static float grid_x_scale = 1.825f;
    public static float grid_y_scale = 1.905f;
    public static float animDuration = 0.2f;

    public int actualLevel;
    public int siguienteNivel = 0;
    int parteTuto = 0;
    WorldController worldController;
    PlayerStats stats;
    BaseUi ui;


    // Persistencia de los stats del player
    float coins = 0;
    float costeHp = 5;
    float costeSword = 5;
    float costeBow = 5;
    float maxHp = 10;
    int damage = 1;
    int bowDamage = 1;

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
        if (worldController == null) worldController = GameObject.Find("WorldController").GetComponent<WorldController>();
        if(ui == null) ui = GameObject.Find("UI").GetComponent<BaseUi>();
    }

    public void LanzaBeat()
    {
        if (worldController == null) UpdateReferences();

        worldController?.GeneraMovimientosObjetos();
        worldController?.AplicaMovimientos();
        worldController?.DestruyeBasura();
        worldController?.SpawneaObjetos();

        EscupeInfo();
    }

    public void CargarPueblo()
    {
        Time.timeScale = 1f;

        CargaInfo(); 
        if(actualLevel < 3) actualLevel++;
        siguienteNivel = 1;
        SceneManager.LoadScene(sceneBuildIndex: 2);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void CargarNivel()
    {
        CargaInfo();
        siguienteNivel = actualLevel;
        SceneManager.LoadScene(sceneBuildIndex: 3);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void CargarTuto()
    {
        CargaInfo();
        siguienteNivel = 5 + parteTuto;
        SceneManager.LoadScene(sceneBuildIndex: 5 + parteTuto);
        parteTuto++;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateReferences();
        worldController?.LoadLevel(siguienteNivel);

        if (siguienteNivel == 1)
        {
            worldController.UpdateaEsNivel(false, 2);
        }
        else if(siguienteNivel >= 5)
        {
            worldController.UpdateaEsNivel(false, 6);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded; // Desuscribirse para evitar múltiples llamadas
    }


    void CargaInfo()
    {
        stats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();

        coins = stats.coins;
        costeHp = stats.costeHp;
        costeSword = stats.costeSword;
        costeBow = stats.costeBow;
        maxHp = stats.maxHp;
        damage = stats.damage;
        bowDamage = stats.bowDamage;
    }


    void EscupeInfo()
    {
        if(stats == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player)
            {
                stats = player.GetComponent<PlayerStats>();

                stats.coins = coins;
                stats.costeHp = costeHp;
                stats.costeSword = costeSword;
                stats.costeBow = costeBow;
                stats.maxHp = maxHp;
                stats.damage = damage;
                stats.bowDamage = bowDamage;
            }
        }
    }
}
