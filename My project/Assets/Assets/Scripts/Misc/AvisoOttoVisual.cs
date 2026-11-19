
using UnityEngine;
using UnityEngine.UI;

public class AvisoOttoVisual : MonoBehaviour
{
    [Header("Interface")]
    [SerializeField] private Text textoAlerta;
    [SerializeField] private Text textoSusto;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Imagens do Otto")]
    [SerializeField] private Image retratoOtto;
    [SerializeField] private Sprite imagemNormal;
    [SerializeField] private Sprite imagemAssustado;

    [Header("Susto")]
    [TextArea(2, 4)]
    [SerializeField]
    private string mensagemSusto =
        "Dona Lurdes! Que susto! Vamos tomar mais cuidado.";

    [SerializeField] private float duracaoSusto = 2.5f;

    [Header("Animação do Painel")]
    [SerializeField] private float velocidadeTransicao = 3f;
    [SerializeField] private float distanciaEntrada = 40f;
    [SerializeField] private float escalaInicial = 0.9f;

    private RectTransform painel;
    private Vector2 posicaoOriginal;
    private Vector3 escalaOriginal;

    private float progresso;
    private float fimSusto = -1f;
    private bool sustoAtivo;

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

        if (retratoOtto != null && imagemNormal != null)
            retratoOtto.sprite = imagemNormal;

        if (textoSusto != null)
        {
            textoSusto.text = mensagemSusto;
            textoSusto.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        bool assustadoAgora = Time.unscaledTime < fimSusto;

        if (assustadoAgora != sustoAtivo)
        {
            sustoAtivo = assustadoAgora;
            AtualizarExpressao();
        }

        bool possuiAviso = sustoAtivo;

        if (textoAlerta != null &&
            !string.IsNullOrWhiteSpace(textoAlerta.text))
        {
            possuiAviso = true;
        }

        float destino = possuiAviso ? 1f : 0f;

        progresso = Mathf.MoveTowards(
            progresso,
            destino,
            velocidadeTransicao * Time.unscaledDeltaTime
        );

        float suave =
            progresso * progresso * (3f - 2f * progresso);

        if (canvasGroup != null)
            canvasGroup.alpha = suave;

        painel.anchoredPosition = Vector2.Lerp(
            posicaoOriginal + Vector2.left * distanciaEntrada,
            posicaoOriginal,
            suave
        );

        painel.localScale = escalaOriginal *
            Mathf.Lerp(escalaInicial, 1f, suave);
    }

    public void MostrarSusto()
    {
        fimSusto = Time.unscaledTime +
            Mathf.Max(0.25f, duracaoSusto);
    }

    void AtualizarExpressao()
    {
        if (retratoOtto != null)
        {
            Sprite imagem = sustoAtivo
                ? imagemAssustado
                : imagemNormal;

            if (imagem != null)
                retratoOtto.sprite = imagem;
        }

        if (textoAlerta != null)
            textoAlerta.gameObject.SetActive(!sustoAtivo);

        if (textoSusto != null)
        {
            textoSusto.text = mensagemSusto;
            textoSusto.gameObject.SetActive(sustoAtivo);
        }
    }
}
