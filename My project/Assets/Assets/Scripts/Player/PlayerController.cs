using UnityEngine;
using UnityEngine.InputSystem;
    
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float velocidade = 3f;

    private Rigidbody2D rb;
    private Vector2 direcao;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        Move();
    }

    public void OnMove(InputValue value)
    {
        direcao = value.Get<Vector2>();
    }

    void Move()
    {
        rb.linearVelocity = direcao * velocidade;
    }
}
