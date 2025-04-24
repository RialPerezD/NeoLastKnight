using UnityEngine;

public class PlayerStats : MonoBehaviour, Combat
{
    public int weaponType = 0;
    public float hp = 10;
    public float coins = 0;
    public int damage = 1;

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

    public bool RecibeDamage(int ammount)
    {
        hp -= ammount;

        if (hp <= 0)
        {
            // muerto
            return true;
        }

        return false;
    }
}
