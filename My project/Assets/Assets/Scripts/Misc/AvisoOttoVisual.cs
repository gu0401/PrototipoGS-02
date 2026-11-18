
using UnityEngine;
using UnityEngine.UI;

public class AvisoOttoVisual : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Text textoAlerta;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animação")]
    [SerializeField] private float velocidadeTransicao = 3f;
    [SerializeField] private float distanciaEntrada = 40f;
    [SerializeField] private float escalaInicial = 0.9f;

    private RectTransform painel;
    private Vector2 posicaoOriginal;
    private Vector3 escalaOriginal;

    private float progresso;

    void Awake()
    {
        painel = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        posicaoOriginal = painel.anchoredPosition;
        escalaOriginal = painel.localScale;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        painel.anchoredPosition =
            posicaoOriginal + Vector2.left * distanciaEntrada;

        painel.localScale = escalaOriginal * escalaInicial;
    }

    void Update()
    {
        if (canvasGroup == null || textoAlerta == null)
            return;

        bool possuiAviso =
            !string.IsNullOrWhiteSpace(textoAlerta.text);

        float destino = possuiAviso ? 1f : 0f;

        progresso = Mathf.MoveTowards(
            progresso,
            destino,
            velocidadeTransicao * Time.unscaledDeltaTime
        );

        float suave = progresso * progresso * (3f - 2f * progresso);

        canvasGroup.alpha = suave;

        painel.anchoredPosition = Vector2.Lerp(
            posicaoOriginal + Vector2.left * distanciaEntrada,
            posicaoOriginal,
            suave
        );

        painel.localScale = escalaOriginal *
            Mathf.Lerp(escalaInicial, 1f, suave);
    }
}
