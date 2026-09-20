using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    private Color corNormal;
    private Color corDestaque;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        corNormal = spriteRenderer.color;
    }

    private void OnMouseEnter ()
    {
        Debug.Log("Mouse entrou em " + gameObject.name);
        spriteRenderer.color = Color.red; // destaca o objeto com a cor vermelha
    }

    private void OnMouseExit ()
    {
        Debug.Log("Mouse saiu de " + gameObject.name);
        spriteRenderer.color = corNormal; // o objeto volta a cor normal
    }
}