using UnityEngine;

namespace MedicalXR.RayMarching
{
    public enum MotionPattern
    {
        None = 0,
        Lissajous = 1,
        Orbit = 2,
        PingPong = 3,
        PerlinDrift = 4,
        Spiral = 5
    }

    public enum MotionOrbitPlane
    {
        XZ = 0, // Piano orizzontale
        XY = 1, // Piano frontale
        YZ = 2  // Piano sagittale
    }

    /// <summary>
    /// Componente modulare di cinematica spaziale procedurale per SdfNode e oggetti di scena.
    /// Supporta molteplici traiettorie matematiche guidabili in tempo reale da biosensori
    /// tramite la Kinematic Modulation Surface di PulseEngine.
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [SelectionBase]
    public class ProceduralMotionDriver : MonoBehaviour, IMotionBoundsProvider
    {
        [Header("Kinematic Pattern")]
        [Tooltip("Traiettoria matematica applicata alle coordinate locali dell'oggetto.")]
        public MotionPattern pattern = MotionPattern.None;

        [Header("Kinematic Modulation Surface (PulseEngine)")]
        [Tooltip("Moltiplicatore universale di velocità/frequenza temporale (ideale per Heart Rate / BVP).")]
        public float speedMultiplier = 1.0f;

        [Tooltip("Moltiplicatore universale di ampiezza/estensione spaziale (ideale per HRV / Respiration).")]
        public float amplitudeMultiplier = 1.0f;

        [Range(0f, 1f)]
        [Tooltip("Intensità di micro-agitazione caotica stocastica ad alta frequenza (ideale per EDA / Galvanic Skin Response).")]
        public float noiseTurbulence = 0.0f;

        [Tooltip("Sfasamento temporale angolare (in cicli o radianti) per coreografie e de-sincronizzazione multi-nodo.")]
        public float phaseOffset = 0.0f;

        [Header("Lissajous Settings")]
        [Tooltip("Ampiezza massima indipendente per ciascun asse X, Y, Z.")]
        public Vector3 lissajousAmplitude = new Vector3(0.06f, 0.06f, 0.06f);
        [Tooltip("Rapporti di frequenza armonica sui tre assi (es. 1, 0, 2 per figura a 8).")]
        public Vector3 lissajousFrequency = new Vector3(1f, 0f, 2f);
        [Tooltip("Sfasamento angolare vettoriale aggiuntivo.")]
        public Vector3 lissajousPhase = Vector3.zero;

        [Header("Orbit Settings")]
        public MotionOrbitPlane orbitPlane = MotionOrbitPlane.XZ;
        [Tooltip("Raggi dell'orbita piana (X = primo asse del piano, Y = secondo asse). Permette orbite ellittiche.")]
        public Vector2 orbitRadius = new Vector2(0.08f, 0.08f);
        public float orbitSpeed = 1.0f;
        public bool orbitClockwise = true;

        [Header("Ping-Pong Settings")]
        [Tooltip("Vettore di direzione dell'oscillazione lineare.")]
        public Vector3 pingPongAxis = Vector3.up;
        [Tooltip("Distanza massima percorsa dall'ancora centrale verso ciascun estremo.")]
        public float pingPongDistance = 0.08f;
        public float pingPongFrequency = 1.0f;
        [Tooltip("Se abilitato usa attenuazione sinusoidale; altrimenti oscillazione lineare continua triangolare.")]
        public bool pingPongSmoothEasing = true;

        [Header("Perlin Drift Settings")]
        [Tooltip("Ampiezza massima del wander organico per ciascun asse.")]
        public Vector3 driftAmplitude = new Vector3(0.05f, 0.05f, 0.05f);
        [Tooltip("Velocità di progressione nel rumore di Perlin per ciascun asse.")]
        public Vector3 driftSpeed = new Vector3(0.5f, 0.5f, 0.5f);
        [Range(1, 3)]
        public int driftOctaves = 2;
        public float driftSeed = 42.0f;

        [Header("Spiral Settings")]
        public MotionOrbitPlane spiralPlane = MotionOrbitPlane.XZ;
        public float spiralRadius = 0.06f;
        public float spiralSpeed = 1.0f;
        [Tooltip("Asse lungo il quale la spirale si sposta avanti e indietro.")]
        public Vector3 spiralTravelAxis = Vector3.up;
        public float spiralTravelDistance = 0.06f;
        public float spiralTravelFrequency = 0.5f;

        [SerializeField, HideInInspector]
        private Vector3 _basePosition;
        private float _timeAccumulator;
        private bool _isInitialized;

        public Vector3 BasePosition
        {
            get => _basePosition;
            set => _basePosition = value;
        }

