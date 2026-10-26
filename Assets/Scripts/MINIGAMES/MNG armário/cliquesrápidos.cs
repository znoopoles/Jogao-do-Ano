using UnityEngine;
using UnityEngine.UI;

// Controla o preenchimento da barra quando o jogador clica no ícone.
public class cliquesrápidos : MonoBehaviour
{
    // Referências às imagens configuradas no Inspector do Unity.
    [SerializeField] private Image iconeClique;
    [SerializeField] private Image barraVermelha;

    // Quantidade máxima adicionada por clique quando a barra está vazia.
    [SerializeField, Min(0f)] private float aumentoPorClique = 0.08f;

    // Parte do aumentoPorClique que ainda será aplicada quando a barra estiver cheia.
    [SerializeField, Range(0.01f, 1f)] private float multiplicadorMinimoDeAumento = 0.1f;

    // Tempo, em segundos, para a barra ir de cheia a vazia sem novos cliques.
    [SerializeField, Min(0.01f)] private float tempoParaEsvaziar = 12f;

    private void Start()
    {
        // Avisa no Console se a referência do ícone não foi configurada.
        if (iconeClique == null)
        {
            Debug.LogError("Atribua a Image do iconeClique no Inspector.", this);
        }

        // Avisa no Console se a referência da barra não foi configurada.
        if (barraVermelha == null)
        {
            Debug.LogError("Atribua a Image da barraVermelha no Inspector.", this);
        }
    }

    private void Update()
    {
        // Não executa a lógica enquanto alguma imagem obrigatória estiver faltando.
        if (iconeClique == null || barraVermelha == null)
        {
            return;
        }

        // Usa a câmera do Canvas, exceto quando o Canvas está sobreposto à tela.
        Camera cameraCanvas = iconeClique.canvas != null
            && iconeClique.canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? iconeClique.canvas.worldCamera
                : null;

        // Confere se o botão esquerdo foi pressionado dentro da área do ícone.
        bool clicouNoIcone = Input.GetMouseButtonDown(0)
            && RectTransformUtility.RectangleContainsScreenPoint(
                iconeClique.rectTransform,
                Input.mousePosition,
                cameraCanvas);

        if (clicouNoIcone)
        {
            // Reduz o aumento do clique conforme a barra se aproxima de 100%.
            float aumentoAtual = aumentoPorClique * Mathf.Lerp(
                1f,
                multiplicadorMinimoDeAumento,
                barraVermelha.fillAmount);

            // Adiciona o aumento sem deixar o preenchimento passar de 100%.
            barraVermelha.fillAmount = Mathf.Clamp01(
                barraVermelha.fillAmount + aumentoAtual);
        }
        else
        {
            // Esvazia a barra no tempo definido, sem ficar abaixo de zero.
            barraVermelha.fillAmount = Mathf.Clamp01(
                barraVermelha.fillAmount - Time.deltaTime / tempoParaEsvaziar);
        }
    }
}
