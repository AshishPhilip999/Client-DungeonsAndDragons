using UnityEngine;

public interface Damageable : Destroyable
{
    public int getTotalHealth();
    public int getHealth();
    public int getArmorHealth();

    public void damage(int damagePoints);
}
