using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	//stats - mutative
	private int maxHp = 100;
	private int healthPoints;
	private Vector2 meleeDamage;
	private Vector2 rangedDamage;
	private int healAmount = 20;
	private float movementSpeed = 1000f;

	//Internals
	private bool isMoving;
	private bool isLoaded = true;
	private bool canHighlight = true;
	private bool isVulnerable = true;
	private float invulnerabilityTimer = 2.0f;
	private Vector3 movement;
	private Vector3 currPos;
	private Vector3 prevPos;
	private float velocity = 0f;
	private float direction;
	private int friendlyTarget = -1;
	private int enemyTarget = -1;
	private GameObject[] friends;
	private GameObject[] enemies;
	private Rigidbody rigid;
	private Animator animator;
	private MeleeAttack melee;
	private RangedAttack ranged;
	private float animationLength;
	[SerializeField]private GameObject sword;
	[SerializeField]private GameObject bow;
	
	//Create all possible States

	public BasicState[] states;
	private Stack<BasicState> playerState;

	public int ID = 1;

	// Use this for initialization (internal)
	void Awake(){
		healthPoints = maxHp;
		meleeDamage = new Vector2(2,4);
		rangedDamage = new Vector2(1,3);

		prevPos = new Vector3(0,0,0);
		currPos = new Vector3(0,0,0);

		//Initialize all possible states
		playerState = new Stack<BasicState> ();
		
		//Call a default state for the player
		if(states != null){
			playerState.Push(states[0]);
			for(int i = 0; i < states.Length; i++){
				states[i].SetPlayerController(this);
			}
		}
	}

	// Use this for initialization (external)
	void Start () {
		rigid = this.GetComponent<Rigidbody>();
		animator = this.GetComponent<Animator>();
		bow.SetActive(false);

		//Initialize melee script
		melee = this.GetComponentInChildren<MeleeAttack>();
		melee.SetAttackDamage(meleeDamage);

		//Initialize ranged script
		ranged = this.GetComponentInChildren<RangedAttack>();
		ranged.SetAttackDamage(rangedDamage);
	}
	
	//Calls the current state's update
	void Update () {
		friends = GameObject.FindGameObjectsWithTag("Targets");
		enemies = GameObject.FindGameObjectsWithTag("Enemy");
		if (healthPoints <= 0){
			Destroy(this.gameObject);
		}
		if (playerState != null){
			playerState.Peek().StateUpdate();
		}
	}

	void FixedUpdate() {
		//direction = transform.rotation.z;
		prevPos = currPos;
		//Call the current state's fixed update
		if (playerState != null){
			playerState.Peek().StateFixedUpdate();
		}
		currPos = transform.position;

		velocity = Vector3.Distance(currPos, prevPos);
		animator.SetInteger("AttackNumber", melee.GetAttackIndex());
		animator.SetFloat("Velocity", velocity);
		rigid.AddForce(GetMovement());

		if( direction <0 )
			direction += 360;
		transform.rotation = Quaternion.Euler(0,0,direction);

		
	}
	
	public void SwitchEnemyTarget(bool up){
		if(enemyTarget >= 0 && enemies.Length > 1) {
			if(up){
				if (enemyTarget < enemies.Length -1)
					enemyTarget += 1;
				else 
					enemyTarget = 0;
			} else {
				if(enemyTarget > 0)
					enemyTarget -= 1;
				else 
					enemyTarget = enemies.Length - 1;
			}
		} else if (enemies.Length < 1)
			enemyTarget = -1;
		else 
			enemyTarget = 0;
		
		Debug.Log ("Player " + ID + " currently targeting Enemy: " + enemies[enemyTarget].gameObject.name);
	}
	public void SwitchFriendlyTarget(bool right){
		if(friendlyTarget >= 0 && friends.Length > 1) {
			if(right){
				if (friendlyTarget < friends.Length -1)
					friendlyTarget += 1;
				else 
					friendlyTarget = 0;
			} else {
				if(friendlyTarget > 0)
					friendlyTarget -= 1;
				else 
					friendlyTarget = friends.Length - 1;
			}
		} else if (friends.Length < 1)
			friendlyTarget = -1;
		else 
			friendlyTarget = 0;

		//HighlightTarget();
		Debug.Log ("Player " + ID + " currently targeting Player " + friendlyTarget.ToString());
	}

	public void HealTarget(){
		if(friendlyTarget >= 0)
			friends[friendlyTarget].GetComponent<PlayerController>().AddHealth(healAmount);
	}


	public void AddHealth(int gainedHp){
		healthPoints += gainedHp;
		if (healthPoints > maxHp)
			healthPoints = maxHp;
		Debug.Log(name + "hp: " + healthPoints);
	}

	public void ReduceHealth(int lostHp) {
		healthPoints -= lostHp;
		animator.SetTrigger("Damaged");
		Debug.Log (name + " - " + lostHp + "hp. " + healthPoints + "hp left!");
	}

	public void ReduceHealthOverTime(int lostHp){
		StartCoroutine(TakeDamageRoutine(invulnerabilityTimer,lostHp));
	}

	IEnumerator TakeDamageRoutine(float duration, int lostHp){
		if(isVulnerable){
			isVulnerable = false;
			animator.SetTrigger("Damaged");
			yield return new WaitForSeconds(duration);
			healthPoints -= lostHp;
			Debug.Log (name + " - " + lostHp + "hp. " + healthPoints + "hp left!");
			isVulnerable = true;
		} else{
			yield return new WaitForSeconds(0.0f);
		}
	}
