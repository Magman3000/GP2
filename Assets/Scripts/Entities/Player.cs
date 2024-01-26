using UnityEngine;

public class Player : NetworkedEntity
{
    public enum PlayerIdentity
    {
        NONE = 0,
        PLAYER_1, //Daredevil
        PLAYER_2 //Coordinator
    }




    private PlayerIdentity assignedPlayerIdentity = PlayerIdentity.NONE;



    public DareDevil _dareDevil = new DareDevil();
    private Coordinator _coordinatior = new Coordinator();

    [SerializeField] public PlayerOneStats playerOneStats;
    public Rigidbody rigidbody;

    public override void Initialize(GameInstance game)
    {
        if (initialized)
            return;


        _dareDevil.Initialize(game, this);
        _coordinatior.Initialize(game, this);
        SetupReference();
        gameInstanceRef = game;
        initialized = true;
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
