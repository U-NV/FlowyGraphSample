using UnityEngine;

namespace FlowyGraph
{
    [System.Serializable]
    public partial class AutoGenNodeDataModule : NodeDataModule
    {
        public override void Init()
        {
            Init(this.GetType());
        }
    }
}
