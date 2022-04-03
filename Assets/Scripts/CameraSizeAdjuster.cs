using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSizeAdjuster : MonoBehaviour
{

    Camera cam;

    public float headerExclusionZone;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        AdjustCamera();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("AdjustCamera")]
    public void AdjustCamera()
    {
        float vertical = 14f * (1f + (float)headerExclusionZone / cam.pixelHeight);
        float horizontal = 14f * (cam.pixelHeight) / cam.pixelWidth;

        var size = Mathf.Max(horizontal, vertical) * 0.5f;
        cam.orthographicSize = size;
        Vector3 camPos = cam.transform.position;
        camPos.z = (vertical - 14f) / 2f;
        cam.transform.position = camPos;
    }
}
