using Framework.Core;

public class Debug_03 : TankController, IDoNotLoad
{
    //Only change color to white
    //Mainly for testing purposes of your AI against this AI

	// Cannot call Awake

	void Start()
    {
		// Called once when the tank is spawned
    }

	void Update()
	{
		// Called every frame after the tank is spawned
	}

	protected override void OnWallCollision()
	{
		// Called when this tank collides with a wall
	}

	protected override void OnTankCollision()
	{
		// Called when this tank collides with another tank
	}
}
