
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackUI : MonoBehaviour
{
    [SerializeField] private Text textoFeedback;
    [SerializeField] private float duracaoMensagem = 2.5f;

    private Coroutine rotinaMensagem;

    void Awake()
    {
        if (textoFeedback != null)
            textoFeedback.text = "";
    }

    public void Mostrar(string mensagem)
    {
        if (textoFeedback == null)
            return;

        if (rotinaMensagem != null)
            StopCoroutine(rotinaMensagem);

        textoFeedback.text = mensagem;
        rotinaMensagem = StartCoroutine(LimparMensagem());
    }

    IEnumerator LimparMensagem()
    {
        yield return new WaitForSecondsRealtime(duracaoMensagem);

        textoFeedback.text = "";
        rotinaMensagem = null;
    }
}
