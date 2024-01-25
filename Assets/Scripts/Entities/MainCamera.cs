using UnityEngine;

public class MainCamera : Entity {

    private Rigidbody _playerRb;
    private Transform _playerTransform;
    private CameraValuesSO _cameraValuesSo;
    private float _currentSpeed;
    private float _maxSpeed;

    private Player _player;

    public override void Initialize(GameInstance game)
    {
        if (initialized)
            return;

        gameInstanceRef = game;
        initialized = true;
        //_playerTransform = gameInstanceRef.GetPlayer().transform;
        _playerRb = _playerTransform.GetComponent<Rigidbody>();
    }

    public override void Tick()
    {
        if (!initialized)
            return;

        _currentSpeed = _playerRb.velocity.magnitude;
        var speedPercentage = _currentSpeed / _maxSpeed;
        var cameraValuesOffSet = _cameraValuesSo.GetOffset();

        var offset = new Vector3(cameraValuesOffSet.x, cameraValuesOffSet.y,
            -Mathf.Lerp(_cameraValuesSo.GetMinCameraZOffsetDependingOnSpeed(),
                _cameraValuesSo.GetMaxCameraZOffsetDependingOnSpeed(), speedPercentage));

        transform.position = Vector3.Lerp(transform.position,
            _playerTransform.position + offset + _playerTransform.forward,
            _cameraValuesSo.GetCameraFollowSpeed() * Time.deltaTime);
        
        transform.LookAt(_playerTransform);
    }

    public void SetPlayerReference(Player player) {
        _player = player;
    }

    public override void FixedTick()
    {
    }
}