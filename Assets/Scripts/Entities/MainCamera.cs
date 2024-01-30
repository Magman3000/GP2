using UnityEngine;

public class MainCamera : Entity
{
    public class CameraShakeType
    {
        public float Intensity;
        public float Duration;
        public Vector3 Direction;
    }

    //SerializeFields
    [SerializeField] private CameraStats cameraStats;
    [SerializeField] private Transform _cameraTransform; //Should I do it without hard referencing it?

    //Refs
    private Player playerRef;
    private Daredevil daredevilData;

    //Private
    private CameraShakeType _cameraShakeType;
    private Quaternion _originalRotation;
    private bool _shakeCamera = false;
    private float _shakeDuration = 0f;


    public override void Initialize(GameInstance game)
    {
        if (initialized)
            return;

        gameInstanceRef = game;
        initialized = true;
    }

    public override void Tick()
    {
        if (!initialized)
            return;
        ShakeCamera();
    }

    public override void FixedTick()
    {
        if (!initialized)
            return;

        var offset = CalculateOffset(out var transform1);
        UpdatePositionAndRotation(transform1, offset);
    }

    private void UpdatePositionAndRotation(Transform transform1, Vector3 offset)
    {
        transform.position = Vector3.Lerp(
            transform.position,
            transform1.position + offset,
            cameraStats.GetCameraFollowSpeed() * Time.deltaTime
        );
        transform.LookAt(transform1);
    }

    private Vector3 CalculateOffset(out Transform transform1)
    {
        var cameraValuesYOffSet = cameraStats.GetYOffset();
        var zOffset = Mathf.Lerp(cameraStats.GetMinCameraZOffset(), cameraStats.GetMaxCameraZOffset(),
            daredevilData.GetCurrentSpeedPercentage());
        var offset = new Vector3(0f, cameraValuesYOffSet, zOffset);
        transform1 = playerRef.transform;
        return offset;
    }

    public void TriggerShake(CameraShakeType cameraShakeType)
    {
        _shakeCamera = true;
        _shakeDuration = cameraShakeType.Duration;
        _cameraShakeType = cameraShakeType;
        _originalRotation = _cameraTransform.localRotation;
    }

    private void ShakeCamera()
    {
        if (!_shakeCamera)
            return;

        Quaternion randomDisplacement;
        var direction = _cameraShakeType.Direction;
        var shakeIntensity = _cameraShakeType.Intensity;
        if (direction == Vector3.up || direction == Vector3.down)
        {
            randomDisplacement = Quaternion.Euler(Random.Range(-shakeIntensity, shakeIntensity), 0f, 0f);
        }
        else if (direction == Vector3.left || direction == Vector3.right)
        {
            randomDisplacement = Quaternion.Euler(0f, Random.Range(-shakeIntensity, shakeIntensity), 0f);
        }
        else
        {
            randomDisplacement = Quaternion.Euler(Random.insideUnitSphere * shakeIntensity);
        }

        _cameraTransform.localRotation = _originalRotation * randomDisplacement;

        _shakeDuration -= Time.deltaTime;

        if (!(_shakeDuration <= 0))
            return;

        _shakeCamera = false;
        _cameraTransform.localRotation = _originalRotation;
    }

    public void SetPlayerReference(Player player)
    {
        playerRef = player;
        daredevilData = playerRef.GetDaredevilData();
    }
}