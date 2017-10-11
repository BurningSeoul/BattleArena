using UnityEngine;
using System.Collections;

public class Normal : BasicState {
	private Transform camTransform;
	private Camera cam;
	//Enumerator bools (axis workarounds and ability activations)
	private bool canSwitchFriendlyTarget = true;
	private bool canSwitchEnemyTarget = true;
	private bool canHeal = true;
	private bool canMelee = true;
	private bool canRanged = true;
	private bool canMove = true;
	private bool isHealing = false;

	//cooldowns
	private float targetSwitchCooldown = 0.25f;
	private float healCooldown = 3.0f;
	private float meleeCooldown = 0.75f;
	//** has two cooldowns based on different animations
	private float rangedStillCooldown = 0.8f;
	private float rangedMovingCooldown = 2.5f;
	// Use this for initialization
	void Start () {
		stateName = "Normal";
		if(cam==null)
			cam = Camera.main;
		camTransform = cam.transform;
	}
	// Update is called once per frame
	void Update () {}
	

	public override void StateEntered(){ Debug.Log ("Entered: " + stateName);}
	public override void StateUpdate(){

		ClampObjectIntoView(player.transform,1.0f);
	}
	public override void StateFixedUpdate(){
//		if(player.GetHasLoaded()){
			HandleInput();
	}
	public override void HandleInput(){

		//Melee
		if(Input.GetButtonDown("LightMelee_" + player.ID.ToString())){
			StartCoroutine(LightMeleeRoutine(meleeCooldown));
		}
		
		//Ranged
		if(Input.GetButtonDown("LightRanged_" + player.ID.ToString())){
			if(player.GetIsMoving()){
				StartCoroutine(LightRangedRoutine(rangedMovingCooldown));
			} else {
				StartCoroutine(LightRangedRoutine(rangedStillCooldown));
			}
		}
		
		//Healing
		if(Input.GetButtonDown("HealOthers_" + player.ID.ToString())){
			if(player.FriendlyTargetIndex() >= 0)
				StartCoroutine(HealTargetRoutine(healCooldown));
		}

	

		//Movement Controls
		if(canMove){
			player.SetMovement (new Vector3(Input.GetAxis ("Horizontal_" + player.ID.ToString()), Input.GetAxis ("Vertical_" + player.ID.ToString()),0) * player.GetMovementSpeed());
			if(Input.GetAxis ("Horizontal_" + player.ID.ToString()) != 0 || Input.GetAxis ("Vertical_" + player.ID.ToString()) != 0){
				player.SetDirection(Mathf.Atan2(Input.GetAxis ("Vertical_" + player.ID.ToString()), Input.GetAxis ("Horizontal_" + player.ID.ToString()))* Mathf.Rad2Deg);
				player.SetIsMoving(true);
				player.GetAnimator().SetBool ("IsMoving", true);

			} else {
				player.SetIsMoving(false);
				player.GetAnimator().SetBool ("IsMoving", false);
				//Target Lock
			} if(Input.GetButton("TargetLock_" + player.ID.ToString()) && player.EnemyTargetIndex() >= 0){
				Debug.Log ("TargetLock!");
				if(player.GetCurrentEnemyTarget() != null){
				}
			}

		} else {


		}
		//Friendly Targeting
		if(Input.GetAxis("FriendlyTargetSwitch_" + player.ID.ToString()) == 1 && player.FriendlyTargetIndex() > 0){
			StartCoroutine(SwitchFriendlyTargetRoutine(targetSwitchCooldown, true));
		} else if (Input.GetAxisRaw("FriendlyTargetSwitch_" + player.ID.ToString()) == -1){
			StartCoroutine(SwitchFriendlyTargetRoutine(targetSwitchCooldown, false));
		}

		//Enemy Targeting
		if(Input.GetAxis("EnemyTargetSwitch_" + player.ID.ToString()) == 1){
			StartCoroutine(SwitchEnemyTargetRoutine(targetSwitchCooldown, true));
		} else if (Input.GetAxis("EnemyTargetSwitch_" + player.ID.ToString()) == -1){
			StartCoroutine(SwitchEnemyTargetRoutine(targetSwitchCooldown, false));
		}

		if(isHealing){
			Debug.Log(player.GetDirection());
		}
	}
	 
	public override void StateExit(){ 
		Debug.Log ("Exited: " + stateName);
		player.SetMovement(player.GetRigidBody().position);
		player.SetIsMoving(false);
	}

	IEnumerator SwitchFriendlyTargetRoutine(float duration,bool positiveDirection)
	{
		if(canSwitchFriendlyTarget)
		{
			canSwitchFriendlyTarget = false;
			player.SetCanHightlight(true);
			player.SwitchFriendlyTarget(positiveDirection);
			yield return new WaitForSeconds( duration );
			canSwitchFriendlyTarget = true;
			player.SetCanHightlight(false);
		}
		else
		{
			yield return new WaitForSeconds( 0f );
		}
	}

