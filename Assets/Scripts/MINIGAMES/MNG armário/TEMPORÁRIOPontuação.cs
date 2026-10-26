using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TEMPORÁRIOPontuação : MonoBehaviour
{
    // Referências da interface e do controlador configuradas na cena.
    [SerializeField] private TextMeshProUGUI textoPontuacao;
    [SerializeField] private cliquesrápidos controladorCliques;
    [SerializeField] private Image barraVermelha;
    [SerializeField] private GameObject painelMNG;
    [SerializeField] private GameObject painelFimDeJogo;

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

        // Escuta os cliques e prepara a interface no início da partida.
        controladorCliques.OnIconClicked += RegistrarClique;
        AtualizarTextoPontuacao();
        painelMNG.SetActive(true);
        painelFimDeJogo.SetActive(false);
    }

    private void Update()
    {
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
        AtualizarTextoPontuacao();
    }

    private void AtualizarTextoPontuacao()
    {
        textoPontuacao.text = $"score = {ScoreManager.Score}";
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
        AtualizarTextoPontuacao();
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
    }
}
