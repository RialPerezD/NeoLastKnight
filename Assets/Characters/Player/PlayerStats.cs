using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerStats : MonoBehaviour, Combat
{
    BaseUi ui;

    public float hp = 10;

    public int weaponType = 0;

    // Estas variables se cargan y descargan, solo esto persiste
    public float coins = 0;

    public float costeHp;
    public float costeSword;
    public float costeBow;

    public float maxHp;

    public int damage;
    public int bowDamage;
    ////////////////////////////////////////////////////////////

    /* Damage Effect */
    Material material;
    float tick;
    Material splashDamage;
    Canvas canvasplayer;
    bool hitted = false;
    bool Splash = false;
    public float maxSplashEffect = 2;

    GameManager manager;

    public GameObject particulasMonedas;

    AudioSource audioSource;
    public List<AudioClip> listaSonidos;

    void Start()
    {
        ui = GameObject.Find("UI").GetComponent<BaseUi>();
        material = GetComponent<Renderer>().material;
        canvasplayer = GameObject.Find("UI").GetComponent<Canvas>();
        UnityEngine.Canvas[] childCanvases = canvasplayer.GetComponentsInChildren<Canvas>(true);
        foreach (Canvas canvas in childCanvases)
        {
            if (canvas.name == "SplashDamage")
            {
                UnityEngine.UI.Image rawImage = canvas.GetComponent<UnityEngine.UI.Image>();
                splashDamage = rawImage.material;
                break;
            }
        }

        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        hp = maxHp;

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(hp < 0.2f * maxHp)
        {
            tick += (1.2f) * Time.deltaTime;
            float flash = Mathf.Abs(Mathf.Sin(tick) * 0.8f);
            material.SetFloat("_Flash", flash);
        }
        if (hitted && !Splash)
        {
            splashDamage.SetFloat("_darkening", maxSplashEffect);
            Splash = true;
        }
        if (Splash)
        {
            float take = (splashDamage.GetFloat("_darkening") - 0.2f * 20.0f * Time.deltaTime);
            splashDamage.SetFloat("_darkening", take);
            if (take <= 0.0f)
            {
                Splash = false;
                hitted = false;
            } 
        }
    }

    public void Interactua(int objectType)
    {
        switch (objectType)
        {
            case 0:
                coins += 5;
                ui.CambiaMonedas(coins);
                SpawnParticles();
                break;
            case 1:
                manager.CargarPueblo();
                break;
            case 2:
                manager.CargarNivel();
                break;
            case 100:
                ui.ActivaTienda(0);
                break;
            case 101:
                ui.ActivaTienda(1);
                break;
            default:
                break;
        }
    }

    public bool RecibeDamage(int ammount)
    {
        hp -= ammount;
        hitted = true;
        ui.CambiaVida((hp / maxHp) * 100f);

        if (hp <= 0)
        {
            Time.timeScale = 0f;
            GameObject.Find("UI").transform.Find("Muerte").gameObject.SetActive(true);
            return true;
        }

        return false;
    }

    public void SpawnParticles()
    {
        audioSource.PlayOneShot(listaSonidos[0]);

        Vector3 pos = transform.position;
        pos.y += 0.5f;
        GameObject particles = Instantiate(particulasMonedas, pos, Quaternion.identity);

        if (particles != null)
        {
            Destroy(particles, 1); // Limpia el objeto
        }
        else
        {
            Debug.LogWarning("El prefab no tiene un ParticleSystem.");
        }
    }
}
