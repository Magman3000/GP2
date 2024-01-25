using UnityEngine;

public class MainCamera : Entity {

    [SerializeField] private CameraValuesSO _cameraValuesSo;
    private Player _player;

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

        var cameraValuesOffSet = _cameraValuesSo.GetOffset();

        var offset = new Vector3(cameraValuesOffSet.x, cameraValuesOffSet.y,
            -Mathf.Lerp(_cameraValuesSo.GetMinCameraZOffsetDependingOnSpeed(),
                _cameraValuesSo.GetMaxCameraZOffsetDependingOnSpeed(), _player.GetCurrentSpeedPercentage()));

        transform.position = Vector3.Lerp(transform.position,
            _player.transform.position + offset + _player.transform.forward,
            _cameraValuesSo.GetCameraFollowSpeed() * Time.deltaTime);
        
        transform.LookAt(_player.transform);
    }

    public void SetPlayerReference(Player player) {
        _player = player;
    }

    public override void FixedTick()
    {
    }
}