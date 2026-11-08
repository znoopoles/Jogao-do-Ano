
using UnityEngine;
using UnityEngine.UI;

// Controla o preenchimento da barra quando o jogador clica no ícone.
public class cliquesrápidos : MonoBehaviour
{
    public event System.Action OnIconClicked;

    [Header("Referências")]
    [SerializeField] private Image iconeClique;
    [SerializeField] private Image barraVermelha;

    [Header("Configurações do clique")]
    [SerializeField, Min(0f)] private float raioAreaClique = 100f;

    [Header("Configurações da barra")]
    [SerializeField, Min(0f)] private float aumentoPorClique = 0.08f;
    [SerializeField, Range(0.01f, 1f)] private float multiplicadorMinimoDeAumento = 0.1f;
    [SerializeField, Min(0.01f)] private float tempoParaEsvaziar = 12f;

    private void Start()
    {
        if (iconeClique == null)
            Debug.LogError("Atribua a Image do iconeClique no Inspector.", this);

        if (barraVermelha == null)
            Debug.LogError("Atribua a Image da barraVermelha no Inspector.", this);
    }

    private void Update()
    {
        if (iconeClique == null || barraVermelha == null)
            return;

        Camera cameraCanvas = iconeClique.canvas != null
            && iconeClique.canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? iconeClique.canvas.worldCamera
                : null;

        bool clicouNoIcone = Input.GetMouseButtonDown(0)
            && CliqueDentroDoCirculo(cameraCanvas);

        if (clicouNoIcone)
        {
            OnIconClicked?.Invoke();

            float aumentoAtual = aumentoPorClique * Mathf.Lerp(
                1f,
                multiplicadorMinimoDeAumento,
                barraVermelha.fillAmount);

            barraVermelha.fillAmount = Mathf.Clamp01(
                barraVermelha.fillAmount + aumentoAtual);
        }
        else
        {
            barraVermelha.fillAmount = Mathf.Clamp01(
                barraVermelha.fillAmount - Time.deltaTime / tempoParaEsvaziar);
        }
    }

    private bool CliqueDentroDoCirculo(Camera cameraCanvas)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            iconeClique.rectTransform,
            Input.mousePosition,
            cameraCanvas,
            out Vector2 pontoLocal);

        return pontoLocal.sqrMagnitude <= raioAreaClique * raioAreaClique;
    }

    public void ReiniciarBarra()
    {
        if (barraVermelha == null)
        {
            Debug.LogError("Atribua a Image da barraVermelha no Inspector.", this);
            return;
        }

        barraVermelha.fillAmount = 0f;
    }
}
