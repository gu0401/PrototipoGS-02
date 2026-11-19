using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueLine
    {
        public string speaker;
        [TextArea(3, 10)]
        public string text;
    }

    [Header("Antonio")]
    public Text dialogueText;

    [TextArea(3, 10)]
    public string[] dialogues;

    [Header("Scene References")]
    public GameObject telaPreta;
    public GameObject pastScene;
    public GameObject presentScene;
    public GameObject dialoguePanel;

    public Text nameText;
    public Text dialoguePanelText;

    [Header("Animators")]
    public Animator pastDonaLurdesAnimator; // Dona Lurdes (talking)_0 no PastScene
    public Animator donaLurdesAnimator;     // Dona Lurdes Idle_0 no PresentScene
    public Animator antonioAnimator;        // Antônio (backwards-Idle)_0 no PresentScene
    public string isSayingParam = "isSaying";

    [Header("Settings")]
    public float typingSpeed = 0.03f;
    public KeyCode advanceKey = KeyCode.E;

    private int currentDialogueIndex;

    private bool isTyping;
    private bool dialogueFinished;

    private string currentSentence;

    private bool showingDonaLurdes = false;
    private bool inPresentScene = false;
    private string currentSpeaker = "";

    private string[] donaLurdesDialogues =
    {
        "Me desculpe, viu?",

        "Queria tanto ir na sua festa. É um evento muito importante, eu sei...",

        "Eu até juntei um dinheirinho, mas não tenho ninguém para me levar.",

        "Por mais que eu tenha o Toninho aqui...",

        "ele é muito distraído e não consegue me guiar direito.",

        "E meus filhos andam sempre ocupados sem tempo para nada.",

        "Me desculpa mesmo, cumadi..."
    };

    private DialogueLine[] presentSceneDialogues =
    {
        new DialogueLine
        {
            speaker = "Dona Lurdes",
            text = "Ô, meu filho! Você sabia que a vó estava com saudades?"
        },
        new DialogueLine
        {
            speaker = "Dona Lurdes",
            text = "Faz tempo que eu não te vejo desde que foi fazer faculdade longe."
        },
        new DialogueLine
        {
            speaker = "Dona Lurdes",
            text = "Olha para você, tá comendo direito? Tá tão magricelo!"
        },
        new DialogueLine
        {
            speaker = "Dona Lurdes",
            text = "Assim tu mata sua avó de preocupação."
        },
        new DialogueLine
        {
            speaker = "Antônio",
            text = "Para, vovó, eu tenho uma surpresa para a senhora."
        },
        new DialogueLine
        {
            speaker = "Dona Lurdes",
            text = "Não precisava se preocupar com presente nenhum, meu filho,"
        },
        new DialogueLine
        {
            speaker = "Dona Lurdes",
            text = "só a sua presença já me faz feliz!"
        },
        new DialogueLine
        {
            speaker = "Antônio",
            text = "*Passa o O.T.T.O para a Dona Lurdes*"
        },
        new DialogueLine
        {
            speaker = "Antônio",
            text = "Aqui, vó. Este é o O.T.T.O,"
        },
        new DialogueLine
        {
            speaker = "Antônio",
            text = "um robozinho que eu desenvolvi para te ajudar a se locomover."
        },
        new DialogueLine
        {
            speaker = "Dona Lurdes",
            text = "*Dona Lurdes tateia o aparelho com curiosidade e carinho*"
        },
        new DialogueLine
        {
            speaker = "Dona Lurdes",
            text = "Oxente, como é a história? Otto? Oto aqui já meu fio."
        },
        new DialogueLine
        {
            speaker = "Dona Lurdes",
            text = "Esse troço faz o quê? Me explique direito menino."
        },
        new DialogueLine
        {
            speaker = "Antônio",
            text = "É um robô vó. É um robô que vai guiar a senhora na rua"
        },
        new DialogueLine
        {
            speaker = "Antônio",
            text = "e levá-la aonde a senhora quiser."
        },
        new DialogueLine
        {
            speaker = "Dona Lurdes",
            text = "Ah, meu filho, muito obrigada…"
        },
        new DialogueLine
        {
            speaker = "Dona Lurdes",
            text = "Mesmo que essa tecnologia toda não funcione direito,"
        },
        new DialogueLine
        {
            speaker = "Dona Lurdes",
            text = "eu já fico feliz pela sua intenção de me ajudar."
        },
        new DialogueLine
        {
            speaker = "Dona Lurdes",
            text = "*animada*"
        },
        new DialogueLine
        {
            speaker = "Dona Lurdes",
            text = "Aliás, eu estava querendo ir visitar a Cecília esses dias,"
        },
        new DialogueLine
        {
            speaker = "Dona Lurdes",
            text = "mas nunca conseguia porque dependia dos outros."
        },
        new DialogueLine
        {
            speaker = "Dona Lurdes",
            text = "Dá para programar esse negócio para me levar lá?"
        },
        new DialogueLine
        {
            speaker = "Antônio",
            text = "Claro, vózinha, ele pode te levar para onde a senhora quiser,"
        },
        new DialogueLine
        {
            speaker = "Antônio",
            text = "deixa comigo que vou lhe mostrar."
        }
    };

    private void Start()
    {
        if (presentScene != null)
            presentScene.SetActive(false);

        SetAllSpeaking(false);
        StartDialogue();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(advanceKey))
            return;

        if (dialogueFinished)
            return;

        if (isTyping)
        {
            StopAllCoroutines();

            if (inPresentScene)
            {
                dialoguePanelText.text = currentSentence;
                SetSpeakerSpeaking(currentSpeaker, false);
            }
            else if (!showingDonaLurdes)
            {
                dialogueText.text = currentSentence;
                SetAllSpeaking(false);
            }
            else
            {
                dialoguePanelText.text = currentSentence;
                SetPastDonaLurdesSpeaking(false);
            }

            isTyping = false;
            return;
        }

        NextDialogue();
    }

    public void StartDialogue()
    {
        currentDialogueIndex = 0;
        dialogueFinished = false;

        ShowDialogue(currentDialogueIndex);
    }

    private void NextDialogue()
    {
        currentDialogueIndex++;

        if (inPresentScene)
        {
            if (currentDialogueIndex >= presentSceneDialogues.Length)
            {
                dialogueFinished = true;
                SetAllSpeaking(false);
                return;
            }
        }
        else if (!showingDonaLurdes)
        {
            if (currentDialogueIndex >= dialogues.Length)
            {
                if (dialogues.Length > 0 && dialogues[dialogues.Length - 1] == "10 anos depois...")
                {
                    StartCoroutine(StartPresentScene());
                }
                else
                {
                    StartCoroutine(StartPastScene());
                }
                return;
            }
        }
        else
        {
            if (currentDialogueIndex >= donaLurdesDialogues.Length)
            {
                SetPastDonaLurdesSpeaking(false);
                StartCoroutine(ReturnToNarrator());
                return;
            }
        }

        ShowDialogue(currentDialogueIndex);
    }

    private void ShowDialogue(int index)
    {
        if (inPresentScene)
        {
            DialogueLine line = presentSceneDialogues[index];
            currentSpeaker = line.speaker;
            currentSentence = line.text.Replace("\"", "").Replace("“", "").Replace("”", "");

            if (nameText != null)
                nameText.text = currentSpeaker;
        }
        else if (!showingDonaLurdes)
        {
            currentSentence = dialogues[index];
        }
        else
        {
            currentSentence = donaLurdesDialogues[index];
        }

        StopAllCoroutines();
        StartCoroutine(TypeSentence());
    }

    private IEnumerator TypeSentence()
    {
        isTyping = true;

        if (inPresentScene)
        {
            dialoguePanelText.text = "";
            SetSpeakerSpeaking(currentSpeaker, true);
        }
        else if (!showingDonaLurdes)
        {
            dialogueText.text = "";
            SetAllSpeaking(false);
        }
        else
        {
            dialoguePanelText.text = "";
            SetPastDonaLurdesSpeaking(true);
        }

        foreach (char letter in currentSentence)
        {
            if (inPresentScene)
                dialoguePanelText.text += letter;
            else if (!showingDonaLurdes)
                dialogueText.text += letter;
            else
                dialoguePanelText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        if (inPresentScene)
            SetSpeakerSpeaking(currentSpeaker, false);
        else if (showingDonaLurdes)
            SetPastDonaLurdesSpeaking(false);

        isTyping = false;
    }

    private IEnumerator StartPastScene()
    {
        dialogueFinished = true;

        dialogueText.text = "";

        telaPreta.SetActive(false);
        pastScene.SetActive(true);

        yield return new WaitForSeconds(3f);

        dialoguePanel.SetActive(true);

        nameText.text = "Dona Lurdes";

        showingDonaLurdes = true;

        currentDialogueIndex = 0;
        dialogueFinished = false;

        ShowDialogue(currentDialogueIndex);
    }

    private IEnumerator ReturnToNarrator()
    {
        dialogueFinished = true;

        SetAllSpeaking(false);
        dialoguePanel.SetActive(false);

        yield return new WaitForSeconds(3f);

        pastScene.SetActive(false);

        telaPreta.SetActive(true);

        yield return new WaitForSeconds(1f);

        showingDonaLurdes = false;
        inPresentScene = false;

        currentDialogueIndex = 0;

        dialogues = new string[]
        {
            "Mas eu decidi mudar isso.",

            "Essa dor virou o combustível que me impulsionou a estudar, ",

            "me formar e criar uma solução para minha avó.",

            "E para milhares de pessoas que enfrentam a mesma dificuldade.",

            "10 anos depois..."
        };

        dialogueFinished = false;

        StartDialogue();
    }

    private IEnumerator StartPresentScene()
    {
        dialogueFinished = true;
        inPresentScene = true;
        showingDonaLurdes = false;

        SetAllSpeaking(false);
        dialogueText.text = "";
        dialoguePanel.SetActive(false);

        // 2 segundos na tela preta após apertar E na última fala
        yield return new WaitForSeconds(2f);

        if (telaPreta != null)
            telaPreta.SetActive(false);

        if (presentScene != null)
            presentScene.SetActive(true);

        // 2,5 segundos na cena do presente antes de soltar a primeira fala da Dona Lurdes
        yield return new WaitForSeconds(2.5f);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        currentDialogueIndex = 0;
        dialogueFinished = false;

        ShowDialogue(currentDialogueIndex);
    }

    private void SafeSetBool(Animator anim, bool value)
    {
        if (anim == null)
            return;

        if (!anim.enabled || !anim.gameObject.activeInHierarchy)
            return;

        if (anim.runtimeAnimatorController == null)
            return;

        anim.SetBool(isSayingParam, value);
    }

    private void SetPastDonaLurdesSpeaking(bool speaking)
    {
        SafeSetBool(donaLurdesAnimator, false);
        SafeSetBool(antonioAnimator, false);
        SafeSetBool(pastDonaLurdesAnimator, speaking);
    }

    private void SetSpeakerSpeaking(string speaker, bool speaking)
    {
        SetAllSpeaking(false);

        if (!speaking || string.IsNullOrEmpty(speaker))
            return;

        string normalized = speaker.ToLower();

        if (normalized.Contains("lurdes"))
        {
            SafeSetBool(donaLurdesAnimator, true);
        }
        else if (normalized.Contains("antônio") || normalized.Contains("antonio"))
        {
            SafeSetBool(antonioAnimator, true);
        }
    }

    private void SetAllSpeaking(bool speaking)
    {
        SafeSetBool(pastDonaLurdesAnimator, speaking);
        SafeSetBool(donaLurdesAnimator, speaking);
        SafeSetBool(antonioAnimator, speaking);
    }
}