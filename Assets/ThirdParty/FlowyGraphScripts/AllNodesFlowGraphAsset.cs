using System.Collections;
using System.Collections.Generic;
using FlowyGraph;
using UnityEngine;
using UnityEngine.Serialization;

namespace FlowyGraph
{
    [CreateAssetMenu(menuName = "FlowyGraph/FlowyGraphAsset", order = 1)]
    public class AllNodesFlowGraphAsset : FlowyGraphAsset
    {
        [FormerlySerializedAs("customNodeDataModule")] public AutoGenNodeDataModule autoGenNodeDataModule;
        protected override void UpdateRegisterDataModuleList()
        {
            _registerDataModuleList.Add(autoGenNodeDataModule);
        }
    }
}
