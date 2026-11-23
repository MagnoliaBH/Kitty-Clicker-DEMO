using UnityEngine;
using UnityEngine.SceneManagement;

public class PausaController : MonoBehaviour
{
    [SerializeField] private string nombreEscenaJuego = "SampleScene";
    [SerializeField] private string nombreEscenaMenu = "MenuPrincipal";

    public void ReanudarJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}