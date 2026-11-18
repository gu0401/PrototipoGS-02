
using UnityEngine;

public class FeedbackObstaculo : MonoBehaviour
{
    public enum TipoObstaculo
    {
        Poste,
        Cerca,
        Galho,
        Buraco,
        Carro,
        Cachorro
    }

    [Header("Tipo")]
    [SerializeField] private TipoObstaculo tipo;

    [Header("Efeitos")]
    [SerializeField] private ObjetoRevelavel objetoRevelavel;
    [SerializeField] private AudioClip somContato;

    [Header("Checkpoint")]
    [SerializeField] private Transform pontoRetorno;

    [Header("Configurações")]
    [SerializeField] private float intervaloContato = 1f;

    private AudioSource audioSource;
    private float proximoContato;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (objetoRevelavel == null)
            objetoRevelavel = GetComponentInParent<ObjetoRevelavel>();
    }

    void OnCollisionEnter2D(Collision2D colisao)
    {
        PlayerController jogador =
            colisao.collider.GetComponentInParent<PlayerController>();

        if (jogador != null)
            AplicarFeedback(jogador);
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        PlayerController jogador =
            outro.GetComponentInParent<PlayerController>();

        if (jogador != null)
            AplicarFeedback(jogador);
    }

    void AplicarFeedback(PlayerController jogador)
    {
        if (Time.time < proximoContato)
            return;

        proximoContato = Time.time + intervaloContato;

        if (objetoRevelavel != null)
            objetoRevelavel.Revelar();

        if (audioSource != null && somContato != null)
            audioSource.PlayOneShot(somContato);

        if (tipo == TipoObstaculo.Buraco ||
            tipo == TipoObstaculo.Carro)
        {
            RetornarCheckpoint(jogador);
        }
    }

    void RetornarCheckpoint(PlayerController jogador)
    {
        if (pontoRetorno == null)
        {
            Debug.LogWarning(
                "Checkpoint não configurado em: " + gameObject.name
            );
            return;
        }

        Rigidbody2D rb = jogador.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = pontoRetorno.position;
        }
        else
        {
            jogador.transform.position = pontoRetorno.position;
        }
    }
}
