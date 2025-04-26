using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour, UpdatePosition
{
    public Tilemap obstacle_tilemap; // Tilemap de los obstáculos

    public Vector2Int posicion;

    Vector2Int lastInputDirection;
    BeatController beatController;
    WorldController worldController;
    Camera myCamera;

    public int alturaActual = 1;
    int alturaMovCam = 3;
    int limiteInferior = -1;
    bool camaraForzada = false;

    private void Awake()
    {
        beatController = GameObject.Find("BeatController").GetComponent<BeatController>();
        worldController = GameObject.Find("WorldController").GetComponent<WorldController>();
        myCamera = GameObject.Find("Camera").GetComponent<Camera>();
    }

    void Update()
    {
        Vector2Int inputDirection = GetInputDirection();

        if (inputDirection != Vector2Int.zero && beatController.PuedoMoverme())
        {
            if (inputDirection.y > 0 || alturaActual + inputDirection.y >= limiteInferior)
            {
                lastInputDirection = inputDirection;
                if (alturaActual == alturaMovCam && inputDirection.y < 0) alturaActual -= 1;
                if (alturaActual + inputDirection.y <= alturaMovCam) alturaActual = alturaActual + inputDirection.y;

                worldController.MovimientoPlayer(posicion, inputDirection);
            }
        }
    }

    public void MueveCamara()
    {
        if (lastInputDirection.y > 0 && alturaActual == alturaMovCam)
        {
            StartCoroutine(MoverCamaraSuavemente());
        }
    }

    public void FuerzaCentroCamara()
    {
        if (!camaraForzada)
        {
            camaraForzada = true;
            alturaActual = -3;
            limiteInferior = -3;
            StartCoroutine(ForzarCamaraCentrada());
        }
    }

    Vector2Int GetInputDirection()
    {
        if (Input.GetKeyDown(KeyCode.W))
            return new Vector2Int(0, 1);     // Arriba
        if (Input.GetKeyDown(KeyCode.S))
            return new Vector2Int(0, -1);    // Abajo
        if (Input.GetKeyDown(KeyCode.A))
            return new Vector2Int(-1, 0);    // Izquierda
        if (Input.GetKeyDown(KeyCode.D))
            return new Vector2Int(1, 0);     // Derecha

        return Vector2Int.zero; // No se presionó ninguna tecla
    }

    public void UpdatePosicion(Vector2Int pos)
    {
        posicion = pos;
    }

    public Vector2Int GetPosition()
    {
        return posicion;
    }

    IEnumerator MoverCamaraSuavemente()
    {
        Vector3 startPos = myCamera.transform.position;
        Vector3 endPos = startPos + new Vector3(0, GameManager.grid_y_scale * lastInputDirection.y, 0);
        float tiempo = 0f;

        while (tiempo < GameManager.animDuration)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.Clamp01(tiempo / GameManager.animDuration);
            myCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        myCamera.transform.position = endPos;
    }

    IEnumerator ForzarCamaraCentrada()
    {
        Vector3 startPos = myCamera.transform.position;
        Vector3 endPos = startPos + new Vector3(0, GameManager.grid_y_scale * 4, 0);
        float tiempo = 0f;

        while (tiempo < GameManager.animDuration * 2)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.Clamp01(tiempo / GameManager.animDuration);
            myCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        myCamera.transform.position = endPos;
    }
}
