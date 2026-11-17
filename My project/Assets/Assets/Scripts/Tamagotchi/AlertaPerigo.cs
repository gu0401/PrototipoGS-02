using UnityEngine;

public class AlertaPerigo : MonoBehaviour
{
    [SerializeField] private string mensagem = "Cuidado! Obstáculo próximo.";

    public string Mensagem => mensagem;
}