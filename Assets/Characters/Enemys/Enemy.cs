using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, UpdatePosition, Combat
{
    static readonly Vector2Int[] mov_dir = new Vector2Int[]
    {
        new Vector2Int(1, 0),  // Derecha
        new Vector2Int(0, 1),  // Arriba
        new Vector2Int(-1, 0), // Izquierda
        new Vector2Int(0, -1)  // Abajo
    };

    static readonly List<Vector2Int[]> mascaras = new List<Vector2Int[]>
    { 
        // Estructura 1: (-1 a +1 en X, -1 a 0 en Y)
        new Vector2Int[]
        {
            new Vector2Int(-1, 1),
            new Vector2Int(-1, 0),
        }
    };

    public int damage;
    public int hp;
    public int type;
    public int range;

    public List<int> movimientos;

    [HideInInspector]
    public Vector2Int posicion;
    public int indiceMovimiento;

    public int cadenciaDisparo = 3;
    public int indiceDisparo = 0;

    // Para boss
    int maxDisparos = 3;
    public int contadorDisparos = 0;
    GameObject enemigoSpawnear;
    int accionActual = 0;

    private void Awake()
    {
        indiceMovimiento = 0;
    }

    public Vector2Int DameMovimiento()
    {
        if(movimientos.Count == 0) return new Vector2Int(0, 0);

        indiceMovimiento = (indiceMovimiento + 1) % movimientos.Count;
        return mov_dir[movimientos[indiceMovimiento]];
    }
    public void UpdatePosicion(Vector2Int pos)
    {
        posicion = pos;
    }

    Vector2Int UpdatePosition.GetPosition()
    {
        return posicion;
    }

    public bool RecibeDamage(int ammount)
    {
        hp -= ammount;

        if (hp <= 0) return true;

        return false;
    }

    public static List<Vector2Int> GeneraMascaraTipo(Vector2Int origen, int tipo)
    {
        List<Vector2Int> mascara = new List<Vector2Int>();

        Vector2Int[] ajustes = mascaras[tipo];

        for (int y = origen.y + ajustes[1].x; y <= origen.y + ajustes[1].y; y++)
        {
            for (int x = origen.x + ajustes[0].x; x <= origen.x + ajustes[0].y; x++)
            {
                mascara.Add(new Vector2Int(x, y));
            }
        }

        return mascara;
    }

    public static void LimpiaMascaraTipo(Vector2Int origen, int tipo, int[,] level)
    {
        Vector2Int[] ajustes = mascaras[tipo];

        for (int y = origen.y + ajustes[1].x; y <= origen.y + ajustes[1].y; y++)
        {
            for (int x = origen.x + ajustes[0].x; x <= origen.x + ajustes[0].y; x++)
            {
                level[y, x] = 0;
            }
        }
    }


    public int QueHago()
    {
        // 0 No hagas nada, 1 dispara, 2 spawnea, 3 muevete
        int accion = 0;
        
        // Si es 0 dispara, si es 1 spawnea, si es 2 se mueve
        if (accionActual == 0)
        {
            if (indiceDisparo < cadenciaDisparo)
            {
                indiceDisparo++;
            }
            else
            {
                accion = 1;

                indiceDisparo = 0;
                contadorDisparos++;

                if (contadorDisparos >= maxDisparos)
                {
                    contadorDisparos = 0;
                    //accionActual++;
                }
            }
        }else if (accionActual == 1)
        {
            accion = 2;
        }
        else
        {
            accion = 3;
        }

        return accion;
    }
}
