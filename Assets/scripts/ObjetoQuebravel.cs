using UnityEngine;

public class ObjetoQuebravel : MonoBehaviour
{
    [Header("Configurações da Quebra")]
    [Tooltip("Quantidade de pedaços que vão se espalhar")]
    public int quantidadePedacos = 6;

    [Tooltip("Força com que os pedaços são arremessados")]
    public float forcaQuebra = 5f;

    [Tooltip("Tempo em segundos para os cacos desaparecerem do chão")]
    public float tempoParaSumir = 3f;

    [Header("Configurações de Respawn")]
    [Tooltip("Distância mínima que o jogador precisa se afastar para o cilindro renascer")]
    public float distanciaParaRespawn = 8f;

    private Transform jogador;
    private MeshRenderer meshRenderer;
    private Collider col;
    private bool estaQuebrado = false;

    void Start()
    {
        // Pega os componentes do próprio cilindro
        meshRenderer = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();

        // Encontra o jogador (Tanque) na cena
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            jogador = playerObj.transform;
        }
        else
        {
            TanqueController tanque = Object.FindAnyObjectByType<TanqueController>();
            if (tanque != null) jogador = tanque.transform;
        }
    }

    void Update()
    {
        // Se o objeto estiver quebrado e temos o jogador localizado:
        if (estaQuebrado && jogador != null)
        {
            // Calcula a distância atual entre o jogador e a posição do cilindro
            float distanciaAtual = Vector3.Distance(transform.position, jogador.position);

            // Quando o jogador se afasta além da distância configurada: RESPAWN!
            if (distanciaAtual >= distanciaParaRespawn)
            {
                Respawn();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Só quebra se não estiver já quebrado e se quem bateu foi o jogador
        if (!estaQuebrado && (collision.gameObject.name == "Cube" || collision.gameObject.GetComponent<Movimento>() != null))
        {
            Quebrar();
        }
    }

    public void Quebrar()
    {
        estaQuebrado = true;

        // 1. Invisibiliza e desativa a colisão (o objeto continua existindo para poder renascer!)
        meshRenderer.enabled = false;
        col.enabled = false;

        // 2. Material original para os pedaços
        Material matAtual = meshRenderer.sharedMaterial;

        // 3. Spawna os pedacinhos que caem com física
        for (int i = 0; i < quantidadePedacos; i++)
        {
            GameObject pedaco = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pedaco.name = "Caco_" + i;
            pedaco.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

            Vector3 variacao = new Vector3(
                Random.Range(-0.3f, 0.3f),
                Random.Range(0f, 0.8f),
                Random.Range(-0.3f, 0.3f)
            );
            pedaco.transform.position = transform.position + variacao;

            if (matAtual != null)
            {
                pedaco.GetComponent<Renderer>().sharedMaterial = matAtual;
            }

            Rigidbody rb = pedaco.AddComponent<Rigidbody>();
            Vector3 direcaoExplosao = (variacao + Vector3.up * 0.5f).normalized;
            rb.AddForce(direcaoExplosao * forcaQuebra, ForceMode.Impulse);

            Destroy(pedaco, tempoParaSumir);
        }

        Debug.Log("💥 Quebrou! Afaste-se " + distanciaParaRespawn + " metros para o cilindro dar respawn.");
    }

    public void Respawn()
    {
        estaQuebrado = false;

        // Torna o cilindro visível e sólido novamente!
        meshRenderer.enabled = true;
        col.enabled = true;

        Debug.Log("✨ Cilindro deu Respawn! Pode quebrar de novo.");
    }
}
