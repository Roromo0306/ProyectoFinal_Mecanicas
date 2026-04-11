using UnityEngine;

public enum PowerUpType
{
    Weapon,
    Modifier,
    Ability,
    Stat,
    Passive,
    Status
}

[CreateAssetMenu(menuName = "PowerUps/PowerUp")]
public class PowerUpData : ScriptableObject
{
    public string id;
    public string title;
    public string description;
    public Sprite icon;

    public PowerUpType type;

    public int rarity;
}