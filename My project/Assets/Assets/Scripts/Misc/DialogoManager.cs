
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogoManager : MonoBehaviour
{
    [System.Serializable]
    public class Fala
    {
        public string personagem;

        [TextArea(2, 5)]
        public string texto;

        public Sprite retrato;
    }

    [Header("Interface")]
    [SerializeField] private GameObject painelDialogo;
    [SerializeField] private Text nomePersonagem;
    [SerializeField] private Text textoDialogo;
    [SerializeField] private Image retratoPersonagem;

    [Header("Configurações")]
    [SerializeField] private float velocidadeTexto = 0.04f;

    [Header("Som da Digitação")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip somDigitacao;
    [SerializeField, Range(0f, 1f)] private float volumeSom = 0.25f;
    [SerializeField, Min(1)] private int letrasPorSom = 2;

    [Header("Controles do Jogador")]
    [SerializeField] private PlayerInput inputJogador;

    private Fala[] falasAtuais;
    private int indiceFala;
    private Coroutine digitacao;
    private bool dialogoAtivo;
    private bool controlesBloqueados;
    private float tempoAnterior = 1f;

    public bool EmDialogo => dialogoAtivo;

    void Awake()
    {
        if (painelDialogo != null)
            painelDialogo.SetActive(false);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!dialogoAtivo || Keyboard.current == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            AvancarDialogo();
    }

    public void IniciarDialogo(Fala[] falas)
    {
        if (dialogoAtivo || falas == null || falas.Length == 0)
            return;

        if (painelDialogo == null ||
            nomePersonagem == null ||
            textoDialogo == null)
            return;

        falasAtuais = falas;
        indiceFala = 0;
        dialogoAtivo = true;

        tempoAnterior = Time.timeScale;
        Time.timeScale = 0f;

        if (inputJogador != null)
        {
            inputJogador.DeactivateInput();
            controlesBloqueados = true;
        }

        painelDialogo.SetActive(true);
        MostrarFala();
    }

    void MostrarFala()
    {
        Fala fala = falasAtuais[indiceFala];

        nomePersonagem.text = fala.personagem;
        textoDialogo.text = "";

        if (retratoPersonagem != null)
        {
            bool possuiRetrato = fala.retrato != null;

            retratoPersonagem.gameObject.SetActive(possuiRetrato);

            if (possuiRetrato)
                retratoPersonagem.sprite = fala.retrato;
        }

        digitacao = StartCoroutine(DigitarTexto(fala.texto ?? ""));
    }

    IEnumerator DigitarTexto(string texto)
    {
        int contadorLetras = 0;

        foreach (char letra in texto)
        {
            textoDialogo.text += letra;

            if (!char.IsWhiteSpace(letra))
            {
                contadorLetras++;

                if ((contadorLetras - 1) % letrasPorSom == 0)
                {
                    if (audioSource != null && somDigitacao != null)
                    {
                        audioSource.PlayOneShot(
                            somDigitacao,
                            volumeSom
                        );
                    }
                }
            }

            yield return new WaitForSecondsRealtime(velocidadeTexto);
        }

        digitacao = null;
    }

    void AvancarDialogo()
    {
        if (digitacao != null)
        {
            StopCoroutine(digitacao);
            digitacao = null;

            textoDialogo.text = falasAtuais[indiceFala].texto ?? "";
            return;
        }

        indiceFala++;

        if (indiceFala >= falasAtuais.Length)
        {
            EncerrarDialogo();
            return;
        }

        MostrarFala();
    }

    void EncerrarDialogo()
    {
        dialogoAtivo = false;

        if (digitacao != null)
        {
            StopCoroutine(digitacao);
            digitacao = null;
        }

        if (painelDialogo != null)
            painelDialogo.SetActive(false);

        if (controlesBloqueados && inputJogador != null)
        {
            inputJogador.ActivateInput();
            controlesBloqueados = false;
        }

        Time.timeScale = tempoAnterior;
    }

    void OnDisable()
    {
        if (dialogoAtivo)
            EncerrarDialogo();
    }
}
