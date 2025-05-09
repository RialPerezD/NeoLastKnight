using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static WorldController;

public class CombatController : MonoBehaviour
{
    ShootManager shootManager;

    List<GameObject> worldObjects;
    List<GameObject> destroyObjects;
    List<GameObject> addObjects;
    GameObject player;
    Tilemap tilemap;
    int[,] level;
    WorldGenerator worldGenerator;

    public List<GameObject> listaWeapons;

    private void Start()
    {
        shootManager = GetComponent<ShootManager>();
    }

    public void Actualiza(List<GameObject> worldObjects_,
        List<GameObject> destroyObjects_,
        List<GameObject> addObjects_,
        GameObject player_,
        Tilemap tilemap_,
        int[,] level_,
        WorldGenerator worldGenerator_)
    {
        worldObjects = worldObjects_;
        destroyObjects = destroyObjects_;
        addObjects = addObjects_;
        player = player_;
        tilemap = tilemap_;
        level = level_;
        worldGenerator = worldGenerator_;
    }

    public bool ComprobarCombate(Vector2Int newMatrixPos, Movimiento mov, Vector2Int matrixPos)
    {
        int numObjective = level[newMatrixPos.y, newMatrixPos.x];

        if (mov.padre_ == 0 && numObjective >= WorldGenerator.enemysStart && numObjective < WorldGenerator.projectileStart)
        {
            CombatePlayerEnemigo(newMatrixPos);
            GeneraArma(mov.actor_.transform.position, mov.direccion_, 0);
            if (mov.direccion_.y > 0) player.GetComponent<PlayerMovement>().alturaActual -= 1;
        }
        else if (mov.padre_ == 0 && numObjective >= WorldGenerator.projectileStart)
        {
            // Si el jugador se topa con un proyectil que no haga nada
        }
        else if (mov.padre_ >= 50 && numObjective != 0)
        {
            // Esto es proyectil
            if (numObjective == WorldGenerator.playerNumber)
            {
                CombateProyectilJugador(mov);
                level[matrixPos.y, matrixPos.x] = 0;
            }
            else if (numObjective <= WorldGenerator.objectsStart
                ||
                numObjective >= WorldGenerator.projectileStart
                ||
                numObjective == WorldGenerator.enemysStart+1)
            {
                level[matrixPos.y, matrixPos.x] = 0;
                destroyObjects.Add(mov.actor_);
            }
        }
        else if (mov.padre_ >= 1 && numObjective == WorldGenerator.playerNumber)
        {
            // Esto es enemigos
            ComabteEnemigoPlayer(mov);
            Vector3 posicion = mov.actor_.transform.position;
            if (mov.padre_ == 1) posicion.y -= GameManager.grid_y_scale / 4.0f;
            GeneraArma(posicion, mov.direccion_, 1);

        }
        else
        {
            // si no hay combate devolvemos true
            return true;
        }

        // si se combatio devolvemos false
        return false;
    }


    void GeneraArma(Vector3 pos, Vector2Int dir, int type)
    {
        Vector2 desplazamiento =
            new Vector2(
                dir.x * GameManager.grid_x_scale,
                dir.y * GameManager.grid_y_scale
                ) / 2;

        Vector3 newPos = new Vector3(
            pos.x + dir.x,
            pos.y + dir.y,
            0
        );

        // Crear una rotación en el eje Z (2D)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        Instantiate(listaWeapons[type], newPos, rotation, tilemap.transform);
    }


