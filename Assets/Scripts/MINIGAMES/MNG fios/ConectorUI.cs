using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ConectorUI : MonoBehaviour, IPointerClickHandler
{
    public ControleConexoesUI controle;

    [SerializeField] private Color corConexaoCorreta = Color.green;

    [HideInInspector]
    public FioUI fioAtual;

    private Image imagem;
    private Color corOriginal;

    private void Awake()
    {
        imagem = GetComponent<Image>();
        corOriginal = imagem.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (controle != null)
        {
            controle.SelecionarConector(this);
        }
    }

    public void AtualizarEstado(bool conexaoCorreta)
    {
        if (imagem == null)
        {
            imagem = GetComponent<Image>();
            if (imagem == null)
            {
                Debug.LogError("O conector precisa de um componente Image.", this);
                return;
            }

            corOriginal = imagem.color;
        }

        imagem.color = conexaoCorreta ? corConexaoCorreta : corOriginal;
    }
}
