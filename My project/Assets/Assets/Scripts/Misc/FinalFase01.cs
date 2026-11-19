
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FinalFase01 : MonoBehaviour
{
    [Header("Dialogo Final")]
    [SerializeField] private DialogoManager gerenciador;
    [SerializeField] private Text textoInteragir;
    [SerializeField] private DialogoManager.Fala[] falas;

    [Header("Conclusao")]
    [SerializeField] private GameObject painelConclusao;
    [SerializeField] private PlayerInput inputJogador;

    private bool jogadorPerto;
    private bool aguardandoDialogo;
    private bool faseFinalizada;

    [Header("Menu")]
    [SerializeField] private string nomeCenaMenu = "Menu";

    void Start()
    {
        if (painelConclusao != null)
            painelConclusao.SetActive(false);

        AtualizarIndicador();
    }

    void Update()
    {
        if (faseFinalizada || gerenciador == null)
            return;

        if (aguardandoDialogo)
        {
            if (!gerenciador.EmDialogo)
            {
                aguardandoDialogo = false;
                ConcluirFase();
            }

            return;
        }

        if (jogadorPerto &&
            !gerenciador.EmDialogo &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            gerenciador.IniciarDialogo(falas);

            if (gerenciador.EmDialogo)
            {
                aguardandoDialogo = true;
                AtualizarIndicador();
            }
        }

        AtualizarIndicador();
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.GetComponentInParent<PlayerController>() == null)
            return;

        jogadorPerto = true;
        AtualizarIndicador();
    }

    public void VoltarAoMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeCenaMenu);
    }

    void OnTriggerExit2D(Collider2D outro)
    {
        if (outro.GetComponentInParent<PlayerController>() == null)
            return;

        jogadorPerto = false;
        AtualizarIndicador();
    }

    void AtualizarIndicador()
    {
        if (textoInteragir == null)
            return;

        bool mostrar = jogadorPerto &&
                       !aguardandoDialogo &&
                       !faseFinalizada &&
                       gerenciador != null &&
                       !gerenciador.EmDialogo;

        textoInteragir.gameObject.SetActive(mostrar);
    }

    void ConcluirFase()
    {
        faseFinalizada = true;
        AtualizarIndicador();

        if (inputJogador != null)
            inputJogador.DeactivateInput();

        if (painelConclusao != null)
            painelConclusao.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ReiniciarFase()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }
}
