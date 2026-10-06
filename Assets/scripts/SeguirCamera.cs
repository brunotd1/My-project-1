using UnityEngine;

public class SeguirCamera : MonoBehaviour
{
    [Header("Alvo a seguir (Tanque)")]
    public Transform alvo;

    [Header("Posicionamento Fixo no Topo")]
    [Tooltip("Altura (Y) e recuo (Z) da câmera em relação ao tanque")]
    public Vector3 offset = new Vector3(0f, 18f, -4f);

    [Header("Ângulo da Câmera (Top-Down)")]
    [Tooltip("Inclinação fixa para olhar o mapa de cima sem girar")]
    public Vector3 rotacaoFixa = new Vector3(75f, 0f, 0f);

    [Header("Suavização")]
    public float tempoSuave = 0.12f;

    private Vector3 velocidadeAtual = Vector3.zero;

    void Start()
    {
        // ⚠️ REGRA FUNDAMENTAL: A Câmera NUNCA pode ser filha do Tanque,
        // senão ela gira junto quando o tanque faz curvas.
        if (transform.parent != null)
        {
            transform.SetParent(null); // Solta a câmera na raiz da cena automaticamente!
        }
    }

    void LateUpdate()
    {
        if (alvo == null) return;

        // 1. Calcula a posição desejada apenas somando a posição do tanque + offset fixo no MUNDO
        Vector3 posicaoAlvo = alvo.position + offset;

        // 2. Desliza suavemente até essa posição (apenas posição, SEM rotação!)
        transform.position = Vector3.SmoothDamp(transform.position, posicaoAlvo, ref velocidadeAtual, tempoSuave);

        // 3. Trava a rotação da câmera permanentemente (ela NUNCA gira quando o tanque vira)
        transform.rotation = Quaternion.Euler(rotacaoFixa);
    }
}