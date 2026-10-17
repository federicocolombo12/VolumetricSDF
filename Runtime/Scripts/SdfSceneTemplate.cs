using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MedicalXR.RayMarching
{
    [System.Serializable]
    public class SdfNodeSnapshot
    {
        public string nodeName = "SdfNode";
        public int siblingIndex = 0;
        public SdfShapeType shapeType = SdfShapeType.Sphere;
        public Texture3D bakedSdfTexture;
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
        public Vector3 localScale = Vector3.one;
        public Vector3 size = new Vector3(0.12f, 0.12f, 0.12f);
        public SdfCombineOp combineOp = SdfCombineOp.SmoothUnion;
        public float blendSoftness = 0f;
        public Color shapeColor = Color.white;
        public float pulseSensitivity = 1.0f;
        public SpatialMotionSnapshot motionSnapshot = new SpatialMotionSnapshot();
    }

    [System.Serializable]
    public class SpatialMotionSnapshot
    {
        public bool hasMotionDriver = false;
        public MotionPattern pattern = MotionPattern.None;
        public float speedMultiplier = 1.0f;
        public float amplitudeMultiplier = 1.0f;
        public float noiseTurbulence = 0.0f;
        public float phaseOffset = 0.0f;

        // Lissajous
        public Vector3 lissajousAmplitude = new Vector3(0.06f, 0.06f, 0.06f);
        public Vector3 lissajousFrequency = new Vector3(1f, 0f, 2f);
        public Vector3 lissajousPhase = Vector3.zero;

        // Orbit
        public MotionOrbitPlane orbitPlane = MotionOrbitPlane.XZ;
        public Vector2 orbitRadius = new Vector2(0.08f, 0.08f);
        public float orbitSpeed = 1.0f;
        public bool orbitClockwise = true;

        // PingPong
        public Vector3 pingPongAxis = Vector3.up;
        public float pingPongDistance = 0.08f;
        public float pingPongFrequency = 1.0f;
        public bool pingPongSmoothEasing = true;

        // PerlinDrift
        public Vector3 driftAmplitude = new Vector3(0.05f, 0.05f, 0.05f);
        public Vector3 driftSpeed = new Vector3(0.5f, 0.5f, 0.5f);
        public int driftOctaves = 2;
        public float driftSeed = 42.0f;

        // Spiral
        public MotionOrbitPlane spiralPlane = MotionOrbitPlane.XZ;
        public float spiralRadius = 0.06f;
        public float spiralSpeed = 1.0f;
        public Vector3 spiralTravelAxis = Vector3.up;
        public float spiralTravelDistance = 0.06f;
        public float spiralTravelFrequency = 0.5f;

        public static SpatialMotionSnapshot CaptureFrom(ProceduralMotionDriver driver)
        {
            if (driver == null || driver.pattern == MotionPattern.None) 
                return new SpatialMotionSnapshot { hasMotionDriver = false };

            return new SpatialMotionSnapshot
            {
                hasMotionDriver = true,
                pattern = driver.pattern,
                speedMultiplier = driver.speedMultiplier,
                amplitudeMultiplier = driver.amplitudeMultiplier,
                noiseTurbulence = driver.noiseTurbulence,
                phaseOffset = driver.phaseOffset,
                lissajousAmplitude = driver.lissajousAmplitude,
                lissajousFrequency = driver.lissajousFrequency,
                lissajousPhase = driver.lissajousPhase,
                orbitPlane = driver.orbitPlane,
                orbitRadius = driver.orbitRadius,
                orbitSpeed = driver.orbitSpeed,
                orbitClockwise = driver.orbitClockwise,
                pingPongAxis = driver.pingPongAxis,
                pingPongDistance = driver.pingPongDistance,
                pingPongFrequency = driver.pingPongFrequency,
                pingPongSmoothEasing = driver.pingPongSmoothEasing,
                driftAmplitude = driver.driftAmplitude,
                driftSpeed = driver.driftSpeed,
                driftOctaves = driver.driftOctaves,
                driftSeed = driver.driftSeed,
                spiralPlane = driver.spiralPlane,
                spiralRadius = driver.spiralRadius,
                spiralSpeed = driver.spiralSpeed,
                spiralTravelAxis = driver.spiralTravelAxis,
                spiralTravelDistance = driver.spiralTravelDistance,
                spiralTravelFrequency = driver.spiralTravelFrequency
            };
        }

        public void ApplyTo(ProceduralMotionDriver driver)
        {
            if (driver == null) return;
            driver.pattern = pattern;
            driver.speedMultiplier = speedMultiplier;
            driver.amplitudeMultiplier = amplitudeMultiplier;
            driver.noiseTurbulence = noiseTurbulence;
            driver.phaseOffset = phaseOffset;
            driver.lissajousAmplitude = lissajousAmplitude;
            driver.lissajousFrequency = lissajousFrequency;
            driver.lissajousPhase = lissajousPhase;
            driver.orbitPlane = orbitPlane;
            driver.orbitRadius = orbitRadius;
            driver.orbitSpeed = orbitSpeed;
            driver.orbitClockwise = orbitClockwise;
            driver.pingPongAxis = pingPongAxis;
            driver.pingPongDistance = pingPongDistance;
            driver.pingPongFrequency = pingPongFrequency;
            driver.pingPongSmoothEasing = pingPongSmoothEasing;
            driver.driftAmplitude = driftAmplitude;
            driver.driftSpeed = driftSpeed;
            driver.driftOctaves = driftOctaves;
            driver.driftSeed = driftSeed;
            driver.spiralPlane = spiralPlane;
            driver.spiralRadius = spiralRadius;
            driver.spiralSpeed = spiralSpeed;
            driver.spiralTravelAxis = spiralTravelAxis;
            driver.spiralTravelDistance = spiralTravelDistance;
            driver.spiralTravelFrequency = spiralTravelFrequency;
        }
    }

    [System.Serializable]
    public class SdfModifierVolumeSnapshot
    {
        public string volumeName = "SdfModifierVolume";
        public int siblingIndex = 0;
        public SdfModifierType modifierType = SdfModifierType.Twist;
        public SdfModifierScope scope = SdfModifierScope.AllNodesInVolume;
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
        public Vector3 localScale = Vector3.one;
        public Vector3 size = new Vector3(0.25f, 0.25f, 0.25f);
        public float falloff = 0.05f;
        public float strength = 1.0f;
        public float frequency = 5.0f;
        public Color gizmoColor = new Color(1f, 0.6f, 0.1f, 0.35f);
    }

    [System.Serializable]
    public class ChannelSnapshot
    {
        public string id;
        public int sourceType = 0;
        public int filterType = 0;
        public string sourceChannelId;
        public Vector2 inputRange = new Vector2(0f, 1f);
        public Vector2 outputRange = new Vector2(0f, 1f);
        public float smoothing = 0.5f;
        public bool isMuted = false;
        public bool isSolo = false;
    }

    [System.Serializable]
    public class PropertyBinderSnapshot
    {
        public string channelId;
        public string targetGameObjectName;
        public string targetComponentType;
        public string targetPropertyName;
        public float weightMultiplier = 1.0f;
        public bool useLocalRemap = false;
        public Vector2 localRemap = new Vector2(0f, 1f);
    }

    [System.Serializable]
    public class ColorBinderSnapshot
    {
        public string channelId;
        public string targetGameObjectName;
        public string targetComponentType;
        public string targetPropertyName;
        public Gradient colorGradient;
    }

    /// <summary>
    /// Snapshot completo dello stato clinico ed estetico della scena:
    /// Nodi SDF, parametri del Voxelizer, canali del DataStreamRegistry
    /// e tutti i binder (inclusi Local Remap e Gradienti di colore).
    /// </summary>
    [CreateAssetMenu(fileName = "New Scene Template", menuName = "MedicalXR/Scene Template (Full)")]
    public class SdfSceneTemplate : ScriptableObject
    {
        [Header("Volume & Optimization")]
        public int textureResolution = 64;
        public float volumeSize = 2.0f;
        public bool renderSolidMesh = false;

        [Header("Surface Texturing")]
        public Texture2D albedoTexture;
        public float textureScale = 2.0f;
        [Range(0f, 1f)] public float textureBlend = 0.0f;
        [Range(1f, 16f)] public float triplanarSharpness = 4.0f;
        public bool enableBumpMap = false;
        public Texture2D bumpMap;
        public float bumpScale = 0.5f;
        public bool enableMatCap = false;
        public Texture2D matCapTexture;
        [Range(0f, 1f)] public float matCapBlend = 0.0f;

        [Header("SDF Blending & Pulse")]
        [Range(0.001f, 2f)] public float globalBlendSoftness = 0.2f;
        public float pulseMultiplier = 1.0f;
        public float pulseDecaySpeed = 4.0f;
        public string shaderPulseParam = "_RWavePulse";

        [Header("SDF Node Hierarchy")]
        public List<SdfNodeSnapshot> nodes = new List<SdfNodeSnapshot>();
        public List<SdfModifierVolumeSnapshot> modifierVolumes = new List<SdfModifierVolumeSnapshot>();

        [Header("DataStream Channels")]
        public List<ChannelSnapshot> channels = new List<ChannelSnapshot>();

        [Header("DataStream Binders")]
        public List<PropertyBinderSnapshot> propertyBinders = new List<PropertyBinderSnapshot>();
        public List<ColorBinderSnapshot> colorBinders = new List<ColorBinderSnapshot>();

        /// <summary>
        /// Crea uno snapshot istantaneo della scena attiva.
        /// </summary>
        public static SdfSceneTemplate CaptureCurrentScene(string templateName = "")
        {
            var template = ScriptableObject.CreateInstance<SdfSceneTemplate>();
            if (!string.IsNullOrEmpty(templateName))
            {
                template.name = templateName;
            }

            // 1. Controller & Volume
            var controller = UnityEngine.Object.FindAnyObjectByType<SdfGroupController>();
            var voxelizer = UnityEngine.Object.FindAnyObjectByType<SdfVolumeVoxelizer>();

            if (controller != null)
            {
                template.globalBlendSoftness = controller.globalBlendSoftness;
                template.pulseMultiplier = controller.pulseMultiplier;
                template.pulseDecaySpeed = controller.pulseDecaySpeed;
                template.shaderPulseParam = controller.shaderPulseParam;
                template.renderSolidMesh = controller.renderSolidMesh;

                // Nodi (ordinati rigorosamente per l'ordine di valutazione CSG top-to-bottom)
                if (controller.nodes != null)
                {
                    var sortedNodes = controller.nodes
                        .Where(n => n != null)
                        .OrderBy(n => n.transform.GetSiblingIndex())
                        .ToList();

                    foreach (var n in sortedNodes)
                    {
                        var snap = new SdfNodeSnapshot
                        {
                            nodeName = n.gameObject.name,
                            siblingIndex = n.transform.GetSiblingIndex(),
                            shapeType = n.shapeType,
                            bakedSdfTexture = n.bakedSdfTexture,
                            localPosition = n.transform.localPosition,
                            localEulerAngles = n.transform.localEulerAngles,
                            localScale = n.transform.localScale,
                            size = n.size,
                            combineOp = n.combineOp,
                            blendSoftness = n.blendSoftness,
                            shapeColor = n.shapeColor,
                            pulseSensitivity = n.pulseSensitivity,
                            motionSnapshot = SpatialMotionSnapshot.CaptureFrom(n.GetComponent<ProceduralMotionDriver>())
                        };
                        template.nodes.Add(snap);
                    }
                }

                // Volumi Modificatori
                if (controller.modifierVolumes != null)
                {
                    var sortedMods = controller.modifierVolumes
                        .Where(m => m != null)
                        .OrderBy(m => m.transform.GetSiblingIndex())
                        .ToList();

                    foreach (var m in sortedMods)
                    {
                        var mSnap = new SdfModifierVolumeSnapshot
                        {
                            volumeName = m.gameObject.name,
                            siblingIndex = m.transform.GetSiblingIndex(),
                            modifierType = m.modifierType,
                            scope = m.scope,
                            localPosition = m.transform.localPosition,
                            localEulerAngles = m.transform.localEulerAngles,
                            localScale = m.transform.localScale,
                            size = m.size,
                            falloff = m.falloff,
                            strength = m.strength,
                            frequency = m.frequency,
                            gizmoColor = m.gizmoColor
                        };
                        template.modifierVolumes.Add(mSnap);
                    }
                }

                var rend = controller.GetComponent<Renderer>();
                if (rend != null && rend.sharedMaterial != null)
                {
                    var mat = rend.sharedMaterial;
                    template.albedoTexture = mat.GetTexture("_MainTex") as Texture2D;
                    template.textureScale = mat.HasProperty("_TextureScale") ? mat.GetFloat("_TextureScale") : 2.0f;
                    template.textureBlend = mat.HasProperty("_TextureBlend") ? mat.GetFloat("_TextureBlend") : 0.0f;
                    template.triplanarSharpness = mat.HasProperty("_TriplanarSharpness") ? mat.GetFloat("_TriplanarSharpness") : 4.0f;
                    template.enableBumpMap = mat.HasProperty("_EnableBumpMap") && mat.GetFloat("_EnableBumpMap") > 0.5f;
                    template.bumpMap = mat.GetTexture("_BumpMap") as Texture2D;
                    template.bumpScale = mat.HasProperty("_BumpScale") ? mat.GetFloat("_BumpScale") : 0.5f;
                    template.enableMatCap = mat.HasProperty("_EnableMatCap") && mat.GetFloat("_EnableMatCap") > 0.5f;
                    template.matCapTexture = mat.GetTexture("_MatCapTex") as Texture2D;
                    template.matCapBlend = mat.HasProperty("_MatCapBlend") ? mat.GetFloat("_MatCapBlend") : 0.0f;
                }
            }

            if (voxelizer != null)
            {
                template.textureResolution = voxelizer.textureResolution;
                template.volumeSize = voxelizer.volumeSize;
            }

            // 2. DataStream Registry & Canali (se PulseEngine e installato)
            var registryType = FindTypeAcrossAssemblies("GenericDataStreaming.DataStreamRegistry");
            if (registryType != null)
            {
                var instProp = registryType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                var regInst = instProp?.GetValue(null);
                if (regInst != null)
                {
                    var channelsField = registryType.GetField("channels");
                    var chList = channelsField?.GetValue(regInst) as System.Collections.IEnumerable;
                    if (chList != null)
                    {
                        foreach (var ch in chList)
                        {
                            var chType = ch.GetType();
                            template.channels.Add(new ChannelSnapshot
                            {
                                id = chType.GetField("id")?.GetValue(ch) as string,
                                sourceType = Convert.ToInt32(chType.GetField("sourceType")?.GetValue(ch) ?? 0),
                                filterType = Convert.ToInt32(chType.GetField("filterType")?.GetValue(ch) ?? 0),
                                sourceChannelId = chType.GetField("sourceChannelId")?.GetValue(ch) as string,
                                inputRange = (Vector2)(chType.GetField("inputRange")?.GetValue(ch) ?? Vector2.zero),
                                outputRange = (Vector2)(chType.GetField("outputRange")?.GetValue(ch) ?? Vector2.one),
                                smoothing = Convert.ToSingle(chType.GetField("smoothing")?.GetValue(ch) ?? 0.5f),
                                isMuted = Convert.ToBoolean(chType.GetField("isMuted")?.GetValue(ch) ?? false),
                                isSolo = Convert.ToBoolean(chType.GetField("isSolo")?.GetValue(ch) ?? false)
                            });
                        }
                    }
                }
            }

            // 3. Property Binders
            var propBinderType = FindTypeAcrossAssemblies("GenericDataStreaming.DataStreamPropertyBinder");
            if (propBinderType != null)
            {
                var pBinders = UnityEngine.Object.FindObjectsByType(propBinderType, FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var pb in pBinders)
                {
                    if (pb == null) continue;
                    var targetComp = propBinderType.GetField("targetComponent")?.GetValue(pb) as Component;
                    if (targetComp == null) continue;

                    template.propertyBinders.Add(new PropertyBinderSnapshot
                    {
                        channelId = propBinderType.GetField("channelId")?.GetValue(pb) as string,
                        targetGameObjectName = targetComp.gameObject.name,
                        targetComponentType = targetComp.GetType().FullName,
                        targetPropertyName = propBinderType.GetField("targetPropertyName")?.GetValue(pb) as string,
                        weightMultiplier = Convert.ToSingle(propBinderType.GetField("weightMultiplier")?.GetValue(pb) ?? 1f),
                        useLocalRemap = Convert.ToBoolean(propBinderType.GetField("useLocalRemap")?.GetValue(pb) ?? false),
                        localRemap = (Vector2)(propBinderType.GetField("localRemap")?.GetValue(pb) ?? new Vector2(0f, 1f))
                    });
                }
            }

            // 4. Color Binders
            var colorBinderType = FindTypeAcrossAssemblies("GenericDataStreaming.DataStreamColorBinder");
            if (colorBinderType != null)
            {
                var cBinders = UnityEngine.Object.FindObjectsByType(colorBinderType, FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var cb in cBinders)
                {
                    if (cb == null) continue;
                    var targetComp = colorBinderType.GetField("targetComponent")?.GetValue(cb) as Component;
                    if (targetComp == null) continue;

                    template.colorBinders.Add(new ColorBinderSnapshot
                    {
                        channelId = colorBinderType.GetField("channelId")?.GetValue(cb) as string,
                        targetGameObjectName = targetComp.gameObject.name,
                        targetComponentType = targetComp.GetType().FullName,
                        targetPropertyName = colorBinderType.GetField("targetPropertyName")?.GetValue(cb) as string,
                        colorGradient = colorBinderType.GetField("colorGradient")?.GetValue(cb) as Gradient
                    });
                }
            }

            return template;
        }

        private static Type FindTypeAcrossAssemblies(string fullName)
        {
            var t = Type.GetType(fullName);
            if (t != null) return t;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                t = asm.GetType(fullName);
                if (t != null) return t;
            }
            return null;
        }

        /// <summary>
        /// Applica questo template ricostruendo e aggiornando la scena corrente.
        /// </summary>
        public void ApplyToScene()
        {
            var controller = UnityEngine.Object.FindAnyObjectByType<SdfGroupController>();
            var voxelizer = UnityEngine.Object.FindAnyObjectByType<SdfVolumeVoxelizer>();

            // 1. Controller & Volume
            if (controller != null)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(controller, "Apply Scene Template");
#endif
                controller.globalBlendSoftness = globalBlendSoftness;
                controller.pulseMultiplier = pulseMultiplier;
                controller.pulseDecaySpeed = pulseDecaySpeed;
                controller.shaderPulseParam = shaderPulseParam;
                controller.renderSolidMesh = renderSolidMesh;

                // Ricostruzione Nodi
                if (nodes != null && nodes.Count > 0)
                {
                    var existingNodes = controller.GetComponentsInChildren<SdfNode>(true).ToList();
                    var matchedNodes = new List<SdfNode>();
                    var unusedExisting = new List<SdfNode>(existingNodes);

                    // 1. Assegna o adatta i nodi, preservando l'identità dei GameObject per nome (e relativi Binders)
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        var snap = nodes[i];
                        SdfNode targetNode = unusedExisting.FirstOrDefault(en => en != null && en.gameObject.name == snap.nodeName);
                        if (targetNode != null)
                        {
                            unusedExisting.Remove(targetNode);
                        }
                        else if (unusedExisting.Count > 0)
                        {
                            targetNode = unusedExisting[0];
                            unusedExisting.RemoveAt(0);
                        }
                        else
                        {
                            var newGo = new GameObject(snap.nodeName);
                            newGo.transform.SetParent(controller.transform, false);
                            targetNode = newGo.AddComponent<SdfNode>();
#if UNITY_EDITOR
                            UnityEditor.Undo.RegisterCreatedObjectUndo(newGo, "Create Sdf Node");
#endif
                        }

#if UNITY_EDITOR
                        UnityEditor.Undo.RecordObject(targetNode.gameObject, "Configure Node");
                        UnityEditor.Undo.RecordObject(targetNode, "Configure Node");
#endif
                        targetNode.gameObject.name = snap.nodeName;
                        targetNode.shapeType = snap.shapeType;
                        targetNode.bakedSdfTexture = snap.bakedSdfTexture;
                        targetNode.transform.localPosition = snap.localPosition;
                        targetNode.transform.localEulerAngles = snap.localEulerAngles;
                        targetNode.transform.localScale = snap.localScale;
                        targetNode.size = snap.size;
                        targetNode.combineOp = snap.combineOp;
                        targetNode.blendSoftness = snap.blendSoftness;
                        targetNode.shapeColor = snap.shapeColor;
                        targetNode.pulseSensitivity = snap.pulseSensitivity;

                        var existingDriver = targetNode.GetComponent<ProceduralMotionDriver>();
                        if (snap.motionSnapshot != null && snap.motionSnapshot.hasMotionDriver)
                        {
                            if (existingDriver == null)
                            {
#if UNITY_EDITOR
                                existingDriver = UnityEditor.Undo.AddComponent<ProceduralMotionDriver>(targetNode.gameObject);
#else
                                existingDriver = targetNode.gameObject.AddComponent<ProceduralMotionDriver>();
#endif
                            }
                            snap.motionSnapshot.ApplyTo(existingDriver);
                        }
                        else if (existingDriver != null)
                        {
#if UNITY_EDITOR
                            UnityEditor.Undo.DestroyObjectImmediate(existingDriver);
#else
                            UnityEngine.Object.Destroy(existingDriver);
#endif
                        }

                        matchedNodes.Add(targetNode);
                    }

                    // 2. Rimuove eventuali nodi in eccesso non presenti nel template
                    foreach (var excess in unusedExisting)
                    {
                        if (excess != null)
                        {
#if UNITY_EDITOR
                            UnityEditor.Undo.DestroyObjectImmediate(excess.gameObject);
#else
                            UnityEngine.Object.Destroy(excess.gameObject);
#endif
                        }
                    }

                    // 3. Ripristina rigorosamente l'ordine gerarchico dei SiblingIndex
                    for (int i = 0; i < matchedNodes.Count; i++)
                    {
                        var snap = nodes[i];
                        int targetSibling = (snap.siblingIndex >= 0) ? snap.siblingIndex : i;
                        matchedNodes[i].transform.SetSiblingIndex(targetSibling);
                    }

                    controller.nodes = matchedNodes;
                }

                // Ricostruzione Volumi Modificatori
                if (modifierVolumes != null)
                {
                    var existingMods = controller.GetComponentsInChildren<SdfModifierVolume>(true).ToList();
                    var matchedMods = new List<SdfModifierVolume>();
                    var unusedMods = new List<SdfModifierVolume>(existingMods);

                    for (int i = 0; i < modifierVolumes.Count; i++)
                    {
                        var snap = modifierVolumes[i];
                        SdfModifierVolume targetMod = unusedMods.FirstOrDefault(em => em != null && em.gameObject.name == snap.volumeName);
                        if (targetMod != null)
                        {
                            unusedMods.Remove(targetMod);
                        }
                        else if (unusedMods.Count > 0)
                        {
                            targetMod = unusedMods[0];
                            unusedMods.RemoveAt(0);
                        }
                        else
                        {
                            var newGo = new GameObject(snap.volumeName);
                            newGo.transform.SetParent(controller.transform, false);
                            targetMod = newGo.AddComponent<SdfModifierVolume>();
#if UNITY_EDITOR
                            UnityEditor.Undo.RegisterCreatedObjectUndo(newGo, "Create Sdf Modifier Volume");
#endif
                        }

#if UNITY_EDITOR
                        UnityEditor.Undo.RecordObject(targetMod.gameObject, "Configure Modifier Volume");
                        UnityEditor.Undo.RecordObject(targetMod, "Configure Modifier Volume");
#endif
                        targetMod.gameObject.name = snap.volumeName;
                        targetMod.modifierType = snap.modifierType;
                        targetMod.scope = snap.scope;
                        targetMod.transform.localPosition = snap.localPosition;
                        targetMod.transform.localEulerAngles = snap.localEulerAngles;
                        targetMod.transform.localScale = snap.localScale;
                        targetMod.size = snap.size;
                        targetMod.falloff = snap.falloff;
                        targetMod.strength = snap.strength;
                        targetMod.frequency = snap.frequency;
                        targetMod.gizmoColor = snap.gizmoColor;

                        matchedMods.Add(targetMod);
                    }

                    foreach (var excess in unusedMods)
                    {
                        if (excess != null)
                        {
#if UNITY_EDITOR
                            UnityEditor.Undo.DestroyObjectImmediate(excess.gameObject);
#else
                            UnityEngine.Object.Destroy(excess.gameObject);
#endif
                        }
                    }

                    for (int i = 0; i < matchedMods.Count; i++)
                    {
                        var snap = modifierVolumes[i];
                        int targetSibling = (snap.siblingIndex >= 0) ? snap.siblingIndex : (nodes != null ? nodes.Count + i : i);
                        matchedMods[i].transform.SetSiblingIndex(targetSibling);
                    }

                    controller.modifierVolumes = matchedMods;
                }

                controller.SynchronizeHierarchyFromNodesList();

                var rend = controller.GetComponent<Renderer>();
                if (rend != null && rend.sharedMaterial != null)
                {
                    var mat = rend.sharedMaterial;
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(mat, "Apply Material Texture Settings");
#endif
                    if (albedoTexture != null) mat.SetTexture("_MainTex", albedoTexture);
                    mat.SetFloat("_TextureScale", textureScale);
                    mat.SetFloat("_TextureBlend", textureBlend);
                    mat.SetFloat("_TriplanarSharpness", triplanarSharpness);
                    mat.SetFloat("_EnableBumpMap", enableBumpMap ? 1.0f : 0.0f);
                    if (bumpMap != null) mat.SetTexture("_BumpMap", bumpMap);
                    mat.SetFloat("_BumpScale", bumpScale);
                    mat.SetFloat("_EnableMatCap", enableMatCap ? 1.0f : 0.0f);
                    if (matCapTexture != null) mat.SetTexture("_MatCapTex", matCapTexture);
                    mat.SetFloat("_MatCapBlend", matCapBlend);
                }
            }

            if (voxelizer != null)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(voxelizer.transform, "Apply Volume Settings");
                UnityEditor.Undo.RecordObject(voxelizer, "Apply Volume Settings");
