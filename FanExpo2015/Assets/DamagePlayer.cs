using UnityEngine;
using System.Collections;

public class DamagePlayer : MonoBehaviour {

	private Vector2 damageRange;
	private PlayerController player;
	// Use this for initialization
	void Start () {
		damageRange = new Vector2(2,5);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col){
		if(col.tag == "Targets"){
			player = col.GetComponent<PlayerController>();
			player.ReduceHealth((int)Random.Range(damageRange.x,damageRange.y + 1));
		}
	}
}
