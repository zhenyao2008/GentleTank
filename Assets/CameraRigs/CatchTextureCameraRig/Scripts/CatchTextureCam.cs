﻿using UnityEngine;

public class CatchTextureCam : MonoBehaviour 
{
    [Range(0.001f,0.1f)]
    public float duration = 0.05f;
    public new Camera camera;
    public Transform stage;

    public bool IsCatching { get { return isCatching; } }

    private bool isCatching =false;
    private Transform currentTarget;
    private Transform lastParent;
    private Vector3 lastPosision;
    private Quaternion lastRotation;

    /// <summary>
    /// 设置要捕获到纹理的目标
    /// </summary>
    /// <param name="target">捕获目标</param>
    /// <param name="targetTexture">捕获到的纹理</param>
    /// <returns>是否设置成功（正在捕获为false）</returns>
    public bool SetCatchTarget(Transform target,RenderTexture targetTexture)
    {
        if (isCatching)
            return false;
        isCatching = true;
        camera.enabled = false;
        currentTarget = target;
        camera.targetTexture = targetTexture;
        lastParent = target.parent;
        lastPosision = target.localPosition;
        lastRotation = target.localRotation;
        target.SetParent(stage);
        GameMathf.ResetLocalTransform(target, true, true, false);
        camera.enabled = true;
        return true;
    }

    /// <summary>
    /// 渲染
    /// </summary>
    private void OnGUI()
    {
        if (currentTarget != null)
        {
            currentTarget.SetParent(lastParent);
            currentTarget.localPosition = lastPosision;
            currentTarget.localRotation = lastRotation;
            currentTarget = null;
            isCatching = false;
            camera.enabled = false;
        }   
    }

}
