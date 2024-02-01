using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CameraValuesSO", menuName = "CameraValues/CameraValuesSO", order = 0)]
public class CameraValuesSO : ScriptableObject
{
    [SerializeField] private float _yOffset;
    [SerializeField] private float _cameraFollowSpeed;
    [SerializeField] private float _minCameraZOffset = 0f;
    [SerializeField] private float _maxCameraZOffset = 10f;

    public float GetYOffset()
    {
        return _yOffset;
    }

    public float GetCameraFollowSpeed()
    {
        return _cameraFollowSpeed;
    }

    public float GetMinCameraZOffset()
    {
        return _minCameraZOffset;
    }

    public float GetMaxCameraZOffset()
    {
        return _maxCameraZOffset;
    }
}