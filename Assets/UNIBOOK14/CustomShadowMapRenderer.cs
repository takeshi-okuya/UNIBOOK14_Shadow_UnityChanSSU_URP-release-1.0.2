using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class CustomShadowMapRenderer : MonoBehaviour
{
    public static List<CustomShadowMapRenderer> Instances { private set; get; }
                                          = new List<CustomShadowMapRenderer>();

    public Material shadowCaster;
    public new Transform light;
    public Transform center;

    public Renderer[] casters;

    public Vector3 range = Vector3.one;
    public Vector2Int resolution = new Vector2Int(1024, 1024);

    RenderTexture shadowMap;
    Matrix4x4 view, proj;

    void LateUpdate()
    {
        if (shadowCaster == null || light == null || center == null) { return; }

        if (shadowMap == null ||
            shadowMap.width != resolution.x || shadowMap.height != resolution.y)
        {
            shadowMap?.Release();
            shadowMap = new RenderTexture(resolution.x, resolution.y,
                                          24, RenderTextureFormat.RFloat);
            shadowMap.Create();
        }

        var lightRotation = light.rotation;
        var rotationMatrix = Matrix4x4.Rotate(lightRotation);
        var lightDirection = rotationMatrix.MultiplyPoint(Vector3.forward);
        var lightPosition = center.position - lightDirection * (range.z * 0.5f);

        view = Matrix4x4.Scale(new Vector3(1, 1, -1))
             * Matrix4x4.TRS(lightPosition, lightRotation, Vector3.one).inverse;

        proj = Matrix4x4.Ortho(-range.x * 0.5f, range.x * 0.5f,
                               -range.y * 0.5f, range.y * 0.5f,
                               0, range.z);
    }

    void OnEnable()
    {
        Instances.Add(this);
    }

    void OnDisable()
    {
        Instances.Remove(this);
    }

    public void renderCasters(CommandBuffer cmd)
    {
        cmd.SetViewProjectionMatrices(view, proj);
        cmd.SetRenderTarget(shadowMap);

        Color clearColor = new Color(float.MaxValue, 0, 0, 0);
        cmd.ClearRenderTarget(true, true, clearColor);

        foreach (var renderer in casters)
        {
            if (renderer == null) { continue; }
            cmd.DrawRenderer(renderer, shadowCaster);
        }
    }
}