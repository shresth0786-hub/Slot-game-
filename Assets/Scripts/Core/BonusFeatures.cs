using UnityEngine;
// Compatibility shim for projects that reference BonusFeatures (plural).
// The canonical implementation is BonusFeature (singular). This file provides
// a correctly-namespaced plural alias so that both filenames compile and
// neither triggers CS1069 (ParticleSystem) nor CS0246 (SlotMachine).
// ParticleSystem requires com.unity.modules.particlesystem in Packages/manifest.json.

namespace SlotGame.Core
{
    /// <summary>
    /// Alias for BonusFeature to support both singular/plural filenames.
    /// Keeps particle references fully qualified for assembly robustness.
    /// </summary>
    public class BonusFeatures : BonusFeature
    {
        // Inherits all Nudge/FreeSpins/Particle logic from BonusFeature.
        // No duplicate ParticleSystem fields — base class handles them.
    }
}
