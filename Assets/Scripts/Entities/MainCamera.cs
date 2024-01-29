using UnityEngine;

public class MainCamera : Entity
{
    //SerializeFields
    [SerializeField] private CameraStats cameraStats;
    [SerializeField] private Animator cameraAnimator; //Should I do it without hard referencing it?
    
    private static readonly int shakeCameraTrigger = Animator.StringToHash("ShakeCamera");
    
    //Refs
    private Player playerRef;
    private Daredevil daredevilData;

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
        var zOffset = (cameraStats.GetMaxCameraZOffset() - cameraStats.GetMinCameraZOffset()) *
                      daredevilData.GetCurrentSpeedPercentage();
        var offset = new Vector3(0f, cameraValuesYOffSet, zOffset);
        transform1 = playerRef.transform;
        return offset;
    }

    public void ShakeCamera()
    {
        cameraAnimator.SetTrigger(shakeCameraTrigger);
    }
    
    public void SetPlayerReference(Player player)
    {
        playerRef = player;
        daredevilData = playerRef.GetDaredevilData();
    }
}