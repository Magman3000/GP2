using UnityEngine;

public class Player : NetworkedEntity
{
    public enum PlayerIdentity
    {
        NONE = 0,
        PLAYER_1, //Daredevil
        PLAYER_2 //Coordinator
    }

    [SerializeField]private CoordinatorHUD _coordinatorHUD;
    [SerializeField]private DaredevilHUD _daredevilHUD;


    private PlayerIdentity assignedPlayerIdentity = PlayerIdentity.NONE;



    public DareDevil _dareDevil = new DareDevil();
    private Coordinator _coordinator = new Coordinator();

    [SerializeField] public PlayerOneStats playerOneStats;
    public Rigidbody rigidbody;

    public override void Initialize(GameInstance game)
    {
        if (initialized)
            return;


        _dareDevil.Initialize(game, this);
        _coordinator.Initialize(game, this);
        SetupReference();
        gameInstanceRef = game;
        initialized = true;
        
        _daredevilHUD.Initialize(game);
        _daredevilHUD.SetPlayerReference(this);
        
        _coordinatorHUD.Initialize(game);
        _coordinatorHUD.SetPlayerReference(this);
    }
    public override void Tick()
    {



    }
    public override void FixedTick()
    {

    }

    public void AssignPlayerIdentity(PlayerIdentity playerIdentity)
    {
        assignedPlayerIdentity = playerIdentity;

    }

    private void SetupReference()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    

}
