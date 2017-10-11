using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossController : EnemyController {
	private float masterTimer = 600.0f;
	public float movementSpeed = 5.0f;
	private int currentPhase = 0;
	private int prevPhase = 0;
	private Transform trans;


	private Vector3 movement;
	private Quaternion  newRotation;
	private bool phase1 = false;
	private bool phase2 = false;
	private bool phase3 = false;
	private bool enraged = false;
	public BasicState[] states;
	public Stack<BasicState> bossState;

	public GameObject[] players;

	[SerializeField] private BoxCollider chargeDamage;
	[SerializeField] private WaveAttack waveAttack;
	[SerializeField] private RadialCharge radial;
	[SerializeField] private CleaveAttack cleave;
	private Rigidbody rigid; 

	void Awake(){
		bossState = new Stack<BasicState> ();

		//Call a default state for the Boss
		if(states != null){
			bossState.Push(states[0]);
			for(int i = 0; i < states.Length; i++){
				states[i].SetBossController(this);
			}
		}
	}
	
	void Start () {
		trans = this.GetComponent<Transform>();
		rigid = this.GetComponent<Rigidbody>();
		chargeDamage.enabled = false;
	}

	void Update () {
		//masterTimer -= Time.deltaTime;

		players = GameObject.FindGameObjectsWithTag("Targets");
		if(masterTimer < 0.0f)
			Debug.Log ("Master Timer < 0f");
			enraged = true;
		if(prevPhase != currentPhase)
			PhaseChange();

		//Phase shifts:
		if(GetBossHealthPercentage() < 0.3f && !phase3){
			Debug.Log ("Phase 4: " +GetBossHealthPercentage());
			phase3 = true;
			ChangeState(states[3]);
		}else if(GetBossHealthPercentage() < 0.5f && !phase2){
			Debug.Log ("Phase 3: " +GetBossHealthPercentage());
			phase2 = true;
			ChangeState(states[2]);
		}else if(GetBossHealthPercentage() < 0.7f && !phase1){
			Debug.Log ("Phase 2: " +GetBossHealthPercentage());
			phase3 = true;
			ChangeState(states[1]);
		}

		if(bossState != null){
			bossState.Peek ().StateUpdate();
		}
	}

	void FixedUpdate(){

		if (bossState != null)
			bossState.Peek().StateFixedUpdate();

		transform.position = Vector3.MoveTowards(transform.position, GetBossMovementVector(), movementSpeed);

		newRotation.x = 0.0f;
		newRotation.y = 0.0f;
		newRotation *= Quaternion.Euler(0, 0, 90);
		transform.rotation = Quaternion.Slerp(GetTransform().rotation, newRotation, Time.deltaTime * GetMovementSpeed());
	}

	private void PhaseChange(){
		prevPhase = currentPhase;
	}
//Mutators/Accessors
	public int GetBossHealth(){return healthPoints;}
	public int GetBossMaxHP() { return maxHP;}
	public float GetMovementSpeed(){return movementSpeed;}
	public float GetBossHealthPercentage(){return ((float)healthPoints/(float)maxHP);}
	public Vector3 GetBossMovementVector(){ return movement;}
	public GameObject[] GetPlayers(){return players;}
	public void SetBossMovementVector(Vector3 move){movement = move;}
	public Quaternion GetDirection(){ return newRotation;}
	public void SetDirection(Quaternion dir){ newRotation = dir;}

	public RadialCharge GetRadialAttack() {return radial;}
	public WaveAttack GetWaveAttack() {return waveAttack;}
	public CleaveAttack GetCleaveAttack(){return cleave;}
	public Transform GetTransform(){return trans;}
	public void ToggleChargeDamage(bool tog){chargeDamage.enabled = tog;}
	public void SetRandomDirection(){
		float dir = Mathf.Atan2(Random.value,Random.value) * 180 / Mathf.PI;
		Vector3 euler = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,dir);
		transform.eulerAngles = euler;
		}
//Stack Operations
	//push state into top of stack
	public void PushState(BasicState pState){
		if (bossState != null)
			bossState.Peek().StateExit();
		bossState.Push(pState);
		bossState.Peek().StateEntered();
	}

	//remove state on top of stack
	public void PopState(){
		bossState.Peek ().StateExit ();
		bossState.Pop ();
		bossState.Peek ().StateEntered ();
	}

	//Condenses both Push/Pop State functions for rare cases where you use both
	public void ChangeState(BasicState pState){
		if (bossState != null) {
			bossState.Peek().StateExit();
			bossState.Pop();
		}
		bossState.Push(pState);
		bossState.Peek().StateEntered();
	}
}
