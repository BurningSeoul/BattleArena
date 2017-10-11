using UnityEngine;
using System.Collections;

public class CleaveAttack : MonoBehaviour {
	public GameObject[] possibleTargets;
	public BoxCollider hitBox;
	private Vector2 attackRange;
	private bool canAttack = true;
	private float hitBoxTimer = 1.3f;
	// Use this for initialization
	void Start () {
		hitBox = this.GetComponent<BoxCollider>();
		hitBox.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		possibleTargets = GameObject.FindGameObjectsWithTag("Targets");
	}

	void OnTriggerStay(Collider target){
		for(int i = 0; i < possibleTargets.Length; i++){
			if(target == possibleTargets[i].GetComponent<Collider>()){
				PlayerController player = target.GetComponent<PlayerController>();
				player.ReduceHealthOverTime((int)Random.Range(attackRange.x,attackRange.y +1));
			}
		}
	}

	IEnumerator CleaveRoutine(float duration){
		if(canAttack){
			canAttack = false;
			HitBoxEnable(true);
			yield return new WaitForSeconds(duration);
			HitBoxEnable(false);
			canAttack = true;
		} else {
			yield return new WaitForSeconds(0.0f);
		}
	}

	public void HitBoxEnable(bool hit){ hitBox.enabled = hit;}
	public void Attack(){StartCoroutine(CleaveRoutine(hitBoxTimer));}
	public bool CanAttack() { return canAttack;}
}
