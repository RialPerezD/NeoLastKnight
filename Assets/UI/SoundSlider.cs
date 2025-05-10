using UnityEngine;
using UnityEngine.UI;
public class SetVolumeSlide : MonoBehaviour
{
    private BeatController live_manager;
    private AudioSource audio_source;
    private Slider soundslider;
    private float lastvol;

    void Start()
    {
        live_manager = FindFirstObjectByType<BeatController>();
        soundslider = gameObject.GetComponent<Slider>();
        audio_source = GameObject.Find("BeatController").GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!soundslider)
        {
            soundslider = GameObject.FindWithTag("Sound").GetComponent<Slider>();
        }
        if (soundslider && lastvol != soundslider.value * 0.01f)
        {
            audio_source.volume = soundslider.value * 0.005f;
            lastvol = soundslider.value * 0.01f;
        }
    }
}
