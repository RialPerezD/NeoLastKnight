using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, UpdatePosition
{
    private static readonly Vector2Int[] mov_dir = new Vector2Int[]
    {
        new Vector2Int(1, 0),  // Derecha
        new Vector2Int(0, 1),  // Arriba
        new Vector2Int(-1, 0), // Izquierda
        new Vector2Int(0, -1)  // Abajo
    };

    int damage;
    int hp;
    int type;

    public List<int> movimientos;

    [HideInInspector]
    public Vector2Int posicion;
    int indiceMovimiento;

    private void Start()
    {
        indiceMovimiento = 0;
    }

    public Vector2Int DameMovimiento()
    {
        indiceMovimiento = (indiceMovimiento + 1) % movimientos.Count;
        return mov_dir[movimientos[indiceMovimiento]];
    }
    public void UpdatePosicion(Vector2Int pos)
    {
        posicion = pos;
    }
}
