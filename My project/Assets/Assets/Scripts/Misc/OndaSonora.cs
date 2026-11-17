
using UnityEngine;

public class OndaSonora : MonoBehaviour
{
    [Header("Expansão")]
    [SerializeField] private float velocidadeExpansao = 3f;
    [SerializeField] private float tamanhoMaximo = 5f;

    [Header("Movimento")]
    [SerializeField] private float velocidadeAvanco = 2f;

    [Header("Detecção Direcional")]
    [SerializeField, Range(10f, 180f)]
    private float aberturaGraus = 90f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private float alphaInicial;
    private float tamanhoInicial;

    private Vector2 direcaoPulso = Vector2.right;
    private Vector2 origemPulso;
    private bool filtroDirecional;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        alphaInicial = spriteRenderer.color.a;
        tamanhoInicial = transform.localScale.x;
    }

    public void ConfigurarDirecao(Vector2 direcao)
    {
        if (direcao.sqrMagnitude > 0.001f)
            direcaoPulso = direcao.normalized;

        origemPulso = transform.position;
        filtroDirecional = true;
    }

    void Update()
    {
        float novoTamanho = transform.localScale.x
            + velocidadeExpansao * Time.deltaTime;

        transform.localScale = new Vector3(
            novoTamanho,
            novoTamanho,
            1f
        );

        float progresso = Mathf.InverseLerp(
            tamanhoInicial,
            tamanhoMaximo,
            novoTamanho
        );

        Color cor = spriteRenderer.color;
        cor.a = Mathf.Lerp(alphaInicial, 0f, progresso);
        spriteRenderer.color = cor;

        if (novoTamanho >= tamanhoMaximo)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (!filtroDirecional || rb == null)
            return;

        rb.MovePosition(
            rb.position
            + direcaoPulso * velocidadeAvanco * Time.fixedDeltaTime
        );
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        ObjetoRevelavel objeto =
            outro.GetComponentInParent<ObjetoRevelavel>();

        if (objeto == null)
            return;

        if (filtroDirecional)
        {
            Vector2 ponto = outro.ClosestPoint(origemPulso);
            Vector2 direcaoObjeto = ponto - origemPulso;

            if (direcaoObjeto.sqrMagnitude < 0.0001f)
                direcaoObjeto = (Vector2)outro.bounds.center - origemPulso;

            if (direcaoObjeto.sqrMagnitude > 0.0001f)
            {
                float alinhamento = Vector2.Dot(
                    direcaoPulso,
                    direcaoObjeto.normalized
                );

                float limite = Mathf.Cos(
                    aberturaGraus * 0.5f * Mathf.Deg2Rad
                );

                if (alinhamento < limite)
                    return;
            }
        }

        objeto.RevelarPorSom();
    }
}
