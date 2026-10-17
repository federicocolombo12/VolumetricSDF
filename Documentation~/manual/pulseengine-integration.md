# Integrating with PulseEngine

While **VolumetricSDF** functions as a fully self-contained procedural implicit modeling and physics engine, it pairs synergistically with **[PulseEngine](https://github.com/federicocolombo12/PulseEngine)** (`com.federicocolombo.pulseengine`) for real-time telemetry-driven generative art, medical XR, and biofeedback.

---

## 1. Architectural Division of Responsibilities

```mermaid
graph LR
    subgraph PulseEngine (Telemetry Engine)
        A["Hardware / Network Sensors (OSC, UDP, BLE)"]
        B["DataStreamRegistry (Zero-GC Bus)"]
        C["DAW Mixer Console (Faders, Mute/Solo, Filtering)"]
        D["DataStreamPropertyBinder / DataStreamColorBinder"]
    end

    subgraph VolumetricSDF (Geometry Engine)
        E["SdfGroupController (CSG Hierarchy)"]
        F["SdfModifierVolume (Force-Fields)"]
        G["SdfVolumeVoxelizer (128³ Compute Grid)"]
        H["Unity VFX Graph (Particle Swarm)"]
    end

    A --> B
    B --> C
    C --> D
    D -- "Float/Color Streams" --> E
    D -- "Torsion/Noise Streams" --> F
    E --> G
    F --> G
    G --> H
```

| Package | Core Responsibility | Output Artifact |
| :--- | :--- | :--- |
| **PulseEngine** | Signal ingestion, anti-jitter filtering, DAW mixing console, property binding. | Normalized numeric and color data streams (`float`, `Color`). |
| **VolumetricSDF** | Implicit CSG raymarching, bounded modifiers, 3D GPU compute voxelization. | Mathematical surfaces and 3D `RenderTexture` for particle collision. |

---

## 2. Standard Telemetry Mappings for Medical XR

When connecting PulseEngine's bio-signal channels to VolumetricSDF, the recommended clinical mappings are:

| PulseEngine Channel | Target Component | Target Property | Perceptual Effect |
| :--- | :--- | :--- | :--- |
| **`heart_rate`** | `SdfGroupController` | `currentPulse` | Rhythmic organ systolic contraction and diastolic relaxation. |
| **`hrv_rmssd`** | `SdfGroupController` | `globalBlendSoftness` | Vagal nerve parasympathetic tone: high HRV relaxes shapes into smooth Bouba; low HRV sharpens edges. |
| **`spo2`** | `VisualEffect` | `ParticleScale` / `Turbulence` | Blood oxygenation: healthy SpO2 ($>97\%$) produces calm coherent flow; hypoxia induces erratic dispersion. |
| **`stress_index`** | `SdfModifierVolume` | `strength` | Autonomic arousal drives twisting and high-frequency surface noise deformation. |

---

## 3. How to Bind Channels via the Inspector

1. Ensure both **`com.federicocolombo.pulseengine`** and **`com.federicocolombo.volumetricsdf`** are imported into your project.
2. Select your `SDF_Volume` GameObject.
3. Add a **`DataStreamPropertyBinder`** component (from `PulseEngine`):
   * **Channel ID**: `heart_rate`
   * **Target Component**: Select `SdfGroupController`
   * **Target Property Name**: `currentPulse`
   * **Local Remap**: `[0.0, 1.0]` $\rightarrow$ `[0.0, 0.2]`
4. Open the **DAW Mixer Window** (**Window** > **PulseEngine** > **Virtual DAW Mixer**):
   * Move the `heart_rate` fader or connect an OSC stream: the SDF geometry breathes and deforms in real time!
