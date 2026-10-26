using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrocadecenaMinigames : MonoBehaviour
{
    // Start is called before the first frame update
    public void TrocaCena(string nomeCena)
    {
        SceneManager.LoadScene(nomeCena);
    }
}
