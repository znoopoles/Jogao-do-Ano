using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Detecta quando o cursor entra ou sai do objeto e aplica um destaque visual e uma legenda.
public class InteractiveObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Componentes visuais opcionais do objeto, atribuídos no Inspector ou encontrados no mesmo objeto.
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Tooltip("Sprite exibido enquanto o mouse estiver sobre o objeto.")]
    [SerializeField] private Sprite spriteDestaque;
    [SerializeField] private Image imagem;

    // Legenda compartilhada e texto que será exibido para este objeto.
    [Tooltip("Componente que exibe a legenda ao passar o mouse.")]
    [SerializeField] private InteractionLabel legenda;
    [Tooltip("Texto exibido ao passar o mouse sobre este objeto.")]
    [SerializeField, TextArea] private string textoLegenda;

    // Guarda a aparência original e a cor usada para destacar cada tipo de componente visual.
    private Sprite spriteNormalRenderer;
    private Sprite spriteNormalImagem;
    private Color corNormalRenderer;
    private Color corNormalImagem;
    private Color corDestaqueRenderer;
    private Color corDestaqueImagem;
    private bool mouseSobre;

    // Inicializa referências e guarda a aparência original antes de qualquer destaque.
    private void Awake()
    {
        // Usa o SpriteRenderer deste objeto se nenhum foi atribuído no Inspector.
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Usa o Image deste objeto se nenhum foi atribuído no Inspector.
        if (imagem == null)
        {
            imagem = GetComponent<Image>();
        }

        // Guarda o sprite e a cor originais; a cor de destaque é calculada multiplicando o brilho.
        if (spriteRenderer != null)
        {
            spriteNormalRenderer = spriteRenderer.sprite;
            corNormalRenderer = spriteRenderer.color;
            corDestaqueRenderer = corNormalRenderer * 3.5f;
        }

        // Faz o mesmo para o componente Image, usado normalmente em elementos de UI.
        if (imagem != null)
        {
            spriteNormalImagem = imagem.sprite;
            corNormalImagem = imagem.color;
            corDestaqueImagem = corNormalImagem * 3.5f;
        }

        // Informa no Console se a referência à legenda estiver faltando.
        if (legenda == null)
        {
            Debug.LogError("Atribua uma legenda ao componente InteractiveObject.", this);
        }

        // O objeto precisa de pelo menos um componente visual compatível.
        if (spriteRenderer == null && imagem == null)
        {
            Debug.LogError("InteractiveObject precisa de um SpriteRenderer ou Image.", this);
        }
    }

    // Eventos do EventSystem usados principalmente por elementos de UI.
    public void OnPointerEnter(PointerEventData eventData)
    {
        DefinirMouseSobre(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DefinirMouseSobre(false);
    }

    // Eventos de mouse usados principalmente por objetos com Collider na cena.
    private void OnMouseEnter()
    {
        DefinirMouseSobre(true);
    }

    private void OnMouseExit()
    {
        DefinirMouseSobre(false);
    }

    // Limpa o destaque e a legenda se o painel desativar este objeto enquanto o cursor está sobre ele.
    private void OnDisable()
    {
        if (mouseSobre)
        {
            DefinirMouseSobre(false);
        }
    }

    // Aplica ou remove o destaque e mostra ou esconde a legenda conforme o estado do cursor.
    private void DefinirMouseSobre(bool sobre)
    {
        // Evita repetir atualizações se o estado já for o solicitado.
        if (mouseSobre == sobre)
        {
            return;
        }

        mouseSobre = sobre;

        // Se houver SpriteRenderer, troca o sprite de destaque ou ajusta a cor.
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sobre && spriteDestaque != null ? spriteDestaque : spriteNormalRenderer;
            spriteRenderer.color = sobre && spriteDestaque == null ? corDestaqueRenderer : corNormalRenderer;
        }

        // Aplica a mesma lógica a um Image de UI.
        if (imagem != null)
        {
            imagem.sprite = sobre && spriteDestaque != null ? spriteDestaque : spriteNormalImagem;
            imagem.color = sobre && spriteDestaque == null ? corDestaqueImagem : corNormalImagem;
        }

        // Mostra a legenda quando o cursor entra e esconde quando sai.
        if (legenda != null)
        {
            if (sobre)
            {
                legenda.Mostrar(textoLegenda);
            }
            else
            {
                legenda.Esconder();
            }
        }
    }
}