        public Vector3 CurrentDisplacement { get; private set; }

        private void OnEnable()
        {
            CaptureBasePosition();
        }

        private void Start()
        {
            CaptureBasePosition();
        }

        private void OnDisable()
        {
            RestoreBasePosition();
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                CaptureBasePosition();
            }
        }

        public void CaptureBasePosition()
        {
            if (!_isInitialized || !Application.isPlaying)
            {
                _basePosition = transform.localPosition;
                _isInitialized = true;
            }
        }

        public void RestoreBasePosition()
        {
            if (_isInitialized)
            {
                transform.localPosition = _basePosition;
                CurrentDisplacement = Vector3.zero;
            }
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                // In edit mode aggiorniamo la base position se l'utente sposta l'oggetto
                _basePosition = transform.localPosition;
                return;
            }

            if (!_isInitialized)
            {
                CaptureBasePosition();
            }

            if (pattern == MotionPattern.None || Mathf.Approximately(amplitudeMultiplier, 0f))
            {
                transform.localPosition = _basePosition;
                CurrentDisplacement = Vector3.zero;
                return;
            }

            _timeAccumulator += Time.deltaTime * speedMultiplier;
            float t = _timeAccumulator + phaseOffset;

            Vector3 patternOffset = EvaluatePattern(t);

            // Micro-agitazione stocastica (EDA/Stress)
            if (noiseTurbulence > 0.001f)
            {
                float noiseTime = t * 6f;
                float nx = (Mathf.PerlinNoise(noiseTime, 13.37f + driftSeed) - 0.5f) * 2f;
                float ny = (Mathf.PerlinNoise(noiseTime, 42.42f + driftSeed) - 0.5f) * 2f;
                float nz = (Mathf.PerlinNoise(noiseTime, 77.77f + driftSeed) - 0.5f) * 2f;
                Vector3 jitter = new Vector3(nx, ny, nz) * (noiseTurbulence * 0.04f);
                patternOffset += jitter;
            }

