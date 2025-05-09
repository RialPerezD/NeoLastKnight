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

    //input wrong audio 
    public AudioSource audioSource;
    private bool lastmoveefective = false;

    public int alturaActual = 1;
    int alturaMovCam = 3;
    int limiteInferior = 0;
    bool camaraForzada = false;

    // Swipe variables
    Vector3 touch_start_pos;
    Vector3 touch_current_pos;
    Vector3 touch_delta;
    bool is_touching = false;
    float lapse = 0.0f;

    public bool esNivel = false;

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
            transform.rotation = Quaternion.Euler(0, 0, 0);
            if ((inputDirection.y > 0 || alturaActual + inputDirection.y >= limiteInferior) && lastmoveefective)
            {
                lastInputDirection = inputDirection;
                if (alturaActual == alturaMovCam && inputDirection.y < 0) alturaActual -= 1;
                if (alturaActual + inputDirection.y <= alturaMovCam) alturaActual = alturaActual + inputDirection.y;

                worldController.MovimientoPlayer(posicion, inputDirection);
            }
            lastmoveefective = true;
        }
        else if (inputDirection != Vector2Int.zero && !beatController.PuedoMoverme())
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            lastmoveefective = false;
            audioSource.Play();
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
#if UNITY_ANDROID
            return GetSwipeInput();
#else
        if (Input.GetKeyDown(KeyCode.W))
            return new Vector2Int(0, 1);     // Arriba
        if (Input.GetKeyDown(KeyCode.S))
            return new Vector2Int(0, -1);    // Abajo
        if (Input.GetKeyDown(KeyCode.A))
            return new Vector2Int(-1, 0);    // Izquierda
        if (Input.GetKeyDown(KeyCode.D))
            return new Vector2Int(1, 0);     // Derecha

        return Vector2Int.zero; // No se presionó ninguna tecla
#endif
    }

    Vector2Int GetSwipeInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touch_start_pos = Camera.main.ScreenToWorldPoint(touch.position);
                touch_start_pos.z = 0;
                is_touching = true;
                lapse = 0f;
            }

            if (is_touching)
            {
                lapse += Time.deltaTime;
            }

            if (touch.phase == TouchPhase.Moved && is_touching)
            {
                touch_current_pos = Camera.main.ScreenToWorldPoint(touch.position);
                touch_current_pos.z = 0;
                touch_delta = touch_current_pos - touch_start_pos;
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                is_touching = false;
                lapse = 1.0f;
            }

            if (!is_touching && lapse >= 1.0f)
            {
                lapse = 0.0f;
                Vector3 direction = touch_delta.normalized;
                touch_delta = Vector3.zero;

                Vector3 swipeDir = GetSwipeDirection(direction);

                if (swipeDir == Vector3.up) return Vector2Int.up;
                if (swipeDir == Vector3.down) return Vector2Int.down;
                if (swipeDir == Vector3.left) return Vector2Int.left;
                if (swipeDir == Vector3.right) return Vector2Int.right;
            }
        }

        return Vector2Int.zero;
    }

    Vector3 GetSwipeDirection(Vector3 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            return dir.x > 0 ? Vector3.right : Vector3.left;
        }
        else
        {
            return dir.y > 0 ? Vector3.up : Vector3.down;
        }
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
