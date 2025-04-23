using UnityEngine;

public class Item : MonoBehaviour, UpdatePosition
{
    public Vector2Int posicion;

    public void UpdatePosicion(Vector2Int pos)
    {
        posicion = pos;
    }

    Vector2Int UpdatePosition.GetPosition()
    {
        return posicion;
    }
}
