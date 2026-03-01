using System.Collections.Generic;
using UnityEngine;

namespace SubSystems.AllScenes.UI
{
    [CreateAssetMenu(fileName = "TypewriteAnimConfig", menuName = "Configs/TypewriteAnimConfig")]
    public class TypewriteAnimConfig : ScriptableObject
    {
        [SerializeField] private float defaultWaitTimePreChar = 0.05f;
        public float DefaultWaitTimePreChar => defaultWaitTimePreChar;
        
        [SerializeField] public List<char> punctuations = new ()
        {
            ',', '.', '!', '?', '\t', '\n', '\r',
            '，', '。', '！', '？','、',
            '~', '…', '—'
        };
        public List<char> Punctuations => punctuations;
        
        [SerializeField] private float punctuationWaitTime = 0.2f;
        public float PunctuationWaitTime => punctuationWaitTime;
    }
}
