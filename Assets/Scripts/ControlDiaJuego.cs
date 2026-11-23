using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ControlDiaJuego : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ContadorClicker contadorClicker;
    [SerializeField] private TMP_Text textoDia;
    [SerializeField] private TMP_Text textoTemporizador;
    [SerializeField] private TMP_Text textoCobroEstimado;

    [Header("Configuración de Día (1 minuto = 60s)")]
    [SerializeField] private float duracionDiaSegundos = 60f;
    [SerializeField] private int cobroBaseDia1 = 150;
    [SerializeField] private int incrementoPorDia = 75;
    [SerializeField] private string nombreEscenaCobro = "Cobro";

    private const string ClaveTiempoRestante = "TiempoRestanteDia";

    private int diaActual;
    private float tiempoRestante;
    private bool transicionando;

    private void Start()
    {
        diaActual = PlayerPrefs.GetInt("DiaActual", 1);

        // Si había un tiempo guardado por pausa, lo recupera; si no, inicia el día completo
        tiempoRestante = PlayerPrefs.GetFloat(ClaveTiempoRestante, duracionDiaSegundos);
        ActualizarUI();
    }

    private void Update()
    {
        if (transicionando)
        {
            return;
        }

        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0f)
        {
            tiempoRestante = 0f;
            transicionando = true;
            IrAEscenaCobro();
            return;
        }

        ActualizarUI();
    }

    // Llama a este método justo antes de cargar la escena de Pausa
    public void GuardarTiempoAntesDePausar()
    {
        contadorClicker?.GuardarProgreso();
        PlayerPrefs.SetFloat(ClaveTiempoRestante, tiempoRestante);
        PlayerPrefs.Save();
    }

    public int ObtenerMontoCobro()
    {
        return cobroBaseDia1 + (diaActual - 1) * incrementoPorDia;
    }

    private void IrAEscenaCobro()
    {
        contadorClicker?.GuardarProgreso();
        PlayerPrefs.DeleteKey(ClaveTiempoRestante); // El siguiente día iniciará completo
        PlayerPrefs.SetInt("DiaActual", diaActual);
        PlayerPrefs.SetInt("MontoCobroActual", ObtenerMontoCobro());
        PlayerPrefs.Save();

        SceneManager.LoadScene(nombreEscenaCobro);
    }

    private void ActualizarUI()
    {
        if (textoDia != null)
        {
            textoDia.text = $"Día {diaActual}";
        }

        if (textoTemporizador != null)
        {
            int segundosTotales = Mathf.CeilToInt(tiempoRestante);
            int minutos = segundosTotales / 60;
            int segundos = segundosTotales % 60;
            textoTemporizador.text = $"Tiempo: {minutos:00}:{segundos:00}";
        }

        if (textoCobroEstimado != null)
        {
            textoCobroEstimado.text = $"Cobro: {ObtenerMontoCobro()}";
        }
    }
}