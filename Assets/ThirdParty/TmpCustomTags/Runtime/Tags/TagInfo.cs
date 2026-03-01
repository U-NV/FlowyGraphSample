using System.Collections.Generic;
using UnityEngine;

namespace Oneiromancer.TMP.Tags
{
    /// Custom tag info: it's name, first and last affected text index.
    [System.Serializable]
    public class TagInfo
    {
        public string Tag => _tag;
        public int StartIndex => _startIndex;
        public int LastIndex => _lastIndex;

        [SerializeField] private string _tag;
        [SerializeField] private int _startIndex;
        [SerializeField] private int _lastIndex = -1;
        [SerializeField] protected List<string> tagArgs;
        public IReadOnlyList<string> TagArgs => tagArgs;
        private int _tagHash;

        public TagInfo(string tag, int startIndex, List<string> tagArgs)
        {
            _tag = tag;
            _startIndex = startIndex;
            this.tagArgs = tagArgs;
            
            _tagHash = tag.GetHashCode();
        }

        public void Close(int lastIndex)
        {
            _lastIndex = lastIndex;
        }

        public bool IsTagEqualAndOpen(string tag)
        {
            return LastIndex == -1 && IsTagEqual(tag);
        }
        
        public bool IsTagEqual(string tag)
        {
            return tag.GetHashCode() == _tagHash;
        }
    }

    public class WaitTagInfo : TagInfo
    {
        public WaitTagInfo(string tag, int startIndex, List<string> tagArgs) : base(tag, startIndex, tagArgs)
        {
        }

        public float WaitTime
        {
            get
            {

                return 0;
            }
        }
    }
}