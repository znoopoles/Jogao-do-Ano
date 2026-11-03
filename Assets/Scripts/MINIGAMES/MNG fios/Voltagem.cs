using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Voltagem : MonoBehaviour
{
    [SerializeField] private TMP_Text textoVoltagem;
    [SerializeField] private Transform raizConexoes;
    [SerializeField] private int voltagemIdeal;

    private FioUI[] fios;
    private ConectorUI[] conectores;
    private int[,] valoresEmVolts;

    public int VoltagemIdeal => voltagemIdeal;
    public long VoltagemAtual { get; private set; }

    private void Awake()
    {
        if (textoVoltagem == null)
        {
            textoVoltagem = GetComponent<TMP_Text>();
        }

        if (textoVoltagem == null)
        {
            Debug.LogError("O componente Voltagem precisa de um texto TextMeshPro.", this);
        }

        if (raizConexoes == null)
        {
            raizConexoes = transform.parent;
        }
    }

    private void Start()
    {
        GerarNovaPartida();
    }

    public void GerarNovaPartida()
    {
        valoresEmVolts = null;
        VoltagemAtual = 0;

        if (textoVoltagem == null)
        {
            Debug.LogError("Não é possível exibir a voltagem sem um texto TextMeshPro.", this);
            return;
        }

        textoVoltagem.text = "0 V";

        if (raizConexoes == null)
        {
            Debug.LogError("Defina a raiz dos fios e conectores no Inspector.", this);
            return;
        }

        fios = raizConexoes.GetComponentsInChildren<FioUI>(true);
        conectores = raizConexoes.GetComponentsInChildren<ConectorUI>(true);

        if (voltagemIdeal <= 0)
        {
            Debug.LogWarning("Defina a voltagem ideal desta caixa no Inspector.", this);
            return;
        }

        if (fios.Length < conectores.Length || conectores.Length == 0)
        {
            Debug.LogError(
                "A caixa precisa ter pelo menos um fio para cada conector.",
                this);
            return;
        }

        if (voltagemIdeal < conectores.Length)
        {
            Debug.LogError(
                "A voltagem ideal precisa ser pelo menos igual ao número de conectores.",
                this);
            return;
        }

        int valorMaximo = conectores.Length == 1
            ? voltagemIdeal
            : (voltagemIdeal - 1) / (conectores.Length - 1);
        if ((long)valorMaximo * conectores.Length < voltagemIdeal)
        {
            Debug.LogError(
                "Não foi possível gerar uma combinação que alcance a voltagem ideal.",
                this);
            return;
        }

        valoresEmVolts = new int[fios.Length, conectores.Length];
        bool[,] conexoesDaSolucao = new bool[fios.Length, conectores.Length];
        FioUI[] fiosSolucao = (FioUI[])fios.Clone();
        ConectorUI[] conectoresSolucao = (ConectorUI[])conectores.Clone();
        Embaralhar(fiosSolucao);
        Embaralhar(conectoresSolucao);

        int restante = voltagemIdeal;
        for (int i = 0; i < conectores.Length; i++)
        {
            int posicoesRestantes = conectores.Length - i - 1;
            int valorMinimo = Mathf.Max(
                1,
                restante - posicoesRestantes * valorMaximo);
            int valorMaximoSolucao = Mathf.Min(
                valorMaximo,
                restante - posicoesRestantes);
            int valorSolucao = i == conectores.Length - 1
                ? restante
                : Random.Range(valorMinimo, valorMaximoSolucao + 1);
            int indiceFio = System.Array.IndexOf(fios, fiosSolucao[i]);
            int indiceConector = System.Array.IndexOf(conectores, conectoresSolucao[i]);

            valoresEmVolts[indiceFio, indiceConector] = valorSolucao;
            conexoesDaSolucao[indiceFio, indiceConector] = true;
            restante -= valorSolucao;
        }

        int quantidadeValoresAleatorios =
            fios.Length * conectores.Length - conectores.Length;
        List<int> valoresAleatorios = new List<int>(quantidadeValoresAleatorios);
        for (int i = 0; i < quantidadeValoresAleatorios; i++)
        {
            valoresAleatorios.Add(1 + i % valorMaximo);
        }

        Embaralhar(valoresAleatorios);
        int indiceValorAleatorio = 0;
        for (int i = 0; i < fios.Length; i++)
        {
            for (int j = 0; j < conectores.Length; j++)
            {
                if (!conexoesDaSolucao[i, j])
                {
                    valoresEmVolts[i, j] = valoresAleatorios[indiceValorAleatorio];
                    indiceValorAleatorio++;
                }
            }
        }

        Recalcular();
    }

    public static void RecalcularTodas()
    {
        Voltagem[] sistemas = FindObjectsOfType<Voltagem>(true);
        foreach (Voltagem sistema in sistemas)
        {
            sistema.Recalcular();
        }
    }

    public void Recalcular()
    {
        if (textoVoltagem == null || valoresEmVolts == null)
        {
            return;
        }

        long totalEmVolts = 0;
        for (int i = 0; i < fios.Length; i++)
        {
            ConectorUI conectorAtual = fios[i].conectorAtual;
            if (conectorAtual == null || conectorAtual.fioAtual != fios[i])
            {
                continue;
            }

            int indiceConector = System.Array.IndexOf(conectores, conectorAtual);
            if (indiceConector >= 0)
            {
                totalEmVolts += valoresEmVolts[i, indiceConector];
            }
        }

        VoltagemAtual = totalEmVolts;
        textoVoltagem.text = $"{VoltagemAtual} V";
    }

    private static void Embaralhar<T>(T[] lista)
    {
        for (int i = lista.Length - 1; i > 0; i--)
        {
            int indiceAleatorio = Random.Range(0, i + 1);
            T temporario = lista[i];
            lista[i] = lista[indiceAleatorio];
            lista[indiceAleatorio] = temporario;
        }
    }

    private static void Embaralhar<T>(List<T> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int indiceAleatorio = Random.Range(0, i + 1);
            T temporario = lista[i];
            lista[i] = lista[indiceAleatorio];
            lista[indiceAleatorio] = temporario;
        }
    }
}
