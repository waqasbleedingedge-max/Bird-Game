using System;
using UnityEngine;

public class Level_2 : MonoBehaviour
{
    public static Action LevelComplete;
 

   
    public void OnLevelCompleted()
    {
        LevelComplete?.Invoke();
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Bird"))
        {
            print("Level 2 Completed");
            LevelComplete?.Invoke();
        }
    }
}
