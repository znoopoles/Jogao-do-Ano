using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MNG : MonoBehaviour
{
    public enum Subject
    {
        Colegas,
        Janela,
        Panfleto,
        Marionette,
        Bolsa,
        Ave
    }

    private enum View
    {
        Environment,
        Hesitation,
        Computer,
        BirdFocus,
        Finished
    }

    private const int DesignWidth = 1920;
    private const int DesignHeight = 1080;
    private const int FlyerClicksToComplete = 3;
    private const int ChoicesToUnlockBird = 2;
    private const float TypingCharactersPerSecond = 48f;

    private readonly List<Subject> completedChoices = new List<Subject>();
    private readonly List<string> writtenTextParts = new List<string>();

    private View currentView = View.Environment;
    private int flyerClicks;
    private float typingStartedAt;
    private string hesitationText;
    private GUIStyle titleStyle;
    private GUIStyle paperTitleStyle;
    private GUIStyle subtitleStyle;
    private GUIStyle bodyStyle;
    private GUIStyle buttonStyle;
    private GUIStyle smallStyle;

    public IReadOnlyList<Subject> CompletedChoices
    {
        get { return completedChoices; }
    }

    public IReadOnlyList<string> WrittenTextParts
    {
        get { return writtenTextParts; }
    }

    public int FlyerClickCount
    {
        get { return flyerClicks; }
    }

    public bool IsFinished
    {
        get { return currentView == View.Finished; }
    }

    private void OnGUI()
    {
        EnsureStyles();

        float scale = Mathf.Min(Screen.width / (float)DesignWidth, Screen.height / (float)DesignHeight);
        float offsetX = (Screen.width - DesignWidth * scale) * 0.5f;
        float offsetY = (Screen.height - DesignHeight * scale) * 0.5f;
        Matrix4x4 previousMatrix = GUI.matrix;
        Color previousColor = GUI.color;
        Color previousBackgroundColor = GUI.backgroundColor;
        GUI.matrix = Matrix4x4.TRS(new Vector3(offsetX, offsetY, 0f), Quaternion.identity, Vector3.one * scale);

        switch (currentView)
        {
            case View.Environment:
                DrawEnvironment();
                break;
            case View.Hesitation:
                DrawHesitation();
                break;
            case View.Computer:
                DrawComputer(false);
                break;
            case View.BirdFocus:
                DrawBirdFocus();
                break;
            case View.Finished:
                DrawComputer(true);
                break;
        }

        GUI.matrix = previousMatrix;
        GUI.color = previousColor;
        GUI.backgroundColor = previousBackgroundColor;
    }

    private void EnsureStyles()
    {
        if (titleStyle != null)
        {
            return;
        }

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 48,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = new Color(0.95f, 0.91f, 0.81f) }
        };
        paperTitleStyle = new GUIStyle(titleStyle);
        paperTitleStyle.normal.textColor = new Color(0.13f, 0.16f, 0.15f);
        subtitleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 25,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = new Color(0.85f, 0.83f, 0.77f) }
        };
        bodyStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 31,
            wordWrap = true,
            alignment = TextAnchor.UpperLeft,
            padding = new RectOffset(28, 28, 24, 24),
            normal = { textColor = new Color(0.13f, 0.16f, 0.15f) }
        };
        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 31,
            fontStyle = FontStyle.Bold,
            wordWrap = true,
            alignment = TextAnchor.MiddleCenter,
            normal = { background = Texture2D.whiteTexture, textColor = new Color(0.12f, 0.14f, 0.13f) },
            hover = { background = Texture2D.whiteTexture, textColor = new Color(0.05f, 0.07f, 0.06f) },
            active = { background = Texture2D.whiteTexture, textColor = Color.black }
        };
        smallStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 21,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = new Color(0.83f, 0.81f, 0.75f) }
        };
    }

    private void DrawEnvironment()
    {
        DrawBackdrop(new Color(0.18f, 0.21f, 0.22f));
        DrawColor(new Rect(0, 0, DesignWidth, 150), new Color(0.11f, 0.14f, 0.15f));
        GUI.Label(new Rect(100, 28, DesignWidth - 200, 70), "UM CENÁRIO PARA ESCREVER", titleStyle);
        GUI.Label(new Rect(100, 96, DesignWidth - 200, 44),
            "Escolha um objeto para inspirar o texto.", subtitleStyle);

        DrawColor(new Rect(0, 875, DesignWidth, 205), new Color(0.12f, 0.15f, 0.15f));
        GUI.Label(new Rect(140, 890, DesignWidth - 280, 44),
            "Escolhas concluídas: " + completedChoices.Count + " / " + ChoicesToUnlockBird, subtitleStyle);
        GUI.Label(new Rect(140, 936, DesignWidth - 280, 40),
            "O panfleto precisa de " + FlyerClicksToComplete + " interações para virar inspiração.", smallStyle);

        DrawSubjectCard(Subject.Colegas, "COLEGAS", "C", new Rect(190, 230, 440, 250),
            new Color(0.83f, 0.67f, 0.47f));
        DrawSubjectCard(Subject.Janela, "JANELA", "J", new Rect(740, 230, 440, 250),
            new Color(0.52f, 0.72f, 0.76f));
        DrawSubjectCard(Subject.Panfleto, "PANFLETO", "P", new Rect(1290, 230, 440, 250),
            new Color(0.83f, 0.78f, 0.58f));
        DrawSubjectCard(Subject.Marionette, "MARIONETTE", "M", new Rect(465, 555, 440, 250),
            new Color(0.69f, 0.56f, 0.68f));
        DrawSubjectCard(Subject.Bolsa, "BOLSA", "B", new Rect(1015, 555, 440, 250),
            new Color(0.68f, 0.62f, 0.48f));
    }

    private void DrawSubjectCard(Subject subject, string label, string symbol, Rect rect, Color color)
    {
        bool isCompleted = completedChoices.Contains(subject);
        string cardText = isCompleted
            ? symbol + "\n\n" + label + "\nCONCLUÍDO"
            : symbol + "\n\n" + label + (subject == Subject.Panfleto
                ? "\n" + flyerClicks + " / " + FlyerClicksToComplete
                : string.Empty);

        if (isCompleted)
        {
            DrawColor(rect, new Color(0.32f, 0.35f, 0.34f));
            GUI.Label(rect, cardText, buttonStyle);
            return;
        }

        GUI.backgroundColor = color;
        if (GUI.Button(rect, cardText, buttonStyle))
        {
            SelectSubject(subject);
        }
        GUI.backgroundColor = Color.white;
    }

    private void DrawHesitation()
    {
        DrawBackdrop(new Color(0.13f, 0.15f, 0.16f));
        DrawColor(new Rect(315, 265, 1290, 550), new Color(0.88f, 0.84f, 0.75f));
        GUI.Label(new Rect(385, 310, 1150, 90), "UMA IDEIA, UMA DÚVIDA", paperTitleStyle);
        GUI.Label(new Rect(440, 430, 1040, 190), hesitationText, bodyStyle);

        if (GUI.Button(new Rect(645, 720, 630, 90), "Voltar ao cenário", buttonStyle))
        {
            currentView = View.Environment;
        }
    }

    private void DrawComputer(bool finished)
    {
        DrawBackdrop(new Color(0.10f, 0.12f, 0.13f));
        DrawColor(new Rect(150, 100, 1620, 800), new Color(0.30f, 0.33f, 0.32f));
        DrawColor(new Rect(190, 140, 1540, 720), new Color(0.87f, 0.85f, 0.77f));
        GUI.Label(new Rect(260, 165, 1400, 70), "CADERNO DIGITAL", paperTitleStyle);
        GUI.Label(new Rect(280, 250, 1360, 560), GetWrittenText(), bodyStyle);

        if (finished)
        {
            string status = IsCurrentTextWritten() ? "FIM DO MINIGAME" : "ESCREVENDO...";
            GUI.Label(new Rect(300, 915, 1320, 70), status, subtitleStyle);
            return;
        }

        if (!IsCurrentTextWritten())
        {
            GUI.Label(new Rect(300, 915, 1320, 70), "ESCREVENDO...", subtitleStyle);
            return;
        }

        if (GUI.Button(new Rect(645, 930, 630, 90), "Continuar", buttonStyle))
        {
            currentView = BirdUnlocked ? View.BirdFocus : View.Environment;
        }
    }

    private void DrawBirdFocus()
    {
        DrawBackdrop(new Color(0.38f, 0.58f, 0.64f));
        DrawColor(new Rect(0, 0, DesignWidth, 190), new Color(0.24f, 0.42f, 0.47f));
        GUI.Label(new Rect(100, 40, DesignWidth - 200, 90), "UM ÚLTIMO ENCONTRO", titleStyle);
        DrawColor(new Rect(0, 760, DesignWidth, 320), new Color(0.22f, 0.31f, 0.25f));
        GUI.Label(new Rect(350, 225, 1220, 100),
            "O cenário fica em silêncio. Uma ave pousa à sua frente.", subtitleStyle);

        GUI.backgroundColor = new Color(0.92f, 0.84f, 0.62f);
        if (GUI.Button(new Rect(610, 385, 700, 310), "AVE\n\nClique para escrever o último trecho", buttonStyle))
        {
            SelectSubject(Subject.Ave);
        }
        GUI.backgroundColor = Color.white;
    }

    private void SelectSubject(Subject subject)
    {
        if (subject == Subject.Ave)
        {
            if (currentView == View.BirdFocus && BirdUnlocked)
            {
                CompleteChoice(Subject.Ave);
            }

            return;
        }

        if (currentView != View.Environment || BirdUnlocked || completedChoices.Contains(subject))
        {
            return;
        }

        if (subject == Subject.Panfleto)
        {
            flyerClicks++;
            if (flyerClicks < FlyerClicksToComplete)
            {
                hesitationText = flyerClicks == 1
                    ? "Será que devo mesmo usar o panfleto como inspiração? Ainda não tenho certeza."
                    : "Continuo hesitando. Talvez exista outra coisa neste cenário que eu queira escrever.";
                currentView = View.Hesitation;
                return;
            }
        }

        CompleteChoice(subject);
    }

    private void CompleteChoice(Subject subject)
    {
        string paragraph = subject == Subject.Ave
            ? BuildBirdParagraph()
            : BuildParagraph(subject);

        completedChoices.Add(subject);
        writtenTextParts.Add(paragraph);
        typingStartedAt = Time.unscaledTime;
        currentView = subject == Subject.Ave ? View.Finished : View.Computer;
    }

    private string BuildParagraph(Subject subject)
    {
        string paragraph;
        switch (subject)
        {
            case Subject.Colegas:
                paragraph = "As vozes dos colegas atravessam a sala como ecos de um dia que ainda não terminou.";
                break;
            case Subject.Janela:
                paragraph = "Pela janela, a luz desenha caminhos que eu ainda não tive coragem de seguir.";
                break;
            case Subject.Panfleto:
                paragraph = "No panfleto, palavras impressas prometem um futuro simples; nas minhas mãos, elas ganham outra voz.";
                break;
            case Subject.Marionette:
                paragraph = "A marionete sorri suspensa por fios, e me pergunto quem escolhe para onde ela vai.";
                break;
            case Subject.Bolsa:
                paragraph = "Dentro da bolsa, pequenos objetos guardam lembranças que ninguém mais consegue ver.";
                break;
            default:
                return string.Empty;
        }

        if (completedChoices.Count > 0)
        {
            paragraph += " " + GetMemoryOfPreviousChoice(completedChoices[completedChoices.Count - 1]);
        }

        return paragraph;
    }

    private string GetMemoryOfPreviousChoice(Subject previousChoice)
    {
        switch (previousChoice)
        {
            case Subject.Colegas:
                return "Ainda levo comigo o rumor das conversas.";
            case Subject.Janela:
                return "A claridade da janela acompanha esse pensamento.";
            case Subject.Panfleto:
                return "As palavras do panfleto insistem em permanecer comigo.";
            case Subject.Marionette:
                return "Os fios da marionete parecem ligar uma lembrança à outra.";
            case Subject.Bolsa:
                return "A lembrança guardada na bolsa também cabe nestas palavras.";
            default:
                return string.Empty;
        }
    }

    private string BuildBirdParagraph()
    {
        Subject previousChoice = completedChoices[completedChoices.Count - 1];
        string ending;
        switch (previousChoice)
        {
            case Subject.Colegas:
                ending = " A ave responde às vozes que ficaram para trás.";
                break;
            case Subject.Janela:
                ending = " A ave encontra no céu o caminho que eu via pela janela.";
                break;
            case Subject.Panfleto:
                ending = " A ave leva consigo as palavras que enfim escolhi escrever.";
                break;
            case Subject.Marionette:
                ending = " A ave abre as asas e, por um instante, nenhum fio a prende.";
                break;
            case Subject.Bolsa:
                ending = " A ave parte levando uma lembrança que já não preciso guardar.";
                break;
            default:
                ending = " A ave parte, e o silêncio finalmente parece completo.";
                break;
        }

        return "A ave pousa por um instante, como se também tivesse algo a dizer." + ending;
    }

    private string GetWrittenText()
    {
        StringBuilder text = new StringBuilder();
        for (int i = 0; i < writtenTextParts.Count; i++)
        {
            if (i > 0)
            {
                text.AppendLine();
                text.AppendLine();
            }

            string paragraph = writtenTextParts[i];
            if (i == writtenTextParts.Count - 1
                && (currentView == View.Computer || currentView == View.Finished))
            {
                int visibleCharacters = Mathf.Min(
                    paragraph.Length,
                    Mathf.FloorToInt((Time.unscaledTime - typingStartedAt) * TypingCharactersPerSecond));
                text.Append(paragraph.Substring(0, visibleCharacters));
            }
            else
            {
                text.Append(paragraph);
            }
        }

        return text.ToString();
    }

    private bool IsCurrentTextWritten()
    {
        if (writtenTextParts.Count == 0)
        {
            return true;
        }

        float typingDuration = writtenTextParts[writtenTextParts.Count - 1].Length / TypingCharactersPerSecond;
        return Time.unscaledTime - typingStartedAt >= typingDuration;
    }

    private bool BirdUnlocked
    {
        get { return completedChoices.Count >= ChoicesToUnlockBird; }
    }

    private static void DrawBackdrop(Color color)
    {
        DrawColor(new Rect(0, 0, DesignWidth, DesignHeight), color);
    }

    private static void DrawColor(Rect rect, Color color)
    {
        Color previousColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = previousColor;
    }
}
