using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager;
    public Transform targetTransform; // camera will follow this object
    public Transform cameraPivot; // object that camera will use to pivot
    public Transform cameraTransform; // transform of camera object in scene
    public LayerMask collisionLayers; // layers the camera will collide with
    private float defaultPosition;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;
    public float cameraCollisionRadius = 1;
    public float cameraCollisionOffSet = 0.5f; // how much the camera will jump off from an object its colliding with
    public float minimumCollisionOffSet = 0.2f;
    public float cameraLookSpeed = 0.1f;
    public float cameraFollowSpeed = 0.2f;
    public float cameraPivotSpeed = 0.1f;
    public float lookAngle; // up and down
    public float pivotAngle; // left and right
    public float minimumPivotAngle = -35;
    public float maximumPivotAngle = 35;
    private void Awake() {
        inputManager = FindObjectOfType<InputManager>();
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }
    public void HandleAllCameraMovement() {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }
    private void FollowTarget() { 
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
        transform.position = targetPosition;
    }

    private void RotateCamera() {
        Vector3 rotation;
        lookAngle = lookAngle + (inputManager.cameraHorizontalInput * cameraLookSpeed);
        pivotAngle = pivotAngle - (inputManager.cameraVerticalInput * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);
        rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;
        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }
    private void HandleCameraCollisions() {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();
        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers)) {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffSet);
        }
        if (Mathf.Abs(targetPosition) < minimumCollisionOffSet) {
            targetPosition = targetPosition - minimumCollisionOffSet;
        }
        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}
