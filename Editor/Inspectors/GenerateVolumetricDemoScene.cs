#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
using MedicalXR.RayMarching;

namespace MedicalXR.EditorScripts
{
    public static class GenerateVolumetricDemoScene
    {
        [MenuItem("VolumetricSDF/Generate Volumetric SDF Demo Scene", priority = 1)]
        public static void GenerateDemoScene()
        {
            // 1. Crea una nuova scena vuota
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 2. Camera Setup
            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            Camera mainCam = camGo.AddComponent<Camera>();
            camGo.AddComponent<AudioListener>();
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.backgroundColor = new Color(0.04f, 0.05f, 0.08f);
            mainCam.transform.position = new Vector3(0, 0, -2.5f);
            mainCam.transform.rotation = Quaternion.identity;

            // 3. Directional Light
            GameObject lightGo = new GameObject("Directional Light");
            Light dirLight = lightGo.AddComponent<Light>();
            dirLight.type = LightType.Directional;
            dirLight.color = new Color(1f, 0.96f, 0.90f);
            dirLight.intensity = 1.2f;
            lightGo.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

            // 4. Crea il volume SDF completo
            GameObject sdfVolumeGo = CreateSdfVolumeHierarchy(null);

            // 5. Salva la scena nel progetto
            string sceneDirectory = "Assets/_Core/VolumetricSDF/Scenes";
            EnsureDirectory(sceneDirectory);

            string scenePath = $"{sceneDirectory}/VolumetricSDFDemo.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);

            Selection.activeGameObject = sdfVolumeGo;
            Debug.Log($"[VolumetricSDF] Scena Demo generata con successo a: {scenePath}");
        }

        [MenuItem("GameObject/3D Object/Volumetric SDF/SDF Volume (Complete)", false, 10)]
        public static void CreateCompleteSdfVolumeMenu(MenuCommand menuCommand)
        {
            Transform parent = menuCommand.context as GameObject != null ? ((GameObject)menuCommand.context).transform : null;
            GameObject go = CreateSdfVolumeHierarchy(parent);
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create Complete SDF Volume");
            Selection.activeGameObject = go;
        }

        [MenuItem("GameObject/3D Object/Volumetric SDF/SDF Node", false, 11)]
        public static void CreateSdfNodeMenu(MenuCommand menuCommand)
        {
            GameObject selected = menuCommand.context as GameObject;
            SdfGroupController controller = selected != null ? selected.GetComponentInParent<SdfGroupController>() : null;

            GameObject nodeGo = new GameObject("SdfNode_Sphere");
            var node = nodeGo.AddComponent<SdfNode>();
            node.shapeType = SdfShapeType.Sphere;
            node.size = new Vector3(0.2f, 0.2f, 0.2f);
            node.combineOp = SdfCombineOp.SmoothUnion;
            node.blendSoftness = 0.2f;

            if (controller != null)
            {
                nodeGo.transform.SetParent(controller.transform, false);
                controller.SynchronizeNodesListFromHierarchy();
            }
            else if (selected != null)
            {
                nodeGo.transform.SetParent(selected.transform, false);
            }

            Undo.RegisterCreatedObjectUndo(nodeGo, "Create SDF Node");
            Selection.activeGameObject = nodeGo;
        }

        [MenuItem("GameObject/3D Object/Volumetric SDF/SDF Modifier Volume", false, 12)]
        public static void CreateSdfModifierMenu(MenuCommand menuCommand)
        {
            GameObject selected = menuCommand.context as GameObject;
            SdfGroupController controller = selected != null ? selected.GetComponentInParent<SdfGroupController>() : null;

            GameObject modGo = new GameObject("SdfModifier_Twist");
            var mod = modGo.AddComponent<SdfModifierVolume>();
            mod.modifierType = SdfModifierType.Twist;
            mod.strength = 1.0f;
            mod.frequency = 4.0f;
            mod.falloff = 0.08f;
            mod.size = new Vector3(0.35f, 0.35f, 0.35f);

            if (controller != null)
            {
                modGo.transform.SetParent(controller.transform, false);
                controller.SynchronizeNodesListFromHierarchy();
            }
            else if (selected != null)
            {
                modGo.transform.SetParent(selected.transform, false);
            }

            Undo.RegisterCreatedObjectUndo(modGo, "Create SDF Modifier Volume");
            Selection.activeGameObject = modGo;
        }

