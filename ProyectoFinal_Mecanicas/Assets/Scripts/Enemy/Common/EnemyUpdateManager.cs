using System.Collections.Generic;
using UnityEngine;

public class EnemyUpdateManager : MonoBehaviour
{
    private static EnemyUpdateManager instance;
    private static readonly List<EnemyInstaller> activeEnemies = new List<EnemyInstaller>(512);

    private static bool isApplicationQuitting;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        instance = null;
        activeEnemies.Clear();
        isApplicationQuitting = false;
    }

    public static void Register(EnemyInstaller enemy)
    {
        if (enemy == null)
            return;

        EnsureInstance();

        if (activeEnemies.Contains(enemy))
            return;

        activeEnemies.Add(enemy);
    }

    public static void Unregister(EnemyInstaller enemy)
    {
        if (enemy == null)
            return;

        activeEnemies.Remove(enemy);
    }

    private static void EnsureInstance()
    {
        if (instance != null || isApplicationQuitting)
            return;

        GameObject managerObject = new GameObject("EnemyUpdateManager");
        instance = managerObject.AddComponent<EnemyUpdateManager>();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            EnemyInstaller enemy = activeEnemies[i];

            if (enemy == null)
            {
                activeEnemies.RemoveAt(i);
                continue;
            }

            if (!enemy.isActiveAndEnabled)
            {
                activeEnemies.RemoveAt(i);
                continue;
            }

            enemy.TickEnemy(deltaTime);
        }
    }

    private void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }
}
