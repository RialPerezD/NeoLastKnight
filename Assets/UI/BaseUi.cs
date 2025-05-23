using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseUi : MonoBehaviour
{
    PlayerStats stats;
    GameManager go;
    Slider barraVida;
    TextMeshProUGUI monedas;
    Transform costeEspada;
    Transform costeArco;
    Transform costeHp;
    Transform scoreBarra;

    Transform nivelEspada;
    Transform nivelArco;
    Transform nivelHp;


    public GameObject canvasToActivePause;
    public GameObject canvasDeath;
    private bool pauseactive = false;

    Vector3 inicioBarra;

    AudioSource audioSource;
    public List<AudioClip> listaSonidos;

    void Start()
    {
        go = GameObject.Find("GameManager").GetComponent<GameManager>();
        barraVida = transform.Find("HealthBar").GetComponent<Slider>();
        scoreBarra = transform.Find("Score").GetComponent<Transform>();
        monedas = scoreBarra.Find("ScoreText").GetComponent<TextMeshProUGUI>();

        costeEspada = transform.Find("Equipment").Find("Buttons").Find("Sword").Find("center")
                .Find("Titulo");

        costeArco = transform.Find("Equipment").Find("Buttons").Find("Bow").Find("center")
                .Find("Titulo");

        costeHp = transform.Find("Armour").Find("Buttons").Find("Hp").Find("center")
                .Find("Titulo");

        nivelEspada = transform.Find("PowrLevels").Find("Sword").Find("shadow");
        nivelArco = transform.Find("PowrLevels").Find("Bow").Find("shadow");
        nivelHp = transform.Find("PowrLevels").Find("Hp").Find("shadow");

        audioSource = GetComponent<AudioSource>();

        inicioBarra = scoreBarra.position;
    }

    private void Update()
    {
        if (stats == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player)
            {
                stats = player.GetComponent<PlayerStats>();
            }
        }

        if (monedas != null)
        {
            monedas.text = stats.coins.ToString();
        }

        if(nivelEspada != null)
        {
            nivelEspada.GetComponent<TextMeshProUGUI>().text = stats.damage.ToString();
            nivelEspada.transform.Find("text").GetComponent<TextMeshProUGUI>().text = stats.damage.ToString();
            nivelArco.GetComponent<TextMeshProUGUI>().text = stats.bowDamage.ToString();
            nivelArco.transform.Find("text").GetComponent<TextMeshProUGUI>().text = stats.bowDamage.ToString();
            nivelHp.GetComponent<TextMeshProUGUI>().text = stats.maxHp.ToString();
            nivelHp.transform.Find("text").GetComponent<TextMeshProUGUI>().text = stats.maxHp.ToString();
        }
    }

    public void CambiaVida(float vida)
    {
        barraVida.value = vida;
    }

    public void CambiaMonedas(float cantidadMonedas)
    {
        monedas.text = cantidadMonedas.ToString();

        int tamanhoNumero = (int)Mathf.Floor(Mathf.Log10(Mathf.Abs(cantidadMonedas)));
        int cantidadDesplazamiento = 300 + (55 * tamanhoNumero);

        StopAllCoroutines();
        StartCoroutine(MoverBarraMonedas(inicioBarra, cantidadDesplazamiento, true));
    }

    public void OpenPauseMenu()
    {
        if (pauseactive)
        {
            pauseactive = false;
            canvasToActivePause.SetActive(pauseactive);
        }
        else
        {

            pauseactive = true;
            canvasToActivePause.SetActive(pauseactive);

        }
    }


    IEnumerator MoverBarraMonedas(Vector3 origen, float cantidad, bool volver)
    {
        Vector3 destino = origen + new Vector3(cantidad, 0, 0);
        float duracion = GameManager.animDuration;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion;
            scoreBarra.position = Vector3.Lerp(origen, destino, t);
            yield return null;
        }

        // Aseguramos que termine exactamente en el destino
        scoreBarra.position = destino;

        if (volver)
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(MoverBarraMonedas(scoreBarra.position, -cantidad, false));
        }
    }

    public void ActivaTienda(int tipo)
    {
        if (tipo == 1)
        {
            transform.Find("Armour").gameObject.SetActive(true);

            costeHp.GetComponent<TextMeshProUGUI>().text = stats.costeHp.ToString();
            costeHp.Find("TituloSombra").GetComponent<TextMeshProUGUI>().text = stats.costeHp.ToString();
        }
        else
        {
            transform.Find("Equipment").gameObject.SetActive(true);

            costeEspada.GetComponent<TextMeshProUGUI>().text = stats.costeSword.ToString();
            costeEspada.Find("TituloSombra").GetComponent<TextMeshProUGUI>().text = stats.costeSword.ToString();

            costeArco.GetComponent<TextMeshProUGUI>().text = stats.costeBow.ToString();
            costeArco.Find("TituloSombra").GetComponent<TextMeshProUGUI>().text = stats.costeBow.ToString();
        }

        ActualizaColores();
    }

    public void CompraAlgo(int cosa)
    {
        if (cosa == 0 && stats.costeSword <= stats.coins)
        {
            audioSource.PlayOneShot(listaSonidos[0]);
            stats.coins -= stats.costeSword;
            stats.costeSword += 10;
            stats.damage += 1;

            costeEspada.GetComponent<TextMeshProUGUI>().text = stats.costeSword.ToString();
            costeEspada.Find("TituloSombra").GetComponent<TextMeshProUGUI>().text = stats.costeSword.ToString();

            monedas.text = stats.coins.ToString();
        }
        else if (cosa == 1 && stats.costeBow <= stats.coins)
        {
            audioSource.PlayOneShot(listaSonidos[0]);
            stats.coins -= stats.costeBow;
            stats.costeBow += 10;
            stats.bowDamage += 1;

            costeArco.GetComponent<TextMeshProUGUI>().text = stats.costeBow.ToString();
            costeArco.Find("TituloSombra").GetComponent<TextMeshProUGUI>().text = stats.costeBow.ToString();

            monedas.text = stats.coins.ToString();
        }
        else if (cosa == 2 && stats.costeHp <= stats.coins)
        {
            audioSource.PlayOneShot(listaSonidos[0]);
            stats.coins -= stats.costeHp;
            stats.costeHp += 10;
            stats.maxHp += 2;

            costeHp.GetComponent<TextMeshProUGUI>().text = stats.costeHp.ToString();
            costeHp.Find("TituloSombra").GetComponent<TextMeshProUGUI>().text = stats.costeHp.ToString();

            monedas.text = stats.coins.ToString();
        }
        else
        {
            audioSource.PlayOneShot(listaSonidos[1]);
        }

        ActualizaColores();
    }

    void ActualizaColores()
    {
        if (stats.costeSword > stats.coins)
        {
            costeEspada.Find("TituloSombra").GetComponent<TextMeshProUGUI>().color = Color.red;
        }
        else
        {
            costeEspada.Find("TituloSombra").GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }

        if (stats.costeBow > stats.coins)
        {
            costeArco.Find("TituloSombra").GetComponent<TextMeshProUGUI>().color = Color.red;
        }
        else
        {
            costeArco.Find("TituloSombra").GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }

        if (stats.costeHp > stats.coins)
        {
            costeHp.Find("TituloSombra").GetComponent<TextMeshProUGUI>().color = Color.red;
        }
        else
        {
            costeHp.Find("TituloSombra").GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }
    }

    public void Muerte()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().CargarPueblo();
        canvasDeath.SetActive(false);
    }

    public void CargaMenuPrincipal()
    {
        go.CargaMenuPrincipal();
    }
}
