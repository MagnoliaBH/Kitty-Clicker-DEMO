using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CobradorEscenaController : MonoBehaviour
{
    [Header("UI de Cobro")]
    [SerializeField] private TMP_Text textoDetalleCobro;
    [SerializeField] private TMP_Text textoPuntosActuales;

    [Header("Escenas")]
    [SerializeField] private string nombreEscenaJuego = "SampleScene";
    [SerializeField] private string nombreEscenaGameOver = "GameOver";

    private int puntosActuales;
    private int montoCobro;
    private int diaActual;

    private void Start()
    {
        Time.timeScale = 1f;
        puntosActuales = PlayerPrefs.GetInt("PuntosJugador", 0);
        montoCobro = PlayerPrefs.GetInt("MontoCobroActual", 150);
        diaActual = PlayerPrefs.GetInt("DiaActual", 1);

        if (textoDetalleCobro != null)
        {
            textoDetalleCobro.text = $"¡Llegó el cobrador de servicios!\nDía {diaActual} — Total a pagar: {montoCobro} puntos";
        }

        if (textoPuntosActuales != null)
        {
            textoPuntosActuales.text = $"Tus puntos: {puntosActuales}";
        }
    }

    public void PagarServicios()
    {
        if (puntosActuales >= montoCobro)
        {
            int puntosRestantes = puntosActuales - montoCobro;
            int siguienteDia = diaActual + 1;

            PlayerPrefs.DeleteKey("TiempoRestanteDia");
            PlayerPrefs.SetInt("PuntosJugador", puntosRestantes);
            PlayerPrefs.SetInt("DiaActual", siguienteDia);
            PlayerPrefs.Save();

            // Al cargar la escena del juego, el contador de 2 minutos inicia desde 120s
            // y se conservan los puntos restantes
            SceneManager.LoadScene(nombreEscenaJuego);
        }
        else
        {
            PlayerPrefs.SetInt("DiaAlcanzado", diaActual);
            PlayerPrefs.SetInt("PuntosFinales", puntosActuales);
            PlayerPrefs.SetInt("CobroPendiente", montoCobro);
            PlayerPrefs.Save();

            SceneManager.LoadScene(nombreEscenaGameOver);
        }
    }
}