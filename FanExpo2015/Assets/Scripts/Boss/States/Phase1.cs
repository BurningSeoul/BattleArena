using UnityEngine;
using System.Collections;

public class Phase1 : BasicState {
	/*	Move Set for Phase 1:
	 * - Radial Charge
	 * - Center in and out AOE
	 * - Cleave x3
	 */
	
	[SerializeField]
	private Transform centre;
	private int actionSequence = 0;
	private int maxActionSequence = 2;
	private int randomTarget = 0;
	private int	passesPerSequence = 3;
	private int cleavePasses = 0;

	private bool firstPass = true;
	private bool startedActionPhase = false;
	private bool canActionSequence = true;
	private Vector3 start;
	private Vector3 destination;

	private float waveAttackBuffer = 3.0f;
	private float waveAttackLength = 5.0f;
	private float cleaveAttackLength = 1.0f;
	private float cleaveAttackBuffer = 2.0f;
	private float actionSequenceBuffer = 4.0f;

	private bool canAttack = true;
	private GameObject[] targets;
	
	void Awake () {
		stateName = "Phase 1";
	}
	
	public override void StateEntered(){ Debug.Log ("Entered: " + stateName);}
	public override void StateUpdate(){ 
		targets = boss.GetPlayers();
	}
	public override void StateFixedUpdate(){
		boss.SetBossMovementVector(boss.transform.position);
		boss.SetDirection(boss.GetTransform().rotation);
		if (actionSequence > maxActionSequence)
			actionSequence = 0;

		if(canActionSequence){
			switch(actionSequence){
			case 0:
				RadialCharge();
				break;
			case 1:
				CenterWaveAttack();
				break;
			case 2:
				Cleave();
				break;
			}


		}
	}
	public override void StateExit(){}
	public override void HandleInput(){}

	//Attack 1:
	private void RadialCharge(){
		if(firstPass){
			Debug.Log ("RadialCharge Starting!");
			start = boss.GetRadialAttack().RandomizeWaypoint();
			destination = boss.GetRadialAttack().GetOppositeWaypoint();
			firstPass = false;
		}

		if(!startedActionPhase){
			boss.SetBossMovementVector(Vector3.Lerp(boss.GetBossMovementVector(),start ,Time.deltaTime * 2));
			boss.SetDirection(Quaternion.LookRotation(boss.GetTransform().position - start, Vector3.forward));
			if(Vector3.Distance(boss.transform.position,start) < 5.0f){
				startedActionPhase = true;
				boss.ToggleChargeDamage(true);
			}
		} else {
			boss.SetBossMovementVector(Vector3.Lerp(boss.transform.position,destination,Time.deltaTime));
			boss.SetDirection(Quaternion.LookRotation(boss.GetTransform().position - destination,Vector3.forward));
		}

		if(Vector3.Distance(boss.GetBossMovementVector(), destination) < 5.0f){
			boss.ToggleChargeDamage(false);
			NextSequence();
		}
	}

	//Attack 2:
	private void CenterWaveAttack(){
		if(firstPass){
			Debug.Log ("Centre Wave Attack Starting!");
			start = centre.position;
			destination = boss.GetRadialAttack().GetWaypoint(6);
			firstPass = false;
		}

		if (!startedActionPhase){
			boss.SetBossMovementVector(Vector3.Lerp (boss.GetBossMovementVector (),start,Time.deltaTime));
			boss.SetDirection(Quaternion.LookRotation(boss.GetTransform().position - boss.GetBossMovementVector(),Vector3.forward));
			if(Vector3.Distance(boss.transform.position,start) < 3.0f){
				startedActionPhase = true;
			}
		} else {
			boss.SetDirection(Quaternion.LookRotation(boss.GetTransform().position - destination, Vector3.forward));
			StartCoroutine(CentreWaveRoutine(5.0f,0));
		}

	}

	//Attack 3:
	private void Cleave(){

		if(firstPass){
			start = centre.position;
			firstPass = false;
			randomTarget = Random.Range(0,targets.Length);
		}

		if(!startedActionPhase){
			boss.SetBossMovementVector(Vector3.Lerp(boss.GetBossMovementVector(),start ,Time.deltaTime));
			boss.SetDirection(Quaternion.LookRotation(boss.GetTransform().position - start,Vector3.forward));
			if(Vector3.Distance(boss.transform.position,start) < 5.0f){
				startedActionPhase = true;
			}
		}else{
			boss.SetDirection(Quaternion.LookRotation(boss.GetTransform().position - targets[randomTarget].transform.position,Vector3.forward));
			StartCoroutine(CleaveRoutine(cleaveAttackLength));
		}

	}

	private void NextSequence(){
		startedActionPhase = false;
		firstPass = true;
		actionSequence++;
		cleavePasses = 0;

	}

	IEnumerator CentreWaveRoutine(float duration, int phase)
	{
		if(canAttack)
		{
			canAttack = false;
			yield return new WaitForSeconds( duration );
			switch(phase){
			case 0:
				canAttack = true;
				boss.SetDirection(Quaternion.LookRotation(boss.GetTransform().position - start,Vector3.forward));
				boss.GetWaveAttack().HitBoxEnable(true);
				StartCoroutine(CentreWaveRoutine(waveAttackLength,1));
				break;
			case 1:
				boss.GetWaveAttack().HitBoxEnable(false);
				NextSequence();
				canAttack = true;
				break;       
			}


		}
		else
		{
			yield return new WaitForSeconds( 0f );
		}
	}

	IEnumerator CleaveRoutine(float duration){
		if(canAttack){
			if(boss.GetCleaveAttack().CanAttack()){
				canAttack = false;
				cleavePasses++;
				boss.GetCleaveAttack().Attack();
				yield return new WaitForSeconds(duration);
				canAttack = true;
				if(cleavePasses >= passesPerSequence){
					NextSequence();
				}
			} else 
			yield return new WaitForSeconds(0f);
		} else {
			yield return new WaitForSeconds(0f);
		}
	}

	IEnumerator WaitRoutine(float duration){
		if(canActionSequence){
			canActionSequence = false;
			yield return new WaitForSeconds(duration);
			canActionSequence = true;
		} else {
			yield return new WaitForSeconds(0.0f);
		}
	}
}
