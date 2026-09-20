using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionLabel : MonoBehaviour
{
    public TextMeshProUGUI texto;
    private RectTransform rectTransform;

    public void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Mostrar(string mensagem)
    {
        texto.text = mensagem;
        gameObject.SetActive(true);
    }

    public void Esconder()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {

        Vector2 mouse= Input.mousePosition;
        float largura = rectTransform.rect.width;
        float altura = rectTransform.rect.height;

        float x = mouse.x + 10f;
        float y = mouse.y - 20f;

        //impede a legenda de atravessar a borda direita
        if (x + largura > Screen.width)
        {
            x = mouse.x - largura - 10f;
        }

        //impede a legenda de atravessar a borda de baixo
        if (y - altura < 0)
        {
            y = mouse.y + altura + 20f;
        }

        rectTransform.position = new Vector2(x, y);
    }
}
