using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "PowerUps/PowerUp")]
public class PowerUpData : ScriptableObject
{
    public string title;
    public string description;
    public Sprite icon;

    public List<PowerUpEffect> effects;
}