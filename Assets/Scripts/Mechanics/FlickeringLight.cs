using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlickerPattern : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Light targetLight;

    [Header("Configuración de Intensidad")]
    [SerializeField] private float intensidadMax = 5f;
    [SerializeField] private float intensidadMin = 0f;

    [Header("Configuración del Patrón")]
    [SerializeField] private float velocidadLenta = 1.5f;
    [SerializeField] private float velocidadRapida = 25.0f;
    
    [Tooltip("Valores más altos hacen que la luz pase más tiempo apagada.")]
    [Range(0f, 1f)]
    [SerializeField] private float umbralApagado = 0.4f;

    private void Awake()
    {
        if (targetLight == null)
        {
            targetLight = GetComponent<Light>();
        }
    }

    private void Update()
    {
        if (targetLight == null) return;

        float tiempo = Time.time;

        // 1. Calculamos la onda lenta y rápida exactamente igual que en el shader
        float slowWave = Mathf.Sin(tiempo * velocidadLenta);
        float fastWave = Mathf.Sin(tiempo * velocidadRapida);

        // 2. Normalizamos la combinación al rango [0, 1]
        float rawPattern = (slowWave + fastWave * 0.5f) * 0.5f + 0.5f;

        // 3. Convertimos a estado binario Encendido (1) o Apagado (0)
        float patronBinario = rawPattern >= umbralApagado ? 1f : 0f;

        // 4. Aplicamos la intensidad correspondiente
        targetLight.intensity = Mathf.Lerp(intensidadMin, intensidadMax, patronBinario);
    }
}