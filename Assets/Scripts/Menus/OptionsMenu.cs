using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    // Escena a la que se vuelve al tocar "Volver"
    public static string ReturnScene = "MainMenuScene"; // poné el nombre real de tu escena

    [Header("Mouse")]
    [SerializeField] Slider mouseSlider;
    [SerializeField] TMP_Text mouseLabel;

    [Header("Joystick")]
    [SerializeField] Slider stickSlider;
    [SerializeField] TMP_Text stickLabel;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetupSlider(mouseSlider, mouseLabel, GameSettings.MouseSensitivity,
            v => GameSettings.MouseSensitivity = v);
        SetupSlider(stickSlider, stickLabel, GameSettings.StickSensitivity,
            v => GameSettings.StickSensitivity = v);
    }

    void SetupSlider(Slider slider, TMP_Text label, float current, System.Action<float> onChange)
    {
        slider.minValue = 0.1f;
        slider.maxValue = 3f;
        slider.SetValueWithoutNotify(current);
        label.text = current.ToString("0.0") + "x";

        slider.onValueChanged.AddListener(v =>
        {
            onChange(v);
            label.text = v.ToString("0.0") + "x";
        });
    }

    public void Back()
    {
        SceneManager.LoadScene(ReturnScene);
    }
}