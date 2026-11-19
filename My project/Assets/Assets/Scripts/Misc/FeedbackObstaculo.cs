
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

    [Header("Obstáculo")]
    [SerializeField] private TipoObstaculo tipo;
    [SerializeField] private ObjetoRevelavel objetoRevelavel;
    [SerializeField] private AudioClip somContato;
    [SerializeField] private float intervaloContato = 1f;

    [Header("Retorno")]
    [SerializeField] private Transform pontoRetorno;
    [SerializeField] private Transform[] pontosRetornoCarro;

    private AudioSource audioSource;
    private float proximoContato;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (objetoRevelavel == null)
            objetoRevelavel =
                GetComponentInParent<ObjetoRevelavel>();
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
        Transform destino = pontoRetorno;

        if (tipo == TipoObstaculo.Carro)
        {
            float menorDistancia = float.MaxValue;

            foreach (Transform ponto in pontosRetornoCarro)
            {
                if (ponto == null)
                    continue;

                float distancia = (
                    ponto.position -
                    jogador.transform.position
                ).sqrMagnitude;

                if (distancia < menorDistancia)
                {
                    menorDistancia = distancia;
                    destino = ponto;
                }
            }
        }

        if (destino == null)
        {
            Debug.LogWarning(
                "Nenhum ponto de retorno configurado!"
            );
            return;
        }

        Rigidbody2D rb =
            jogador.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = destino.position;
        }
        else
        {
            jogador.transform.position = destino.position;
        }
    }
}