        public static GameObject CreateSdfVolumeHierarchy(Transform parent)
        {
            GameObject sdfGo = new GameObject("SDF_Volume");
            if (parent != null) sdfGo.transform.SetParent(parent, false);

            sdfGo.AddComponent<MeshFilter>();
            var mr = sdfGo.AddComponent<MeshRenderer>();
            var mat = AssetDatabase.LoadAssetAtPath<Material>("Packages/com.federicocolombo.volumetricsdf/Runtime/Materials/BioSignalSDF_Mat.mat")
                   ?? AssetDatabase.LoadAssetAtPath<Material>("Assets/_Core/RayMarching/Material/BioSignalSDF_Mat.mat");
            if (mat != null) mr.sharedMaterial = mat;

            var controller = sdfGo.AddComponent<SdfGroupController>();
            controller.globalBlendSoftness = 0.25f;
            controller.renderSolidMesh = false;
            controller.autoEnableMeshIfNoVfx = true;

            var voxelizer = sdfGo.AddComponent<SdfVolumeVoxelizer>();
            voxelizer.volumeSize = 2.0f;
            voxelizer.textureResolution = 96;

            ComputeShader computeAsset = AssetDatabase.LoadAssetAtPath<ComputeShader>("Packages/com.federicocolombo.volumetricsdf/Runtime/Shaders/SdfVolumeVoxelizer.compute")
                                      ?? AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/_Core/RayMarching/Shaders/SdfVolumeVoxelizer.compute");
            if (computeAsset != null)
            {
                voxelizer.voxelizerShader = computeAsset;
            }

            // Nodo 1: Sfera Centrale con Kinematic Lissajous Driver
            GameObject sphereGo = new GameObject("SdfNode_CoreSphere");
            sphereGo.transform.SetParent(sdfGo.transform, false);
            var nodeSphere = sphereGo.AddComponent<SdfNode>();
            nodeSphere.shapeType = SdfShapeType.Sphere;
            nodeSphere.size = new Vector3(0.2f, 0.2f, 0.2f);
            nodeSphere.combineOp = SdfCombineOp.SmoothUnion;
            nodeSphere.shapeColor = new Color(0.0f, 0.85f, 0.95f, 1.0f);

            var driver = sphereGo.AddComponent<ProceduralMotionDriver>();
            driver.pattern = MotionPattern.Lissajous;
            driver.lissajousAmplitude = new Vector3(0.08f, 0f, 0.08f);
            driver.lissajousFrequency = new Vector3(1f, 0f, 2f);
            driver.speedMultiplier = 1.0f;
            driver.amplitudeMultiplier = 1.0f;

            // Nodo 2: Petali / Torus
            GameObject petalGo = new GameObject("SdfNode_Petals");
            petalGo.transform.SetParent(sdfGo.transform, false);
            var nodePetal = petalGo.AddComponent<SdfNode>();

            var texFiore = AssetDatabase.LoadAssetAtPath<Texture3D>("Packages/com.federicocolombo.volumetricsdf/Runtime/Textures/Baked3D/3fiore2.asset")
                        ?? AssetDatabase.LoadAssetAtPath<Texture3D>("Assets/_Core/RayMarching/Textures/Baked3D/3fiore2.asset")
                        ?? AssetDatabase.LoadAssetAtPath<Texture3D>("Assets/_Core/VFX/3fiore2.asset");
            if (texFiore != null)
            {
                nodePetal.shapeType = SdfShapeType.BakedTexture3D;
                nodePetal.bakedSdfTexture = texFiore;
                nodePetal.size = new Vector3(0.25f, 0.25f, 0.25f);
            }
            else
            {
                nodePetal.shapeType = SdfShapeType.Torus;
                nodePetal.size = new Vector3(0.25f, 0.25f, 0.25f);
            }
            nodePetal.combineOp = SdfCombineOp.SmoothUnion;
            nodePetal.blendSoftness = 0.35f;
            nodePetal.shapeColor = new Color(0.1f, 0.5f, 0.95f, 1.0f);

            // 5. Visual Effect Graph
            GameObject vfxGo = new GameObject("VfxDisplay_Target");
            vfxGo.transform.SetParent(sdfGo.transform, false);
            var vfxComponent = vfxGo.AddComponent<VisualEffect>();
            VisualEffectAsset vfxAsset = AssetDatabase.LoadAssetAtPath<VisualEffectAsset>("Packages/com.federicocolombo.volumetricsdf/Runtime/VFX/SdfParticleDisplay.vfx")
                                      ?? AssetDatabase.LoadAssetAtPath<VisualEffectAsset>("Packages/com.federicocolombo.pulseengine/Runtime/VFX/VfxCheckme.vfx")
                                      ?? AssetDatabase.LoadAssetAtPath<VisualEffectAsset>("Assets/_Core/CheckmeIntegration/Vfx/VfxCheckme.vfx");
            if (vfxAsset != null)
            {
                vfxComponent.visualEffectAsset = vfxAsset;
                voxelizer.targetVfxGraph = vfxComponent;
            }

            controller.SynchronizeNodesListFromHierarchy();
            return sdfGo;
        }

        private static void EnsureDirectory(string relativeAssetPath)
        {
            if (string.IsNullOrEmpty(relativeAssetPath)) return;

            string fullPath = Path.Combine(Application.dataPath, relativeAssetPath.StartsWith("Assets/") ? relativeAssetPath.Substring(7) : (relativeAssetPath.StartsWith("Assets") ? relativeAssetPath.Substring(6) : relativeAssetPath));
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                AssetDatabase.Refresh();
            }

            if (!AssetDatabase.IsValidFolder(relativeAssetPath))
            {
                string[] parts = relativeAssetPath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    string current = parts[0];
                    for (int i = 1; i < parts.Length; i++)
                    {
                        string next = current + "/" + parts[i];
                        if (!AssetDatabase.IsValidFolder(next))
                        {
                            AssetDatabase.CreateFolder(current, parts[i]);
                        }
                        current = next;
                    }
                }
            }
        }
    }
}
#endif
