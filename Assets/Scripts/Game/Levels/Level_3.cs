using System;
using UnityEngine;

public class Level_3 : MonoBehaviour
{
    public static Action LevelComplete;

    [Header("Level 3 Settings")]
    public int totalRings = 3;

    private int ringsCollected = 0;

    // ✅ Subscribe
    private void OnEnable()
    {
        RingTrigger.OnRingTriggered += RingTriggered;
    }

    // ✅ Unsubscribe
    private void OnDisable()
    {
        RingTrigger.OnRingTriggered -= RingTriggered;
    }

    // ✅ Called when any ring is triggered
    private void RingTriggered()
    {
        ringsCollected++;
        Debug.Log("Ring Triggered : " + ringsCollected);

        if (ringsCollected >= totalRings)
        {
            Debug.Log("Level 3 Completed");
            LevelComplete?.Invoke();
        }
    }
}
