using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MNG_semaforo : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite redSprite;
    public Sprite greenSprite;

    [Tooltip("Se true, o semáforo começa verde.")]
    public bool isGreen = false;

    [Tooltip("Pontos ganhos ao transformar um vermelho em verde.")]
    public int pointsForCorrect = 1;

    [Tooltip("Pontos perdidos ao clicar num semáforo que já é verde.")]
    public int penaltyForWrong = 1;

    private void Reset()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // garante que a sprite atual reflita o estado
            spriteRenderer.sprite = isGreen && greenSprite != null ? greenSprite : redSprite;
        }
    }

    private void OnMouseDown()
    {
        if (spriteRenderer == null) return;

        if (!isGreen)
        {
            // estava vermelho: vira verde e adiciona pontos
            if (greenSprite != null) spriteRenderer.sprite = greenSprite;
            isGreen = true;
            ScoreManager.AddPoints(pointsForCorrect);
        }
        else
        {
            // já estava verde: penaliza
            ScoreManager.AddPoints(-penaltyForWrong);
        }
    }
}
