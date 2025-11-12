// File: Assets/Editor/GenerateTextureResolutions.cs
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class GenerateTextureResolutions {
    static readonly int[] sizes = new int[] { 2048, 1024, 512, 256, 128 };

    [MenuItem("Assets/Create/Generate Resolutions from Texture", validate = false)]
    static void GenerateFromSelection() {
        var textures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets).Cast<Texture2D>().ToArray();
        if (textures.Length == 0) { Debug.LogWarning("Select at least one Texture2D asset."); return; }

        foreach (var srcTex in textures) {
            string srcPath = AssetDatabase.GetAssetPath(srcTex);                // e.g. Assets/Textures/MyTex.png
            string parentDir = Path.GetDirectoryName(srcPath).Replace("\\", "/"); // e.g. Assets/Textures
            string baseName = Path.GetFileNameWithoutExtension(srcPath);       // e.g. MyTex
            string targetFolder = parentDir + "/" + baseName;                 // e.g. Assets/Textures/MyTex

            if (!AssetDatabase.IsValidFolder(targetFolder))
                AssetDatabase.CreateFolder(parentDir, baseName);

            // ensure source texture is readable (temporarily if needed)
            var srcImporter = (TextureImporter)TextureImporter.GetAtPath(srcPath);
            bool origReadable = srcImporter.isReadable;
            TextureImporterCompression origCompression = srcImporter.textureCompression;
            int origMax = srcImporter.maxTextureSize;

            if (!origReadable || srcImporter.textureCompression != TextureImporterCompression.Uncompressed) {
                srcImporter.isReadable = true;
                srcImporter.textureCompression = TextureImporterCompression.Uncompressed;
                srcImporter.SaveAndReimport();
            }

            // Reload the texture (now readable)
            Texture2D readableSource = AssetDatabase.LoadAssetAtPath<Texture2D>(srcPath);

            foreach (int size in sizes) {
                // Create scaled texture via RenderTexture -> ReadPixels
                RenderTexture rt = RenderTexture.GetTemporary(size, size, 0, RenderTextureFormat.ARGB32);
                Graphics.Blit(readableSource, rt);

                RenderTexture prev = RenderTexture.active;
                RenderTexture.active = rt;

                Texture2D newTex = new Texture2D(size, size, TextureFormat.RGBA32, false);
                newTex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
                newTex.Apply();

                RenderTexture.active = prev;
                RenderTexture.ReleaseTemporary(rt);

                // Encode to PNG and write to disk
                byte[] png = newTex.EncodeToPNG();
                Object.DestroyImmediate(newTex);

                string assetPath = targetFolder + "/" + size + ".png"; // Assets/...
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), assetPath).Replace("\\", "/");

                // ensure directory exists on disk
                string diskDir = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(diskDir)) Directory.CreateDirectory(diskDir);

                File.WriteAllBytes(fullPath, png);
                AssetDatabase.ImportAsset(assetPath);

                // set importer max size to the name (size)
                var newImp = (TextureImporter)TextureImporter.GetAtPath(assetPath);
                newImp.maxTextureSize = size;
                // optional: keep compression default; you can adjust newImp.textureCompression here
                newImp.SaveAndReimport();
            }

            // restore original source importer settings if we changed them
            if (srcImporter.isReadable != origReadable || srcImporter.textureCompression != origCompression || srcImporter.maxTextureSize != origMax) {
                srcImporter.isReadable = origReadable;
                srcImporter.textureCompression = origCompression;
                srcImporter.maxTextureSize = origMax;
                srcImporter.SaveAndReimport();
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Generated resolution assets for selected textures.");
    }

    [MenuItem("Assets/Create/Generate Resolutions from Texture", validate = true)]
    static bool ValidateGenerateFromSelection() {
        return Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets).Length > 0;
    }
}