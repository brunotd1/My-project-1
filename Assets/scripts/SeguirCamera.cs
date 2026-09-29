using UnityEngine;

public class SeguirCamera : MonoBehaviour
{
    [Header("Alvo a seguir (Arraste o Cubo aqui)")]
    public Transform alvo;

    [Header("Posicionamento (X = lados, Y = altura, Z = distância)")]
    public Vector3 offset = new Vector3(0f, 6f, -9f);

    [Header("Suavidade da Câmera")]
    public float velocidadeSuave = 5f;

    void LateUpdate()
    {
        if (alvo == null) return;

        // Calcula a posição ideal atrás do cubo
        Vector3 posicaoDesejada = alvo.position + offset;

        // Interpolação suave para não ficar tremido
        transform.position = Vector3.Lerp(transform.position, posicaoDesejada, velocidadeSuave * Time.deltaTime);

        // Faz a câmera sempre apontar os "olhos" para o cubo
        transform.LookAt(alvo.position + Vector3.up * 0.5f);
    }
}