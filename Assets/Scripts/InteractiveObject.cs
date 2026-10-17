using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractiveObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Tooltip("Sprite exibido enquanto o mouse estiver sobre o objeto.")]
    [SerializeField] private Sprite spriteDestaque;
    [SerializeField] private Image imagem;
    [Tooltip("Componente que exibe a legenda ao passar o mouse.")]
    [SerializeField] private InteractionLabel legenda;
    [Tooltip("Texto exibido ao passar o mouse sobre este objeto.")]
    [SerializeField, TextArea] private string textoLegenda;

    private Sprite spriteNormalRenderer;
    private Sprite spriteNormalImagem;
    private Color corNormalRenderer;
    private Color corNormalImagem;
    private Color corDestaqueRenderer;
    private Color corDestaqueImagem;
    private bool mouseSobre;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (imagem == null)
        {
            imagem = GetComponent<Image>();
        }

        if (spriteRenderer != null)
        {
            spriteNormalRenderer = spriteRenderer.sprite;
            corNormalRenderer = spriteRenderer.color;
            corDestaqueRenderer = corNormalRenderer * 3.5f;
        }

        if (imagem != null)
        {
            spriteNormalImagem = imagem.sprite;
            corNormalImagem = imagem.color;
            corDestaqueImagem = corNormalImagem * 3.5f;
        }

        if (legenda == null)
        {
            Debug.LogError("Atribua uma legenda ao componente InteractiveObject.", this);
        }

        if (spriteRenderer == null && imagem == null)
        {
            Debug.LogError("InteractiveObject precisa de um SpriteRenderer ou Image.", this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DefinirMouseSobre(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DefinirMouseSobre(false);
    }

    private void OnMouseEnter()
    {
        DefinirMouseSobre(true);
    }

    private void OnMouseExit()
    {
        DefinirMouseSobre(false);
    }

    private void DefinirMouseSobre(bool sobre)
    {
        if (mouseSobre == sobre)
        {
            return;
        }

        mouseSobre = sobre;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sobre && spriteDestaque != null ? spriteDestaque : spriteNormalRenderer;
            spriteRenderer.color = sobre && spriteDestaque == null ? corDestaqueRenderer : corNormalRenderer;
        }

        if (imagem != null)
        {
            imagem.sprite = sobre && spriteDestaque != null ? spriteDestaque : spriteNormalImagem;
            imagem.color = sobre && spriteDestaque == null ? corDestaqueImagem : corNormalImagem;
        }

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
