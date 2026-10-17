using UnityEngine;

namespace MedicalXR.RayMarching
{
    /// <summary>
    /// Interfaccia implementata da componenti di cinematica procedurale per informare
    /// il voxelizer e il renderer dell'estensione massima del movimento spaziale,
    /// prevenendo clipping al di fuori dei volumi di calcolo (es. AABB 3D).
    /// </summary>
    public interface IMotionBoundsProvider
    {
        /// <summary>
        /// Restituisce lo spostamento massimo (in coordinate locali) che l'oggetto può raggiungere rispetto alla sua posizione base.
        /// </summary>
        Vector3 GetMaxDisplacement();
    }
}
