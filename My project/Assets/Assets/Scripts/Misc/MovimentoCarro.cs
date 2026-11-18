
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovimentoCarro : MonoBehaviour
{
    [Header("Percurso")]
    [SerializeField] private Transform[] pontos;

    [Header("Movimento")]
    [SerializeField] private float velocidade = 3f;
    [SerializeField] private float intervalo = 2f;

    [Header("Animação")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool spriteLadoOlhaDireita = true;

    private Rigidbody2D rb;
    private int proximoPonto;
    private float tempoEspera;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        if (pontos == null || pontos.Length < 2)
        {
            Debug.LogWarning("O carro precisa de pelo menos dois pontos.");
            enabled = false;
            return;
        }

        foreach (Transform ponto in pontos)
        {
            if (ponto == null)
            {
                Debug.LogWarning("Existe um ponto vazio no percurso.");
                enabled = false;
                return;
            }
        }

        rb.position = pontos[0].position;
        proximoPonto = 1;
        tempoEspera = intervalo;

        AtualizarAnimacao();
    }

    void FixedUpdate()
    {
        if (tempoEspera > 0f)
        {
            tempoEspera -= Time.fixedDeltaTime;
            return;
        }

        Vector2 destino = pontos[proximoPonto].position;

        if (Vector2.Distance(rb.position, destino) <= 0.05f)
        {
            ChegarAoPonto();
            return;
        }

        Vector2 novaPosicao = Vector2.MoveTowards(
            rb.position,
            destino,
            velocidade * Time.fixedDeltaTime
        );

        rb.MovePosition(novaPosicao);
    }

    void ChegarAoPonto()
    {
        if (proximoPonto == pontos.Length - 1)
        {
            rb.position = pontos[0].position;
            proximoPonto = 1;
            tempoEspera = intervalo;
        }
        else
        {
            proximoPonto++;
        }

        AtualizarAnimacao();
    }

    void AtualizarAnimacao()
    {
        Vector2 direcao =
            (Vector2)pontos[proximoPonto].position - rb.position;

        if (direcao.sqrMagnitude < 0.001f)
            return;

        if (Mathf.Abs(direcao.x) > Mathf.Abs(direcao.y))
        {
            if (animator != null)
                animator.Play("CarroLado");

            if (spriteRenderer != null)
            {
                bool indoDireita = direcao.x > 0f;

                spriteRenderer.flipX =
                    indoDireita != spriteLadoOlhaDireita;
            }
        }
        else
        {
            if (spriteRenderer != null)
                spriteRenderer.flipX = false;

            if (animator != null)
            {
                if (direcao.y > 0f)
                    animator.Play("CarroCima");
                else
                    animator.Play("CarroBaixo");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (pontos == null)
            return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < pontos.Length; i++)
        {
            if (pontos[i] == null)
                continue;

            Gizmos.DrawSphere(pontos[i].position, 0.12f);

            if (i > 0 && pontos[i - 1] != null)
            {
                Gizmos.DrawLine(
                    pontos[i - 1].position,
                    pontos[i].position
                );
            }
        }
    }
}