#endif
                voxelizer.transform.localScale = Vector3.one;
                voxelizer.volumeSize = volumeSize;
                voxelizer.textureResolution = textureResolution;
            }

            // 2. Canali del DataStreamRegistry (se PulseEngine e presente)
            var registryType = FindTypeAcrossAssemblies("GenericDataStreaming.DataStreamRegistry");
            var chType = FindTypeAcrossAssemblies("GenericDataStreaming.DataStreamChannel");
            if (registryType != null && chType != null && channels != null && channels.Count > 0)
            {
                var instProp = registryType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                var regInst = instProp?.GetValue(null);
                if (regInst != null)
                {
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject((UnityEngine.Object)regInst, "Apply Channels");
#endif
                    var listType = typeof(List<>).MakeGenericType(chType);
                    var list = (System.Collections.IList)Activator.CreateInstance(listType);

                    var srcEnumType = FindTypeAcrossAssemblies("GenericDataStreaming.ChannelSourceType");
                    var fltEnumType = FindTypeAcrossAssemblies("GenericDataStreaming.ChannelFilterType");

                    foreach (var chSnap in channels)
                    {
                        var chObj = Activator.CreateInstance(chType);
                        chType.GetField("id")?.SetValue(chObj, chSnap.id);
                        if (srcEnumType != null) chType.GetField("sourceType")?.SetValue(chObj, Enum.ToObject(srcEnumType, chSnap.sourceType));
                        if (fltEnumType != null) chType.GetField("filterType")?.SetValue(chObj, Enum.ToObject(fltEnumType, chSnap.filterType));
                        chType.GetField("sourceChannelId")?.SetValue(chObj, chSnap.sourceChannelId);
                        chType.GetField("inputRange")?.SetValue(chObj, chSnap.inputRange);
                        chType.GetField("outputRange")?.SetValue(chObj, chSnap.outputRange);
                        chType.GetField("smoothing")?.SetValue(chObj, chSnap.smoothing);
                        chType.GetField("isMuted")?.SetValue(chObj, chSnap.isMuted);
                        chType.GetField("isSolo")?.SetValue(chObj, chSnap.isSolo);
                        list.Add(chObj);
                    }

                    registryType.GetField("channels")?.SetValue(regInst, list);
                    registryType.GetMethod("RefreshRegistry")?.Invoke(regInst, null);
                }
            }

            // 3. Ripristino Binders (se PulseEngine e presente)
            var propBinderType = FindTypeAcrossAssemblies("GenericDataStreaming.DataStreamPropertyBinder");
            var colorBinderType = FindTypeAcrossAssemblies("GenericDataStreaming.DataStreamColorBinder");

            if (propBinderType != null)
            {
                var oldFloatBinders = UnityEngine.Object.FindObjectsByType(propBinderType, FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var b in oldFloatBinders)
                {
                    if (b is UnityEngine.Object uo)
                    {
#if UNITY_EDITOR
                        UnityEditor.Undo.DestroyObjectImmediate(uo);
#else
                        UnityEngine.Object.Destroy(uo);
#endif
                    }
                }
            }

            if (colorBinderType != null)
            {
                var oldColorBinders = UnityEngine.Object.FindObjectsByType(colorBinderType, FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var b in oldColorBinders)
                {
                    if (b is UnityEngine.Object uo)
                    {
#if UNITY_EDITOR
                        UnityEditor.Undo.DestroyObjectImmediate(uo);
#else
                        UnityEngine.Object.Destroy(uo);
#endif
                    }
                }
            }

            // Ricrea Property Binders
            if (propBinderType != null && propertyBinders != null)
            {
                foreach (var pbSnap in propertyBinders)
                {
                    var targetGo = GameObject.Find(pbSnap.targetGameObjectName);
                    if (targetGo == null)
                    {
                        if (!string.IsNullOrEmpty(pbSnap.targetComponentType) && pbSnap.targetComponentType.Contains("VisualEffect"))
                        {
                            var vfx = UnityEngine.Object.FindAnyObjectByType<UnityEngine.VFX.VisualEffect>();
                            if (vfx != null) targetGo = vfx.gameObject;
                        }
                        if (targetGo == null && controller != null) targetGo = controller.gameObject;
                    }

                    if (targetGo != null)
                    {
                        Component targetComp = null;
                        if (!string.IsNullOrEmpty(pbSnap.targetComponentType))
                        {
                            targetComp = targetGo.GetComponents<Component>().FirstOrDefault(c => c != null && c.GetType().FullName == pbSnap.targetComponentType);
                        }
                        if (targetComp == null)
                        {
                            targetComp = targetGo.GetComponent<UnityEngine.VFX.VisualEffect>() as Component
                                      ?? targetGo.GetComponent<Renderer>() as Component
                                      ?? targetGo.transform;
                        }

#if UNITY_EDITOR
                        var newBinder = UnityEditor.Undo.AddComponent(targetGo, propBinderType);
#else
                        var newBinder = targetGo.AddComponent(propBinderType);
#endif
                        propBinderType.GetField("channelId")?.SetValue(newBinder, pbSnap.channelId);
                        propBinderType.GetField("targetComponent")?.SetValue(newBinder, targetComp);
                        propBinderType.GetField("targetPropertyName")?.SetValue(newBinder, pbSnap.targetPropertyName);
                        propBinderType.GetField("weightMultiplier")?.SetValue(newBinder, pbSnap.weightMultiplier);
                        propBinderType.GetField("useLocalRemap")?.SetValue(newBinder, pbSnap.useLocalRemap);
                        propBinderType.GetField("localRemap")?.SetValue(newBinder, pbSnap.localRemap);
                        propBinderType.GetMethod("InitializeBindings")?.Invoke(newBinder, null);
                    }
                }
            }

            // Ricrea Color Binders
            if (colorBinderType != null && colorBinders != null)
            {
                foreach (var cbSnap in colorBinders)
                {
                    var targetGo = GameObject.Find(cbSnap.targetGameObjectName);
                    if (targetGo == null)
                    {
                        var vfx = UnityEngine.Object.FindAnyObjectByType<UnityEngine.VFX.VisualEffect>();
                        if (vfx != null && (cbSnap.targetPropertyName == "Color" || (!string.IsNullOrEmpty(cbSnap.targetComponentType) && cbSnap.targetComponentType.Contains("VisualEffect"))))
                        {
                            targetGo = vfx.gameObject;
                        }
                        else if (controller != null)
                        {
                            targetGo = controller.gameObject;
                        }
                    }

                    if (targetGo != null)
                    {
                        Component targetComp = null;
                        if (!string.IsNullOrEmpty(cbSnap.targetComponentType))
                        {
                            targetComp = targetGo.GetComponents<Component>().FirstOrDefault(c => c != null && c.GetType().FullName == cbSnap.targetComponentType);
                        }
                        if (targetComp == null)
                        {
                            targetComp = targetGo.GetComponent<UnityEngine.VFX.VisualEffect>() as Component
                                      ?? targetGo.GetComponent<Renderer>() as Component;
                        }

#if UNITY_EDITOR
                        var newColorBinder = UnityEditor.Undo.AddComponent(targetGo, colorBinderType);
#else
                        var newColorBinder = targetGo.AddComponent(colorBinderType);
#endif
                        colorBinderType.GetField("channelId")?.SetValue(newColorBinder, cbSnap.channelId);
                        colorBinderType.GetField("targetComponent")?.SetValue(newColorBinder, targetComp);
                        colorBinderType.GetField("targetPropertyName")?.SetValue(newColorBinder, cbSnap.targetPropertyName);
                        colorBinderType.GetField("colorGradient")?.SetValue(newColorBinder, cbSnap.colorGradient);
                        colorBinderType.GetMethod("InitializeBindings")?.Invoke(newColorBinder, null);
                    }
                }
            }

#if UNITY_EDITOR
            if (controller != null) UnityEditor.EditorUtility.SetDirty(controller);
            if (voxelizer != null) UnityEditor.EditorUtility.SetDirty(voxelizer);
#endif

            // Solo se il template non specificava una dimensione valida, calcolala con FitToNodes
            if (voxelizer != null && voxelizer.volumeSize < 0.1f)
            {
                voxelizer.FitToNodes();
            }

            Debug.Log($"[SdfSceneTemplate] Template '{name}' applicato con successo alla scena!");
        }
    }
}
