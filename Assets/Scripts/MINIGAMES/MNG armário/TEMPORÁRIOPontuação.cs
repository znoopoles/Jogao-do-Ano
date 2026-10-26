using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TEMPORÁRIOPontuação : MonoBehaviour
{
    // Referências da interface e do controlador configuradas na cena.
    [SerializeField] private TextMeshProUGUI textoPontuacao;
    [SerializeField] private TextMeshProUGUI textoStatusBarra;
    [SerializeField] private cliquesrápidos controladorCliques;
    [SerializeField] private Image barraVermelha;
    [SerializeField] private GameObject painelMNG;
    [SerializeField] private GameObject painelFimDeJogo;

    private const float LIMIAR_AVISO = 0.7f;
    private const float LIMIAR_PERIGO = 0.9f;

    // O fim de jogo só pode acontecer depois do primeiro clique.
    private bool primeiroCliqueRealizado;
    private bool fimDeJogo;

    private void Start()
    {
        // Impede a inicialização se alguma referência obrigatória estiver faltando.
        bool referenciasValidas = true;

        if (textoPontuacao == null)
        {
            Debug.LogError("Atribua o TextMeshPro da pontuação no Inspector.", this);
            referenciasValidas = false;
        }

        if (textoStatusBarra == null)
        {
            Debug.LogWarning("Atribua o TextMeshPro do status da barra no Inspector para mostrar alertas do limite.", this);
        }

        if (controladorCliques == null)
        {
            Debug.LogError("Atribua o controlador de cliques no Inspector.", this);
            referenciasValidas = false;
        }

        if (barraVermelha == null)
        {
            Debug.LogError("Atribua a Image da barraVermelha no Inspector.", this);
            referenciasValidas = false;
        }

        if (painelMNG == null)
        {
            Debug.LogError("Atribua o Painel MNG no Inspector.", this);
            referenciasValidas = false;
        }

        if (painelFimDeJogo == null)
        {
            Debug.LogError("Atribua o painel Fim de Jogo no Inspector.", this);
            referenciasValidas = false;
        }

        if (!referenciasValidas)
        {
            return;
        }

        // Escuta os cliques, as mudanças de pontuação e prepara a interface no início da partida.
        controladorCliques.OnIconClicked += RegistrarClique;
        ScoreManager.OnScoreChanged += AtualizarTextoPontuacao;
        AtualizarTextoPontuacao(ScoreManager.Score);
        AtualizarTextoStatusBarra();
        painelMNG.SetActive(true);
        painelFimDeJogo.SetActive(false);
    }

    private void Update()
    {
        if (barraVermelha == null)
        {
            return;
        }

        AtualizarTextoStatusBarra();

        // Aguarda o primeiro clique e o esvaziamento completo da barra.
        if (!primeiroCliqueRealizado || fimDeJogo || barraVermelha.fillAmount > 0f)
        {
            return;
        }

        // Encerra a partida apenas uma vez.
        fimDeJogo = true;
        painelMNG.SetActive(false);
        painelFimDeJogo.SetActive(true);
    }

    private void RegistrarClique()
    {
        if (fimDeJogo)
        {
            return;
        }

        // Cada clique válido soma um ponto e atualiza o texto.
        primeiroCliqueRealizado = true;
        ScoreManager.AddPoints(1);
    }

    private void AtualizarTextoPontuacao(int score)
    {
        if (textoPontuacao == null)
        {
            return;
        }

        textoPontuacao.text = $"Pontos: {score}";
    }

    private void AtualizarTextoStatusBarra()
    {
        if (textoStatusBarra == null || barraVermelha == null)
        {
            return;
        }

        if (fimDeJogo)
        {
            textoStatusBarra.text = "Partida encerrada";
            textoStatusBarra.color = new Color(1f, 0.85f, 0.2f);
            return;
        }

        float percentual = barraVermelha.fillAmount * 100f;

        if (percentual >= 100f)
        {
            textoStatusBarra.text = "Limite da barra!";
            textoStatusBarra.color = new Color(1f, 0.25f, 0.25f);
        }
        else if (percentual >= LIMIAR_PERIGO * 100f)
        {
            textoStatusBarra.text = "Quase no limite!";
            textoStatusBarra.color = new Color(1f, 0.7f, 0.2f);
        }
        else if (percentual >= LIMIAR_AVISO * 100f)
        {
            textoStatusBarra.text = "A barra está subindo";
            textoStatusBarra.color = new Color(0.95f, 0.95f, 0.5f);
        }
        else if (!primeiroCliqueRealizado)
        {
            textoStatusBarra.text = "Prepare-se";
            textoStatusBarra.color = new Color(0.8f, 0.8f, 0.8f);
        }
        else
        {
            textoStatusBarra.text = "Mantenha o ritmo";
            textoStatusBarra.color = new Color(0.8f, 0.9f, 1f);
        }
    }

    public void Reiniciar()
    {
        // Valida as referências antes de restaurar o estado da partida.
        if (textoPontuacao == null || controladorCliques == null || painelMNG == null || painelFimDeJogo == null)
        {
            Debug.LogError("Configure todas as referências do minigame antes de reiniciar.", this);
            return;
        }

        // Zera pontuação, barra e estado, depois volta ao painel do minigame.
        ScoreManager.Reset();
        controladorCliques.ReiniciarBarra();
        primeiroCliqueRealizado = false;
        fimDeJogo = false;
        AtualizarTextoPontuacao(ScoreManager.Score);
        AtualizarTextoStatusBarra();
        painelFimDeJogo.SetActive(false);
        painelMNG.SetActive(true);
    }

    private void OnDestroy()
    {
        // Remove a inscrição para evitar manter uma referência após destruir este objeto.
        if (controladorCliques != null)
        {
            controladorCliques.OnIconClicked -= RegistrarClique;
        }

        ScoreManager.OnScoreChanged -= AtualizarTextoPontuacao;
    }
}
