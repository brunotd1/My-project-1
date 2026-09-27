using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class MontarCenaEditor : Editor
{
    [MenuItem("Escola de Jogos/Montar Cena")]
    static void MontarCena()
    {
        // ─────────────────────────────────────────
        // 1. CRIAR O PLANE
        // ─────────────────────────────────────────
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "Plane";
        plane.transform.position = new Vector3(0f, 0f, 0f);
        plane.transform.localScale = new Vector3(10f, 1f, 10f);

        // ─────────────────────────────────────────
        // 2. CRIAR O CUBE acima do Plane
        // ─────────────────────────────────────────
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "Cube";
        cube.transform.position = new Vector3(0f, 1f, 0f); // Y=1 para ficar acima do Plane

        // ─────────────────────────────────────────
        // 3. CRIAR PASTA "cores" em Assets
        // ─────────────────────────────────────────
        if (!AssetDatabase.IsValidFolder("Assets/cores"))
        {
            AssetDatabase.CreateFolder("Assets", "cores");
        }

        // ─────────────────────────────────────────
        // 4. CRIAR MATERIAL VERDE
        // ─────────────────────────────────────────
        Material matVerde = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        matVerde.color = Color.green;
        AssetDatabase.CreateAsset(matVerde, "Assets/cores/verde.mat");

        // ─────────────────────────────────────────
        // 5. CRIAR MATERIAL VERMELHO
        // ─────────────────────────────────────────
        Material matVermelho = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        matVermelho.color = Color.red;
        AssetDatabase.CreateAsset(matVermelho, "Assets/cores/vermelho.mat");

        // ─────────────────────────────────────────
        // 6. APLICAR MATERIAIS NOS OBJETOS
        // ─────────────────────────────────────────
        plane.GetComponent<Renderer>().sharedMaterial = matVerde;
        cube.GetComponent<Renderer>().sharedMaterial  = matVermelho;

        // ─────────────────────────────────────────
        // 7. ADICIONAR RIGIDBODY AO CUBE (física)
        // ─────────────────────────────────────────
        cube.AddComponent<Rigidbody>();

        // ─────────────────────────────────────────
        // 8. POSICIONAR E INCLINAR A CÂMERA
        // ─────────────────────────────────────────
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(0f, 8f, -12f);
            mainCamera.transform.rotation = Quaternion.Euler(25f, 0f, 0f);
        }
        else
        {
            Debug.LogWarning("[Escola de Jogos] Main Camera não encontrada na cena!");
        }

        // ─────────────────────────────────────────
        // 9. SALVAR ASSETS E CENA
        // ─────────────────────────────────────────
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("[Escola de Jogos] Cena montada com sucesso! ✅");
    }
}
