
using UnityEngine;
using UnityEngine.EventSystems;

public class TrocarPainel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject painelDestino;
    [SerializeField] private GameObject painelAtual;
    [SerializeField] private bool desativarPainelAtual = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        Trocar();
    }

    private void OnMouseDown()
    {
        Trocar();
    }

    public void Trocar()
    {
        if (painelDestino == null)
        {
            Debug.LogError("Escolha um painel de destino no Inspector.");
            return;
        }

        if (desativarPainelAtual && painelAtual != null)
            painelAtual.SetActive(false);

        painelDestino.SetActive(true);
    }
}
