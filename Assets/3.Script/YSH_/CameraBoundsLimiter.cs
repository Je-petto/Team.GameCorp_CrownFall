using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraBoundsLimiter : MonoBehaviour
{
    [SerializeField] private BoxCollider boundingArea;

    private Vector3 minBounds;
    private Vector3 maxBounds;

    private Transform _camTransform;
    private CinemachineVirtualCamera _vcam;
    private Camera _mainCam;

    void Start()
    {
        if (boundingArea != null)
        {
            Bounds bounds = boundingArea.bounds;
            minBounds = bounds.min;
            maxBounds = bounds.max;
        }
    }

    void LateUpdate()
    {
        if (_vcam.Follow == null) return;

        if (_camTransform == null)
            _camTransform = _vcam.VirtualCameraGameObject.transform;

        Vector3 pos = _camTransform.position;

        // Clamp camera position to the bounding box
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        pos.z = Mathf.Clamp(pos.z, minBounds.z, maxBounds.z);

        _camTransform.position = pos;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((minBounds + maxBounds) / 2, maxBounds - minBounds);
    }
}