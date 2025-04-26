using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShootManager : MonoBehaviour
{
    public List<GameObject> disparables;


    public GameObject EnemyShoot(Vector2Int posicion, Enemy enemy, int[,] level, Tilemap tilemap)
    {
        for (int i = -enemy.range; i <= enemy.range; i++)
        {
            Vector2Int compruebaX = new Vector2Int(
                Mathf.Clamp(posicion.x + i, 0, WorldGenerator.worldWidth),
                posicion.y);
            Vector2Int compruebaY = new Vector2Int(
                posicion.x,
                Mathf.Clamp(posicion.y + i, 0, level.Length/WorldGenerator.worldWidth));

            Vector2Int matrixPos = new Vector2Int(0, 0);
            bool disparo = false;
            if (level[compruebaX.y, compruebaX.x] == 100)
            {
                disparo = true;
                matrixPos = compruebaX;
            }else if (level[compruebaY.y, compruebaY.x] == 100)
            {
                disparo = true;
                matrixPos = compruebaY;
            }

            if (disparo)
            {
                Vector2Int v = matrixPos - posicion;
                Vector2Int dirNorm = new Vector2Int(
                    v.x == 0 ? 0 : (v.x > 0 ? 1 : -1),
                    v.y == 0 ? 0 : (v.y > 0 ? 1 : -1)
                );

                Vector2Int ubicacionSpawn = posicion + dirNorm;
                level[ubicacionSpawn.y, ubicacionSpawn.x] = WorldGenerator.projectileStart;

                Vector3 UbicacionMundo = new Vector3(
                    dirNorm.x * GameManager.grid_x_scale + enemy.transform.position.x,
                    -dirNorm.y * GameManager.grid_y_scale + enemy.transform.position.y,
                    0
                );

                // Crear una rotación en el eje Z (2D)
                float angle = Mathf.Atan2(-dirNorm.y, dirNorm.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0, 0, angle - 90);

                GameObject disparable = Instantiate(disparables[0], UbicacionMundo, rotation, tilemap.transform);
                disparable.GetComponent<Disparable>().posicion = new Vector2Int(matrixPos.x - 5, level.GetLength(0) - matrixPos.y);
                disparable.GetComponent<Disparable>().direccion = dirNorm;
                disparable.GetComponent<Disparable>().type = 1;
                disparable.GetComponent<Disparable>().damage = enemy.damage;

                return disparable;
            }
        }

        return null;
    }
}
