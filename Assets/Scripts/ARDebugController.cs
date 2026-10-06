using Google.XR.ARCoreExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARDebugController : MonoBehaviour
{
    [Header("Core AR References")]
    [SerializeField] private AREarthManager earthManager;
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private Transform gisModelContainer;

    [Header("Manual Offset")]
    [SerializeField]
    [Min(0.01f)]
    private float movementStep = 0.5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI statusText;

    private ARGeospatialAnchor currentAnchor;
    private Vector3 currentOffset;

    public Transform GisModelContainer => gisModelContainer;

    private void Start()
    {
        if (gisModelContainer != null)
        {
            currentOffset = gisModelContainer.localPosition;
        }
    }

    public void RestartGeospatialPosition()
    {
        if (earthManager == null ||
            anchorManager == null ||
            gisModelContainer == null)
        {
            SetStatus("Missing AR references.");
            return;
        }

        if (earthManager.EarthTrackingState != TrackingState.Tracking)
        {
            SetStatus("Geospatial tracking is not available.");
            return;
        }

        gisModelContainer.SetParent(null);

        if (currentAnchor != null)
        {
            Destroy(currentAnchor.gameObject);
        }

        GeospatialPose pose = earthManager.CameraGeospatialPose;

        currentAnchor = anchorManager.AddAnchor(
            pose.Latitude,
            pose.Longitude,
            pose.Altitude,
            Quaternion.identity
        );

        if (currentAnchor == null)
        {
            SetStatus("Unable to create geospatial anchor.");
            return;
        }

        gisModelContainer.SetParent(currentAnchor.transform);

        gisModelContainer.localRotation = Quaternion.identity;
        gisModelContainer.localPosition = currentOffset;

        SetStatus("Geospatial anchor updated.");
    }

    public void MoveModelX(int direction)
    {
        ApplyOffset(
            new Vector3(movementStep * direction, 0f, 0f)
        );
    }

    public void MoveModelY(int direction)
    {
        ApplyOffset(
            new Vector3(0f, movementStep * direction, 0f)
        );
    }

    public void MoveModelZ(int direction)
    {
        ApplyOffset(
            new Vector3(0f, 0f, movementStep * direction)
        );
    }

    private void ApplyOffset(Vector3 delta)
    {
        currentOffset += delta;

        if (gisModelContainer != null)
        {
            gisModelContainer.localPosition = currentOffset;
        }

        SetStatus($"Offset: {currentOffset}");
    }

    public void UpdateFadeRadius(float radius)
    {
        Shader.SetGlobalFloat("_GlobalFadeRadius", radius);
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}