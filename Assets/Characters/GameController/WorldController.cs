using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldController : MonoBehaviour
{
    struct Movimiento {
        public GameObject actor_;
        public Vector2Int posicionActual_;
        public Vector2Int direccion_;

        public Movimiento(GameObject actor, Vector2Int origen, Vector2Int direccion) : this()
        {
            actor_ = actor;
            posicionActual_ = origen;
            direccion_ = direccion;
        }
    };

    WorldGenerator worldGenerator;
    Tilemap tilemap;
    int[,] level;

    List<GameObject> worldObjects;
    GameObject player;

    List<Movimiento> movimientos;

    void Start()
    {
        movimientos = new List<Movimiento>();
        worldGenerator = GetComponent<WorldGenerator>();
        tilemap = GameObject.Find("TMColisiones").GetComponent<Tilemap>();
    }

    public void LoadLevel(int level_index)
    {
        worldGenerator.GenerateWorldLayout(level_index, level_index != 0);
        level = worldGenerator.level;

        worldGenerator.GenerateObjects(tilemap);
        worldObjects = worldGenerator.objectsInScene;
        player = worldGenerator.player;
    }

    public void AplicaMovimientos()
    {
        foreach(Movimiento mov in movimientos)
        {
            MoveInGrid(mov);
        }

        movimientos.Clear();
    }


    void MoveInGrid(Movimiento mov)
    {
        int rows = level.GetLength(0); // Altura
        int cols = level.GetLength(1); // Ancho

        // Invertir Y porque la matriz tiene el origen en la esquina superior izquierda
        Vector2Int matrixPos = new Vector2Int(mov.posicionActual_.x, rows - mov.posicionActual_.y);
        Vector2Int moveDir = new Vector2Int(mov.direccion_.x, -mov.direccion_.y);
        Vector2Int newMatrixPos = matrixPos + moveDir;

        // Validación de límites
        if (newMatrixPos.x < 0 || newMatrixPos.x >= cols || newMatrixPos.y < 0 || newMatrixPos.y >= rows)
        {
            Debug.LogWarning("Movimiento fuera de los límites.");
            return;
        }

        // Mover el valor y poner 0 en la antigua posición
        level[newMatrixPos.y, newMatrixPos.x] = level[matrixPos.y, matrixPos.x];
        level[matrixPos.y, matrixPos.x] = 0;

        // Actualizamos la posicion del objeto y en el mundo
        mov.actor_.GetComponent<UpdatePosition>().UpdatePosicion(
            new Vector2Int(
                mov.direccion_.x + mov.posicionActual_.x,
                mov.direccion_.y + mov.posicionActual_.y
                )
            );
        StartCoroutine(MoveGameObject(mov.actor_, mov.direccion_));
    }

    IEnumerator MoveGameObject(GameObject go, Vector2Int direccion)
    {
        Vector3 startPos = go.transform.localPosition;
        Vector3 endPos = startPos + new Vector3(
            direccion.x * GameManager.grid_x_scale,
            direccion.y * GameManager.grid_y_scale,
            0);

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            go.transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        go.transform.localPosition = endPos;
    }

    public void MovimientoPlayer(Vector2Int origen, Vector2Int direccion)
    {
        // Comprobamos si casilla destino esta vacia
        Vector2Int destino = CoordenadaEnMatrix(origen, direccion);

        if (level[destino.y, destino.x] == 0 || level[destino.y, destino.x] >= 50)
        {
            movimientos.Add(new Movimiento(player, origen, direccion));
        }
    }


    public void GeneraMovimientosObjetos()
    {
        foreach (GameObject objeto in worldObjects)
        {
            Enemy enemy = objeto.GetComponent<Enemy>();
            if (enemy)
            {
                movimientos.Add(new Movimiento(objeto, enemy.posicion, enemy.DameMovimiento()));
            }
        }
    }


    Vector2Int CoordenadaEnMatrix(Vector2Int origen, Vector2Int direccion)
    {
        Vector2Int matrixPos = new Vector2Int(origen.x, level.GetLength(0) - origen.y);
        Vector2Int moveDir = new Vector2Int(direccion.x, -direccion.y);
        return matrixPos + moveDir;
    }
}
