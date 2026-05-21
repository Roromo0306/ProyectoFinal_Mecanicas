using UnityEngine;

public struct DamageableHealthChangedEvent
{
    public GameObject target;
    public float currentHealth;
    public float maxHealth;
    public bool isDead;

    public DamageableHealthChangedEvent(GameObject target, float currentHealth, float maxHealth, bool isDead)
    {
        this.target = target;
        this.currentHealth = currentHealth;
        this.maxHealth = maxHealth;
        this.isDead = isDead;
    }
}
