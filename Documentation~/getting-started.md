# Getting Started with VolumetricSDF

This guide walks you through creating your first procedural SDF volume, adding CSG nodes, and connecting the real-time GPU Voxelizer to Unity VFX Graph.

---

## 1. Creating the SDF Volume Master

1. In your Unity scene hierarchy, create an empty GameObject and name it `SDF_Volume`.
2. Add a `MeshFilter` and a `MeshRenderer` component to it.
3. Attach the **`SdfGroupController`** component.
   * `SdfGroupController` automatically checks and assigns the required `BioSignalSDF_Mat` material to the renderer.
   * Check **Render Solid Mesh** to enable visual fragment raymarching in the Scene view.

---

## 2. Adding CSG Primitives

1. Create a child GameObject under `SDF_Volume` and name it `SdfNode_BaseSphere`.
2. Attach the **`SdfNode`** component.
   * **Shape Type**: `Sphere`
   * **Size**: `(0.2, 0.2, 0.2)`
   * **Combine Op**: `SmoothUnion`
   * **Shape Color**: Cyan
3. Duplicate this node to create a second child named `SdfNode_Torus`.
   * **Shape Type**: `Torus`
   * **Size**: `(0.15, 0.04, 0.15)`
   * **Combine Op**: `SmoothUnion`
   * **Blend Softness**: `0.08`
   * **Shape Color**: Magenta
4. Move and rotate the Torus using standard Unity transform gizmos in the Scene view: you will immediately see the two shapes smoothly melt into one another with continuous normal interpolation!

<p align="center">
  <img src="images/csg_modeling_demo.gif" width="80%" alt="Procedural CSG Modeling" />
</p>

---

## 3. Adding a Bounded Modifier Volume

1. Create another child under `SDF_Volume` and attach **`SdfModifierVolume`**.
2. Select **Modifier Type**: `Twist` or `Noise`.
3. Adjust **Strength** and **Frequency**.
4. Notice how only the parts of the SDF geometry lying inside the modifier's bounding box are twisted, leaving the exterior completely undisturbed!

---

## 4. Connecting to VFX Graph (Particle Swarm Collision)

1. On the root `SDF_Volume` GameObject, attach **`SdfVolumeVoxelizer`**.
2. Set **Texture Resolution** to `64` or `96`.
3. In your target **VFX Graph**:
   * Add Blackboard attributes:
     * `SdfVolumeTexture` (Texture3D)
     * `SdfVolumeSize` (Vector3)
     * `SdfVolumeCenter` (Vector3)
     * `SdfVolumeTransform` (Matrix4x4)
   * In the **Update Context**, add the **Conform to SDF** block.
   * In the **Output Context**, add the **Sample SDF Texture** block to read the RGB channels for per-particle color tinting.
4. Drag your VFX GameObject into the **Target VFX Graph** slot on `SdfVolumeVoxelizer`.
5. Run the scene: your particles will instantly cling to the implicit zero-isosurface!

<p align="center">
  <img src="images/vfx_swarm_adhesion.gif" width="80%" alt="VFX Swarm Adhesion" />
</p>
