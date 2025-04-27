using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class PlayerStats : MonoBehaviour, Combat
{
    BaseUi ui;

    public float maxHp = 10;
    public float hp = 10;

    public int weaponType = 0;
    public float coins = 0;
    public int damage = 1;

    /* Damage Effect */
    Renderer renderer;
    Material material;
    float tick;
    Material splashDamage;
    Canvas canvasplayer;
    bool hitted = false;
    bool Splash = false;
    public float maxSplashEffect = 2;



    void Start()
    {
        ui = GameObject.Find("UI").GetComponent<BaseUi>();
        renderer = GetComponent<Renderer>();
        material = renderer.material;
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
                break;
            case 1:
                coins += 5;
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
            // muerto
            return true;
        }

        return false;
    }
}
