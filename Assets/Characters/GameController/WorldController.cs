using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldController : MonoBehaviour
{
    public struct Movimiento
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
    CombatController combatController;
    Tilemap tilemap;
    int[,] level;

    List<GameObject> worldObjects;
    List<GameObject> destroyObjects;
    List<GameObject> addObjects;
    GameObject player;

    List<Movimiento> movimientos;

    bool esNivel = true;
    int ajusteVertical = 0;

    void Awake()
    {
        destroyObjects = new List<GameObject>();
        addObjects = new List<GameObject>();
        movimientos = new List<Movimiento>();
        worldGenerator = GetComponent<WorldGenerator>();
        combatController = GetComponent<CombatController>();
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
        int rows = level.GetLength(0);
        int cols = level.GetLength(1);

        Vector2Int matrixPos = mov.padre_ >= 50
            ? new Vector2Int(mov.posicionActual_.x, mov.posicionActual_.y)
            : new Vector2Int(mov.posicionActual_.x, rows - mov.posicionActual_.y);

        Vector2Int moveDir = new Vector2Int(mov.direccion_.x, -mov.direccion_.y);
        Vector2Int newMatrixPos = matrixPos + moveDir;

        if (newMatrixPos.x < 0 || newMatrixPos.x >= cols || newMatrixPos.y < 0 || newMatrixPos.y >= rows)
        {
            Debug.LogWarning("Movimiento fuera de los límites. Obj " + mov.actor_ + " pos " + mov.posicionActual_ + " dir " + mov.direccion_);
            return;
        }

        bool playerMueveCam = false;
        combatController.Actualiza(worldObjects, destroyObjects, addObjects, player, tilemap, level, worldGenerator);

        bool hayCombate = true;
        if (mov.padre_ != 3)
        {
            hayCombate = combatController.ComprobarCombate(newMatrixPos, mov, matrixPos);
        }
        else
        {
            for (int i = -1; i < 1; i++)
            {
                if (!combatController.ComprobarCombate(
                    new Vector2Int(newMatrixPos.x + moveDir.x, newMatrixPos.y + i + moveDir.y),
                    mov,
                    matrixPos
                    ))
                {
                    hayCombate = false;
                }
            }
        }

        if (hayCombate)
        {
            if (mov.padre_ == 0)
            {
                playerMueveCam = newMatrixPos.y >= (6 - ajusteVertical) ? true : false;
                if (!playerMueveCam && esNivel) player.GetComponent<PlayerMovement>().FuerzaCentroCamara();
            }

            if (mov.padre_ != 3)
            {
                level[newMatrixPos.y, newMatrixPos.x] = level[matrixPos.y, matrixPos.x];
                level[matrixPos.y, matrixPos.x] = 0;
            }
            else
            {
                if (mov.actor_.GetComponent<Enemy>().noMuerto)
                {
                    Enemy.LimpiaMascaraTipo(matrixPos, 0, level);
                    List<Vector2Int> posiciones = Enemy.GeneraMascaraTipo(newMatrixPos, 0);
                    foreach (Vector2Int vec in posiciones)
                    {
                        level[vec.y, vec.x] = WorldGenerator.enemysStart + 2;
                    }
                }
            }

            int esLanzable = (mov.padre_ >= 50) ? -1 : 1;
            mov.actor_.GetComponent<UpdatePosition>().UpdatePosicion(
                new Vector2Int(
                    mov.direccion_.x + mov.posicionActual_.x,
                    esLanzable * mov.direccion_.y + mov.posicionActual_.y
                )
            );
            StartCoroutine(MoveGameObject(mov.actor_, mov.direccion_));
        }

        if (mov.padre_ == 0 && playerMueveCam)
        {
            player.GetComponent<PlayerMovement>().MueveCamara();
        }
    }

    IEnumerator MoveGameObject(GameObject go, Vector2Int direccion)
    {
        Vector3 startPos = go.transform.localPosition;
        Vector3 endPos = startPos + new Vector3(
            direccion.x * GameManager.grid_x_scale,
            direccion.y * GameManager.grid_y_scale,
            0);

        float elapsed = 0f;

        while (elapsed < GameManager.animDuration)
        {
            if (go == null) break;

            go.transform.localPosition = Vector3.Lerp(startPos, endPos, elapsed / GameManager.animDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (go != null) go.transform.localPosition = endPos;
    }

    public void MovimientoPlayer(Vector2Int origen, Vector2Int direccion)
    {
        // Comprobamos si casilla destino esta vacia
        Vector2Int destino = CoordenadaEnMatrix(origen, direccion, level);

        // Si es equipamiento abrimos ese menu
        if (level[destino.y, destino.x] == WorldGenerator.equipmentNumber)
        {
            player.GetComponent<PlayerStats>().Interactua(100);
        }
        // Si es pasivas abrimos ese menu
        else if (level[destino.y, destino.x] == WorldGenerator.pasivesNumber)
        {
            player.GetComponent<PlayerStats>().Interactua(101);
        }
        // Comprobar si es un enemigo o espacio en blanco
        else if (level[destino.y, destino.x] == 0 || level[destino.y, destino.x] >= WorldGenerator.enemysStart)
        {
            movimientos.Add(new Movimiento(player, origen, direccion, 0));
        }
        // Comprobar si es un objeto
        else if (level[destino.y, destino.x] >= WorldGenerator.objectsStart)
        {
            movimientos.Add(new Movimiento(player, origen, direccion, 0));

            // Compruebo si choco contra un cofre o algo
            CompruebaIteracciones(CoordenadaEnMatrix(origen, direccion, level));
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
            Disparable disparable = objeto.GetComponent<Disparable>();

            if (enemy)
            {
                if (enemy.type == 2)
                {
                    combatController.Actualiza(worldObjects, destroyObjects, addObjects, player, tilemap, level, worldGenerator);
                    switch (enemy.QueHago())
                    {
                        case 1:
                            combatController.CompruebaDisparos(enemy);
                            break;
                        case 2:
                            List<GameObject> objetosCreados = combatController.GeneraEnemigos(enemy, worldGenerator); ;
                            foreach (var obj in objetosCreados)
                            {
                                Vector2Int ubi = obj.GetComponent<Enemy>().posicion;
                                addObjects.Add(obj);
                            }
                            break;
                        case 3:
                            Vector2Int mov = enemy.DameMovimiento();
                            movimientos.Add(new Movimiento(objeto, enemy.posicion, mov, 1 + enemy.type));
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Vector2Int mov = enemy.DameMovimiento();
                    if (mov != new Vector2Int(0, 0))
                    {
                        movimientos.Add(new Movimiento(objeto, enemy.posicion, mov, 1 + enemy.type));
                    }
                    else
                    {
                        combatController.Actualiza(worldObjects, destroyObjects, addObjects, player, tilemap, level, worldGenerator);
                        combatController.CompruebaDisparos(enemy);
                    }
                }
            }
            else if (disparable)
            {
                movimientos.Add(new Movimiento(objeto, disparable.posicion, disparable.direccion, 50 + disparable.type));
            }
        }
    }


    public static Vector2Int CoordenadaEnMatrix(Vector2Int origen, Vector2Int direccion, int[,] level_)
    {
        Vector2Int matrixPos = new Vector2Int(origen.x, level_.GetLength(0) - origen.y);
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
                if (go == null) continue;

                if (go.GetComponent<UpdatePosition>() != null)
                {
                    if (CoordenadaEnMatrix(go.GetComponent<UpdatePosition>().GetPosition(), new Vector2Int(0, 0), level) == posicion)
                    {
                        destroyObjects.Add(go);
                    }
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

        destroyObjects.Clear();

        worldObjects.RemoveAll(obj => obj == null);
    }


    public void SpawneaObjetos()
    {
        foreach (GameObject go in addObjects)
        {
            worldObjects.Add(go);
        }

        addObjects.Clear();

        worldObjects.RemoveAll(obj => obj == null);
    }


    public void UpdateaEsNivel(bool esNiv, int ajuste)
    {
        esNivel = esNiv;
        ajusteVertical = ajuste;
        player.GetComponent<PlayerMovement>().esNivel = esNiv;
    }
}
