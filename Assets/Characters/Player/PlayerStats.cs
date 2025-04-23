using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int weaponType;

    float hp;
    public float coins;

    private void Start()
    {
        weaponType = 0;
        hp = 100;
        coins = 0;
    }

    public void Interactua(int objectType)
    {
        switch (objectType)
        {
            case 0:
                coins += 5;
                break;
            case 1:
                coins += 5;
                break;
            default:
                break;
        }
    }
}
