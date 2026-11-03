using UnityEngine;

public class ControleConexoesUI : MonoBehaviour
{
    private FioUI fioSelecionado;
    private bool interacaoBloqueada;

    public void BloquearInteracao()
    {
        interacaoBloqueada = true;
        CancelarSelecao();
    }

    public void LiberarInteracao()
    {
        interacaoBloqueada = false;
    }

    public void CancelarSelecao(FioUI fio)
    {
        if (fioSelecionado != fio)
        {
            return;
        }

        CancelarSelecao();
    }

    public void CancelarSelecao()
    {
        if (fioSelecionado == null)
        {
            return;
        }

        fioSelecionado.Destacar(false);
        fioSelecionado = null;
    }

    public void SelecionarFio(FioUI fio)
    {
        if (interacaoBloqueada)
        {
            return;
        }

        if (fio == null)
        {
            Debug.LogError("Não é possível selecionar um fio nulo.", this);
            return;
        }

        // Dois fios já conectados também podem trocar de posição ao serem clicados em sequência.
        if (fioSelecionado != null &&
            fioSelecionado != fio &&
            fioSelecionado.conectorAtual != null &&
            fio.conectorAtual != null)
        {
            TrocarFios(fioSelecionado, fio);
            return;
        }

        if (fioSelecionado == fio)
        {
            if (fio.conectorAtual != null)
            {
                DesconectarFio(fio);
            }

            return;
        }

        // Ao selecionar primeiro um fio solto e depois um conectado,
        // o conectado volta ao início e o solto ocupa o conector liberado.
        if (fioSelecionado != null &&
            fioSelecionado.conectorAtual == null &&
            fio.conectorAtual != null)
        {
            SubstituirFioConectado(fioSelecionado, fio);
            return;
        }

        // Se já existe um fio selecionado, remove o destaque dele.
        if (fioSelecionado != null)
        {
            fioSelecionado.Destacar(false);
        }

        // Seleciona o novo fio.
        fioSelecionado = fio;
        fioSelecionado.Destacar(true);

        Debug.Log("Fio selecionado: " + fio.name);
    }

    private void DesconectarFio(FioUI fio)
    {
        ConectorUI conector = fio.conectorAtual;
        if (conector == null || conector.fioAtual != fio)
        {
            Debug.LogError(
                $"Não foi possível desconectar '{fio.name}': a conexão está inconsistente.",
                this);
            fio.Destacar(false);
            fioSelecionado = null;
            return;
        }

        conector.fioAtual = null;
        fio.conectorAtual = null;
        fio.RestaurarPosicaoInicial();
        fio.AtualizarLinhaVisual();
        fio.Destacar(false);
        fioSelecionado = null;

        Debug.Log($"Fio '{fio.name}' desconectado e restaurado à posição inicial.");
        Voltagem.RecalcularTodas();
    }

