using TMPro;
using UnityEngine;

public class TemporizadorGlobal : MonoBehaviour
{
    [Header("Interface")]
    [SerializeField] private TMP_Text textoTempo;
    [SerializeField] private GameObject painelInicio;

    [Header("Partida")]
    [SerializeField, Min(0f)] private float tempoInicialSegundos = 60f;
    [SerializeField] private GerenciadorPartidaFios gerenciadorPartida;

    private float tempoRestante;
    private bool iniciado;
    private bool contando;
    private bool encerrado;

    private void Awake()
    {
        if (textoTempo == null)
        {
            textoTempo = GetComponent<TMP_Text>();
        }

        if (textoTempo == null)
        {
            Debug.LogError("Atribua um texto TextMeshPro para o temporizador.", this);
            enabled = false;
            return;
        }

        if (painelInicio == null)
        {
            Debug.LogError("Atribua o painel que inicia o temporizador no Inspector.", this);
            enabled = false;
            return;
        }

        if (gerenciadorPartida == null)
        {
            Debug.LogError("Atribua o GerenciadorPartidaFios no Inspector.", this);
            enabled = false;
        }
    }

    private void Start()
    {
        if (float.IsNaN(tempoInicialSegundos) ||
            float.IsInfinity(tempoInicialSegundos) ||
            tempoInicialSegundos < 0f)
        {
            Debug.LogError("O tempo inicial deve ser um valor finito igual ou superior a zero.", this);
            enabled = false;
            return;
        }

        tempoRestante = tempoInicialSegundos;
        AtualizarTexto();

        if (painelInicio.activeInHierarchy)
            IniciarTemporizador();
    }

    private void Update()
    {
        if (!iniciado && painelInicio.activeInHierarchy)
            IniciarTemporizador();

        if (!contando)
            return;

        tempoRestante = Mathf.Max(0f, tempoRestante - Time.deltaTime);
        AtualizarTexto();

        if (tempoRestante <= 0f)
        {
            EncerrarTemporizador();
        }
    }

    public void Pausar()
    {
        if (encerrado || tempoRestante <= 0f)
            return;

        contando = false;
    }

    public void Retomar()
    {
        if (!iniciado || encerrado || tempoRestante <= 0f)
            return;

        contando = true;
    }

    private void IniciarTemporizador()
    {
        if (iniciado || encerrado)
            return;

        iniciado = true;

        if (tempoRestante <= 0f)
        {
            EncerrarTemporizador();
            return;
        }

        contando = true;
    }

    private void AtualizarTexto()
    {
        int segundosExibidos = Mathf.CeilToInt(tempoRestante);
        int minutos = segundosExibidos / 60;
        int segundos = segundosExibidos % 60;

        textoTempo.text = $"{minutos:00}:{segundos:00}";
    }

    private void EncerrarTemporizador()
    {
        if (encerrado)
            return;

        contando = false;
        encerrado = true;

        gerenciadorPartida.DerrotarPorTempo();
    }
}
