using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GerenciadorPartidaFios : MonoBehaviour
{
    [Header("Painéis")]
    [SerializeField] private GameObject painelHUD;
    [SerializeField] private GameObject painelFimDeJogo;

    [Header("Texto do resultado")]
    [SerializeField] private TMP_Text textoResultado;

    [Header("Mensagens")]
    [SerializeField] private string mensagemDerrota =
        "VOCÊ PERDEU! DUAS TENTATIVAS INCORRETAS.";

    [SerializeField] private string mensagemTempoEsgotado =
        "TEMPO ESGOTADO!";

    [SerializeField] private string mensagemVitoria =
        "VOCÊ VENCEU!";

    private int caixasResolvidas;
    private bool partidaEncerrada;

    private void Start()
    {
        if (painelHUD == null ||
            painelFimDeJogo == null ||
            textoResultado == null)
        {
            Debug.LogError(
                "Configure os painéis e o texto de resultado no Inspector.",
                this);
            enabled = false;
            return;
        }

        caixasResolvidas = 0;
        partidaEncerrada = false;

        painelHUD.SetActive(true);
        painelFimDeJogo.SetActive(false);
    }

    public bool RegistrarCaixaResolvida()
    {
        if (partidaEncerrada)
            return false;

        caixasResolvidas++;

        Debug.Log($"Caixas resolvidas: {caixasResolvidas}/4.");

        if (caixasResolvidas >= 4)
        {
            Vencer();
            return true;
        }

        return false;
    }

    public void Derrotar()
    {
        EncerrarPartida(mensagemDerrota);
    }

    public void DerrotarPorTempo()
    {
        EncerrarPartida(mensagemTempoEsgotado);
    }

    public void Vencer()
    {
        EncerrarPartida(mensagemVitoria);
    }

    private void EncerrarPartida(string mensagem)
    {
        if (partidaEncerrada)
            return;

        partidaEncerrada = true;

        textoResultado.text = mensagem;

        painelHUD.SetActive(false);
        painelFimDeJogo.SetActive(true);
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene("MinigameFios");
    }

    public void VoltarAoMenu()
    {
        SceneManager.LoadScene("TEMPORÁRIO - MenuMinigames");
    }
}
