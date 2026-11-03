using System.Collections;
using UnityEngine;

public class ResolucaoCaixa : MonoBehaviour
{
    [SerializeField] private TemporizadorGlobal temporizadorGlobal;
    [SerializeField] private ControleConexoesUI controleConexoes;
    [SerializeField] private TrocarPainel painelRetorno;
    [SerializeField] private TrocarPainel caixaNoCenario;
    [SerializeField] private GerenciadorPartidaFios gerenciadorPartida;
    [SerializeField, Min(0f)] private float atrasoFechamentoSegundos = 1.5f;

    private Coroutine fechamentoAutomatico;
    private bool resolvida;
    private bool transicaoPendente;

    private void Awake()
    {
        if (painelRetorno == null)
        {
            Debug.LogError(
                "Atribua o TrocarPainel do botão Voltar no Inspector.",
                this);
            enabled = false;
            return;
        }

        painelRetorno.AntesDeTrocar += AoTrocarPainel;
    }

    private void OnDestroy()
    {
        if (painelRetorno != null)
        {
            painelRetorno.AntesDeTrocar -= AoTrocarPainel;
        }
    }

    public bool IniciarResolucao()
    {
        if (resolvida || !enabled)
            return false;

        if (temporizadorGlobal == null ||
            controleConexoes == null ||
            painelRetorno == null ||
            caixaNoCenario == null ||
            gerenciadorPartida == null ||
            float.IsNaN(atrasoFechamentoSegundos) ||
            float.IsInfinity(atrasoFechamentoSegundos) ||
            atrasoFechamentoSegundos < 0f)
        {
            Debug.LogError(
                "Configure todas as referências e um atraso válido no Inspector.",
                this);
            return false;
        }

        resolvida = true;
        transicaoPendente = true;

        caixaNoCenario.enabled = false;
        temporizadorGlobal.Pausar();
        controleConexoes.BloquearInteracao();

        fechamentoAutomatico = StartCoroutine(AguardarFechamento());

        return true;
    }

    private IEnumerator AguardarFechamento()
    {
        yield return new WaitForSecondsRealtime(atrasoFechamentoSegundos);

        fechamentoAutomatico = null;

        if (!transicaoPendente)
            yield break;

        painelRetorno.Trocar();

        // A troca chama AoTrocarPainel pelo evento AntesDeTrocar.
    }

    private void AoTrocarPainel()
    {
        if (!transicaoPendente)
            return;

        if (fechamentoAutomatico != null)
        {
            StopCoroutine(fechamentoAutomatico);
            fechamentoAutomatico = null;
        }

        FinalizarTransicao();
    }

    private void FinalizarTransicao()
    {
        if (!transicaoPendente)
            return;

        transicaoPendente = false;

        controleConexoes.LiberarInteracao();

        bool venceu = gerenciadorPartida.RegistrarCaixaResolvida();

        if (!venceu)
        {
            temporizadorGlobal.Retomar();
        }
    }
}
