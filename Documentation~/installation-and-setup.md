# Installation & Setup

## Requirements
* **Unity Version**: Unity 6 (6000.0+) or Unity 2022.3 / 2023 LTS
* **Render Pipeline**: Universal Render Pipeline (URP 17.0+)
* **Dependencies**:
  * `com.unity.render-pipelines.universal`: 17.0.0+
  * `com.unity.visualeffectgraph`: 17.0.0+
  * `com.unity.mathematics`: 1.3.2+

---

## Installing via Unity Package Manager (UPM)

1. In the Unity Editor menu bar, go to **Window** > **Package Manager**.
2. Click the **+** (plus icon) in the upper-left corner and select **Add package from git URL...**
3. Paste the following URL and click **Add**:
   ```text
   https://github.com/federicocolombo12/VolumetricSDF.git
   ```
4. Unity will resolve dependencies and import the `Volumetric SDF Engine` package cleanly into your project's `Packages` folder.

---

## Verifying URP & VFX Graph Configuration

1. In **Project Settings** > **Graphics** > **URP Global Settings**, ensure your active URP Asset has Compute Shaders enabled (enabled by default in modern URP).
2. In **Project Settings** > **VFX**, ensure the **Compute Shader** option is enabled.
3. If developing for **Meta Quest 3 Standalone**:
   * Set Color Space to **Linear**.
   * Under **XR Plug-in Management** > **Oculus**, select **Vulkan** as the Graphics API for optimal compute dispatch throughput.
