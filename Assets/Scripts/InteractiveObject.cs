using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Color corNormal;
    private Color corDestaque;

    public UnityEngine.UI.Image imagem;
    public InteractionLabel legenda;
    public string textoLegenda;

    private void Start()
    {
        corNormal = imagem.color;
        corDestaque = corNormal * 3.5f;
    }

    private void OnPointerEnter (PointerEventData eventData)
    {
        Debug.Log("Mouse entrou em " + gameObject.name);
        imagem.color = corDestaque; // destaca o objeto com a cor vermelha
        legenda.Mostrar(textoLegenda); // mostra a legenda
    }

    private void OnPointerExit (PointerEventData eventData)
    {
        Debug.Log("Mouse saiu de " + gameObject.name);
        imagem.color = corNormal; // o objeto volta a cor normal
        legenda.Esconder(); // esconde a legenda
    }
}