	IEnumerator SwitchEnemyTargetRoutine(float duration,bool positiveDirection)
	{
		if(canSwitchEnemyTarget)
		{
			canSwitchEnemyTarget = false;
			player.SwitchEnemyTarget(positiveDirection);
			player.SetCanHightlight(true);
			yield return new WaitForSeconds( duration );
			canSwitchEnemyTarget = true;
			player.SetCanHightlight(false);
		}
		else
		{
			yield return new WaitForSeconds( 0f );
		}
	}

	IEnumerator HealTargetRoutine(float duration)
	{
		if(canHeal)
		{
			canHeal = false;
			canMelee = false;
			canRanged = false;
			canMove = false;
			isHealing = true;
			player.GetSword().SetActive(false);
			player.GetBow().SetActive(false);
			Debug.Log ("Player " + player.ID.ToString() + ": Heal");
			player.SetDirection(Mathf.Atan2 (player.transform.position.y - player.GetCurrentFriendlyTarget().transform.position.y, player.transform.position.x - player.GetCurrentFriendlyTarget().transform.position.x) / Mathf.PI);
			player.HealTarget();
			player.GetAnimator().SetTrigger("Heal");
			yield return new WaitForSeconds( duration );
			player.GetSword().SetActive(true);
			player.GetBow().SetActive(false);
			canHeal = true;
			canMelee = true;
			canRanged = true;
			canMove = true;
			isHealing = false;
		}
		else
		{
			yield return new WaitForSeconds( 0f );
		}
	}

	IEnumerator LightMeleeRoutine(float duration)
	{
		if(canMelee && player.GetMelee().GetCanAttack())
		{
			canHeal = false;
			canMelee = false;
			canRanged = false;
			Debug.Log ("Player " + player.ID.ToString() + ": Melee attack");
			player.GetMelee().Attack();

			yield return new WaitForSeconds( duration );
			canHeal = true;
			canMelee = true;
			canRanged = true;
		}
		else
		{
			yield return new WaitForSeconds( 0f );
		}
	}

	IEnumerator LightRangedRoutine(float duration)
	{
		float movingRelease = 1.5f;
		float stillRelease = 1.7f;

		if(canRanged)
		{
			canHeal = false;
			canMelee = false;
			canRanged = false;
			player.GetSword().SetActive(false);
			player.GetBow().SetActive(true);
			player.GetAnimator().SetTrigger("Bow");

			if (player.GetIsMoving()){
				yield return new WaitForSeconds(movingRelease);
				player.GetRanged().Shoot();
				yield return new WaitForSeconds(duration - movingRelease);
			} else {
				yield return new WaitForSeconds(stillRelease);
				player.GetRanged().Shoot();
				yield return new WaitForSeconds(duration - stillRelease);
			}
			player.GetSword().SetActive(true);
			player.GetBow ().SetActive(false);
			canHeal = true;
			canMelee = true;
			canRanged = true;

		}
		else
		{
			yield return new WaitForSeconds( 0f );
		}

	}

	IEnumerator ResetRangedRoutine(float duration){
		yield return new WaitForSeconds(duration);
		player.GetSword().SetActive(true);
		player.GetBow ().SetActive(false);
		canHeal = true;
		canMelee = true;
		canRanged = true;
	}
	private void ClampObjectIntoView(Transform t, float clampBorderOffset){
		Vector3 objectPos = t.position;
		Vector3 newPos = objectPos;
		float frustrumPosTopY = cam.ViewportToWorldPoint(new Vector3(0,1,t.position.z-camTransform.position.z)).y;
		float frustrumPosBotY = cam.ViewportToWorldPoint(new Vector3(0,0,t.position.z-camTransform.position.z)).y;
		float frustrumPosLeftX = cam.ViewportToWorldPoint(new Vector3(0,0,t.position.z-camTransform.position.z)).x;
		float frustrumPosRightX =  cam.ViewportToWorldPoint(new Vector3(1,0,t.position.z-camTransform.position.z)).x;

		if(objectPos.y > frustrumPosTopY - clampBorderOffset)
			newPos.y = frustrumPosTopY - clampBorderOffset;
		else if(objectPos.y < frustrumPosBotY+clampBorderOffset)
			newPos.y = frustrumPosBotY +clampBorderOffset;

		if(objectPos.x < frustrumPosLeftX + clampBorderOffset)
			newPos.x = frustrumPosLeftX + clampBorderOffset;
		else if (objectPos.x > frustrumPosRightX - clampBorderOffset)
			newPos.x = frustrumPosRightX - clampBorderOffset;

		t.position = newPos;
	}

	
	public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
	{
		return Mathf.Atan2(
			Vector3.Dot(n, Vector3.Cross(v1, v2)),
			Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
	}
}

