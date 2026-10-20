#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;
using MedicalXR.RayMarching;

namespace MedicalXR.EditorScripts
{
    [CustomEditor(typeof(SdfSceneTemplate))]
    public class SdfSceneTemplateEditor : Editor
    {
        private bool showNodes = true;
        private bool showModifiers = true;
        private bool showChannels = false;

        public override void OnInspectorGUI()
        {
            SdfSceneTemplate template = (SdfSceneTemplate)target;
            if (template == null) return;

            serializedObject.Update();

            // Header Banner
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField($"🔮 SdfSceneTemplate: {template.name}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Volume: {template.volumeSize:F1}m³ | Voxel Res: {template.textureResolution}³ | Solid Mesh: {(template.renderSolidMesh ? "ON" : "OFF (VFX Particles)")}", EditorStyles.miniLabel);
                int nodeCount = template.nodes != null ? template.nodes.Count : 0;
                int modCount = template.modifierVolumes != null ? template.modifierVolumes.Count : 0;
                int chCount = template.channels != null ? template.channels.Count : 0;
                EditorGUILayout.LabelField($"Nodi: {nodeCount} | Modificatori: {modCount} | Canali Bio: {chCount}", EditorStyles.miniLabel);
            }

            EditorGUILayout.Space(10);

            // Action Buttons
            GUI.backgroundColor = new Color(0.2f, 0.8f, 0.3f);
            if (GUILayout.Button("🎬 Applica alla Scena Corrente", GUILayout.Height(36)))
            {
                Undo.RecordObjects(UnityEngine.Object.FindObjectsByType<Component>(FindObjectsSortMode.None), "Apply SdfSceneTemplate");
                template.ApplyToScene();
                Debug.Log($"[SdfSceneTemplate] Template '{template.name}' applicato con successo alla scena!");
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("📸 Cattura dalla Scena Corrente", GUILayout.Height(26)))
            {
                if (EditorUtility.DisplayDialog("Sovrascrivi Template", $"Sei sicuro di voler catturare la scena corrente e sovrascrivere '{template.name}'?", "Sì, Cattura", "Annulla"))
                {
                    Undo.RecordObject(template, "Capture Scene into Template");
                    template.CaptureFromScene();
                    EditorUtility.SetDirty(template);
                    AssetDatabase.SaveAssets();
                    Debug.Log($"[SdfSceneTemplate] Scena catturata con successo nel template '{template.name}'!");
                }
            }
            if (GUILayout.Button("🔍 Seleziona Volume in Scena", GUILayout.Height(26)))
            {
                var controller = UnityEngine.Object.FindAnyObjectByType<SdfGroupController>();
                if (controller != null) Selection.activeGameObject = controller.gameObject;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("⚡ Scambio Rapido Preset Clinici & Artistici", EditorStyles.boldLabel);

            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.BeginHorizontal();
                DrawPresetButton("Calma", "Calma.asset");
                DrawPresetButton("Stress", "Stress.asset");
                DrawPresetButton("Ipossia", "Ipossia.asset");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                DrawPresetButton("BoubaKiki", "BoubaKiki.asset");
                DrawPresetButton("Infinito", "Infinito.asset");
                DrawPresetButton("Fiore & Palla", "Fiore e Palla.asset");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                DrawPresetButton("Ciambella", "CiambellaNoised.asset");
                DrawPresetButton("InfinitoPalla", "InfinitoPalla.asset");
                DrawPresetButton("HeartRateScuro", "HeartRateScuro.asset");
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Parametri Globali Template", EditorStyles.boldLabel);
            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPresetButton(string label, string fileName)
        {
            if (GUILayout.Button(label, EditorStyles.miniButton))
            {
                string path = $"Packages/com.federicocolombo.volumetricsdf/Runtime/Templates/{fileName}";
                var preset = AssetDatabase.LoadAssetAtPath<SdfSceneTemplate>(path);
                if (preset == null)
                {
                    path = $"Assets/_Core/SDFTemplate/{fileName}";
                    preset = AssetDatabase.LoadAssetAtPath<SdfSceneTemplate>(path);
                }

                if (preset != null)
                {
                    Selection.activeObject = preset;
                    preset.ApplyToScene();
                    Debug.Log($"[SdfSceneTemplate] Preset '{label}' applicato alla scena!");
                }
                else
                {
                    Debug.LogWarning($"[SdfSceneTemplate] Preset '{fileName}' non trovato.");
                }
            }
        }
    }
}
#endif
