
using System.Collections;
using UnityEngine;

public class EventoPassagem : MonoBehaviour
{
    [Header("Evento")]
    [SerializeField] private GameObject prefabPersonagem;

    [Range(0, 100)]
    [SerializeField] private int chanceAtivacao = 50;

    [Header("Percurso")]
    [SerializeField] private Transform pontoInicial;
    [SerializeField] private Transform pontoFinal;
    [SerializeField] private float velocidade = 4f;

    [Header("Quantidade")]
    [SerializeField] private int quantidade = 1;
    [SerializeField] private float intervaloSpawn = 1f;

    [Header("Interação Opcional")]
    [SerializeField] private DialogoManager gerenciadorDialogos;
    [SerializeField] private AudioClip somContato;
    [SerializeField] private DialogoManager.Fala[] falasContato;

    private bool eventoExecutado;

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (eventoExecutado)
            return;

        if (outro.GetComponentInParent<PlayerController>() == null)
            return;

        eventoExecutado = true;

        int sorteio = Random.Range(0, 100);

        if (sorteio >= chanceAtivacao)
            return;

        if (prefabPersonagem == null ||
            pontoInicial == null ||
            pontoFinal == null)
        {
            Debug.LogWarning("Configure o EventoPassagem!");
            return;
        }

        StartCoroutine(GerarPersonagens());
    }

    IEnumerator GerarPersonagens()
    {
        for (int i = 0; i < Mathf.Max(1, quantidade); i++)
        {
            GameObject personagem = Instantiate(
                prefabPersonagem,
                pontoInicial.position,
                prefabPersonagem.transform.rotation
            );

            MovimentoPassante movimento =
                personagem.GetComponent<MovimentoPassante>();

            if (movimento != null)
            {
                movimento.Configurar(
                    pontoFinal.position,
                    velocidade,
                    gerenciadorDialogos,
                    somContato,
                    falasContato
                );
            }
            else
            {
                Debug.LogWarning(
                    "O prefab precisa de MovimentoPassante."
                );

                Destroy(personagem);
            }

            if (i < quantidade - 1)
                yield return new WaitForSeconds(intervaloSpawn);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (pontoInicial == null || pontoFinal == null)
            return;

        Gizmos.color = Color.cyan;

        Gizmos.DrawSphere(pontoInicial.position, 0.15f);
        Gizmos.DrawSphere(pontoFinal.position, 0.15f);

        Gizmos.DrawLine(
            pontoInicial.position,
            pontoFinal.position
        );
    }
}