    void CombateProyectilJugador(Movimiento mov)
    {
        player.GetComponent<Combat>().RecibeDamage(mov.actor_.GetComponent<Disparable>().damage);
        destroyObjects.Add(mov.actor_);
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
        if (level[futurePlayerPos.y, futurePlayerPos.x] == WorldGenerator.enemysStart + 2)
        {
            bool salir = false;
            foreach (GameObject go in worldObjects)
            {
                Enemy enemy = go.GetComponent<Enemy>();
                if (enemy != null)
                {
                    if (enemy.type == 2)
                    {
                        Vector2Int pos = CoordenadaEnMatrix(go.GetComponent<UpdatePosition>().GetPosition(), new Vector2Int(0, 0), level);
                        foreach (Vector2Int actualPos in Enemy.GeneraMascaraTipo(pos, 0))
                        {
                            if (actualPos == futurePlayerPos)
                            {
                                if (go.GetComponent<Combat>().RecibeDamage(player.GetComponent<PlayerStats>().damage))
                                {
                                    Enemy.LimpiaMascaraTipo(pos, 0, level);
                                    enemy.noMuerto = false;
                                    destroyObjects.Add(go);

                                    level[futurePlayerPos.y, futurePlayerPos.x] = 51;
                                    worldGenerator.InstanciaPortal(enemy.transform.position, tilemap);

                                    salir = true;
                                    break;
                                }
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
                Vector2Int pos = CoordenadaEnMatrix(go.GetComponent<UpdatePosition>().GetPosition(), new Vector2Int(0, 0), level);
                Enemy enemy = go.GetComponent<Enemy>();
                if (enemy)
                {
                    if (enemy.generated) pos = enemy.posicion;
                }

                if (pos == futurePlayerPos)
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


    public List<GameObject> GeneraEnemigos(Enemy enemy, WorldGenerator worldGenerator)
    {
        Vector2Int boss = enemy.posicion;
        List<Vector2Int> matrixPos = new List<Vector2Int>();
        matrixPos.Add(new Vector2Int(boss.x - 4, level.GetLength(0) - boss.y + 1));
        matrixPos.Add(new Vector2Int(boss.x + 4, level.GetLength(0) - boss.y + 2));
        List<Vector3> worldPos = new List<Vector3>();
        worldPos.Add(new Vector3(
            -4 * GameManager.grid_x_scale + enemy.transform.position.x,
            -2 * GameManager.grid_y_scale + enemy.transform.position.y + 0.9f,
            0
        ));
        worldPos.Add(new Vector3(
            4 * GameManager.grid_x_scale + enemy.transform.position.x,
            -3 * GameManager.grid_y_scale + enemy.transform.position.y + 0.9f,
            0
        ));

        return worldGenerator.GeneraObjetos(matrixPos, worldPos, 76, tilemap, level);
    }


    public void CompruebaDisparos(Enemy enemy)
    {
        if (enemy.type == 1)
        {
            // Planta
            if (enemy.indiceDisparo == 0)
            {
                Vector2Int posicion;
                if (enemy.generated)
                {
                    posicion = enemy.posicion;
                }
                else
                {
                    posicion = CoordenadaEnMatrix(enemy.posicion, new Vector2Int(0, 0), level);
                }
                GameObject disparable = shootManager.EnemigoCompruebaDisparo(posicion, enemy, level, tilemap);

                if (disparable)
                {
                    addObjects.Add(disparable);
                    enemy.indiceDisparo = enemy.cadenciaDisparo;
                }
            }
            else
            {
                enemy.indiceDisparo--;
            }
        }else if (enemy.type == 2)
        {
            // Boss dragon
            int lugar = enemy.contadorDisparos - 1;

            for (int i = -1; i<2; i++)
            {
                Vector2Int posicion = new Vector2Int(enemy.posicion.x + (4 * lugar) + i, level.GetLength(0) - enemy.posicion.y);
                Vector3 wordPos = enemy.transform.position;
                wordPos.x += ((4 * lugar) + i) * GameManager.grid_x_scale;
                wordPos.y -= GameManager.grid_y_scale * 0.5f;

                if (lugar != 0)
                {
                    wordPos.y += GameManager.grid_y_scale;
                    posicion.y -= 1;
                }

                addObjects.Add(shootManager.CreaDisparo(new Vector2Int(0, 1), posicion, wordPos, enemy.damage, level, tilemap));
            }
        }
    }
}
