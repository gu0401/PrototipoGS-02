
using UnityEngine;
using UnityEngine.UI;

public class Tamagotchi : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform vovo;
    [SerializeField] private Text textoAlerta;
    [SerializeField] private Image retratoOtto;
    [SerializeField] private Sprite imagemPadraoOtto;

    [Header("Alertas")]
    [SerializeField] private float raioDeteccao = 2f;
    [SerializeField] private LayerMask camadaObstaculos;
    [SerializeField] private float intervaloVerificacao = 0.2f;

    [Header("Pulso Sonoro")]
    [SerializeField] private GameObject pulsoPrefab;
    [SerializeField] private float cooldownPulso = 2.5f;
    [SerializeField] private AudioClip somPulso;

    private AudioSource audioSource;
    private float temporizador;
    private float proximoPulsoPermitido;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (imagemPadraoOtto == null && retratoOtto != null)
            imagemPadraoOtto = retratoOtto.sprite;
    }

    void Start()
    {
        if (textoAlerta != null)
            textoAlerta.text = "";

        AtualizarRetrato(null);
    }

    void Update()
    {
        temporizador -= Time.deltaTime;

        if (temporizador <= 0f)
        {
            VerificarPerigos();
            temporizador = intervaloVerificacao;
        }
    }

    public void EmitirPulso(Vector2 direcao)
    {
        if (Time.time < proximoPulsoPermitido || pulsoPrefab == null)
            return;

        float angulo = Mathf.Atan2(
            direcao.y,
            direcao.x
        ) * Mathf.Rad2Deg;

        GameObject pulso = Instantiate(
            pulsoPrefab,
            transform.position,
            Quaternion.Euler(0f, 0f, angulo)
        );

        OndaSonora onda = pulso.GetComponent<OndaSonora>();

        if (onda == null)
        {
            Debug.LogWarning(
                "O Prefab precisa do script OndaSonora."
            );

            Destroy(pulso);
            return;
        }

        onda.ConfigurarDirecao(direcao);

        proximoPulsoPermitido =
            Time.time + cooldownPulso;

        if (audioSource != null && somPulso != null)
            audioSource.PlayOneShot(somPulso);
    }

    void VerificarPerigos()
    {
        if (vovo == null || textoAlerta == null)
            return;

        Collider2D[] objetos = Physics2D.OverlapCircleAll(
            vovo.position,
            raioDeteccao,
            camadaObstaculos
        );

        AlertaPerigo perigoProximo = null;
        float menorDistancia = float.MaxValue;

        foreach (Collider2D colisor in objetos)
        {
            AlertaPerigo perigo =
                colisor.GetComponentInParent<AlertaPerigo>();

            if (perigo == null)
                continue;

            Vector2 ponto =
                colisor.ClosestPoint(vovo.position);

            float distancia = Vector2.Distance(
                vovo.position,
                ponto
            );

            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                perigoProximo = perigo;
            }
        }

        if (perigoProximo != null)
        {
            textoAlerta.text = perigoProximo.Mensagem;
            AtualizarRetrato(perigoProximo.ImagemOtto);
        }
        else
        {
            textoAlerta.text = "";
            AtualizarRetrato(null);
        }
    }

    void AtualizarRetrato(Sprite novaImagem)
    {
        if (retratoOtto == null)
            return;

        Sprite imagem = novaImagem != null
            ? novaImagem
            : imagemPadraoOtto;

        retratoOtto.sprite = imagem;
        retratoOtto.enabled = imagem != null;
    }

    void OnDrawGizmosSelected()
    {
        if (vovo == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            vovo.position,
            raioDeteccao
        );
    }
}
