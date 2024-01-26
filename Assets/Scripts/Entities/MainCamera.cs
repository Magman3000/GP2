using UnityEngine;

public class MainCamera : Entity
{
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

        var zOffset = (_cameraValuesSo.GetMaxCameraZOffset() - _cameraValuesSo.GetMinCameraZOffset()) *
                      _player.GetCurrentSpeedPercentage();
        var offset = new Vector3(0f, cameraValuesOffSet.y, zOffset);

        var transform1 = _player.transform;
        transform.position = Vector3.Lerp(transform.position, transform1.position + offset,
            _cameraValuesSo.GetCameraFollowSpeed() * Time.deltaTime);

        transform.LookAt(transform1);
    }

    public void SetPlayerReference(Player player)
    {
        _player = player;
    }

    public override void FixedTick()
    {
    }
}