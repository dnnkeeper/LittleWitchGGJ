using System.IO;
using UnityEditor;
using UnityEngine;

public class MaterialsUtility : EditorWindow
{
    [MenuItem("Assets/Materials/Change URP Lit to Simple Lit (Selected Materials)")]
    private static void ChangeSelectedMaterialsToSimpleLit()
    {
        Material[] selectedMaterials = Selection.GetFiltered<Material>(SelectionMode.DeepAssets);

        foreach (Material material in selectedMaterials)
        {
            if (material.shader.name.Contains("Universal Render Pipeline/Lit"))
            {
                // Change the shader to Simple Lit
                Shader simpleLitShader = Shader.Find("Universal Render Pipeline/Simple Lit");
                if (simpleLitShader != null)
                {
                    material.shader = simpleLitShader;
                    Debug.Log($"Changed material '{material.name}' to Simple Lit Shader.");
                }
                else
                {
                    Debug.LogError("Simple Lit Shader not found. Make sure it's included in your project.");
                }
            }
        }
    }

    [MenuItem("Assets/Materials/Duplicate material assets")]
    static void DuplicateMaterialAssets()
    {
        // Get the selected GameObject
        GameObject selectedObject = Selection.activeObject as GameObject;
        if (selectedObject == null)
        {
            Debug.LogError("Please select a GameObject.");
            return;
        }

        // Get all renderers in the selected object and its children
        Renderer[] renderers = selectedObject.GetComponentsInChildren<Renderer>(true);

        // Create copies of the materials
        foreach (Renderer renderer in renderers)
        {
            Material[] newSharedMaterials = renderer.sharedMaterials;
            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                int n = i;
                Material material = renderer.sharedMaterials[n];
                if (material == null)
                    continue;
                Material copy = new Material(renderer.sharedMaterials[n]);
                var assetPath = AssetDatabase.GetAssetPath(material);
                string newPath = assetPath + "_copy.mat";

                if (!assetPath.EndsWith(".mat"))
                {
                    Debug.Log("Extract material from "+assetPath);
                    //Remove file extension from assetPath
                    assetPath = Path.GetDirectoryName(assetPath);
                    //Check if directory exists
                    if (!AssetDatabase.IsValidFolder(assetPath + "/Materials"))
                        AssetDatabase.CreateFolder(assetPath, "Materials");
                    newPath = assetPath + "/Materials/" + material.name + "_copy.mat";
                }
                Debug.Log($"update {renderer} sharedMaterials {i}", renderer);
                
                //Check if asset already exists at newPath
                var existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(newPath);
                if (existingMaterial != null)
                {
                    Debug.Log($"Material already exists at {newPath}");
                    newSharedMaterials[n] = existingMaterial;
                    continue;
                }
                newSharedMaterials[n] = copy;
                AssetDatabase.CreateAsset(copy, newPath);
            }
            renderer.sharedMaterials = newSharedMaterials;
            EditorUtility.SetDirty(renderer);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