// Mutators/Accessors
	public Stack<BasicState> GetPlayerState(){return playerState;}
	public bool GetIsMoving(){ return isMoving;}
	public float GetMovementSpeed(){ return movementSpeed;}
	public Vector3 GetMovement(){ return movement; }
	public EnemyController GetCurrentEnemyTarget(){return enemies[enemyTarget].GetComponent<EnemyController>();}
	public PlayerController GetCurrentFriendlyTarget(){return friends[friendlyTarget].GetComponent<PlayerController>();}
	public int EnemyTargetIndex(){return enemyTarget;}
	public int FriendlyTargetIndex(){return friendlyTarget;}
	public bool GetHasLoaded() { return isLoaded;}
	public Rigidbody GetRigidBody() { return rigid;}
	public Animator GetAnimator() { return animator;}
	public BasicState[] GetStates() { return states; }
	public MeleeAttack GetMelee() { return melee;}
	public RangedAttack GetRanged(){ return ranged;}
	public GameObject GetSword() {return sword;}
	public GameObject GetBow() { return bow;}
	
	public void SetHasLoaded(bool load){isLoaded = load;}
	public void SetMovement(Vector3 move){movement = move;}
	public void SetDirection(float dir){direction = dir;}
	public float GetDirection(){ return direction;}
	public void SetIsMoving(bool move){isMoving = move;}
	public void SetCanHightlight(bool target){ canHighlight = target;}

	public void SetPosition(Vector3 newPosition){ transform.position = newPosition; }	//Used primarily when changing scenes
	
//Stack Operations
	public void PushState(BasicState pState){
		
		//push state into top of stack
		if (playerState != null) {
			playerState.Peek().StateExit();
		}
		playerState.Push (pState);
		playerState.Peek ().StateEntered ();
	}
	
	public void PopState(){
		
		//remove state on top of stack
		playerState.Peek ().StateExit ();
		playerState.Pop ();
		playerState.Peek ().StateEntered ();
	}

	public void ChangeState(BasicState pState){
		if (playerState != null) {
			playerState.Peek().StateExit();
			playerState.Pop();
		}
		playerState.Push(pState);
		playerState.Peek().StateEntered();
	}

//	void OnDrawGizmos(){
//		if(enemyTarget >= 0){
//			switch(ID){
//			case 1:
//				UnityEditor.Handles.color = Color.green;
//				break;
//			case 2:
//				UnityEditor.Handles.color = Color.yellow;
//				break;
//			case 3:
//				UnityEditor.Handles.color = Color.blue;
//				break;
//			case 4:
//				UnityEditor.Handles.color = Color.red;
//				break;
//			}
//
//			if (canHighlight){
//				UnityEditor.Handles.DrawSolidDisc(enemies[enemyTarget].transform.position, enemies[enemyTarget].transform.forward,10);
//			}
//		}
//		if(friendlyTarget >= 0){
//			switch(ID){
//			case 1:
//				UnityEditor.Handles.color = Color.green;
//				break;
//			case 2:
//				UnityEditor.Handles.color = Color.yellow;
//				break;
//			case 3:
//				UnityEditor.Handles.color = Color.blue;
//				break;
//			case 4:
//				UnityEditor.Handles.color = Color.red;
//				break;
//			}
//			
//			if (canHighlight){
//				UnityEditor.Handles.DrawSolidDisc(friends[friendlyTarget].transform.position, friends[friendlyTarget].transform.forward,10);
//			}
//		}
//	}
}