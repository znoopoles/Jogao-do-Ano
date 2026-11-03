
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AlavancaTeste : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Voltagem voltagem;
    [SerializeField] private ControleConexoesUI controle;
    [SerializeField] private ResolucaoCaixa resolucaoCaixa;
    [SerializeField] private GerenciadorPartidaFios gerenciadorPartida;

    private const int MAX_TENTATIVAS_FALHAS = 2;
    private int tentativasFalhas;

    private void Awake()
    {
        if (voltagem == null)
            Debug.LogError("Atribua a voltagem da caixa no Inspector.", this);

        if (controle == null)
            Debug.LogError("Atribua o controlador dos fios no Inspector.", this);

        if (resolucaoCaixa == null)
            Debug.LogError("Atribua o sistema de resolução desta caixa no Inspector.", this);

        if (gerenciadorPartida == null)
            Debug.LogError("Atribua o GerenciadorPartidaFios no Inspector.", this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (controle != null)
            controle.CancelarSelecao();

        if (voltagem == null || resolucaoCaixa == null || gerenciadorPartida == null)
        {
            Debug.LogError("Confira as referências da alavanca no Inspector.", this);
            return;
        }

        if (voltagem.VoltagemAtual == voltagem.VoltagemIdeal)
        {
            if (!resolucaoCaixa.IniciarResolucao())
                return;

            Debug.Log("Caixa resolvida.", this);
        }
        else
        {
            tentativasFalhas++;

            Debug.Log($"Tentativa incorreta: {tentativasFalhas}/{MAX_TENTATIVAS_FALHAS}.", this);

            if (tentativasFalhas >= MAX_TENTATIVAS_FALHAS)
                gerenciadorPartida.Derrotar();
        }
    }
}
