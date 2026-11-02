using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Garante que o objeto do fio tenha os componentes necessários para exibição e posicionamento.
[RequireComponent(typeof(Image), typeof(RectTransform))]
public class FioUI : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    // Controlador que recebe os cliques neste fio e imagem usada para desenhar sua linha.
    public ControleConexoesUI controle;
    public Image linhaLigacao;

    // Conector ao qual este fio está ligado; não precisa ser preenchido manualmente no Inspector.
    [HideInInspector]
    public ConectorUI conectorAtual;

    // Cores aplicadas quando o cursor passa sobre o fio ou quando ele é selecionado.
    [Header("Cores")]
    public Color corDestaque = Color.grey;
    public Color corSelecionado = Color.white;

    // Referências e valores usados para restaurar a aparência e atualizar a linha.
    private Image imagem;
    private Color corOriginal;
    private RectTransform rectTransform;
    private RectTransform rectTransformLinha;
    private Vector3 pontoInicialLinha;

    // Estado atual da seleção e da posição do cursor sobre o fio.
    private bool selecionado = false;
    private bool mouseSobre = false;

    // Obtém os componentes, registra a extremidade original do fio e prepara sua linha.
    private void Awake()
    {
        imagem = GetComponent<Image>();
        corOriginal = imagem.color;
        rectTransform = GetComponent<RectTransform>();

        // Guarda o centro da extremidade esquerda, onde a linha permanecerá conectada.
        Vector3[] cantosFio = new Vector3[4];
        rectTransform.GetWorldCorners(cantosFio);
        pontoInicialLinha = (cantosFio[0] + cantosFio[1]) / 2f;

        // Se a linha não foi atribuída no Inspector, tenta encontrá-la pelo nome.
        if (linhaLigacao == null)
        {
            linhaLigacao = EncontrarLinha();
        }

        if (linhaLigacao == null)
        {
            Debug.LogError(
                $"Não foi encontrada a linha correspondente ao fio '{name}'.",
                this);
            return;
        }

        // Guarda o RectTransform da linha, impede que ela capture cliques e começa oculta.
        rectTransformLinha = linhaLigacao.GetComponent<RectTransform>();
        linhaLigacao.raycastTarget = false;
        linhaLigacao.gameObject.SetActive(false);
    }

    // Informa ao controlador que este fio foi clicado para ser selecionado.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (controle != null)
        {
            controle.SelecionarFio(this);
        }
    }

    // Ativa a aparência de foco quando o cursor entra no fio.
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseSobre = true;
        AtualizarVisual();
    }

    // Remove a aparência de foco quando o cursor sai do fio.
    public void OnPointerExit(PointerEventData eventData)
    {
        mouseSobre = false;
        AtualizarVisual();
    }

    // Altera o estado de seleção e atualiza a cor exibida.
    public void Destacar(bool estado)
    {
        selecionado = estado;
        AtualizarVisual();
    }

    // Escolhe a cor conforme a prioridade: selecionado, sob o cursor ou aparência original.
    private void AtualizarVisual()
    {
        if (selecionado)
        {
            imagem.color = corSelecionado;
        }
        else if (mouseSobre)
        {
            imagem.color = corDestaque;
        }
        else
        {
            imagem.color = corOriginal;
        }
    }

    // Move o fio até o conector, alinhando as extremidades que devem se tocar.
    public void MoverPara(ConectorUI conector)
    {
        if (conector == null)
        {
            Debug.LogError("Não é possível mover o fio para um conector nulo.", this);
            return;
        }

        RectTransform rectTransformConector = conector.GetComponent<RectTransform>();
        if (rectTransformConector == null)
        {
            Debug.LogError("O conector precisa ter um RectTransform.", conector);
            return;
        }

        // Atualiza o layout antes de calcular as extremidades na tela.
        Canvas.ForceUpdateCanvases();

        // Calcula os cantos dos dois elementos em coordenadas globais da tela.
        Vector3[] cantosFio = new Vector3[4];
        Vector3[] cantosConector = new Vector3[4];
        rectTransform.GetWorldCorners(cantosFio);
        rectTransformConector.GetWorldCorners(cantosConector);

        // Alinha a extremidade direita do fio à extremidade esquerda do conector.
        Vector3 extremidadeDireitaFio = (cantosFio[2] + cantosFio[3]) / 2f;
        Vector3 extremidadeEsquerdaConector = (cantosConector[0] + cantosConector[1]) / 2f;
        transform.position += extremidadeEsquerdaConector - extremidadeDireitaFio;
    }

    // Mostra e dimensiona a linha entre a posição inicial do fio e sua posição atual.
    public void AtualizarLinhaVisual()
    {
        if (linhaLigacao == null || rectTransformLinha == null)
        {
            return;
        }

        if (conectorAtual == null)
        {
            linhaLigacao.gameObject.SetActive(false);
            return;
        }

        linhaLigacao.gameObject.SetActive(true);
        linhaLigacao.enabled = true;

        // Obtém a extremidade atual do fio para traçar a linha até ela.
        Vector3[] cantosFio = new Vector3[4];
        rectTransform.GetWorldCorners(cantosFio);

        // A linha vai do ponto original do fio até sua extremidade esquerda atual.
        Vector3 extremidadeEsquerdaFio = (cantosFio[0] + cantosFio[1]) / 2f;
        Transform paiLinha = rectTransformLinha.parent;

        // Converte os pontos para o espaço local do objeto que contém a linha.
        Vector3 inicio = paiLinha != null
            ? paiLinha.InverseTransformPoint(pontoInicialLinha)
            : pontoInicialLinha;
        Vector3 fim = paiLinha != null
            ? paiLinha.InverseTransformPoint(extremidadeEsquerdaFio)
            : extremidadeEsquerdaFio;
        Vector3 direcao = fim - inicio;

        // Posiciona a linha no meio do trajeto, ajusta seu comprimento e sua rotação.
        rectTransformLinha.localPosition = (inicio + fim) / 2f;
        rectTransformLinha.sizeDelta = new Vector2(
            direcao.magnitude,
            rectTransformLinha.sizeDelta.y);
        rectTransformLinha.localRotation = Quaternion.Euler(
            0f,
            0f,
            Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg);
    }

    // Atualiza a linha depois do layout da UI, mantendo-a alinhada ao fio.
    private void LateUpdate()
    {
        // Mantém a linha acompanhando o fio e eventuais mudanças no layout da UI.
        if (conectorAtual != null)
        {
            AtualizarLinhaVisual();
        }
    }

    // Procura a imagem da linha correspondente, por exemplo, LinhaAzul para FioAzul.
    private Image EncontrarLinha()
    {
        if (!name.StartsWith("Fio") || name.Length <= 3)
        {
            return null;
        }

        string nomeLinha = "Linha" + name.Substring(3);

        // Procura primeiro no ancestral mais próximo, para encontrar a linha deste painel.
        for (Transform ancestral = transform.parent; ancestral != null; ancestral = ancestral.parent)
        {
            // Inclui objetos inativos, pois a linha começa desativada.
            Image[] imagens = ancestral.GetComponentsInChildren<Image>(true);
            foreach (Image imagemEncontrada in imagens)
            {
                if (imagemEncontrada.name == nomeLinha)
                {
                    return imagemEncontrada;
                }
            }
        }

        return null;
    }
}
