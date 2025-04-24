using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class WorldController : MonoBehaviour
{
    struct Movimiento
    {
        public GameObject actor_;
        public Vector2Int posicionActual_;
        public Vector2Int direccion_;
        public int padre_;

        public Movimiento(GameObject actor, Vector2Int origen, Vector2Int direccion, int padre) : this()
        {
            actor_ = actor;
            posicionActual_ = origen;
            direccion_ = direccion;
            padre_ = padre;
        }
    };

    WorldGenerator worldGenerator;
    Tilemap tilemap;
    int[,] level;

    List<GameObject> worldObjects;
    List<GameObject> destroyObjects;
    GameObject player;

    List<Movimiento> movimientos;

    void Awake()
    {
        destroyObjects = new List<GameObject>();
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
        foreach (Movimiento mov in movimientos)
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
            Debug.LogWarning("Movimiento fuera de los límites. Obj " + mov.actor_ + " pos " + mov.posicionActual_ + " dir " + mov.direccion_);
            return;
        }


        if (mov.padre_ == 0 && level[newMatrixPos.y, newMatrixPos.x] >= WorldGenerator.enemysStart)
        {
            CombatePlayerEnemigo(newMatrixPos);
        }
        else if (mov.padre_ == 1 && level[newMatrixPos.y, newMatrixPos.x] == WorldGenerator.playerNumber)
        {
            ComabteEnemigoPlayer(mov);
        }
        else
        {
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
    }


    void ComabteEnemigoPlayer(Movimiento mov)
    {
        Enemy enemy = mov.actor_.GetComponent<Enemy>();
        enemy.indiceMovimiento--;
        player.GetComponent<Combat>().RecibeDamage(enemy.damage);
    }


    void CombatePlayerEnemigo(Vector2Int futurePlayerPos)
    {
        // Boss Dragon
        if (level[futurePlayerPos.y, futurePlayerPos.x] == WorldGenerator.enemysStart+2)
        {
            bool salir = false;
            foreach (GameObject go in worldObjects)
            {
                Enemy enemy = go.GetComponent<Enemy>();
                if (enemy.type == 1)
                {
                    Vector2Int pos = CoordenadaEnMatrix(go.GetComponent<UpdatePosition>().GetPosition(), new Vector2Int(0, 0));
                    foreach (Vector2Int actualPos in Enemy.GeneraMascaraTipo(pos, 0))
                    {
                        if (actualPos == futurePlayerPos)
                        {
                            if (go.GetComponent<Combat>().RecibeDamage(player.GetComponent<PlayerStats>().damage))
                            {
                                Enemy.LimpiaMascaraTipo(pos, 0, level);
                                destroyObjects.Add(go);

                                salir = true;
                                break;
                            }
                        }
                    }
                }
                if (salir) break;
            }
        }
        else
        {
            foreach (GameObject go in worldObjects)
            {
                if (CoordenadaEnMatrix(go.GetComponent<UpdatePosition>().GetPosition(), new Vector2Int(0, 0)) == futurePlayerPos)
                {
                    if (go.GetComponent<Combat>().RecibeDamage(player.GetComponent<PlayerStats>().damage))
                    {
                        level[futurePlayerPos.y, futurePlayerPos.x] = 0;
                        destroyObjects.Add(go);
                    }
                }
            }
        }
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
            if (go == null) break;

            go.transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (go != null) go.transform.localPosition = endPos;
    }

    public void MovimientoPlayer(Vector2Int origen, Vector2Int direccion)
    {
        // Comprobamos si casilla destino esta vacia
        Vector2Int destino = CoordenadaEnMatrix(origen, direccion);

        // Comprobar si es un enemigo o espacio en blanco
        if (level[destino.y, destino.x] == 0 || level[destino.y, destino.x] >= WorldGenerator.enemysStart)
        {
            movimientos.Add(new Movimiento(player, origen, direccion, 0));
        }
        // Comprobar si es un objeto
        else if (level[destino.y, destino.x] >= WorldGenerator.objectsStart)
        {
            movimientos.Add(new Movimiento(player, origen, direccion, 0));

            // Compruebo si choco contra un cofre o algo
            CompruebaIteracciones(CoordenadaEnMatrix(origen, direccion));
        }
    }


    public void GeneraMovimientosObjetos()
    {
        // Limpio la lista de nulls
        worldObjects.RemoveAll(obj => obj == null);

        // Recorro la lista
        foreach (GameObject objeto in worldObjects)
        {
            Enemy enemy = objeto.GetComponent<Enemy>();
            if (enemy)
            {
                Vector2Int mov = enemy.DameMovimiento();
                if (mov != new Vector2Int(0,0))
                {
                    movimientos.Add(new Movimiento(objeto, enemy.posicion, mov, 1));
                }
            }
        }
    }


    Vector2Int CoordenadaEnMatrix(Vector2Int origen, Vector2Int direccion)
    {
        Vector2Int matrixPos = new Vector2Int(origen.x, level.GetLength(0) - origen.y);
        Vector2Int moveDir = new Vector2Int(direccion.x, -direccion.y);
        return matrixPos + moveDir;
    }


    void CompruebaIteracciones(Vector2Int posicion)
    {
        if (level[posicion.y, posicion.x] < WorldGenerator.enemysStart && level[posicion.y, posicion.x] != 0)
        {
            player.GetComponent<PlayerStats>().Interactua(level[posicion.y, posicion.x] - WorldGenerator.objectsStart);

            foreach (GameObject go in worldObjects)
            {
                if (CoordenadaEnMatrix(go.GetComponent<UpdatePosition>().GetPosition(), new Vector2Int(0, 0)) == posicion)
                {
                    destroyObjects.Add(go);
                }
            }
        }
    }


    public void DestruyeBasura()
    {
        foreach (GameObject go in destroyObjects)
        {
            Destroy(go);
        }
    }
}
