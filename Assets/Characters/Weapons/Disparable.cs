using UnityEngine;

public class Disparable : MonoBehaviour, UpdatePosition
{

    public Vector2Int posicion;
    public Vector2Int direccion;
    public int type;
    public int damage;

    public Vector2Int GetPosition()
    {
        return posicion;
    }

    public void UpdatePosicion(Vector2Int pos)
    {
        posicion = pos;
    }
}
