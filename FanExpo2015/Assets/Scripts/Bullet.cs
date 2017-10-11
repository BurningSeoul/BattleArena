using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	private float bulletForce = 2000.0f;
	private Vector2 attackDamage;
	public Rigidbody rig;
	private EnemyController enemy;
	private float destructionTime = 4.0f;

	void Start () {

	}

	void OnEnable(){
		Invoke("Destroy",destructionTime);
		rig.AddForce((transform.forward) * bulletForce);
	}

	void Destroy(){
		gameObject.SetActive(false);
	}

	void OnDisable(){
		rig.velocity = Vector3.zero;
		CancelInvoke();
	}

	void OnTriggerEnter(Collider col){
		if(!col.isTrigger){
			if (this.tag == "Player" && col.tag == "Enemy"){
				enemy = col.GetComponent<EnemyController>();
				enemy.ReduceHp((int)Random.Range(attackDamage.x, attackDamage.y + 1));
				Destroy();
			} else if (col.tag != "Player" || col.tag != "Targets"){
				Destroy();
			} 
		}
	}

	public void SetAttackDamage(Vector2 attackRange){ attackDamage = attackRange;}
}
