using Unity.VisualScripting;
using UnityEngine;
using static MyUtility.Utility;

public class Player : NetworkedEntity {
    public enum PlayerIdentity {
        NONE = 0,
        DAREDEVIL, //Daredevil
        COORDINATOR //Coordinator
    }

    [SerializeField] private DaredevilStats daredevilStats;

    private PlayerIdentity assignedPlayerIdentity = PlayerIdentity.NONE;

    private Daredevil daredevilData = new Daredevil();
    private Coordinator coordinatorData = new Coordinator(); //Director

    private CoordinatorHUD coordinatorHUD;
    private DaredevilHUD daredevilHUD;

    public Rigidbody rigidbodyComp;


    public override void Initialize(GameInstance game) {
        if (initialized)
            return;


        SetupReference();

        daredevilData.Initialize(game, this);
        coordinatorData.Initialize(game, this);

        gameInstanceRef = game;
        initialized = true;
    }
    public override void Tick() {
        if (!initialized) {
            Warning("Attempted to tick player before it was initialized!");
            return;
        }

        if (assignedPlayerIdentity == PlayerIdentity.DAREDEVIL) {
            daredevilData.Tick();
            daredevilHUD.Tick();
        }
        else if (assignedPlayerIdentity == PlayerIdentity.COORDINATOR) {
            coordinatorData.Tick();
            coordinatorHUD.Tick();
        }
    }
    public override void FixedTick() {
        if (!initialized) {
            Warning("Attempted to fixed tick player before it was initialized!");
            return;
        }

        if (assignedPlayerIdentity == PlayerIdentity.DAREDEVIL) {
            daredevilData.FixedTick();
            daredevilHUD.FixedTick();
        }
        else if (assignedPlayerIdentity == PlayerIdentity.COORDINATOR) {
            coordinatorData.FixedTick();
            coordinatorHUD.FixedTick();
        }
    }


    public void AssignPlayerIdentity(PlayerIdentity playerIdentity) {
        assignedPlayerIdentity = playerIdentity;
    }
    public void SetDaredevilHUD(DaredevilHUD hud) { daredevilHUD = hud; }
    public void SetCoordinatorHUD(CoordinatorHUD hud) { coordinatorHUD = hud; }

    private void SetupReference() {
        rigidbodyComp = GetComponent<Rigidbody>();
        Validate(rigidbodyComp, "Failed to get reference to Rigidbody component!", ValidationLevel.ERROR, true);
    }


    public DaredevilStats GetDaredevilStats() { return daredevilStats; }
    //Coordinator stats getter
    public Daredevil GetDaredevilData() { return daredevilData; }
    public Coordinator GetCoordinatorData() { return coordinatorData; }
}
