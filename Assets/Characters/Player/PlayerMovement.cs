using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour, UpdatePosition
{
    public Tilemap obstacle_tilemap; // Tilemap de los obstáculos

    public Vector2Int posicion;

    BeatController beatController;
    WorldController worldController;

    private void Start()
    {
        beatController = GameObject.Find("BeatController").GetComponent<BeatController>();
        worldController = GameObject.Find("WorldController").GetComponent<WorldController>();
    }

    void Update()
    {
        Vector2Int inputDirection = GetInputDirection();

        if (inputDirection != Vector2Int.zero && beatController.PuedoMoverme())
        {
            worldController.MovimientoPlayer(posicion, inputDirection);
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
}
