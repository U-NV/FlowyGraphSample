using System;
using System.Collections;
using System.Collections.Generic;
using FlowyGraph;
using UnityEngine;

public class FlowyGrapProperty : MonoBehaviour
{
    [SerializeField] private FlowyGraphAsset graphAsset;

    private FlowyGraphRuntime graphRuntime;

    public void OnFlowyGraphStart()
    {
        graphRuntime = new FlowyGraphRuntime(graphAsset);
        graphRuntime.OnGraphOver += OnGraphOver;
        graphRuntime.Start();
    }
    private void OnGraphOver(FlowyGraphRuntime runtime)
    {
        
    }
}
