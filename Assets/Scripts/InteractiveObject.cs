using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractiveObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Color corNormal;
    private Color corDestaque;

    public UnityEngine.UI.Image imagem;
    public InteractionLabel legenda;
    public string textoLegenda;

    private void Start()
    {
        if (imagem != null)
        {
        corNormal = imagem.color;
        corDestaque = corNormal * 3.5f;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse entrou em " + gameObject.name);
        if (imagem != null)
        {
            imagem.color = corDestaque; // destaca o objeto com a cor vermelha
        }
        if (legenda != null)
        {
            legenda.Mostrar(textoLegenda); // mostra a legenda
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse saiu de " + gameObject.name);
        if (imagem != null)
        {
            imagem.color = corNormal; // o objeto volta a cor normal
        }
        if (legenda != null)
        {
            legenda.Esconder(); // esconde a legenda
        }
    }
}