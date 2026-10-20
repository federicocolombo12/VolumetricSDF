# Volumetric SDF Showcase

Questo sample dimostra le funzionalità autonome di **Volumetric SDF Engine** in Unity 6:

1. **Raymarching Implicito CSG**: Rendering di primitive matematiche (Sfere, Tori, Box) e texture 3D con fusione organica Smooth CSG $C^1$.
2. **Volumi Modificatori MudBun**: Deformazioni spaziali orientate (Twist, Bend, Noise) con campo di decadimento delimitato.
3. **Cinematica Spaziale (`ProceduralMotionDriver`)**: Movimento continuo parametrico (Lissajous figure-8, orbita, ping-pong, spiral, perlin drift) in tempo reale.
4. **Voxelizzazione GPU 3D per VFX Graph**: Cattura volumetrica a $96^3 / 128^3$ in `RenderTexture` 3D (RHalf) e conformazione delle particelle della Visual Effect Graph alla superficie SDF.
5. **Template di Scena (`SdfSceneTemplate`)**: Supporto integrato per caricare e applicare 9 configurazioni geometriche (Calma, Stress, Ipossia, BoubaKiki, Infinito, ecc.) direttamente dall'Ispettore.

### Come utilizzarlo:
1. Apri la scena `VolumetricSDFDemo.unity`.
2. Premi **Play** per osservare l'animazione cinematica e la conformazione delle particelle.
3. Seleziona l'oggetto `SDF_Volume` per modificare la morbidezza CSG o aggiungere nuovi nodi ed effetti procedurali.
4. Per cambiare preset geometrico, seleziona un file `.asset` nella cartella `Runtime/Templates/` e clicca sul pulsante **[🎬 Applica alla Scena Corrente]**.
