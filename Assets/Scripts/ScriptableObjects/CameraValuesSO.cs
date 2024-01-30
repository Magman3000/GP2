using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CameraValuesSO", menuName = "CameraValues/CameraValuesSO", order = 0)]
public class CameraValuesSO : ScriptableObject
{
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _cameraFollowSpeed;
    [SerializeField] private float _minCameraZOffset = 0f;
    [SerializeField] private float _maxCameraZOffset = 10f;

    public Vector3 GetOffset()
    {
        return _offset;
    }

    public float GetCameraFollowSpeed()
    {
        return _cameraFollowSpeed;
    }

    public float GetMinCameraZOffsetDependingOnSpeed()
    {
        return _minCameraZOffset;
    }

    public float GetMaxCameraZOffsetDependingOnSpeed()
    {
        return _maxCameraZOffset;
    }
}