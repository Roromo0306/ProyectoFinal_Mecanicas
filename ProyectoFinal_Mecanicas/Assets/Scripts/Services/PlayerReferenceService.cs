using UnityEngine;

public static class PlayerReferenceService
{
    private static Transform cachedPlayer;

    public static Transform PlayerTransform
    {
        get
        {
            if (cachedPlayer == null)
                RefreshPlayerReference();

            return cachedPlayer;
        }
    }

    public static bool TryGetPlayer(out Transform player)
    {
        player = PlayerTransform;
        return player != null;
    }

    public static void SetPlayer(Transform player)
    {
        cachedPlayer = player;
    }

    public static void Clear()
    {
        cachedPlayer = null;
    }

    private static void RefreshPlayerReference()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            cachedPlayer = playerObject.transform;
    }
}