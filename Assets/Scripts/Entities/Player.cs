using UnityEngine;

public class Player : NetworkedEntity {
    public enum PlayerIdentity {
        NONE = 0,
        PLAYER_1, //Daredevil
        PLAYER_2 //Coordinator
    }



    [Header("Speed Settings")]
    [Tooltip("Speed of acceleration")]
    [SerializeField] private float accelerationSpeed;
    [Tooltip("Speed of deAcceleration")]
    [SerializeField] private float deccelerationSpeed;
    [Tooltip("Maximum amount of speed attainable")]
    [SerializeField] private float maxSpeed;

    [Tooltip("How much you turn left and right each turning action")]
    [SerializeField] private float turnSpeed;

    [Header("Speed Boost Settings")]
    [Tooltip("The multiplied increase of speed during the boost")]
    [SerializeField] private float boostMultiplier;
    [Tooltip("The amount of boost charges")]
    [SerializeField] private float boostCharges;
    [Tooltip("The duration of the boost")]
    [SerializeField] private float boostDuration;

    private PlayerIdentity assignedPlayerIdentity = PlayerIdentity.NONE;


    private float currentSpeed;
    private float timer = 0.0f;
    private bool boosting = false;


    private Rigidbody rigidbody;


    public override void Initialize(GameInstance game) {
        if (initialized)
            return;

        SetupReference();
        gameInstanceRef = game;
        initialized = true;
    }
    public override void Tick() {



    }
    public override void FixedTick() {

    }

    public void AssignPlayerIdentity(PlayerIdentity playerIdentity)
    {
        assignedPlayerIdentity = playerIdentity;
    }

    private void SetupReference() {
        rigidbody = GetComponent<Rigidbody>();
    }


    private void UpdateVelocity()
    {
        rigidbody.velocity = transform.forward * currentSpeed;
    }

    private void IncreaseCurrentSpeed()
    {
        currentSpeed += accelerationSpeed * Time.deltaTime;
        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
    }
    private void DecreaseCurrentSpeed()
    {
        currentSpeed -= deccelerationSpeed * Time.deltaTime;
        if (currentSpeed < 0.0f)
        {
            currentSpeed = 0.0f;
        }
    }


    private void TurnRight()
    {
        transform.eulerAngles += new Vector3(0, turnSpeed, 0) * Time.deltaTime;
    }
    private void TurnLeft()
    {
        transform.eulerAngles += new Vector3(0, -turnSpeed, 0) * Time.deltaTime;
    }


    private void SpeedBoost()
    {
        if (boostCharges <= 0.0f || boosting)
            return;


        boostCharges -= 1;
        boosting = true;
        timer = boostDuration;

        rigidbody.velocity = (transform.forward * currentSpeed) * boostMultiplier;
    }
    private void BoostTimer()
    {
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                timer = 0.0f;
                boosting = false;
            }
        }

    }


    public float GetCurrentSpeed() {  return currentSpeed; }
    public float GetCurrentSpeedPercentage() { return currentSpeed / maxSpeed; }
}
