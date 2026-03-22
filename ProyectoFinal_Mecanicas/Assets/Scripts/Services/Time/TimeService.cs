using UnityEngine;

public class TimeService : ITimeService
{
    public float GetTime()
    {
        return Time.time;
    }
}