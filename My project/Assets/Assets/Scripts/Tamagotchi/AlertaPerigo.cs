
using UnityEngine;

public class AlertaPerigo : MonoBehaviour
{
    [Header("Aviso do Otto")]
    [SerializeField]
    private string mensagem =
        "Cuidado! Obstáculo próximo.";

    [SerializeField] private Sprite imagemOtto;

    public string Mensagem => mensagem;
    public Sprite ImagemOtto => imagemOtto;
}
