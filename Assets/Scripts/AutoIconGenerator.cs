using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectPickUp))]
public class AutoIconGenerator : MonoBehaviour
{
    [Header("Icon Settings")]
    public int iconSize = 128;
    public Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f); // Dark gray

    [Header("Zoom Settings")]
    public float zoomLevel = 0.8f; // Smaller = more zoomed in
    public Vector3 cameraOffset = new Vector3(0, 0, -2.5f);

    private IEnumerator Start()
    {
        ObjectPickUp pickup = GetComponent<ObjectPickUp>();
        if (pickup == null || pickup.icon != null)
            yield break;

        yield return new WaitForEndOfFrame();
        GenerateIconWithSolidBackground();
    }

    private void GenerateIconWithSolidBackground()
    {
        GameObject camObj = new GameObject("IconCamera");
        camObj.hideFlags = HideFlags.HideAndDontSave;

        Camera renderCam = camObj.AddComponent<Camera>();
        renderCam.enabled = false;
        renderCam.orthographic = true;
        renderCam.clearFlags = CameraClearFlags.SolidColor;
        renderCam.backgroundColor = backgroundColor;
        renderCam.cullingMask = 1 << gameObject.layer;

        // Calculate optimal zoom based on object size
        Bounds bounds = CalculateObjectBounds();
        float objectSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

        // SET ZOOM LEVEL 
        renderCam.orthographicSize = objectSize * zoomLevel;

        // Position camera to focus on object
        renderCam.transform.position = bounds.center + cameraOffset;
        renderCam.transform.LookAt(bounds.center);

        RenderTexture rt = new RenderTexture(iconSize, iconSize, 24);
        renderCam.targetTexture = rt;
        renderCam.Render();

        Texture2D tex = new Texture2D(iconSize, iconSize, TextureFormat.ARGB32, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, iconSize, iconSize), 0, 0);
        tex.Apply();

        // Create sprite
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, iconSize, iconSize), new Vector2(0.5f, 0.5f));
        sprite.name = $"{gameObject.name}_Icon";

        // Assign to pickup
        GetComponent<ObjectPickUp>().icon = sprite;

        Debug.Log($"Generated icon for {gameObject.name} with zoom: {zoomLevel}");

        // Cleanup
        RenderTexture.active = null;
        DestroyImmediate(rt);
        DestroyImmediate(camObj);
    }

    private Bounds CalculateObjectBounds()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(transform.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }

    // Editor method to test different zoom levels
#if UNITY_EDITOR
    [ContextMenu("Test Zoom 0.5 (Very Zoomed In)")]
    private void TestZoom05() { TestZoomLevel(0.5f); }
    
    [ContextMenu("Test Zoom 0.8 (Zoomed In)")]
    private void TestZoom08() { TestZoomLevel(0.8f); }
    
    [ContextMenu("Test Zoom 1.2 (Normal)")]
    private void TestZoom12() { TestZoomLevel(1.2f); }
    
    [ContextMenu("Test Zoom 2.0 (Zoomed Out)")]
    private void TestZoom20() { TestZoomLevel(2.0f); }

    private void TestZoomLevel(float testZoom)
    {
        zoomLevel = testZoom;
        GenerateIconWithSolidBackground();
        Debug.Log($"Testing zoom level: {testZoom}");
    }
#endif
}