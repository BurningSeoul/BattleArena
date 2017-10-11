using UnityEngine;
using System.Collections;

public class WaveAttack : MonoBehaviour {

	public SphereCollider radiusCollider;
	private float innerRadius;
	public GameObject[] possibleTargets;
	private float scale;
	public bool outerAttack = true;	// defualt attack has the inside ring as safe.
	private Vector2 attackRange;
	// Use this for initialization
	void Start () {
		attackRange = new Vector2(1,4);
		radiusCollider = this.GetComponent<SphereCollider>();
		scale = transform.parent.parent.localScale.z;
		radiusCollider.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		innerRadius = radiusCollider.radius*scale/4;
		possibleTargets = GameObject.FindGameObjectsWithTag("Targets");
	}

	void OnTriggerStay(Collider target){

		for(int i = 0; i < possibleTargets.Length; i++){
			if(target == possibleTargets[i].GetComponent<Collider>()){
				PlayerController player = target.GetComponent<PlayerController>();

				if(outerAttack){
					if(Vector3.Distance(transform.position, target.transform.position) > innerRadius)
						player.ReduceHealthOverTime((int)Random.Range(attackRange.x,attackRange.y +1));
				} else {
					if(Vector3.Distance(transform.position, target.transform.position) < innerRadius)
						player.ReduceHealthOverTime((int)Random.Range(attackRange.x,attackRange.y +1));
				}
			}
		}

	}

	public void HitBoxEnable(bool hit){ radiusCollider.enabled = hit;}
}
