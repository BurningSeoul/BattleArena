using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	protected int healthPoints = 10000;
	protected int maxHP = 10000;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(healthPoints <= 0){
			Destroy(this.gameObject);
		}
	}

	public void ReduceHp(int hp){
		healthPoints -= hp;
		Debug.Log(name + " -" + hp + "hp, " + healthPoints + "hp left!");
	}
}
