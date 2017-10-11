using UnityEngine;
using System.Collections;

public class MeleeAttack : MonoBehaviour {
	
	private Vector2 attackDamage;
	private EnemyController enemy;
	private BoxCollider hitBox;
	private float hitBoxTimer = 0.3f;
	private float comboDefault = 1.0f;
	private float comboTimer = 0;
	private bool canAttack = true;
	private bool startComboTimer = true;
	private int attackIndex = 0;

	// Use this for initialization
	void Start () {
		comboTimer = comboDefault;
		hitBox = gameObject.GetComponent<BoxCollider>();
		hitBox.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

		if(startComboTimer){
			if(comboTimer >= 0){
				comboTimer -= Time.deltaTime;
			} else if (comboTimer <= 0){
				attackIndex = 0;
				startComboTimer = false;
				comboTimer = comboDefault;
			}
		}
	}

	void OnTriggerEnter(Collider col){
		if (col.tag == "Enemy"){
			enemy = col.GetComponent<EnemyController>();
			if(enemy != null){
				enemy.ReduceHp((int)Random.Range(attackDamage.x, attackDamage.y + 1));
				hitBox.enabled = false;
			}
		}
	}

	//disables the collider
	IEnumerator AttackRoutine(float duration){
		if(canAttack){
			if (attackIndex < 3)
				attackIndex++;
			else 
				attackIndex = 1;

			comboTimer = comboDefault;
			startComboTimer = true;
			canAttack = false;
			switch(attackIndex){
			case 1:
				yield return new WaitForSeconds( 0.5f );
				StartCoroutine(HitRoutine(duration));
				break;
			case 2:
				yield return new WaitForSeconds( 0.25f );
				StartCoroutine(HitRoutine(duration));
				break;
			case 3:
				yield return new WaitForSeconds( 0.35f );
				StartCoroutine(HitRoutine(duration));
				break;
			default:
				yield return new WaitForSeconds( 0f );
				break;
			}
		} else {
			yield return new WaitForSeconds( 0f );
		}
	}
	IEnumerator HitRoutine(float duration){
		hitBox.enabled = true;
		canAttack = true;
		yield return new WaitForSeconds(duration);
		hitBox.enabled = false;
		canAttack = true;
	}
	public void Attack(){

		StartCoroutine(AttackRoutine(hitBoxTimer));
	}

	public void SetAttackDamage(Vector2 attackRange){ attackDamage = attackRange;}
	public bool GetCanAttack(){return canAttack;}
	public int GetAttackIndex() { return attackIndex;}
}
