using Unity.VisualScripting;
using UnityEngine;
using static MyUtility.Utility;

public class Player : NetworkedEntity
{
    public enum PlayerIdentity
    {
        NONE = 0,
        DAREDEVIL = 1, //Daredevil
        COORDINATOR = 2 //Coordinator
    }

    [SerializeField] private DaredevilStats daredevilStats;
    [SerializeField] private CoordinatorStats coordinatorStats;

    private PlayerIdentity assignedPlayerIdentity = PlayerIdentity.NONE;

    private Daredevil daredevilData = new Daredevil();
    private Coordinator coordinatorData = new Coordinator(); //Director

    private CoordinatorHUD coordinatorHUD;
    private DaredevilHUD daredevilHUD;

    private Rigidbody rigidbodyComp;

    private bool speedBoostBool = false;


    public override void Initialize(GameInstance game)
    {
        if (initialized)
            return;


        SetupReference();

        daredevilData.Initialize(game, this);
        coordinatorData.Initialize(game, this);

        gameInstanceRef = game;
        initialized = true;
    }
    public override void Tick()
    {
        if (!initialized)
        {
            Warning("Attempted to tick player before it was initialized!");
            return;
        }

        if (assignedPlayerIdentity == PlayerIdentity.DAREDEVIL)
        {
            daredevilData.Tick();
            daredevilHUD.Tick();
        }
        else if (assignedPlayerIdentity == PlayerIdentity.COORDINATOR)
        {
            coordinatorData.Tick();
            coordinatorHUD.Tick();
        }
    }
    public override void FixedTick()
    {
        if (!initialized)
        {
            Warning("Attempted to fixed tick player before it was initialized!");
            return;
        }

        if (assignedPlayerIdentity == PlayerIdentity.DAREDEVIL)
        {
            daredevilData.FixedTick();
            daredevilHUD.FixedTick();
        }
        else if (assignedPlayerIdentity == PlayerIdentity.COORDINATOR)
        {
            coordinatorData.FixedTick();
            coordinatorHUD.FixedTick();
        }
    }


    public void AssignPlayerIdentity(PlayerIdentity playerIdentity)
    {
        assignedPlayerIdentity = playerIdentity;
    }
    public void SetDaredevilHUD(DaredevilHUD hud) { daredevilHUD = hud; }
    public void SetCoordinatorHUD(CoordinatorHUD hud) { coordinatorHUD = hud; }

    private void SetupReference()
    {
        rigidbodyComp = GetComponent<Rigidbody>();
        Validate(rigidbodyComp, "Failed to get reference to Rigidbody component!", ValidationLevel.ERROR, true);
    }



    public DaredevilStats GetDaredevilStats() { return daredevilStats; }
    public CoordinatorStats GetCoordinatorStats() { return coordinatorStats; }
    public Daredevil GetDaredevilData() { return daredevilData; }
    public Coordinator GetCoordinatorData() { return coordinatorData; }

    public Rigidbody GetRigidbody() { return rigidbodyComp; }


    public bool GetBoostCheck() { return speedBoostBool; }
    public void SetBoostCheck(bool boostCheck) {  speedBoostBool = boostCheck; }

}
