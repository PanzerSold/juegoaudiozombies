using System;
using UnityEngine;

public static class GameSettings
{
    const string MouseKey = "sens_mouse";
    const string StickKey = "sens_stick";

    public static event Action OnChanged;

    public static float MouseSensitivity
    {
        get => PlayerPrefs.GetFloat(MouseKey, 1f);
        set { PlayerPrefs.SetFloat(MouseKey, value); PlayerPrefs.Save(); OnChanged?.Invoke(); }
    }

    public static float StickSensitivity
    {
        get => PlayerPrefs.GetFloat(StickKey, 1f);
        set { PlayerPrefs.SetFloat(StickKey, value); PlayerPrefs.Save(); OnChanged?.Invoke(); }
    }
}