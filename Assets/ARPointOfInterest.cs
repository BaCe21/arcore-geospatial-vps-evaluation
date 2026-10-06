using UnityEngine;

public class ARPointOfInterest : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Canvas infoCanvas;

    [SerializeField]
    [Min(0f)]
    private float activationDistance = 5f;

    private Transform mainCameraTransform;

    private void Start()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            mainCameraTransform = mainCamera.transform;
        }

        if (infoCanvas != null)
        {
            infoCanvas.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (mainCameraTransform == null || infoCanvas == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            mainCameraTransform.position
        );

        bool shouldBeVisible =
            distance <= activationDistance;

        if (infoCanvas.gameObject.activeSelf != shouldBeVisible)
        {
            infoCanvas.gameObject.SetActive(shouldBeVisible);
        }

        if (!shouldBeVisible)
            return;

        infoCanvas.transform.LookAt(
            infoCanvas.transform.position +
            mainCameraTransform.rotation * Vector3.forward,
            mainCameraTransform.rotation * Vector3.up
        );
    }
}