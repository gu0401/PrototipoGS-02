using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Cena Inicial")]
    [SerializeField] private string cenaInicial = "Cutscene 1";

    [Header("Audio dos Botoes")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip somClique;
    [SerializeField, Range(0f, 1f)]
    private float volumeClique = 0.5f;

    private bool carregando;

    void Awake()
    {
        Time.timeScale = 1f;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void Jogar()
    {
        if (carregando)
            return;

        if (string.IsNullOrWhiteSpace(cenaInicial))
        {
            Debug.LogError("Nome da cena inicial esta vazio no MenuPrincipal!");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(cenaInicial))
        {
            Debug.LogError(
                "A cena '" + cenaInicial +
                "' nao foi encontrada na Build Settings (Scenes In Build)!"
            );
            return;
        }

        StartCoroutine(IniciarJogo());
    }

    IEnumerator IniciarJogo()
    {
        carregando = true;
        TocarClique();

        yield return new WaitForSecondsRealtime(0.15f);

        SceneManager.LoadScene(cenaInicial);
    }

    public void Sair()
    {
        if (carregando)
            return;

        StartCoroutine(FecharJogo());
    }

    IEnumerator FecharJogo()
    {
        carregando = true;
        TocarClique();

        yield return new WaitForSecondsRealtime(0.15f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void TocarClique()
    {
        if (audioSource != null && somClique != null)
            audioSource.PlayOneShot(somClique, volumeClique);
    }
}