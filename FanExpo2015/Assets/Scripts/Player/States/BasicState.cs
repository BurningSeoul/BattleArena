using UnityEngine;
using System.Collections;

public abstract class BasicState: MonoBehaviour {
	protected string stateName;
	protected PlayerController player;
	protected BossController boss;
	public abstract void StateEntered();
	public abstract void StateFixedUpdate();
	public abstract void StateExit();
	public abstract void StateUpdate();

	public abstract void HandleInput();

	public string GetStateName() { return stateName; }

	public void SetPlayerController(PlayerController playah){player = playah;}
	public void SetBossController(BossController bous){boss = bous;}
}
