using UnityEngine;
using UnityEditor;

namespace MedicalXR.RayMarching
{
    /// <summary>
    /// Ispettore personalizzato per SdfGroupController.
    /// Gestisce l'ordinamento CSG top-to-bottom, la creazione rapida di nodi e volumi modificatori,
    /// e l'applicazione diretta dei Template SdfSceneTemplate.
    /// </summary>
    [CustomEditor(typeof(SdfGroupController))]
    public class SdfGroupControllerEditor : Editor
    {
        private SdfShapeType shapeTypeToAdd = SdfShapeType.Sphere;
        private SdfModifierType modifierTypeToAdd = SdfModifierType.Twist;
        private SdfModifierScope modifierScopeToAdd = SdfModifierScope.AllNodesInVolume;
        private SdfSceneTemplate templateToApply;

        public override void OnInspectorGUI()
        {
            SdfGroupController controller = (SdfGroupController)target;

            // Visual Fallback Warning
            if (!controller.renderSolidMesh)
            {
                var vox = controller.GetComponent<SdfVolumeVoxelizer>();
                bool hasVfx = (vox != null && vox.targetVfxGraph != null && vox.targetVfxGraph.gameObject.activeInHierarchy);
                if (!hasVfx)
                {
                    EditorGUILayout.HelpBox("⚠️ 'renderSolidMesh' è disabilitato e nessun VFX Graph è assegnato. La mesh solida è mantenuta attiva come fallback di sicurezza per evitare lo schermo nero.", MessageType.Warning);
                }
            }

            // Disegna l'ispettore standard per le proprietà pubbliche
            DrawDefaultInspector();

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("🎬 CSG Sequence & Blend Order (MudBun Top-to-Bottom)", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("L'ordine qui sotto definisce la fusione CSG: il nodo #1 è la base, e ogni nodo successivo si fonde con quelli sopra di lui. Usa i tasti ▲ e ▼ per riordinare la sequenza: l'ordine viene salvato fedelmente nei Template.", MessageType.Info);

            if (controller.nodes != null && controller.nodes.Count > 0)
            {
                for (int i = 0; i < controller.nodes.Count; i++)
                {
                    var node = controller.nodes[i];
                    if (node == null) continue;

                    EditorGUILayout.BeginHorizontal("box");

                    // Indice d'ordine
                    GUILayout.Label($"#{i + 1}", EditorStyles.boldLabel, GUILayout.Width(26));

                    // Nome e shape come pulsante per selezionarlo
                    string label = $"{node.gameObject.name} ({node.shapeType})";
                    if (GUILayout.Button(label, EditorStyles.linkLabel, GUILayout.Width(160)))
                    {
                        Selection.activeGameObject = node.gameObject;
                    }

                    // Operazione e blend softness
                    GUILayout.Label($"[{node.combineOp}] (Blend: {node.blendSoftness:F2})", EditorStyles.miniLabel);

                    GUILayout.FlexibleSpace();

                    // Pulsanti Sposta Su / Sposta Giù
                    GUI.enabled = (i > 0);
                    if (GUILayout.Button("▲", GUILayout.Width(26), GUILayout.Height(20)))
                    {
                        Undo.RegisterCompleteObjectUndo(controller, "Reorder Node Up");
                        Undo.RegisterCompleteObjectUndo(controller.nodes[i].gameObject, "Reorder Node Up");
                        Undo.RegisterCompleteObjectUndo(controller.nodes[i - 1].gameObject, "Reorder Node Up");

                        var temp = controller.nodes[i];
                        controller.nodes[i] = controller.nodes[i - 1];
                        controller.nodes[i - 1] = temp;

                        controller.SynchronizeHierarchyFromNodesList();
                        EditorUtility.SetDirty(controller);
                        GUIUtility.ExitGUI();
                    }

                    GUI.enabled = (i < controller.nodes.Count - 1);
                    if (GUILayout.Button("▼", GUILayout.Width(26), GUILayout.Height(20)))
                    {
                        Undo.RegisterCompleteObjectUndo(controller, "Reorder Node Down");
                        Undo.RegisterCompleteObjectUndo(controller.nodes[i].gameObject, "Reorder Node Down");
                        Undo.RegisterCompleteObjectUndo(controller.nodes[i + 1].gameObject, "Reorder Node Down");

                        var temp = controller.nodes[i];
                        controller.nodes[i] = controller.nodes[i + 1];
                        controller.nodes[i + 1] = temp;

                        controller.SynchronizeHierarchyFromNodesList();
                        EditorUtility.SetDirty(controller);
                        GUIUtility.ExitGUI();
                    }
                    GUI.enabled = true;

                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("🔄 Allinea Ordine da Gerarchia", GUILayout.Height(22)))
                {
                    Undo.RecordObject(controller, "Sync From Hierarchy");
                    controller.SynchronizeNodesListFromHierarchy();
                    EditorUtility.SetDirty(controller);
                }
            }

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("🔮 SdfSceneTemplate Quick Switch", EditorStyles.boldLabel);
            templateToApply = (SdfSceneTemplate)EditorGUILayout.ObjectField("Template", templateToApply, typeof(SdfSceneTemplate), false);
            if (GUILayout.Button("🎬 Applica Template Selezionato", GUILayout.Height(28)))
            {
                if (templateToApply != null)
                {
                    templateToApply.ApplyToScene();
                    EditorUtility.SetDirty(controller);
                }
            }

            EditorGUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Calma", EditorStyles.miniButtonLeft)) LoadPreset("Calma.asset");
            if (GUILayout.Button("Stress", EditorStyles.miniButtonMid)) LoadPreset("Stress.asset");
            if (GUILayout.Button("Ipossia", EditorStyles.miniButtonMid)) LoadPreset("Ipossia.asset");
            if (GUILayout.Button("BoubaKiki", EditorStyles.miniButtonRight)) LoadPreset("BoubaKiki.asset");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Procedural Node Creator", EditorStyles.boldLabel);

            // Menu a tendina per selezionare quale forma creare
            shapeTypeToAdd = (SdfShapeType)EditorGUILayout.EnumPopup("Shape to Add", shapeTypeToAdd);

            // Pulsante per aggiungere il nodo
            if (GUILayout.Button("Add Node", GUILayout.Height(30)))
            {
                SdfNode node = controller.AddNode(shapeTypeToAdd);
                Undo.RegisterCreatedObjectUndo(node.gameObject, "Create SDF Node");
                Selection.activeGameObject = node.gameObject;
                EditorUtility.SetDirty(controller);
            }

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Procedural Modifier Volume Creator (MudBun)", EditorStyles.boldLabel);

            modifierTypeToAdd = (SdfModifierType)EditorGUILayout.EnumPopup("Modifier Type", modifierTypeToAdd);
            modifierScopeToAdd = (SdfModifierScope)EditorGUILayout.EnumPopup("Influence Scope", modifierScopeToAdd);

            if (GUILayout.Button("Add Modifier Volume", GUILayout.Height(30)))
            {
                SdfModifierVolume mod = controller.AddModifierVolume(modifierTypeToAdd, modifierScopeToAdd);
                Undo.RegisterCreatedObjectUndo(mod.gameObject, "Create SDF Modifier Volume");
                Selection.activeGameObject = mod.gameObject;
                EditorUtility.SetDirty(controller);
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ Twist Volume", EditorStyles.miniButtonLeft))
            {
                SdfModifierVolume mod = controller.AddModifierVolume(SdfModifierType.Twist, modifierScopeToAdd);
                Undo.RegisterCreatedObjectUndo(mod.gameObject, "Create Twist Modifier Volume");
                Selection.activeGameObject = mod.gameObject;
                EditorUtility.SetDirty(controller);
            }
            if (GUILayout.Button("+ Bend Volume", EditorStyles.miniButtonMid))
            {
                SdfModifierVolume mod = controller.AddModifierVolume(SdfModifierType.Bend, modifierScopeToAdd);
                Undo.RegisterCreatedObjectUndo(mod.gameObject, "Create Bend Modifier Volume");
                Selection.activeGameObject = mod.gameObject;
                EditorUtility.SetDirty(controller);
            }
            if (GUILayout.Button("+ Noise Volume", EditorStyles.miniButtonRight))
            {
                SdfModifierVolume mod = controller.AddModifierVolume(SdfModifierType.Noise, modifierScopeToAdd);
                Undo.RegisterCreatedObjectUndo(mod.gameObject, "Create Noise Modifier Volume");
                Selection.activeGameObject = mod.gameObject;
                EditorUtility.SetDirty(controller);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void LoadPreset(string fileName)
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
                preset.ApplyToScene();
                Debug.Log($"[SdfGroupControllerEditor] Preset '{fileName}' applicato con successo!");
            }
            else
            {
                Debug.LogWarning($"[SdfGroupControllerEditor] Preset '{fileName}' non trovato.");
            }
        }
    }
}
