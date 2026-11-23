using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuPrincipalController : MonoBehaviour
{
    [Header("Escena del Juego")]
    [SerializeField] private string nombreEscenaJuego = "SampleScene";

    [Header("Configuración de Reinicio")]
    [SerializeField] private int cobroBaseDia1 = 150;

    [Header("Nombre del Jugador (Opcional)")]
    [SerializeField] private TMP_InputField inputNombreJugador;
    [SerializeField] private TMP_Text textoSaludoJugador;

    private const string ClaveNombreJugador = "NombreJugador";

    private void Start()
    {

        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.fullScreen = true;
        Time.timeScale = 1f;
        string nombreGuardado = PlayerPrefs.GetString(ClaveNombreJugador, "");

        if (inputNombreJugador != null)
        {
            inputNombreJugador.text = nombreGuardado;
        }

        ActualizarTextoJugador(nombreGuardado);


    }

    public void GuardarNombreJugador(string nuevoNombre)
    {
        string nombreLimpio = (nuevoNombre ?? "").Trim();
        PlayerPrefs.SetString(ClaveNombreJugador, nombreLimpio);
        PlayerPrefs.Save();
        ActualizarTextoJugador(nombreLimpio);
    }

    private void ActualizarTextoJugador(string nombre)
    {
        if (textoSaludoJugador != null)
        {
            textoSaludoJugador.text = string.IsNullOrWhiteSpace(nombre)
                ? "Jugador"
                : $"Jugador: {nombre}";
        }
    }

    public void IniciarJuego()
    {
        if (inputNombreJugador != null && !string.IsNullOrWhiteSpace(inputNombreJugador.text))
        {
            GuardarNombreJugador(inputNombreJugador.text);
        }

        // Reiniciar economía, hornos y días al Día 1 antes de cambiar de escena
        PlayerPrefs.DeleteKey("TiempoRestanteDia");
        PlayerPrefs.SetInt("PuntosJugador", 0);
        PlayerPrefs.SetInt("CantidadHornosJugador", 0);
        PlayerPrefs.SetInt("CostoHornoJugador", 100);
        PlayerPrefs.SetInt("DiaActual", 1);
        PlayerPrefs.SetInt("MontoCobroActual", cobroBaseDia1);

        // Limpiar datos de derrotas anteriores
        PlayerPrefs.DeleteKey("DiaAlcanzado");
        PlayerPrefs.DeleteKey("PuntosFinales");
        PlayerPrefs.DeleteKey("CobroPendiente");

        PlayerPrefs.Save();
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void CerrarJuego()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}