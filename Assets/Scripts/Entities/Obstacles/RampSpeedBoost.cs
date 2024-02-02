using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyUtility.Utility;


public class RampSpeedBoost : Obstacle {

    [SerializeField] float amount = 2.0f;

    public override void Initialize(GameInstance game) {
        base.Initialize(game);
        //Anything specific to this class
    }
    public override void Tick() {
        
        //Anything specific to this!
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player") || !activated)
            return;

        Player player = other.GetComponent<Player>();
        player.GetDaredevilData().ApplyRampBoost(amount);
        Log("Boosted!");
    }
}
