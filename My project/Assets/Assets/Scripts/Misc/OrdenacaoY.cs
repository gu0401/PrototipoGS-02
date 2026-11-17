
using UnityEngine;

public class OrdenacaoY : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Transform pontoBase;
    [SerializeField] private int multiplicador = 100;

    void Awake()
    {
        if (sprite == null)
            sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (sprite == null)
            return;

        float posicaoY = pontoBase != null
            ? pontoBase.position.y
            : transform.position.y;

        sprite.sortingOrder = -Mathf.RoundToInt(
            posicaoY * multiplicador
        );
    }
}
