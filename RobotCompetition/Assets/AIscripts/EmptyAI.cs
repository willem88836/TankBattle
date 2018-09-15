using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyAI : RobotControl {

    //Only change color to white
    //Mainly for testing purposes of your AI against this AI
	protected override void Start()
    {
        base.Start();
        //Change the complete color to white
        ChangeColor(Color.white);
    }
}
