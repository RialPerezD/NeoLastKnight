using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseUi : MonoBehaviour
{
    Slider barraVida;
    TextMeshProUGUI monedas;
    Transform scoreBarra;

    Vector3 inicioBarra;

    void Start()
    {
        barraVida = transform.Find("HealthBar").GetComponent<Slider>();
        scoreBarra = transform.Find("Score").GetComponent<Transform>();
        monedas = scoreBarra.Find("ScoreText").GetComponent<TextMeshProUGUI>();

        inicioBarra = scoreBarra.position;
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


    IEnumerator MoverBarraMonedas(Vector3 origen, float cantidad, bool volver)
    {
        Vector3 destino = origen + new Vector3(cantidad,0,0);
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
            StartCoroutine(MoverBarraMonedas(scoreBarra.position, - cantidad, false));
        }
    }
}