    public void SelecionarConector(ConectorUI conector)
    {
        if (interacaoBloqueada)
        {
            return;
        }

        if (conector == null)
        {
            Debug.LogError("Não é possível selecionar um conector nulo.", this);
            return;
        }

        // Não permite conectar sem selecionar um fio.
        if (fioSelecionado == null)
        {
            Debug.Log("Selecione um fio primeiro!");
            return;
        }

        // Evita fazer alguma operação se o fio já estiver nesse conector.
        if (fioSelecionado.conectorAtual == conector)
        {
            fioSelecionado.Destacar(false);
            fioSelecionado = null;
            return;
        }

        ConectorUI conectorAnterior = fioSelecionado.conectorAtual;
        FioUI fioNoConector = conector.fioAtual;

        // Confirma que a conexão anterior do fio selecionado está consistente.
        if (conectorAnterior != null && conectorAnterior.fioAtual != fioSelecionado)
        {
            Debug.LogError(
                $"A conexão entre o fio '{fioSelecionado.name}' e o conector '{conectorAnterior.name}' está inconsistente.",
                this);
            fioSelecionado.Destacar(false);
            fioSelecionado = null;
            return;
        }

        if (fioNoConector != null && fioNoConector.conectorAtual != conector)
        {
            Debug.LogError(
                $"A conexão do conector '{conector.name}' está inconsistente.",
                this);
            fioSelecionado.Destacar(false);
            fioSelecionado = null;
            return;
        }

        // Um fio ainda solto não pode substituir e desalojar um fio já conectado.
        if (conectorAnterior == null && fioNoConector != null)
        {
            Debug.Log("Esse conector já está ocupado. Selecione um fio conectado para trocá-lo.");
            return;
        }

        if (fioNoConector != null)
        {
            TrocarFios(fioSelecionado, fioNoConector);
            return;
        }

        FioUI fioMovido = fioSelecionado;

        // Libera o conector anterior quando o fio for movido para um conector vazio.
        if (conectorAnterior != null)
        {
            conectorAnterior.fioAtual = null;
        }

        conector.fioAtual = fioMovido;
        fioMovido.conectorAtual = conector;

        // Move cada fio para a posição do conector que agora lhe pertence.
        fioMovido.MoverPara(conector);
        // Atualiza a linha e encerra a seleção do fio.
        fioMovido.AtualizarLinhaVisual();
        fioMovido.Destacar(false);
        Debug.Log($"Fio '{fioMovido.name}' conectado ao conector '{conector.name}'.");
        fioSelecionado = null;
        Voltagem.RecalcularTodas();
    }

    private void TrocarFios(FioUI primeiro, FioUI segundo)
    {
        ConectorUI conectorPrimeiro = primeiro.conectorAtual;
        ConectorUI conectorSegundo = segundo.conectorAtual;

        if (conectorPrimeiro == null || conectorSegundo == null)
        {
            Debug.LogError("A troca exige que os dois fios já estejam conectados.", this);
            return;
        }

        if (conectorPrimeiro.fioAtual != primeiro || conectorSegundo.fioAtual != segundo)
        {
            Debug.LogError("Não foi possível trocar os fios: as conexões estão inconsistentes.", this);
            primeiro.Destacar(false);
            fioSelecionado = null;
            return;
        }

        // Troca as referências entre fios e conectores.
        conectorPrimeiro.fioAtual = segundo;
        conectorSegundo.fioAtual = primeiro;
        primeiro.conectorAtual = conectorSegundo;
        segundo.conectorAtual = conectorPrimeiro;

        // Move os fios e atualiza as linhas correspondentes.
        primeiro.MoverPara(conectorSegundo);
        segundo.MoverPara(conectorPrimeiro);
        primeiro.AtualizarLinhaVisual();
        segundo.AtualizarLinhaVisual();

        primeiro.Destacar(false);
        segundo.Destacar(false);
        fioSelecionado = null;

        Debug.Log($"Fios '{primeiro.name}' e '{segundo.name}' trocaram de conector.");
        Voltagem.RecalcularTodas();
    }

    private void SubstituirFioConectado(FioUI fioSolto, FioUI fioConectado)
    {
        ConectorUI conector = fioConectado.conectorAtual;
        if (conector == null || conector.fioAtual != fioConectado)
        {
            Debug.LogError(
                $"Não foi possível substituir '{fioConectado.name}': a conexão está inconsistente.",
                this);
            fioSolto.Destacar(false);
            fioSelecionado = null;
            return;
        }

        // O fio solto assume o conector; o fio substituído volta à sua posição de origem.
        conector.fioAtual = fioSolto;
        fioSolto.conectorAtual = conector;
        fioConectado.conectorAtual = null;

        fioConectado.RestaurarPosicaoInicial();
        fioSolto.MoverPara(conector);

        // Oculta a linha do fio que voltou e ativa a linha do fio que foi conectado.
        fioConectado.AtualizarLinhaVisual();
        fioSolto.AtualizarLinhaVisual();

        fioSolto.Destacar(false);
        fioConectado.Destacar(false);
        fioSelecionado = null;

        Debug.Log(
            $"Fio '{fioSolto.name}' substituiu '{fioConectado.name}' no conector '{conector.name}'.");
        Voltagem.RecalcularTodas();
    }
}
