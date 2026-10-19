using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using UnityEngine.UI;

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

    [MenuItem("Escola de Jogos/Battle City/Configurar Inimigos, Spawner e Placar UI")]
    static void ConfigurarInimigosESpawnerEPlacar()
    {
        // ─────────────────────────────────────────
        // 1. MATERIAIS: TANQUE AZUL E BALA VERMELHA
        // ─────────────────────────────────────────
        if (!AssetDatabase.IsValidFolder("Assets/cores")) AssetDatabase.CreateFolder("Assets", "cores");
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) AssetDatabase.CreateFolder("Assets", "Prefabs");

        Material matInimigo = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        Color azulClassico = new Color(0.15f, 0.45f, 0.95f);
        matInimigo.color = azulClassico;
        if (matInimigo.HasProperty("_BaseColor")) matInimigo.SetColor("_BaseColor", azulClassico);
        AssetDatabase.CreateAsset(matInimigo, "Assets/cores/tanque_azul.mat");

        Material matBalaVermelha = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        Color vermelhoTiro = new Color(1f, 0.2f, 0.2f);
        matBalaVermelha.color = vermelhoTiro;
        if (matBalaVermelha.HasProperty("_BaseColor")) matBalaVermelha.SetColor("_BaseColor", vermelhoTiro);
        AssetDatabase.CreateAsset(matBalaVermelha, "Assets/cores/bala_vermelha.mat");

        // ─────────────────────────────────────────
        // 2. CRIAR PREFAB DA BALA INIMIGA
        // ─────────────────────────────────────────
        GameObject balaInimigaObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        balaInimigaObj.name = "BalaInimiga";
        balaInimigaObj.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
        balaInimigaObj.GetComponent<Renderer>().sharedMaterial = matBalaVermelha;

        Collider colBala = balaInimigaObj.GetComponent<Collider>();
        if (colBala != null) colBala.isTrigger = true;

        Rigidbody rbBala = balaInimigaObj.AddComponent<Rigidbody>();
        rbBala.isKinematic = true;
        rbBala.useGravity = false;

        TrailRenderer trailBala = balaInimigaObj.AddComponent<TrailRenderer>();
        trailBala.time = 0.2f;
        trailBala.startWidth = 0.25f;
        trailBala.endWidth = 0.02f;
        trailBala.material = matBalaVermelha;

        Projetil projInimigo = balaInimigaObj.AddComponent<Projetil>();
        projInimigo.ehTiroInimigo = true;
        projInimigo.velocidade = 14f;

        string pathBalaInimiga = "Assets/Prefabs/BalaInimiga.prefab";
        GameObject prefabBalaInimigaSalva = PrefabUtility.SaveAsPrefabAsset(balaInimigaObj, pathBalaInimiga);
        DestroyImmediate(balaInimigaObj);

        // ─────────────────────────────────────────
        // 3. CRIAR PREFAB DO TANQUE INIMIGO AZUL
        // ─────────────────────────────────────────
        GameObject inimigoObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        inimigoObj.name = "TanqueInimigo";
        inimigoObj.tag = "Enemy";
        inimigoObj.transform.localScale = new Vector3(1.3f, 0.5f, 1.3f);
        inimigoObj.GetComponent<Renderer>().sharedMaterial = matInimigo;

        Rigidbody rbInimigo = inimigoObj.AddComponent<Rigidbody>();
        rbInimigo.freezeRotation = true;

        // Torre
        GameObject torreInimiga = GameObject.CreatePrimitive(PrimitiveType.Cube);
        torreInimiga.name = "Torre";
        torreInimiga.transform.SetParent(inimigoObj.transform);
        torreInimiga.transform.localPosition = new Vector3(0f, 0.65f, 0f);
        torreInimiga.transform.localScale = new Vector3(0.6f, 0.45f, 0.6f);
        torreInimiga.GetComponent<Renderer>().sharedMaterial = matInimigo;
        Collider colTorre = torreInimiga.GetComponent<Collider>();
        if (colTorre != null) DestroyImmediate(colTorre);

        // Canhão
        Material matCanhao = AssetDatabase.LoadAssetAtPath<Material>("Assets/cores/canhao_cinza.mat");
        GameObject canhaoInimigo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        canhaoInimigo.name = "Canhao";
        canhaoInimigo.transform.SetParent(torreInimiga.transform);
        canhaoInimigo.transform.localPosition = new Vector3(0f, 0f, 0.7f);
        canhaoInimigo.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        canhaoInimigo.transform.localScale = new Vector3(0.22f, 0.6f, 0.22f);
        if (matCanhao != null) canhaoInimigo.GetComponent<Renderer>().sharedMaterial = matCanhao;
        Collider colCanhao = canhaoInimigo.GetComponent<Collider>();
        if (colCanhao != null) DestroyImmediate(colCanhao);

        // Ponto de Disparo
        GameObject pontoDisparo = new GameObject("PontoDisparo");
        pontoDisparo.transform.SetParent(torreInimiga.transform);
        pontoDisparo.transform.localPosition = new Vector3(0f, 0f, 1.4f);

        // Scripts da IA e de Vida
        InimigoController inimigoCtrl = inimigoObj.AddComponent<InimigoController>();
        inimigoCtrl.pontoDisparo = pontoDisparo.transform;
        inimigoCtrl.balaInimigaPrefab = prefabBalaInimigaSalva;
        inimigoCtrl.velocidade = 4f;

        Vida vidaInimigo = inimigoObj.AddComponent<Vida>();
        vidaInimigo.ehJogador = false;
        vidaInimigo.vidaMaxima = 1;
        vidaInimigo.pontosAoMorrer = 100;

        string pathTanqueInimigo = "Assets/Prefabs/TanqueInimigo.prefab";
        GameObject prefabTanqueInimigoSalvo = PrefabUtility.SaveAsPrefabAsset(inimigoObj, pathTanqueInimigo);
        DestroyImmediate(inimigoObj);

        // ─────────────────────────────────────────
        // 4. CONFIGURAR VIDA DO JOGADOR
        // ─────────────────────────────────────────
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) player = GameObject.Find("TanqueJogador");
        if (player != null)
        {
            Vida vidaPlayer = player.GetComponent<Vida>();
            if (vidaPlayer == null) vidaPlayer = player.AddComponent<Vida>();
            vidaPlayer.ehJogador = true;
            vidaPlayer.vidaMaxima = 3;
            EditorUtility.SetDirty(player);
        }

        // ─────────────────────────────────────────
        // 5. CRIAR INTERFACE (CANVAS UI DE PONTOS E VIDAS)
        // ─────────────────────────────────────────
        GameObject canvasObj = GameObject.Find("Canvas");
        if (canvasObj == null)
        {
            canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Texto de Pontos
        Transform tPontos = canvasObj.transform.Find("TextoPontos");
        GameObject goPontos = (tPontos != null) ? tPontos.gameObject : new GameObject("TextoPontos");
        goPontos.transform.SetParent(canvasObj.transform, false);
        TextMeshProUGUI txtPontos = goPontos.GetComponent<TextMeshProUGUI>();
        if (txtPontos == null) txtPontos = goPontos.AddComponent<TextMeshProUGUI>();
        txtPontos.text = "PONTOS: 0000";
        txtPontos.fontSize = 28;
        txtPontos.color = Color.white;
        RectTransform rtPontos = txtPontos.rectTransform;
        rtPontos.anchorMin = new Vector2(0f, 1f);
        rtPontos.anchorMax = new Vector2(0f, 1f);
        rtPontos.pivot = new Vector2(0f, 1f);
        rtPontos.anchoredPosition = new Vector2(30f, -30f);
        rtPontos.sizeDelta = new Vector2(300f, 50f);

        // Texto de Vidas
        Transform tVidas = canvasObj.transform.Find("TextoVidas");
        GameObject goVidas = (tVidas != null) ? tVidas.gameObject : new GameObject("TextoVidas");
        goVidas.transform.SetParent(canvasObj.transform, false);
        TextMeshProUGUI txtVidas = goVidas.GetComponent<TextMeshProUGUI>();
        if (txtVidas == null) txtVidas = goVidas.AddComponent<TextMeshProUGUI>();
        txtVidas.text = "VIDAS: ❤️ ❤️ ❤️";
        txtVidas.fontSize = 28;
        txtVidas.color = Color.white;
        RectTransform rtVidas = txtVidas.rectTransform;
        rtVidas.anchorMin = new Vector2(1f, 1f);
        rtVidas.anchorMax = new Vector2(1f, 1f);
        rtVidas.pivot = new Vector2(1f, 1f);
        rtVidas.anchoredPosition = new Vector2(-30f, -30f);
        rtVidas.sizeDelta = new Vector2(300f, 50f);

        // GameManager na cena
        GameObject gmObj = GameObject.Find("GameManager");
        if (gmObj == null) gmObj = new GameObject("GameManager");
        GameManager gm = gmObj.GetComponent<GameManager>();
        if (gm == null) gm = gmObj.AddComponent<GameManager>();
        gm.textoPontos = txtPontos;
        gm.textoVidas = txtVidas;
        gm.vidasJogador = 3;
        gm.pontuacao = 0;
        EditorUtility.SetDirty(gmObj);

        // ─────────────────────────────────────────
        // 6. CRIAR O SPAWNER DE INIMIGOS
        // ─────────────────────────────────────────
        GameObject spawnerObj = GameObject.Find("SpawnerInimigos");
        if (spawnerObj == null) spawnerObj = new GameObject("SpawnerInimigos");
        SpawnerInimigos spawner = spawnerObj.GetComponent<SpawnerInimigos>();
        if (spawner == null) spawner = spawnerObj.AddComponent<SpawnerInimigos>();
        spawner.inimigoPrefab = prefabTanqueInimigoSalvo;
        spawner.intervaloSpawn = 4f;
        spawner.maxInimigosSimultaneos = 3;
        EditorUtility.SetDirty(spawnerObj);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("[Battle City] 👾 Tanque Inimigo Azul, Spawner e Placar UI configurados com sucesso! 🏆");
    }

    [MenuItem("Escola de Jogos/Battle City/Configurar Menu Iniciar e Game Over")]
    static void ConfigurarMenuIniciarEGameOver()
    {
        // 1. EVENT SYSTEM (Obrigatório para cliques de mouse e botões de UI no Unity!)
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // 2. CANVAS
        GameObject canvasObj = GameObject.Find("Canvas");
        if (canvasObj == null)
        {
            canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // 2. PAINEL HUD (Pontos e Vidas no topo durante o jogo)
        Transform tHUD = canvasObj.transform.Find("PainelHUD");
        GameObject painelHUD = (tHUD != null) ? tHUD.gameObject : new GameObject("PainelHUD");
        painelHUD.transform.SetParent(canvasObj.transform, false);
        RectTransform rtHUD = painelHUD.GetComponent<RectTransform>();
        if (rtHUD == null) rtHUD = painelHUD.AddComponent<RectTransform>();
        rtHUD.anchorMin = Vector2.zero;
        rtHUD.anchorMax = Vector2.one;
        rtHUD.offsetMin = Vector2.zero;
        rtHUD.offsetMax = Vector2.zero;

        // Move Textos de Pontos e Vidas para dentro do HUD se estiverem soltos
        Transform tPontos = canvasObj.transform.Find("TextoPontos");
        if (tPontos != null) tPontos.SetParent(painelHUD.transform, false);
        Transform tVidas = canvasObj.transform.Find("TextoVidas");
        if (tVidas != null) tVidas.SetParent(painelHUD.transform, false);

        // 3. PAINEL MENU INICIAR
        Transform tMenu = canvasObj.transform.Find("PainelMenuInicial");
        GameObject painelMenu = (tMenu != null) ? tMenu.gameObject : new GameObject("PainelMenuInicial");
        painelMenu.transform.SetParent(canvasObj.transform, false);
        RectTransform rtMenu = painelMenu.GetComponent<RectTransform>();
        if (rtMenu == null) rtMenu = painelMenu.AddComponent<RectTransform>();
        rtMenu.anchorMin = Vector2.zero;
        rtMenu.anchorMax = Vector2.one;
        rtMenu.offsetMin = Vector2.zero;
        rtMenu.offsetMax = Vector2.zero;
        Image imgMenu = painelMenu.GetComponent<Image>();
        if (imgMenu == null) imgMenu = painelMenu.AddComponent<Image>();
        imgMenu.color = new Color(0.04f, 0.04f, 0.07f, 0.94f);

        // Título do Jogo
        Transform tTitulo = painelMenu.transform.Find("Titulo");
        GameObject goTitulo = (tTitulo != null) ? tTitulo.gameObject : new GameObject("Titulo");
        goTitulo.transform.SetParent(painelMenu.transform, false);
        TextMeshProUGUI txtTitulo = goTitulo.GetComponent<TextMeshProUGUI>();
        if (txtTitulo == null) txtTitulo = goTitulo.AddComponent<TextMeshProUGUI>();
        txtTitulo.text = "BATTLE CITY 3D";
        txtTitulo.fontSize = 54;
        txtTitulo.fontStyle = FontStyles.Bold;
        txtTitulo.alignment = TextAlignmentOptions.Center;
        txtTitulo.color = new Color(1f, 0.85f, 0.1f);
        RectTransform rtTitulo = txtTitulo.rectTransform;
        rtTitulo.anchoredPosition = new Vector2(0f, 90f);
        rtTitulo.sizeDelta = new Vector2(600f, 80f);

        // Subtítulo
        Transform tSub = painelMenu.transform.Find("Subtitulo");
        GameObject goSub = (tSub != null) ? tSub.gameObject : new GameObject("Subtitulo");
        goSub.transform.SetParent(painelMenu.transform, false);
        TextMeshProUGUI txtSub = goSub.GetComponent<TextMeshProUGUI>();
        if (txtSub == null) txtSub = goSub.AddComponent<TextMeshProUGUI>();
        txtSub.text = "Destrua os tanques azuis e sobreviva!";
        txtSub.fontSize = 20;
        txtSub.alignment = TextAlignmentOptions.Center;
        txtSub.color = Color.white;
        RectTransform rtSub = txtSub.rectTransform;
        rtSub.anchoredPosition = new Vector2(0f, 30f);
        rtSub.sizeDelta = new Vector2(600f, 40f);

        // Botão Iniciar Jogo
        Transform tBtnIniciar = painelMenu.transform.Find("BotaoIniciar");
        GameObject goBtnIniciar = (tBtnIniciar != null) ? tBtnIniciar.gameObject : new GameObject("BotaoIniciar");
        goBtnIniciar.transform.SetParent(painelMenu.transform, false);
        Image imgBtnIniciar = goBtnIniciar.GetComponent<Image>();
        if (imgBtnIniciar == null) imgBtnIniciar = goBtnIniciar.AddComponent<Image>();
        imgBtnIniciar.color = new Color(0.18f, 0.72f, 0.28f);
        Button btnIniciar = goBtnIniciar.GetComponent<Button>();
        if (btnIniciar == null) btnIniciar = goBtnIniciar.AddComponent<Button>();
        RectTransform rtBtnIniciar = goBtnIniciar.GetComponent<RectTransform>();
        rtBtnIniciar.anchoredPosition = new Vector2(0f, -50f);
        rtBtnIniciar.sizeDelta = new Vector2(250f, 60f);

        Transform tTxtBtn = goBtnIniciar.transform.Find("Text");
        GameObject goTxtBtn = (tTxtBtn != null) ? tTxtBtn.gameObject : new GameObject("Text");
        goTxtBtn.transform.SetParent(goBtnIniciar.transform, false);
        TextMeshProUGUI txtBtn = goTxtBtn.GetComponent<TextMeshProUGUI>();
        if (txtBtn == null) txtBtn = goTxtBtn.AddComponent<TextMeshProUGUI>();
        txtBtn.text = "INICIAR JOGO";
        txtBtn.fontSize = 22;
        txtBtn.fontStyle = FontStyles.Bold;
        txtBtn.alignment = TextAlignmentOptions.Center;
        txtBtn.color = Color.white;
        RectTransform rtTxtBtn = txtBtn.rectTransform;
        rtTxtBtn.anchorMin = Vector2.zero;
        rtTxtBtn.anchorMax = Vector2.one;
        rtTxtBtn.offsetMin = Vector2.zero;
        rtTxtBtn.offsetMax = Vector2.zero;

        // 4. PAINEL GAME OVER
        Transform tGO = canvasObj.transform.Find("PainelGameOver");
        GameObject painelGO = (tGO != null) ? tGO.gameObject : new GameObject("PainelGameOver");
        painelGO.transform.SetParent(canvasObj.transform, false);
        RectTransform rtGO = painelGO.GetComponent<RectTransform>();
        if (rtGO == null) rtGO = painelGO.AddComponent<RectTransform>();
        rtGO.anchorMin = Vector2.zero;
        rtGO.anchorMax = Vector2.one;
        rtGO.offsetMin = Vector2.zero;
        rtGO.offsetMax = Vector2.zero;
        Image imgGO = painelGO.GetComponent<Image>();
        if (imgGO == null) imgGO = painelGO.AddComponent<Image>();
        imgGO.color = new Color(0.12f, 0.02f, 0.02f, 0.95f);

        // Título Game Over
        Transform tTituloGO = painelGO.transform.Find("TituloGO");
        GameObject goTituloGO = (tTituloGO != null) ? tTituloGO.gameObject : new GameObject("TituloGO");
        goTituloGO.transform.SetParent(painelGO.transform, false);
        TextMeshProUGUI txtTituloGO = goTituloGO.GetComponent<TextMeshProUGUI>();
        if (txtTituloGO == null) txtTituloGO = goTituloGO.AddComponent<TextMeshProUGUI>();
        txtTituloGO.text = "GAME OVER";
        txtTituloGO.fontSize = 60;
        txtTituloGO.fontStyle = FontStyles.Bold;
        txtTituloGO.alignment = TextAlignmentOptions.Center;
        txtTituloGO.color = new Color(1f, 0.2f, 0.2f);
        RectTransform rtTituloGO = txtTituloGO.rectTransform;
        rtTituloGO.anchoredPosition = new Vector2(0f, 90f);
        rtTituloGO.sizeDelta = new Vector2(600f, 80f);

        // Pontuação Final Conquistada
        Transform tPtsFinal = painelGO.transform.Find("TextoPontosFinal");
        GameObject goPtsFinal = (tPtsFinal != null) ? tPtsFinal.gameObject : new GameObject("TextoPontosFinal");
        goPtsFinal.transform.SetParent(painelGO.transform, false);
        TextMeshProUGUI txtPtsFinal = goPtsFinal.GetComponent<TextMeshProUGUI>();
        if (txtPtsFinal == null) txtPtsFinal = goPtsFinal.AddComponent<TextMeshProUGUI>();
        txtPtsFinal.text = "PONTUAÇÃO FINAL: 0000";
        txtPtsFinal.fontSize = 26;
        txtPtsFinal.alignment = TextAlignmentOptions.Center;
        txtPtsFinal.color = Color.white;
        RectTransform rtPtsFinal = txtPtsFinal.rectTransform;
        rtPtsFinal.anchoredPosition = new Vector2(0f, 25f);
        rtPtsFinal.sizeDelta = new Vector2(600f, 40f);

        // Botão Jogar Novamente / Reiniciar
        Transform tBtnReiniciar = painelGO.transform.Find("BotaoReiniciar");
        GameObject goBtnReiniciar = (tBtnReiniciar != null) ? tBtnReiniciar.gameObject : new GameObject("BotaoReiniciar");
        goBtnReiniciar.transform.SetParent(painelGO.transform, false);
        Image imgBtnReiniciar = goBtnReiniciar.GetComponent<Image>();
        if (imgBtnReiniciar == null) imgBtnReiniciar = goBtnReiniciar.AddComponent<Image>();
        imgBtnReiniciar.color = new Color(0.92f, 0.5f, 0.12f);
        Button btnReiniciar = goBtnReiniciar.GetComponent<Button>();
        if (btnReiniciar == null) btnReiniciar = goBtnReiniciar.AddComponent<Button>();
        RectTransform rtBtnReiniciar = goBtnReiniciar.GetComponent<RectTransform>();
        rtBtnReiniciar.anchoredPosition = new Vector2(0f, -50f);
        rtBtnReiniciar.sizeDelta = new Vector2(260f, 60f);

        Transform tTxtBtnR = goBtnReiniciar.transform.Find("Text");
        GameObject goTxtBtnR = (tTxtBtnR != null) ? tTxtBtnR.gameObject : new GameObject("Text");
        goTxtBtnR.transform.SetParent(goBtnReiniciar.transform, false);
        TextMeshProUGUI txtBtnR = goTxtBtnR.GetComponent<TextMeshProUGUI>();
        if (txtBtnR == null) txtBtnR = goTxtBtnR.AddComponent<TextMeshProUGUI>();
        txtBtnR.text = "JOGAR NOVAMENTE";
        txtBtnR.fontSize = 20;
        txtBtnR.fontStyle = FontStyles.Bold;
        txtBtnR.alignment = TextAlignmentOptions.Center;
        txtBtnR.color = Color.white;
        RectTransform rtTxtBtnR = txtBtnR.rectTransform;
        rtTxtBtnR.anchorMin = Vector2.zero;
        rtTxtBtnR.anchorMax = Vector2.one;
        rtTxtBtnR.offsetMin = Vector2.zero;
        rtTxtBtnR.offsetMax = Vector2.zero;

        // 5. CONECTAR TUDO NO GAMEMANAGER
        GameObject gmObj = GameObject.Find("GameManager");
        if (gmObj != null)
        {
            GameManager gm = gmObj.GetComponent<GameManager>();
            if (gm != null)
            {
                gm.painelMenuInicial = painelMenu;
                gm.painelHUD = painelHUD;
                gm.painelGameOver = painelGO;
                gm.botaoIniciar = btnIniciar;
                gm.botaoReiniciar = btnReiniciar;
                gm.textoPontosFinal = txtPtsFinal;

                Transform pTexto = painelHUD.transform.Find("TextoPontos");
                if (pTexto != null) gm.textoPontos = pTexto.GetComponent<TextMeshProUGUI>();
                Transform vTexto = painelHUD.transform.Find("TextoVidas");
                if (vTexto != null) gm.textoVidas = vTexto.GetComponent<TextMeshProUGUI>();

                EditorUtility.SetDirty(gmObj);
            }
        }

        // Estado inicial da tela
        painelMenu.SetActive(true);
        painelHUD.SetActive(false);
        painelGO.SetActive(false);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("[Battle City] 🎮 Menu Iniciar e Game Over com Reinício configurados com sucesso! 🏆");
    }
}
