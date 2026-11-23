using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text textoResumen;

    [Header("Escenas")]
    [SerializeField] private string nombreEscenaJuego = "SampleScene";
    [SerializeField] private string nombreEscenaMenu = "MenuPrincipal";

    private void Start()
    {
        Time.timeScale = 1f;
        string jugador = PlayerPrefs.GetString("NombreJugador", "Invitado");
        int dia = PlayerPrefs.GetInt("DiaAlcanzado", 1);
        int puntos = PlayerPrefs.GetInt("PuntosFinales", 0);
        int cobro = PlayerPrefs.GetInt("CobroPendiente", 0);

        if (textoResumen != null)
        {
            textoResumen.text = $"Jugador: {jugador}\nNo alcanzó para pagar el cobro de {cobro} en el Día {dia}.\nPuntos finales: {puntos}";
        }
    }

    public void ReintentarPartida()
    {
        PlayerPrefs.DeleteKey("TiempoRestanteDia");
        PlayerPrefs.SetInt("PuntosJugador", 0);
        PlayerPrefs.SetInt("CantidadHornosJugador", 0);
        PlayerPrefs.SetInt("CostoHornoJugador", 100);
        PlayerPrefs.SetInt("DiaActual", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}