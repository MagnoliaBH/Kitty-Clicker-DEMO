using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class ContadorClicker : MonoBehaviour
{


    private const string ClaveDiaActual = "DiaActual";
    private const string ClavePuntos = "PuntosJugador";
    private const string ClaveCantidadHornos = "CantidadHornosJugador";
    private const string ClaveCostoHorno = "CostoHornoJugador";

    [Header("Marcador, Animación y Sonido")]
    [SerializeField] private TMP_Text textoPuntos;
    [SerializeField] private Animator animatorGato;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoPop;
    [SerializeField] private AudioClip sonidoCompra;

    [Header("Galletas al Clicar")]
    [SerializeField] private GameObject prefabMiniGalleta;
    [SerializeField] private Transform puntoAparicionGalleta;
    [SerializeField] private Sprite[] spritesGalletas;
    [SerializeField] private float distanciaSubida = 45f;
    [SerializeField] private float duracionMiniGalleta = 0.35f;

    [Header("Mejora: Horno")]
    [SerializeField] private int puntos = 0;
    [SerializeField] private int costoHorno = 100;
    [SerializeField] private int incrementoCosto = 20;
    [SerializeField] private int galletasPorHorno = 3;
    [SerializeField] private float intervaloSegundos = 0.1f;
    [SerializeField] private int cantidadHornos = 0;

    [Header("UI del Horno y Pop-up")]
    [SerializeField] private Button botonComprarHorno;
    [SerializeField] private TMP_Text textoCostoHorno;
    [SerializeField] private TMP_Text textoCantidadHornos;
    [SerializeField] private GameObject visualHornoEnEscena;
    [SerializeField] private TMP_Text textoPopUpHorno;
    [SerializeField] private float duracionPopUp = 0.45f;

    [Header("Jugador")]
    [SerializeField] private TMP_Text textoNombreJugador;

    [Header("Escenas")]
    [SerializeField] private string nombreEscenaPausa = "Pausa";
    [SerializeField] private ControlDiaJuego controlDiaJuego;

    private const string ClaveNombreJugador = "NombreJugador";

    private int indiceGalletaActual = 0;
    private Coroutine rutinaPopUp;

    private void Start()
    {
        Time.timeScale = 1f;
        CargarProgreso();
        
        StartCoroutine(GenerarGalletasPorHornos());
        ActualizarUI();
        if (textoNombreJugador != null)
        {
            string nombre = PlayerPrefs.GetString(ClaveNombreJugador, "");
            textoNombreJugador.text = string.IsNullOrWhiteSpace(nombre)
                ? "Jugador: Invitado"
                : $"Jugador: {nombre}";
        }
    }

    public void SumarPunto()
    {
        puntos += 1;

        if (audioSource != null && sonidoPop != null)
        {
            audioSource.PlayOneShot(sonidoPop);
        }

        if (animatorGato != null)
        {
            animatorGato.SetTrigger("ClicGato");
        }

        SpawnMiniGalleta();
        ActualizarUI();
    }

    private void SpawnMiniGalleta()
    {
        if (prefabMiniGalleta == null || puntoAparicionGalleta == null)
        {
            return;
        }

        GameObject instancia = Instantiate(prefabMiniGalleta, puntoAparicionGalleta.parent);
        instancia.transform.position = puntoAparicionGalleta.position;

        Image imagenGalleta = instancia.GetComponent<Image>();
        if (imagenGalleta != null && spritesGalletas != null && spritesGalletas.Length > 0)
        {
            imagenGalleta.sprite = spritesGalletas[indiceGalletaActual];
            indiceGalletaActual = (indiceGalletaActual + 1) % spritesGalletas.Length;
        }

        StartCoroutine(AnimarYDestruirMiniGalleta(instancia, imagenGalleta));
    }

    private IEnumerator AnimarYDestruirMiniGalleta(GameObject instancia, Image imagenGalleta)
    {
        RectTransform rect = instancia.GetComponent<RectTransform>();
        Vector3 inicio = rect.anchoredPosition;
        Vector3 fin = inicio + Vector3.up * distanciaSubida;

        Color colorInicial = imagenGalleta != null ? imagenGalleta.color : Color.white;
        float tiempo = 0f;

        while (tiempo < duracionMiniGalleta)
        {
            tiempo += Time.deltaTime;
            float progreso = Mathf.Clamp01(tiempo / duracionMiniGalleta);

            rect.anchoredPosition = Vector3.Lerp(inicio, fin, progreso);

            if (imagenGalleta != null)
            {
                Color c = colorInicial;
                c.a = Mathf.Lerp(colorInicial.a, 0f, progreso);
                imagenGalleta.color = c;
            }

            yield return null;
        }

        Destroy(instancia);
    }

    public void ComprarHorno()
    {
        if (puntos < costoHorno)
        {
            return;
        }

        puntos -= costoHorno;
        cantidadHornos += 1;
        costoHorno += incrementoCosto;

        if (audioSource != null && sonidoCompra != null)
        {
            audioSource.PlayOneShot(sonidoCompra);
        }

        if (visualHornoEnEscena != null && !visualHornoEnEscena.activeSelf)
        {
            visualHornoEnEscena.SetActive(true);
        }

        ActualizarUI();
    }

    public void GuardarProgreso()
    {
        PlayerPrefs.SetInt(ClavePuntos, puntos);
        PlayerPrefs.SetInt(ClaveCantidadHornos, cantidadHornos);
        PlayerPrefs.SetInt(ClaveCostoHorno, costoHorno);
        PlayerPrefs.Save();
    }

    private void CargarProgreso()
    {
        puntos = PlayerPrefs.GetInt(ClavePuntos, 0);
        cantidadHornos = PlayerPrefs.GetInt(ClaveCantidadHornos, 0);
        costoHorno = PlayerPrefs.GetInt(ClaveCostoHorno, 100);

        if (cantidadHornos > 0 && visualHornoEnEscena != null)
        {
            visualHornoEnEscena.SetActive(true);
        }
    }

    public bool IntentarDescontarPuntos(int cantidad)
    {
        if (puntos < cantidad)
        {
            return false;
        }

        puntos -= cantidad;
        GuardarProgreso();
        ActualizarUI();
        return true;
    }

    public void AbrirMenuPausa()
    {
        GuardarProgreso();

        ControlDiaJuego controlDia = FindFirstObjectByType<ControlDiaJuego>();
        if (controlDia != null)
        {
            controlDia.GuardarTiempoAntesDePausar();
        }

        SceneManager.LoadScene("Pausa"); 
    }
    private IEnumerator GenerarGalletasPorHornos()
    {
        WaitForSeconds espera = new WaitForSeconds(intervaloSegundos);

        while (true)
        {
            yield return espera;

            if (cantidadHornos > 0)
            {
                int ganancia = cantidadHornos * galletasPorHorno;
                puntos += ganancia;
                ActualizarUI();

                if (textoPopUpHorno != null)
                {
                    if (rutinaPopUp != null)
                    {
                        StopCoroutine(rutinaPopUp);
                    }

                    rutinaPopUp = StartCoroutine(MostrarPopUpHorno(ganancia));
                }
            }
        }
    }

    private IEnumerator MostrarPopUpHorno(int ganancia)
    {
        textoPopUpHorno.text = $"+{ganancia}";
        textoPopUpHorno.gameObject.SetActive(true);

        yield return new WaitForSeconds(duracionPopUp);

        textoPopUpHorno.gameObject.SetActive(false);
        rutinaPopUp = null;
    }

    private void ActualizarUI()
    {
        if (textoPuntos != null)
        {
            textoPuntos.text = puntos.ToString();
        }

        if (textoCostoHorno != null)
        {
            textoCostoHorno.text = $"Horno: {costoHorno} pts";
        }

        if (textoCantidadHornos != null)
        {
            textoCantidadHornos.text = $"x{cantidadHornos}";
        }

        if (botonComprarHorno != null)
        {
            botonComprarHorno.interactable = puntos >= costoHorno;
        }
    }
}