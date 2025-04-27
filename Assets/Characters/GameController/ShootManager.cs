using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ShootManager : MonoBehaviour
{
    public List<GameObject> disparables;


    public GameObject EnemigoCompruebaDisparo(Vector2Int actualPos, Enemy enemy, int[,] level, Tilemap tilemap)
    {
        for (int i = -enemy.range; i <= enemy.range; i++)
        {
            Vector2Int compruebaX = new Vector2Int(
                Mathf.Clamp(actualPos.x + i, 0, WorldGenerator.worldWidth),
                actualPos.y);
            Vector2Int compruebaY = new Vector2Int(
                actualPos.x,
                Mathf.Clamp(actualPos.y + i, 0, level.Length/WorldGenerator.worldWidth));

            Vector2Int futuroPos = new Vector2Int(0, 0);
            bool disparo = false;
            if (level[compruebaX.y, compruebaX.x] == 100)
            {
                disparo = true;
                futuroPos = compruebaX;
            }else if (level[compruebaY.y, compruebaY.x] == 100)
            {
                disparo = true;
                futuroPos = compruebaY;
            }

            if (disparo)
            {
                Vector2Int dir = futuroPos - actualPos;
                Vector2Int dirNorm = new Vector2Int(
                    dir.x == 0 ? 0 : (dir.x > 0 ? 1 : -1),
                    dir.y == 0 ? 0 : (dir.y > 0 ? 1 : -1)
                );

                return CreaDisparo(dirNorm, actualPos, enemy.transform.position, enemy.damage, level, tilemap);
            }
        }

        return null;
    }


    public GameObject CreaDisparo(Vector2Int dir, Vector2Int posicion, Vector3 worldPosition, int projDamage, int[,] level, Tilemap tilemap)
    {
        GameObject disparable;

        Vector2Int ubicacionSpawn = posicion + dir;
        level[ubicacionSpawn.y, ubicacionSpawn.x] = WorldGenerator.projectileStart;

        Vector3 UbicacionMundo = new Vector3(
            dir.x * GameManager.grid_x_scale + worldPosition.x,
            -dir.y * GameManager.grid_y_scale + worldPosition.y,
            0
        );

        // Crear una rotación en el eje Z (2D)
        float angle = Mathf.Atan2(-dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle - 90);

        disparable = Instantiate(disparables[0], UbicacionMundo, rotation, tilemap.transform);
        disparable.GetComponent<Disparable>().posicion = new Vector2Int(ubicacionSpawn.x, ubicacionSpawn.y);
        disparable.GetComponent<Disparable>().direccion = new Vector2Int(dir.x, -dir.y);
        disparable.GetComponent<Disparable>().type = 1;
        disparable.GetComponent<Disparable>().damage = projDamage;

        return disparable;
    }
}
