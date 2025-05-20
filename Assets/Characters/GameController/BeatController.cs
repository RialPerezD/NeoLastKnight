using System.Collections.Generic;
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
    public List<AudioClip> listaMusica;

    float timer;
    double startingTime;
    float currentBeatPosition;
    bool beatTriggered;

    Image bolaIzquierdaBarra;
    Image bolaDerechaBarra;
    Image centroBarra;
    Image estrella;
    Material materialPelota;
    float flashvalue = 0;
    bool palpita = false;

    public float tiempoMovimiento = 0.4f;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        bolaIzquierdaBarra = GameObject.FindWithTag("BSI").GetComponent<Image>();
        bolaDerechaBarra = GameObject.FindWithTag("BSD").GetComponent<Image>();
        centroBarra = GameObject.FindWithTag("BSM").GetComponent<Image>();
        estrella = GameObject.FindWithTag("star").GetComponent<Image>();
        materialPelota = bolaIzquierdaBarra.material;

        beatsPerSecond = 60.0f / beatsPerMinute;
        beatTriggered = false;

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (gameManager.siguienteNivel == 1)
        {
            audioSource.clip = listaMusica[0];
            beatsPerSecond = 60.0f / 112f;
        }
        else
        {
            audioSource.clip = listaMusica[1];
            beatsPerSecond = 60.0f / 100f;
        }

        audioSource.loop = true;
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
            float t_normalizado = timer * 2f; // de min 0.0 -> max 0.5 a min 0 -> max 1
            float valor = 1f + (Mathf.Abs(t_normalizado - 0.5f) / 2);
            centroBarra.rectTransform.localScale = new Vector3(valor, valor, 1f);
        }
    }


    void CalculaBeat()
    {
        // Calcula el porcentaje donde esta en cada beat
        timer = 1.0f - ((float)((AudioSettings.dspTime - startingTime - 0.3f) / beatsPerSecond) % 1.0f);

        // Posicion del beat
        currentBeatPosition = reset_position * timer;

        Color color = estrella.color;
        if (timer >= 0.8f)
        {
            color.a = (timer - 0.8f) / 0.2f * 0.75f;
        } else {
            color.a = 0;
        }
        estrella.color = color;

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
        if (timer < tiempoMovimiento)
        {
            if (debug) print("Bien " + timer);
            return true;
        }
        if (debug) print("Mal " + timer);
        return false;
    }


}
