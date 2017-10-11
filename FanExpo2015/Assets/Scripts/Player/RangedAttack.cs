using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RangedAttack : MonoBehaviour {
	private Vector2 attackDamage;
	void Start () {
	
	}

	public void Shoot(){
		GameObject obj = ObjectPooler.current.GetPooledObject();

		if(obj == null) return;

		obj.transform.position = transform.position;
		obj.transform.rotation = transform.rotation; 

		obj.SetActive(true);
		obj.tag = "Player";
		obj.GetComponent<Bullet>().SetAttackDamage(attackDamage);


	}

	public void SetAttackDamage(Vector2 attackRange){ attackDamage = attackRange;}
}
