using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Movimento : MonoBehaviour
{
    [Header("Velocidade do Cubo")]
    public float velocidade = 5f;

    void Update()
    {
        float mover_x = 0f;
        float mover_z = 0f;

        // 1. Suporte ao Novo Input System (Padrão do Unity 6)
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) mover_z += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) mover_z -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) mover_x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) mover_x += 1f;
        }
#endif

        // 2. Suporte ao Sistema Antigo
#if ENABLE_LEGACY_INPUT_MANAGER
        mover_x += Input.GetAxis("Horizontal");
        mover_z += Input.GetAxis("Vertical");
#endif

        Vector3 direcao = new Vector3(mover_x, 0f, mover_z).normalized;
        transform.Translate(direcao * velocidade * Time.deltaTime, Space.World);
    }

    // 💥 DETECÇÃO DE COLISÃO
    private void OnCollisionEnter(Collision collision)
    {
        // Se bater no chão (Plane), não faz nada
        if (collision.gameObject.name == "Plane") return;

        // Mensagem de impacto no Console!
        Debug.Log("💥 Colisão detectada com: " + collision.gameObject.name);
    }
}