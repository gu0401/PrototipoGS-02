
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TutorialCasa : MonoBehaviour
{
    private enum Etapa
    {
        Introducao,
        Movimento,
        Bengala,
        Otto,
        Saida
    }

    [Header("Referencias")]
    [SerializeField] private Transform vovo;
    [SerializeField] private DialogoManager gerenciador;
    [SerializeField] private Text textoTutorial;

    [Header("Conversa Inicial")]
    [SerializeField] private DialogoManager.Fala[] falasIntroducao;

    [Header("Configuracoes")]
    [SerializeField] private float distanciaMovimento = 1f;
    [SerializeField] private string cenaDestino = "Fase01";

    private Etapa etapa = Etapa.Introducao;
    private Vector2 posicaoInicial;
    private bool jogadorNaPorta;

    void Start()
    {
        if (vovo == null || gerenciador == null)
        {
            Debug.LogError(
                "Configure Vovo e Gerenciador no TutorialCasa."
            );
            enabled = false;
            return;
        }

        if (textoTutorial != null)
            textoTutorial.text = "";

        gerenciador.IniciarDialogo(falasIntroducao);
    }

    void Update()
    {
        if (etapa == Etapa.Introducao)
        {
            if (gerenciador.EmDialogo)
                return;

            etapa = Etapa.Movimento;
            posicaoInicial = vovo.position;
            AtualizarTexto();
            return;
        }

        if (gerenciador.EmDialogo)
            return;

        Keyboard teclado = Keyboard.current;

        switch (etapa)
        {
            case Etapa.Movimento:
                if (Vector2.Distance(
                    posicaoInicial,
                    vovo.position
                ) >= distanciaMovimento)
                {
                    etapa = Etapa.Bengala;
                    AtualizarTexto();
                }
                break;

            case Etapa.Bengala:
                if (teclado != null &&
                    teclado.fKey.wasPressedThisFrame)
                {
                    etapa = Etapa.Otto;
                    AtualizarTexto();
                }
                break;

            case Etapa.Otto:
                if (teclado != null &&
                    teclado.gKey.wasPressedThisFrame)
                {
                    etapa = Etapa.Saida;
                    AtualizarTexto();
                }
                break;

            case Etapa.Saida:
                if (jogadorNaPorta &&
                    teclado != null &&
                    teclado.eKey.wasPressedThisFrame)
                {
                    Time.timeScale = 1f;
                    SceneManager.LoadScene(cenaDestino);
                }
                break;
        }
    }

    void AtualizarTexto()
    {
        if (textoTutorial == null)
            return;

        switch (etapa)
        {
            case Etapa.Movimento:
                textoTutorial.text =
                    "Use WASD para caminhar pela casa.";
                break;

            case Etapa.Bengala:
                textoTutorial.text =
                    "Pressione F para utilizar a bengala.";
                break;

            case Etapa.Otto:
                textoTutorial.text =
                    "Pressione G para emitir o pulso do Otto.";
                break;

            case Etapa.Saida:
                textoTutorial.text = jogadorNaPorta
                    ? "Pressione E para sair de casa."
                    : "Muito bem! Há caixas e um buraco na casa, teste as Mêcanicas neles e quando quiser saia pela porta";
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.GetComponentInParent<PlayerController>() == null)
            return;

        jogadorNaPorta = true;
        AtualizarTexto();
    }

    void OnTriggerExit2D(Collider2D outro)
    {
        if (outro.GetComponentInParent<PlayerController>() == null)
            return;

        jogadorNaPorta = false;
        AtualizarTexto();
    }
}
