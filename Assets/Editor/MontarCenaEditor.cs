using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class MontarCenaEditor : Editor
{
    [MenuItem("Escola de Jogos/Montar Cena Base")]
    static void MontarCena()
    {
        // 1. CRIAR O PLANE
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "Plane";
        plane.transform.position = new Vector3(0f, 0f, 0f);
        plane.transform.localScale = new Vector3(10f, 1f, 10f);

        // 2. CRIAR O CUBE
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "Cube";
        cube.transform.position = new Vector3(0f, 1f, 0f);

        // 3. CRIAR PASTA "cores"
        if (!AssetDatabase.IsValidFolder("Assets/cores"))
        {
            AssetDatabase.CreateFolder("Assets", "cores");
        }

        // 4. MATERIAIS
        Material matVerde = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        matVerde.color = Color.green;
        AssetDatabase.CreateAsset(matVerde, "Assets/cores/verde.mat");

        Material matVermelho = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        matVermelho.color = Color.red;
        AssetDatabase.CreateAsset(matVermelho, "Assets/cores/vermelho.mat");

        plane.GetComponent<Renderer>().sharedMaterial = matVerde;
        cube.GetComponent<Renderer>().sharedMaterial  = matVermelho;
        cube.AddComponent<Rigidbody>();

        // 5. CÂMERA
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(0f, 8f, -12f);
            mainCamera.transform.rotation = Quaternion.Euler(25f, 0f, 0f);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("[Escola de Jogos] Cena base montada com sucesso! ✅");
    }

    [MenuItem("Escola de Jogos/Battle City/Transformar em Tanque e Câmera Aérea")]
    static void MontarBattleCity()
    {
        // ─────────────────────────────────────────
        // 1. LOCALIZAR OU CRIAR O OBJETO DO JOGADOR
        // ─────────────────────────────────────────
        GameObject jogador = GameObject.Find("Cube");
        if (jogador == null) jogador = GameObject.Find("TanqueJogador");
        if (jogador == null)
        {
            jogador = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }

        jogador.name = "TanqueJogador";
        jogador.tag = "Player";
        jogador.transform.position = new Vector3(0f, 0.4f, -3f);
        jogador.transform.rotation = Quaternion.identity;
        jogador.transform.localScale = new Vector3(1.3f, 0.5f, 1.3f); // Chassi achatado

        // Remove scripts antigos de movimento para não conflitar
        Movimento movAntigo = jogador.GetComponent<Movimento>();
        if (movAntigo != null) DestroyImmediate(movAntigo);

        // Rigidbody travado para física sólida
        Rigidbody rb = jogador.GetComponent<Rigidbody>();
        if (rb == null) rb = jogador.AddComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Limpa partes visuais antigas se houver
        Transform torreAntiga = jogador.transform.Find("Torre");
        if (torreAntiga != null) DestroyImmediate(torreAntiga.gameObject);

        // ─────────────────────────────────────────
        // 2. CRIAR A TORRE DO TANQUE (cubo central)
        // ─────────────────────────────────────────
        GameObject torre = GameObject.CreatePrimitive(PrimitiveType.Cube);
        torre.name = "Torre";
        torre.transform.SetParent(jogador.transform);
        torre.transform.localPosition = new Vector3(0f, 0.65f, 0f);
        torre.transform.localRotation = Quaternion.identity;
        torre.transform.localScale = new Vector3(0.6f, 0.45f, 0.6f);
        Collider colTorre = torre.GetComponent<Collider>();
        if (colTorre != null) DestroyImmediate(colTorre);

        // ─────────────────────────────────────────
        // 3. CRIAR O CANHÃO (cilindro virado para a frente)
        // ─────────────────────────────────────────
        GameObject canhao = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        canhao.name = "Canhao";
        canhao.transform.SetParent(torre.transform);
        canhao.transform.localPosition = new Vector3(0f, 0f, 0.7f);
        canhao.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        canhao.transform.localScale = new Vector3(0.22f, 0.6f, 0.22f);
        Collider colCanhao = canhao.GetComponent<Collider>();
        if (colCanhao != null) DestroyImmediate(colCanhao);

        // ─────────────────────────────────────────
        // 4. PONTO DE DISPARO (na ponta do cano)
        // ─────────────────────────────────────────
        GameObject pontoDisparo = new GameObject("PontoDisparo");
        pontoDisparo.transform.SetParent(torre.transform);
        pontoDisparo.transform.localPosition = new Vector3(0f, 0f, 1.4f);

        // ─────────────────────────────────────────
        // 5. CORES: AMARELO CLÁSSICO DO BATTLE CITY
        // ─────────────────────────────────────────
        if (!AssetDatabase.IsValidFolder("Assets/cores")) AssetDatabase.CreateFolder("Assets", "cores");

        Material matTanque = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        Color amareloClassico = new Color(0.95f, 0.78f, 0.15f);
        matTanque.color = amareloClassico;
        if (matTanque.HasProperty("_BaseColor")) matTanque.SetColor("_BaseColor", amareloClassico);
        AssetDatabase.CreateAsset(matTanque, "Assets/cores/tanque_amarelo.mat");

        jogador.GetComponent<Renderer>().sharedMaterial = matTanque;
        torre.GetComponent<Renderer>().sharedMaterial = matTanque;

        Material matCanhao = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        Color cinzaEscuro = new Color(0.2f, 0.2f, 0.22f);
        matCanhao.color = cinzaEscuro;
        if (matCanhao.HasProperty("_BaseColor")) matCanhao.SetColor("_BaseColor", cinzaEscuro);
        AssetDatabase.CreateAsset(matCanhao, "Assets/cores/canhao_cinza.mat");
        canhao.GetComponent<Renderer>().sharedMaterial = matCanhao;

        // ─────────────────────────────────────────
        // 6. CONTROLE DO TANQUE E TIRO
        // ─────────────────────────────────────────
        TanqueController controller = jogador.GetComponent<TanqueController>();
        if (controller == null) controller = jogador.AddComponent<TanqueController>();
        controller.pontoDisparo = pontoDisparo.transform;
        controller.velocidade = 6f;
        controller.velocidadeGiro = 900f;

        // ─────────────────────────────────────────
        // 7. CÂMERA AÉREA TOP-DOWN (ESTILO BATTLE CITY)
        // ─────────────────────────────────────────
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(null); // Garante que a câmera NUNCA fique dentro do tanque!

            SeguirCamera seguir = mainCamera.GetComponent<SeguirCamera>();
            if (seguir == null) seguir = mainCamera.gameObject.AddComponent<SeguirCamera>();

            seguir.alvo = jogador.transform;
            seguir.offset = new Vector3(0f, 18f, -4f); // Vista de cima fixa
            seguir.rotacaoFixa = new Vector3(75f, 0f, 0f);
            seguir.tempoSuave = 0.12f;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("[Battle City] Tanque do Jogador e Câmera Aérea configurados com sucesso! 🎖️🚀");
    }

    [MenuItem("Escola de Jogos/Destravar e Corrigir Câmera")]
    static void DestravarCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) cam = Object.FindAnyObjectByType<Camera>();

        if (cam != null)
        {
            cam.transform.SetParent(null); // Tira de dentro do tanque!

            GameObject tanque = GameObject.Find("TanqueJogador");
            if (tanque == null) tanque = GameObject.Find("Cube");

            SeguirCamera seguir = cam.GetComponent<SeguirCamera>();
            if (seguir == null) seguir = cam.gameObject.AddComponent<SeguirCamera>();

            if (tanque != null)
            {
                seguir.alvo = tanque.transform;
                cam.transform.position = tanque.transform.position + new Vector3(0f, 18f, -4f);
            }
            else
            {
                cam.transform.position = new Vector3(0f, 18f, -4f);
            }

            cam.transform.rotation = Quaternion.Euler(75f, 0f, 0f);
            seguir.offset = new Vector3(0f, 18f, -4f);
            seguir.rotacaoFixa = new Vector3(75f, 0f, 0f);
            seguir.tempoSuave = 0.12f;

            EditorSceneManager.SaveOpenScenes();
            Debug.Log("[Câmera] Câmera destravada e reposicionada no topo com sucesso! 🎥✅");
        }
    }

    [MenuItem("Escola de Jogos/Battle City/Criar Prefab da Bala (com Rastro)")]
    static void CriarPrefabBala()
    {
        // 1. Criar pasta Prefabs se não existir
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // 2. Criar o objeto temporário da bala
        GameObject balaObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        balaObj.name = "BalaTanque";
        balaObj.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

        // 3. Material amarelo brilhante
        if (!AssetDatabase.IsValidFolder("Assets/cores")) AssetDatabase.CreateFolder("Assets", "cores");
        Material matBala = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        Color amareloTiro = new Color(1f, 0.85f, 0.1f);
        matBala.color = amareloTiro;
        if (matBala.HasProperty("_BaseColor")) matBala.SetColor("_BaseColor", amareloTiro);
        AssetDatabase.CreateAsset(matBala, "Assets/cores/bala_brilhante.mat");
        balaObj.GetComponent<Renderer>().sharedMaterial = matBala;

        // 4. Collider como Trigger
        Collider col = balaObj.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        // 5. Rigidbody kinematic
        Rigidbody rb = balaObj.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // 6. TrailRenderer (rastro arcade clássico!)
        TrailRenderer trail = balaObj.AddComponent<TrailRenderer>();
        trail.time = 0.2f;
        trail.startWidth = 0.25f;
        trail.endWidth = 0.02f;
        trail.material = matBala;

        // 7. Script Projetil
        balaObj.AddComponent<Projetil>();

        // 8. Salvar como Prefab no disco!
        string prefabPath = "Assets/Prefabs/BalaTanque.prefab";
        GameObject prefabSalvo = PrefabUtility.SaveAsPrefabAsset(balaObj, prefabPath);

        // Remove o objeto temporário da cena
        DestroyImmediate(balaObj);

        // 9. Conectar automaticamente no TanqueJogador
        GameObject tanque = GameObject.Find("TanqueJogador");
        if (tanque == null) tanque = GameObject.Find("Cube");
        if (tanque != null)
        {
            TanqueController controller = tanque.GetComponent<TanqueController>();
            if (controller != null)
            {
                controller.balaPrefab = prefabSalvo;
                EditorUtility.SetDirty(controller);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("[Battle City] 🚀 Prefab criado com sucesso em Assets/Prefabs/BalaTanque.prefab e vinculado ao Tanque!");
    }
}
