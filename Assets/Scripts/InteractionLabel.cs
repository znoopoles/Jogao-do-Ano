using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionLabel : MonoBehaviour
{
    // Referência ao componente TextMeshProUGUI que exibirá a mensagem da legenda.
    public TextMeshProUGUI texto;

    // Referência ao RectTransform usado para posicionar e medir a legenda na tela.
    private RectTransform rectTransform;

    // Executado quando o objeto é inicializado; obtém seu RectTransform.
    public void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Define o conteúdo da legenda e ativa o objeto para torná-lo visível.
    public void Mostrar(string mensagem)
    {
        texto.text = mensagem;
        gameObject.SetActive(true);
    }

    // Desativa o objeto para esconder a legenda.
    public void Esconder()
    {
        gameObject.SetActive(false);
    }

    // Executado a cada quadro para acompanhar o cursor do mouse.
    private void Update()
    {
        // Obtém a posição do cursor e as dimensões da legenda.
        Vector2 mouse= Input.mousePosition;
        float largura = rectTransform.rect.width;
        float altura = rectTransform.rect.height;

        // Começa posicionando a legenda um pouco à direita e abaixo do cursor.
        float x = mouse.x + 10f;
        float y = mouse.y - 20f;

        // Se a legenda ultrapassar a borda direita, coloca-a à esquerda do cursor.
//        if (x + largura > Screen.width)
//        {
//            x = mouse.x - largura - 10f;
//        }

        // Se a legenda ultrapassar a borda inferior, coloca-a acima do cursor.
//        if (y - altura < 0)
//        {
//            y = mouse.y + altura + 20f;
//        }

        // Aplica a posição calculada à legenda.
        rectTransform.position = new Vector2(x, y);
    }
}
