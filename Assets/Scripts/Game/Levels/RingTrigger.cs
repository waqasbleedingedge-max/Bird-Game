using System;
using UnityEngine;

public class RingTrigger : MonoBehaviour
{
    public static Action OnRingTriggered;
    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Bird"))
        {
            triggered = true;
            OnRingTriggered?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
