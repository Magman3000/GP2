using UnityEngine;

public class Player : NetworkedEntity {
    public enum PlayerIdentity {
        NONE = 0,
        PLAYER_1, //Daredevil
        PLAYER_2 //Coordinator
    }




    private PlayerIdentity assignedPlayerIdentity = PlayerIdentity.NONE;


    
    private float currentSpeed;
    private float timer = 0.0f;
    private bool boosting = false;
    private int boostCharges;

    PlayerOneStats playerOneStats;
    private Rigidbody rigidbody;


    public override void Initialize(GameInstance game) {
        if (initialized)
            return;

        SetupReference();
        boostCharges = playerOneStats.GetBoostCharges();
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
        
        currentSpeed += playerOneStats.GetAccelerationSpeed() * Time.deltaTime;
        if (currentSpeed > playerOneStats.GetMaxSpeed() && !boosting)
        {
            currentSpeed = playerOneStats.GetMaxSpeed();
        }
        if (boosting && currentSpeed > playerOneStats.GetMaxBoostSpeed())
        {
            currentSpeed = playerOneStats.GetMaxBoostSpeed();
        }

    }
    private void DecreaseCurrentSpeed()
    {
        currentSpeed -= playerOneStats.GetDeccelerationSpeed() * Time.deltaTime;
        if (currentSpeed < 0.0f)
        {
            currentSpeed = 0.0f;
        }
    }


    private void TurnRight()
    {
        transform.eulerAngles += new Vector3(0, playerOneStats.GetTurnSpeed(), 0) * Time.deltaTime;
    }
    private void TurnLeft()
    {
        transform.eulerAngles -= new Vector3(0, playerOneStats.GetTurnSpeed(), 0) * Time.deltaTime;
    }


    private void SpeedBoost()
    {
        if (playerOneStats.GetBoostCharges() <= 0 || boosting)
            return;


        boostCharges -= 1;
        boosting = true;
        timer = playerOneStats.GetBoostCharges();

        
    }
    private void BoostTimer()
    {
        if (timer <= 0.0f)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            timer = 0.0f;
            boosting = false;
        }

    }


    public float GetCurrentSpeed() {  return currentSpeed; }
    public float GetCurrentSpeedPercentage() { return currentSpeed / playerOneStats.GetMaxSpeed(); }
}
