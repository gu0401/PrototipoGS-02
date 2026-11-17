
using UnityEngine;

public class ObjetoRevelavel : MonoBehaviour
{
    [Header("Revelação")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private bool revelarComSom = true;

    [Header("Tempo")]
    [SerializeField] private float tempoVisivel = 0.5f;
    [SerializeField, Min(0.1f)] private float duracaoDesaparecimento = 1.5f;

    private float temporizador;
    private bool revelado;

    void Awake()
    {
        if (sprite == null)
            sprite = GetComponentInChildren<SpriteRenderer>();

        if (sprite == null)
        {
            Debug.LogError("SpriteRenderer não encontrado!", this);
            enabled = false;
            return;
        }

        Color cor = sprite.color;
        cor.a = 0f;
        sprite.color = cor;
    }

    void Update()
    {
        if (!revelado)
            return;

        if (temporizador > 0f)
        {
            temporizador -= Time.deltaTime;
            return;
        }

        Color cor = sprite.color;

        cor.a = Mathf.MoveTowards(
            cor.a,
            0f,
            Time.deltaTime / duracaoDesaparecimento
        );

        sprite.color = cor;

        if (cor.a <= 0f)
            revelado = false;
    }

    public void RevelarPorSom()
    {
        if (revelarComSom)
            Revelar();
    }

    public void Revelar()
    {
        Color cor = sprite.color;
        cor.a = 1f;
        sprite.color = cor;

        temporizador = tempoVisivel;
        revelado = true;
    }
}
