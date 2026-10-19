using UnityEngine;

public class Vida : MonoBehaviour
{
    [Header("Configuração de Vida")]
    public bool ehJogador = false;
    public int vidaMaxima = 1;
    public int pontosAoMorrer = 100;

    private int vidaAtual;
    private bool estaMorto = false;

    void Start()
    {
        vidaAtual = vidaMaxima;
    }

    public void ReceberDano(int quantidade)
    {
        if (estaMorto) return;

        vidaAtual -= quantidade;
        Debug.Log("💥 " + gameObject.name + " recebeu dano! Vida restante: " + vidaAtual);

        if (ehJogador)
        {
            if (GameManager.Instancia != null)
            {
                GameManager.Instancia.TomarDanoJogador();
            }
        }

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    private void Morrer()
    {
        estaMorto = true;

        if (!ehJogador)
        {
            // Adiciona pontos no placar
            if (GameManager.Instancia != null)
            {
                GameManager.Instancia.AdicionarPontos(pontosAoMorrer);
            }

            // Cria efeito de explosão com pequenos cacos azuis
            CriarExplosao();

            // Destrói o tanque inimigo
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("💀 Tanque do Jogador destruído!");
            CriarExplosao();
            gameObject.SetActive(false);
        }
    }

    private void CriarExplosao()
    {
        Material matAtual = null;
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null) matAtual = rend.sharedMaterial;

        // Spawna 8 cacos que voam com física
        for (int i = 0; i < 8; i++)
        {
            GameObject caco = GameObject.CreatePrimitive(PrimitiveType.Cube);
            caco.name = "Explosao_Caco";
            caco.transform.position = transform.position + new Vector3(
                Random.Range(-0.4f, 0.4f),
                Random.Range(0.2f, 0.7f),
                Random.Range(-0.4f, 0.4f)
            );
            caco.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

            if (matAtual != null) caco.GetComponent<Renderer>().sharedMaterial = matAtual;

            Rigidbody rb = caco.AddComponent<Rigidbody>();
            Vector3 forca = (caco.transform.position - transform.position + Vector3.up * 0.8f).normalized * 6f;
            rb.AddForce(forca, ForceMode.Impulse);

            Destroy(caco, 3f);
        }
    }
}
