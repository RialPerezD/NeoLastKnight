using UnityEngine;
using UnityEngine.UI;

public class BeatController : MonoBehaviour
{

    bool debug = false;

    public float reset_position = -400f;          // Posicion inicial de la barra

    public float beatsPerMinute = 100;
    float beatsPerSecond;

    AudioSource audioSource;
    GameManager gameManager;

    float timer;
    double startingTime;
    float currentBeatPosition;
    bool beatTriggered;

    Image bolaIzquierdaBarra;
    Image bolaDerechaBarra;
    Image centroBarra;
    Material materialPelota;
    float flashvalue = 0;
    bool palpita = false;
    float contador = 0;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        bolaIzquierdaBarra = GameObject.FindWithTag("BSI").GetComponent<Image>();
        bolaDerechaBarra = GameObject.FindWithTag("BSD").GetComponent<Image>();
        centroBarra = GameObject.FindWithTag("BSM").GetComponent<Image>();
        materialPelota = bolaIzquierdaBarra.material;

        beatsPerSecond = 60.0f / beatsPerMinute;
        beatTriggered = false;

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        audioSource.Play();
        startingTime = AudioSettings.dspTime;
    }

    void Update()
    {
        CalculaBeat(); // Calcula en que parte del beat estamos
        ActualizaBarra();
    }


    void ActualizaBarra()
    {
        // Movemos las pelotas
        bolaIzquierdaBarra.rectTransform.anchoredPosition = new Vector2(currentBeatPosition, bolaIzquierdaBarra.rectTransform.anchoredPosition.y);
        bolaDerechaBarra.rectTransform.anchoredPosition = new Vector2(-1 * currentBeatPosition, bolaDerechaBarra.rectTransform.anchoredPosition.y);

        // Damos brillo si se acerca al medio
        materialPelota.SetFloat("_Flash", flashvalue);

        if (palpita)
        {
            float scale = 1f + Mathf.Sin(Time.time * Mathf.PI) * 0.2f; // bump suave
            centroBarra.rectTransform.localScale = new Vector3(scale, scale, 1f);
        }
    }


    void CalculaBeat()
    {
        // Calcula el porcentaje donde esta en cada beat
        timer = 1.0f - ((float)((AudioSettings.dspTime - startingTime - 0.3f) / beatsPerSecond) % 1.0f);

        // Posicion del beat
        currentBeatPosition = reset_position * timer;

        // El beat acaba de pasar, hacer lo que haya que hacer
        if (timer >= 0.95f && !beatTriggered)
        {
            palpita = false;
            flashvalue = 0.0f;

            gameManager.LanzaBeat();
            beatTriggered = true;
        }
        else if (timer < 0.5f)
        {
            palpita = true;
            flashvalue += 0.01f;
            if (flashvalue > 1.0f) flashvalue = 1.0f;

            beatTriggered = false;
        }
    }

    public bool PuedoMoverme()
    {
        if (timer < 0.2f)
        {
            if(debug)print("Bien "+timer);
            return true;
        }
        if (debug) print("Mal " + timer);
        return false;
    }
}