            CurrentDisplacement = patternOffset * amplitudeMultiplier;
            transform.localPosition = _basePosition + CurrentDisplacement;
        }

        private Vector3 EvaluatePattern(float t)
        {
            switch (pattern)
            {
                case MotionPattern.Lissajous:
                {
                    return new Vector3(
                        Mathf.Sin(t * lissajousFrequency.x * Mathf.PI * 2f + lissajousPhase.x) * lissajousAmplitude.x,
                        Mathf.Sin(t * lissajousFrequency.y * Mathf.PI * 2f + lissajousPhase.y) * lissajousAmplitude.y,
                        Mathf.Sin(t * lissajousFrequency.z * Mathf.PI * 2f + lissajousPhase.z) * lissajousAmplitude.z
                    );
                }

                case MotionPattern.Orbit:
                {
                    float dir = orbitClockwise ? -1f : 1f;
                    float angle = t * orbitSpeed * Mathf.PI * 2f * dir;
                    float u = Mathf.Cos(angle) * orbitRadius.x;
                    float v = Mathf.Sin(angle) * orbitRadius.y;
                    return ProjectToPlane(u, v, orbitPlane);
                }

                case MotionPattern.PingPong:
                {
                    Vector3 dir = pingPongAxis.sqrMagnitude > 0.001f ? pingPongAxis.normalized : Vector3.up;
                    float progress;
                    if (pingPongSmoothEasing)
                    {
                        progress = Mathf.Sin(t * pingPongFrequency * Mathf.PI * 2f);
                    }
                    else
                    {
                        progress = (Mathf.PingPong(t * pingPongFrequency * 2f, 2f) - 1f);
                    }
                    return dir * (progress * pingPongDistance);
                }

                case MotionPattern.PerlinDrift:
                {
                    Vector3 drift = Vector3.zero;
                    float currentFreq = 1f;
                    float currentAmp = 1f;
                    float totalWeight = 0f;

                    for (int o = 0; o < driftOctaves; o++)
                    {
                        float nx = (Mathf.PerlinNoise(t * driftSpeed.x * currentFreq + driftSeed, (o + 1) * 11.1f) - 0.5f) * 2f;
                        float ny = (Mathf.PerlinNoise(t * driftSpeed.y * currentFreq + driftSeed, (o + 1) * 33.3f) - 0.5f) * 2f;
                        float nz = (Mathf.PerlinNoise(t * driftSpeed.z * currentFreq + driftSeed, (o + 1) * 55.5f) - 0.5f) * 2f;

                        drift += new Vector3(nx * driftAmplitude.x, ny * driftAmplitude.y, nz * driftAmplitude.z) * currentAmp;
                        totalWeight += currentAmp;
                        currentFreq *= 2f;
                        currentAmp *= 0.5f;
                    }

                    return totalWeight > 0.001f ? drift / totalWeight : drift;
                }

                case MotionPattern.Spiral:
                {
                    float angle = t * spiralSpeed * Mathf.PI * 2f;
                    float u = Mathf.Cos(angle) * spiralRadius;
                    float v = Mathf.Sin(angle) * spiralRadius;
                    Vector3 planarPart = ProjectToPlane(u, v, spiralPlane);

                    Vector3 travelDir = spiralTravelAxis.sqrMagnitude > 0.001f ? spiralTravelAxis.normalized : Vector3.up;
                    float travelPart = Mathf.Sin(t * spiralTravelFrequency * Mathf.PI * 2f) * spiralTravelDistance;

                    return planarPart + travelDir * travelPart;
                }

                default:
                    return Vector3.zero;
            }
        }

        private static Vector3 ProjectToPlane(float u, float v, MotionOrbitPlane plane)
        {
            switch (plane)
            {
                case MotionOrbitPlane.XZ:
                    return new Vector3(u, 0f, v);
                case MotionOrbitPlane.XY:
                    return new Vector3(u, v, 0f);
                case MotionOrbitPlane.YZ:
                    return new Vector3(0f, u, v);
                default:
                    return new Vector3(u, 0f, v);
            }
        }

        /// <summary>
        /// Calcola analiticamente la massima escursione spaziale possibile per l'attuale configurazione,
        /// permettendo a SdfVolumeVoxelizer di espandere l'AABB per prevenire volume clipping.
        /// </summary>
        public Vector3 GetMaxDisplacement()
        {
            if (pattern == MotionPattern.None) return Vector3.zero;

            float absAmp = Mathf.Abs(amplitudeMultiplier);
            Vector3 baseDisp = Vector3.zero;

            switch (pattern)
            {
                case MotionPattern.Lissajous:
                    baseDisp = new Vector3(
                        Mathf.Abs(lissajousAmplitude.x),
                        Mathf.Abs(lissajousAmplitude.y),
                        Mathf.Abs(lissajousAmplitude.z)
                    );
                    break;

                case MotionPattern.Orbit:
                    switch (orbitPlane)
                    {
                        case MotionOrbitPlane.XZ:
                            baseDisp = new Vector3(Mathf.Abs(orbitRadius.x), 0f, Mathf.Abs(orbitRadius.y));
                            break;
                        case MotionOrbitPlane.XY:
                            baseDisp = new Vector3(Mathf.Abs(orbitRadius.x), Mathf.Abs(orbitRadius.y), 0f);
                            break;
                        case MotionOrbitPlane.YZ:
                            baseDisp = new Vector3(0f, Mathf.Abs(orbitRadius.x), Mathf.Abs(orbitRadius.y));
                            break;
                    }
                    break;

                case MotionPattern.PingPong:
                    Vector3 pDir = pingPongAxis.sqrMagnitude > 0.001f ? pingPongAxis.normalized : Vector3.up;
                    baseDisp = new Vector3(
                        Mathf.Abs(pDir.x * pingPongDistance),
                        Mathf.Abs(pDir.y * pingPongDistance),
                        Mathf.Abs(pDir.z * pingPongDistance)
                    );
                    break;

                case MotionPattern.PerlinDrift:
                    baseDisp = new Vector3(
                        Mathf.Abs(driftAmplitude.x),
                        Mathf.Abs(driftAmplitude.y),
                        Mathf.Abs(driftAmplitude.z)
                    );
                    break;

                case MotionPattern.Spiral:
                    Vector3 sPlane = Vector3.zero;
                    switch (spiralPlane)
                    {
                        case MotionOrbitPlane.XZ: sPlane = new Vector3(spiralRadius, 0f, spiralRadius); break;
                        case MotionOrbitPlane.XY: sPlane = new Vector3(spiralRadius, spiralRadius, 0f); break;
                        case MotionOrbitPlane.YZ: sPlane = new Vector3(0f, spiralRadius, spiralRadius); break;
                    }
                    Vector3 sTravel = (spiralTravelAxis.sqrMagnitude > 0.001f ? spiralTravelAxis.normalized : Vector3.up) * spiralTravelDistance;
                    baseDisp = sPlane + new Vector3(Mathf.Abs(sTravel.x), Mathf.Abs(sTravel.y), Mathf.Abs(sTravel.z));
                    break;
            }

            Vector3 totalDisp = baseDisp * absAmp;
            if (noiseTurbulence > 0.001f)
            {
                totalDisp += Vector3.one * (noiseTurbulence * 0.04f * absAmp);
            }

            return totalDisp;
        }
    }
}
