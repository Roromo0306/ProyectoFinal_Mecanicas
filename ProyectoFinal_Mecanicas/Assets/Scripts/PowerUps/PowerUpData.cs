using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "PowerUps/PowerUp")]
public class PowerUpData : ScriptableObject
{

    [Header("Unlock")]
    public int unlockLevel = 1;

    public string title;
    public string description;
    public Sprite icon;

    public List<PowerUpEffect> effects;
}