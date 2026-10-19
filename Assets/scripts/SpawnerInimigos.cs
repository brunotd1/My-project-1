using UnityEngine;

public class SpawnerInimigos : MonoBehaviour
{
    [Header("Configurações do Spawner")]
    public GameObject inimigoPrefab;
    public float intervaloSpawn = 5f;
    public int maxInimigosSimultaneos = 3;

    [Header("Pontos de Nascimento")]
    public Transform[] pontosDeSpawn;

    private float proximoSpawn = 0f;

    void Start()
    {
        // Se nenhum ponto foi configurado, cria 3 posições padrão no topo da arena
        if (pontosDeSpawn == null || pontosDeSpawn.Length == 0)
        {
            GameObject p1 = new GameObject("Spawn_Norte_Esquerda");
            p1.transform.position = new Vector3(-8f, 0.4f, 8f);

            GameObject p2 = new GameObject("Spawn_Norte_Centro");
            p2.transform.position = new Vector3(0f, 0.4f, 8f);

            GameObject p3 = new GameObject("Spawn_Norte_Direita");
            p3.transform.position = new Vector3(8f, 0.4f, 8f);

            pontosDeSpawn = new Transform[] { p1.transform, p2.transform, p3.transform };
        }

        proximoSpawn = Time.time + 1.5f; // Primeiro spawn logo no início
    }

    void Update()
    {
        if (inimigoPrefab == null) return;

        // Só faz spawn se não ultrapassou o limite de inimigos vivos na arena
        if (Time.time >= proximoSpawn)
        {
            proximoSpawn = Time.time + intervaloSpawn;

            int inimigosVivos = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (inimigosVivos < maxInimigosSimultaneos)
            {
                SpawnarInimigo();
            }
        }
    }

    private void SpawnarInimigo()
    {
        // Escolhe um ponto de spawn aleatório
        int index = Random.Range(0, pontosDeSpawn.Length);
        Transform spawnPoint = pontosDeSpawn[index];

        if (spawnPoint != null)
        {
            // Spawna o tanque inimigo virado para o Sul (Z-)
            Instantiate(inimigoPrefab, spawnPoint.position, Quaternion.Euler(0f, 180f, 0f));
            Debug.Log("👾 Novo Tanque Inimigo entrou na arena em: " + spawnPoint.name);
        }
    }
}
