using UnityEngine;
using UnityEngine.Audio;

public class BeatController : MonoBehaviour
{   

    public float reset_position = -400f;          // Posicion inicial de la barra

    public float beatsPerMinute = 100;
    float beatsPerSecond;

    AudioSource audioSource;
    GameManager gameManager;

    float timer;
    double startingTime;
    float currentBeatPosition;
    private bool beatTriggered;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>(); ;

        beatsPerSecond = 60.0f / beatsPerMinute;
        beatTriggered = false;

        audioSource = GetComponent<AudioSource>();

        audioSource.Play();
        startingTime = AudioSettings.dspTime;
    }

    void Update()
    {
        CalculateBeat(); // Calcula en que parte del beat estamos
    }

    void CalculateBeat()
    {
        // Calcula el porcentaje donde esta en cada beat
        timer = 1.0f - ((float)((AudioSettings.dspTime - startingTime - 0.3f) / beatsPerSecond) % 1.0f);

        // Posicion del beat
        currentBeatPosition = reset_position * timer;

        // El beat acaba de pasar, hacer lo que haya que hacer
        if (timer >= 0.95f && !beatTriggered)
        {
            gameManager.LanzaBeat();
            beatTriggered = true;
        }
        else if (timer < 0.5f)
        {
            beatTriggered = false;
        }
    }

    public bool PuedoMoverme()
    {
        if (timer < 0.2f)
        {
            return true;
        }
        return false;
    }
}
