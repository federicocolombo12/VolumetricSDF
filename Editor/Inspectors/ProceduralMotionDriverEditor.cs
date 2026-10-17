using UnityEditor;
using UnityEngine;

namespace MedicalXR.RayMarching
{
    [CustomEditor(typeof(ProceduralMotionDriver))]
    [CanEditMultipleObjects]
    public class ProceduralMotionDriverEditor : UnityEditor.Editor
    {
        private SerializedProperty _pattern;
        private SerializedProperty _speedMultiplier;
        private SerializedProperty _amplitudeMultiplier;
        private SerializedProperty _noiseTurbulence;
        private SerializedProperty _phaseOffset;

        private SerializedProperty _lissajousAmplitude;
        private SerializedProperty _lissajousFrequency;
        private SerializedProperty _lissajousPhase;

        private SerializedProperty _orbitPlane;
        private SerializedProperty _orbitRadius;
        private SerializedProperty _orbitSpeed;
        private SerializedProperty _orbitClockwise;

        private SerializedProperty _pingPongAxis;
        private SerializedProperty _pingPongDistance;
        private SerializedProperty _pingPongFrequency;
        private SerializedProperty _pingPongSmoothEasing;

        private SerializedProperty _driftAmplitude;
        private SerializedProperty _driftSpeed;
        private SerializedProperty _driftOctaves;
        private SerializedProperty _driftSeed;

        private SerializedProperty _spiralPlane;
        private SerializedProperty _spiralRadius;
        private SerializedProperty _spiralSpeed;
        private SerializedProperty _spiralTravelAxis;
        private SerializedProperty _spiralTravelDistance;
        private SerializedProperty _spiralTravelFrequency;

        private void OnEnable()
        {
            _pattern = serializedObject.FindProperty("pattern");
            _speedMultiplier = serializedObject.FindProperty("speedMultiplier");
            _amplitudeMultiplier = serializedObject.FindProperty("amplitudeMultiplier");
            _noiseTurbulence = serializedObject.FindProperty("noiseTurbulence");
            _phaseOffset = serializedObject.FindProperty("phaseOffset");

            _lissajousAmplitude = serializedObject.FindProperty("lissajousAmplitude");
            _lissajousFrequency = serializedObject.FindProperty("lissajousFrequency");
            _lissajousPhase = serializedObject.FindProperty("lissajousPhase");

            _orbitPlane = serializedObject.FindProperty("orbitPlane");
            _orbitRadius = serializedObject.FindProperty("orbitRadius");
            _orbitSpeed = serializedObject.FindProperty("orbitSpeed");
            _orbitClockwise = serializedObject.FindProperty("orbitClockwise");

            _pingPongAxis = serializedObject.FindProperty("pingPongAxis");
            _pingPongDistance = serializedObject.FindProperty("pingPongDistance");
            _pingPongFrequency = serializedObject.FindProperty("pingPongFrequency");
            _pingPongSmoothEasing = serializedObject.FindProperty("pingPongSmoothEasing");

            _driftAmplitude = serializedObject.FindProperty("driftAmplitude");
            _driftSpeed = serializedObject.FindProperty("driftSpeed");
            _driftOctaves = serializedObject.FindProperty("driftOctaves");
            _driftSeed = serializedObject.FindProperty("driftSeed");

            _spiralPlane = serializedObject.FindProperty("spiralPlane");
            _spiralRadius = serializedObject.FindProperty("spiralRadius");
            _spiralSpeed = serializedObject.FindProperty("spiralSpeed");
            _spiralTravelAxis = serializedObject.FindProperty("spiralTravelAxis");
            _spiralTravelDistance = serializedObject.FindProperty("spiralTravelDistance");
            _spiralTravelFrequency = serializedObject.FindProperty("spiralTravelFrequency");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var driver = (ProceduralMotionDriver)target;

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Kinematic Trajectory", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_pattern);

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Kinematic Modulation Surface (PulseEngine)", EditorStyles.boldLabel);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(_speedMultiplier, new GUIContent("Speed (Temporal Freq)"));
                EditorGUILayout.PropertyField(_amplitudeMultiplier, new GUIContent("Amplitude (Spatial Reach)"));
                EditorGUILayout.PropertyField(_noiseTurbulence, new GUIContent("Noise Turbulence (EDA)"));
                EditorGUILayout.PropertyField(_phaseOffset, new GUIContent("Phase Offset"));
            }

            var patternType = (MotionPattern)_pattern.enumValueIndex;
            if (patternType != MotionPattern.None)
            {
                EditorGUILayout.Space(6);
                EditorGUILayout.LabelField($"{patternType} Trajectory Settings", EditorStyles.boldLabel);
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    switch (patternType)
                    {
                        case MotionPattern.Lissajous:
                            EditorGUILayout.PropertyField(_lissajousAmplitude);
                            EditorGUILayout.PropertyField(_lissajousFrequency);
                            EditorGUILayout.PropertyField(_lissajousPhase);
                            break;

                        case MotionPattern.Orbit:
                            EditorGUILayout.PropertyField(_orbitPlane);
                            EditorGUILayout.PropertyField(_orbitRadius);
                            EditorGUILayout.PropertyField(_orbitSpeed);
                            EditorGUILayout.PropertyField(_orbitClockwise);
                            break;

                        case MotionPattern.PingPong:
                            EditorGUILayout.PropertyField(_pingPongAxis);
                            EditorGUILayout.PropertyField(_pingPongDistance);
                            EditorGUILayout.PropertyField(_pingPongFrequency);
                            EditorGUILayout.PropertyField(_pingPongSmoothEasing);
                            break;

                        case MotionPattern.PerlinDrift:
                            EditorGUILayout.PropertyField(_driftAmplitude);
                            EditorGUILayout.PropertyField(_driftSpeed);
                            EditorGUILayout.PropertyField(_driftOctaves);
                            EditorGUILayout.PropertyField(_driftSeed);
                            break;

                        case MotionPattern.Spiral:
                            EditorGUILayout.PropertyField(_spiralPlane);
                            EditorGUILayout.PropertyField(_spiralRadius);
                            EditorGUILayout.PropertyField(_spiralSpeed);
                            EditorGUILayout.PropertyField(_spiralTravelAxis);
                            EditorGUILayout.PropertyField(_spiralTravelDistance);
                            EditorGUILayout.PropertyField(_spiralTravelFrequency);
                            break;
                    }
                }
            }

            EditorGUILayout.Space(6);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                Vector3 maxDisp = driver.GetMaxDisplacement();
                EditorGUILayout.LabelField("Max Bounding Reach", $"X: {maxDisp.x:F3}, Y: {maxDisp.y:F3}, Z: {maxDisp.z:F3}");

                if (Application.isPlaying)
                {
                    Vector3 curDisp = driver.CurrentDisplacement;
                    EditorGUILayout.LabelField("Current Displacement", $"X: {curDisp.x:F3}, Y: {curDisp.y:F3}, Z: {curDisp.z:F3}");
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Capture Base Position"))
                {
                    Undo.RecordObject(driver, "Capture Base Position");
                    driver.CaptureBasePosition();
                }
                if (GUILayout.Button("Reset to Base Position"))
                {
                    Undo.RecordObject(driver.transform, "Reset Position");
                    driver.RestoreBasePosition();
                }
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
