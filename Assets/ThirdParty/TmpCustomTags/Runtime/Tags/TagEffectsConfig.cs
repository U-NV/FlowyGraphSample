using System.Collections;
using System.Collections.Generic;
using Oneiromancer.TMP.Effects;
using UnityEngine;

namespace Oneiromancer.TMP
{
    [CreateAssetMenu(menuName = "Configs/TagEffectsConfig")]
    public class TagEffectsConfig : ScriptableObject
    {
        [SerializeField] private BaseTextEffectSo[] _tagEffects;
        public IReadOnlyCollection<BaseTextEffectSo> TagEffects => _tagEffects;
    }
}
