using System;
using UnityEngine;

public class Level_1 : MonoBehaviour
{
    public GameObject camereIndicator;
    public GameObject joyStickIndicator;
    public GameObject flyButton;

    public static Action LevelComplete;
    private void Start()
    {
        camereIndicator.SetActive(true);
        joyStickIndicator.SetActive(false);
        flyButton.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerRotateYOnly.cameraIndicatorClick += CameraIndicatorClick;
        Joystick.OnClickJoyStick += OnClickJoyStick;
        ParrotController.TakeOff += TakeOff;
    }
    private void OnDisable()
    {
        PlayerRotateYOnly.cameraIndicatorClick -= CameraIndicatorClick;
        Joystick.OnClickJoyStick -= OnClickJoyStick;
        ParrotController.TakeOff -= TakeOff;

    }


    void CameraIndicatorClick()
    {
        ClearPanel();
        joyStickIndicator.SetActive(true);
    }
    void OnClickJoyStick()
    {
        ClearPanel();
        flyButton.SetActive(true);
    }
     void TakeOff()
    {
        ClearPanel();
        Invoke(nameof(CompleteLevel), 3);
    }


    void CompleteLevel()
    {
        LevelComplete?.Invoke();
        print("Level 1 completed");
    }


    void ClearPanel()
    {
        camereIndicator.SetActive(false);
        joyStickIndicator.SetActive(false);
        flyButton.SetActive(false);
    }
}
