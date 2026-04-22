using System;
using UnityEngine;


public class RuntimeCard
{
    public string id;
    public PowerUpData data;
    public int assignedSlot = -1; // -1 = deck

    public RuntimeCard(PowerUpData data)
    {
        this.id = Guid.NewGuid().ToString();
        this.data = data;
        this.assignedSlot = -1;
    }
}