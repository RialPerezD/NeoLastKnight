using UnityEngine;

public class PlayerStats : MonoBehaviour, Combat
{
    BaseUi ui;

    public float maxHp = 10;
    public float hp = 10;

    public int weaponType = 0;
    public float coins = 0;
    public int damage = 1;

    void Start()
    {
        ui = GameObject.Find("UI").GetComponent<BaseUi>();
    }

    public void Interactua(int objectType)
    {
        switch (objectType)
        {
            case 0:
                coins += 5;
                ui.CambiaMonedas(coins);
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

        ui.CambiaVida((hp / maxHp) * 100f);

        if (hp <= 0)
        {
            // muerto
            return true;
        }

        return false;
    }
}
