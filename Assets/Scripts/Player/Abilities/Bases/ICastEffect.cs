using UnityEngine;

namespace Hypersycos.RogueFrame
{
    public interface ICastEffect
    {
        void Cast(Vector3 cameraPosition, Quaternion lookDirection, PlayerState caster);
    }
}