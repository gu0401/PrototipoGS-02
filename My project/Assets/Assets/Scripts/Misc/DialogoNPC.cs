
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogoNPC : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private DialogoManager gerenciador;
    [SerializeField] private Text textoInteragir;

    [Header("Diálogo")]
    [SerializeField] private DialogoManager.Fala[] falas;

    private bool jogadorPerto;

    void Start()
    {
        AtualizarIndicador();
    }

    void Update()
    {
        if (gerenciador == null)
            return;

        if (jogadorPerto &&
            !gerenciador.EmDialogo &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            gerenciador.IniciarDialogo(falas);
        }

        AtualizarIndicador();
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.GetComponentInParent<PlayerController>() != null)
        {
            jogadorPerto = true;
            AtualizarIndicador();
        }
    }

    void OnTriggerExit2D(Collider2D outro)
    {
        if (outro.GetComponentInParent<PlayerController>() != null)
        {
            jogadorPerto = false;
            AtualizarIndicador();
        }
    }

    void AtualizarIndicador()
    {
        if (textoInteragir == null)
            return;

        bool mostrar = jogadorPerto &&
                       gerenciador != null &&
                       !gerenciador.EmDialogo;

        textoInteragir.gameObject.SetActive(mostrar);
    }
}
