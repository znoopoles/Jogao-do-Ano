using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConectorUI : MonoBehaviour, IPointerClickHandler
{
    public ControleConexoesUI controle;

    [HideInInspector]
    public FioUI fioAtual;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (controle != null)
        {
            controle.SelecionarConector(this);
        }
    }
}

