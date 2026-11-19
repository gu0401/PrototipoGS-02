
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovimentoPassante : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool spriteOlhaDireita = true;

    private Rigidbody2D rb;
    private AudioSource audioSource;

    private Vector2 destino;
    private float velocidade;
    private bool configurado;
    private bool interagiu;

    private DialogoManager gerenciador;
    private AudioClip somContato;
    private DialogoManager.Fala[] falas;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Configurar(
        Vector2 novoDestino,
        float novaVelocidade,
        DialogoManager novoGerenciador,
        AudioClip novoSom,
        DialogoManager.Fala[] novasFalas)
    {
        destino = novoDestino;
        velocidade = Mathf.Max(0.1f, novaVelocidade);

        gerenciador = novoGerenciador;
        somContato = novoSom;
        falas = novasFalas;

        configurado = true;

        Vector2 direcao = destino - rb.position;

        if (spriteRenderer != null &&
            Mathf.Abs(direcao.x) > Mathf.Abs(direcao.y))
        {
            bool indoDireita = direcao.x > 0f;

            spriteRenderer.flipX =
                indoDireita != spriteOlhaDireita;
        }
    }

    void FixedUpdate()
    {
        if (!configurado)
            return;

        if (Vector2.Distance(rb.position, destino) <= 0.06f)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 novaPosicao = Vector2.MoveTowards(
            rb.position,
            destino,
            velocidade * Time.fixedDeltaTime
        );

        rb.MovePosition(novaPosicao);
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (interagiu)
            return;

        if (outro.GetComponentInParent<PlayerController>() == null)
            return;

        if (somContato == null &&
            (falas == null || falas.Length == 0))
            return;

        interagiu = true;

        if (audioSource != null && somContato != null)
            audioSource.PlayOneShot(somContato);

        if (gerenciador != null &&
            !gerenciador.EmDialogo &&
            falas != null &&
            falas.Length > 0)
        {
            gerenciador.IniciarDialogo(falas);
        }
    }
}
