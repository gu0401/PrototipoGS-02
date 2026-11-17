
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimentação")]
    [SerializeField] private float velocidade = 3f;

    [Header("Efeito dos Passos")]
    [SerializeField] private GameObject efeitoPassoPrefab;
    [SerializeField] private float intervaloPassos = 0.5f;
    [SerializeField] private float offsetPassoY = -0.5f;

    [Header("Bengala")]
    [SerializeField] private float alcanceBengala = 1.2f;
    [SerializeField] private float raioBengala = 0.3f;
    [SerializeField] private LayerMask camadaObstaculos;

    [Header("Efeito da Bengala")]
    [SerializeField] private GameObject efeitoBengalaPrefab;
    [SerializeField] private float distanciaEfeito = 0.7f;
    [SerializeField] private float duracaoEfeito = 0.3f;

    [Header("Otto")]
    [SerializeField] private Tamagotchi otto;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator animator;

    private Vector2 direcao;
    private Vector2 ultimaDirecao = Vector2.down;

    private float temporizadorPassos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        GerarEfeitoPassos();
    }

    void FixedUpdate()
    {
        Move();
    }

    public void OnMove(InputValue value)
    {
        direcao = value.Get<Vector2>().normalized;

        if (direcao != Vector2.zero)
            ultimaDirecao = direcao;

        Flip();
        Animar();
    }

    void Move()
    {
        rb.linearVelocity = direcao * velocidade;
    }

    void Flip()
    {
        if (Mathf.Abs(direcao.x) >= Mathf.Abs(direcao.y) && direcao.x != 0)
        {
            sprite.flipX = direcao.x > 0;
        }
        else if (direcao.y != 0)
        {
            sprite.flipX = true;
        }
    }

    void Animar()
    {
        if (animator == null)
            return;

        bool andando = direcao.sqrMagnitude > 0.01f;

        animator.SetBool("pAndando", andando);

        if (!andando)
            return;

        if (Mathf.Abs(direcao.x) >= Mathf.Abs(direcao.y))
            animator.SetFloat("pDirecao", 2f);
        else if (direcao.y > 0)
            animator.SetFloat("pDirecao", 1f);
        else
            animator.SetFloat("pDirecao", 0f);
    }

    void GerarEfeitoPassos()
    {
        if (direcao == Vector2.zero || efeitoPassoPrefab == null)
        {
            temporizadorPassos = 0f;
            return;
        }

        temporizadorPassos -= Time.deltaTime;

        if (temporizadorPassos <= 0f)
        {
            Vector3 posicao = transform.position
                + new Vector3(0f, offsetPassoY, 0f);

            Instantiate(
                efeitoPassoPrefab,
                posicao,
                Quaternion.identity
            );

            temporizadorPassos = intervaloPassos;
        }
    }

    public void OnBengala(InputValue value)
    {
        if (value.isPressed)
        {
            UsarBengala();
            EfeitoBengala();
        }
    }

    void UsarBengala()
    {
        RaycastHit2D hit = Physics2D.CircleCast(
            transform.position,
            raioBengala,
            ultimaDirecao,
            alcanceBengala,
            camadaObstaculos
        );

        if (hit.collider != null)
        {
            ObjetoRevelavel objeto =
                hit.collider.GetComponentInParent<ObjetoRevelavel>();

            if (objeto != null)
                objeto.Revelar();
        }
    }

    void EfeitoBengala()
    {
        if (efeitoBengalaPrefab == null)
            return;

        Vector3 posicao = transform.position
            + (Vector3)ultimaDirecao * distanciaEfeito;

        float angulo = 90f + Mathf.Atan2(
            ultimaDirecao.y,
            ultimaDirecao.x
        ) * Mathf.Rad2Deg;

        GameObject efeito = Instantiate(
            efeitoBengalaPrefab,
            posicao,
            Quaternion.Euler(0f, 0f, angulo)
        );

        Destroy(efeito, duracaoEfeito);
    }

    public void OnPulsoOtto(InputValue value)
    {
        if (value.isPressed && otto != null)
        {
            otto.EmitirPulso(ultimaDirecao);
        }
    }
}
