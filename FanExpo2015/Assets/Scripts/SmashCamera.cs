using UnityEngine;
using System.Collections;

public class SmashCamera : MonoBehaviour {
	
	//////////////////////////////////
	/// Non-modifiable variables
	//////////////////////////////////
	public GameObject[] allTargets;						//----All targets must be tagged at "Targets" in the tag manager to be considered a target
	
	private float[] listOfXPos;							//----Contains a list of "allTargets" X positions
	private float[] listOfYPos;							//----Contains a list of "allTargets" Y positions
	private float maxX;									//----Max X Value
	private float maxY;									//----Max Y Value
	private float minX;									//----Min X Value
	private float minY;									//----Min Y Value
	private float distanceBetweenMinMax;				//----calculated distance between min and max values
	private Vector2 maxPosition;						//----Calulated value of max possible position
	private Vector2 minPosition;						//----Calulated value of min possible position
	private Vector2 centerPoint;						//----The point the camera will be looking at
	private Vector3 zoomFactor;							//----Vector in which the zoom factor will be given to the camera
	private bool cameraToFar;							//----Bool if the camera is to far
	private Vector3 cameraTarget;

	
	
	//////////////////////////////////////////////////////////////////////////////////////////////////
	/// modifiable variables (make public if u wanted to change values in inspector)
	//////////////////////////////////////////////////////////////////////////////////////////////////
	/// Default Values
	//////////////////////////////////////////////////////////////////////////////////////////////////
	public float zoomScaleFactor = 1.0f;				//----sets how fast the scale will zoom in/out
	public float padX = 1.0f;							//----set the padding offset for X
	public float padY = 1.0f;							//----set the padding offset for Y
	public float camMoveSpeed = 4.0f;					//set how fast the camera will move to track targets
	public float maxDistance = 15.0f;					//set how far the max distance will be before the camera will stop targeting a target when it leaves
	public bool limitCamRange = false;					//bool that turns on/off if you want the camera to stop moving at max distance
	public float zoomMin = 10.0f;						//Sets how close the camera can look at the target before it is stopped
	public float zoomMax = 25.0f;						//Sets how far the camera can look at the target before it is stopped	//CanTargets move out of frame?

	//----Use this for initialization----/
	void Start () {
		
		//////////////////////////////////
		//	Default Values
		//////////////////////////////////
		zoomScaleFactor = 1.0f;
		padX = 1.0f;
		padY = 1.0f;
		camMoveSpeed = 4.0f;
		maxDistance = 35.0f;
		zoomMin = 10.0f;
		zoomMax = 25.0f;
	}
	
	//----Update is called once per frame----/
	void Update () {
		//----if the the distance between max/min objects is greater than the max distance allowed, stop moving the camera.----/
		if (distanceBetweenMinMax >= maxDistance && limitCamRange) {
			cameraToFar = true;
		} else {
			cameraToFar = false;
		}
		UpdateCam ();
	}
	
	private void UpdateCam(){
		CalculateCamTarget ();
		CameraLookAtPoint ();
		CalculateZoom ();
	}
	
	private void CalculateCamTarget(){
		//----check number of targets to follow----/
		//----This script will search the Hierachy for all objects marked Target----//
		//----to remove and Object call it's destroy function or untag it and it will be automatically removed from the list----//
		allTargets = GameObject.FindGameObjectsWithTag ("Targets");
		listOfXPos = new float [allTargets.Length];
		listOfYPos = new float [allTargets.Length];
		
		//----add positions of each target to follow into the int arrays----/
		for (int i = 0; i < allTargets.Length; i++) {
			listOfXPos[i] = allTargets[i].transform.position.x;
			listOfYPos[i] = allTargets[i].transform.position.y;

		}
		
		//----find the max/mins of positions----/
		maxX = (Mathf.Max(listOfXPos) + padX);
		maxY = (Mathf.Max(listOfYPos) + padY);
		minX = (Mathf.Min(listOfXPos) - padX);
		minY = (Mathf.Min(listOfYPos) - padY);
		
		//----turn values into vector2----/
		maxPosition = new Vector2(maxX, maxY);
		minPosition = new Vector2(minX, minY);
		
		//----find the distance between the max/min and divide to find center point---/
		distanceBetweenMinMax = (Vector2.Distance (maxPosition, minPosition)) / 2;
		
		centerPoint = (maxPosition + minPosition) / 2;
	}
	
	private void CameraLookAtPoint(){
		//smoothly lerp the camera's look.----/
		//if the distance is to far then dont move the camera. (player flys off screen)----/
		if(!cameraToFar){
			float posX;
			float posY;
			posX = centerPoint.x;
			posY = centerPoint.y;
			
			cameraTarget = new Vector3 (posX, posY, transform.position.z);
			transform.position = Vector3.Lerp (transform.position, cameraTarget, Time.deltaTime * camMoveSpeed);
		}
	}
	
	private void CalculateZoom(){
		//----create temp vector3s----/
		Vector3 centerPoint3 = new Vector3 (distanceBetweenMinMax, distanceBetweenMinMax, 0);
		Vector3 scaling3 = Vector3.Normalize(Vector3.up);
		
		//----cross product both vector to find orthographic size----/
		zoomFactor = Vector3.Cross (centerPoint3 , scaling3);
		zoomFactor.z = Mathf.Clamp(zoomFactor.z, zoomMin, zoomMax);
		//----if the distance is to far then dont zoom the camera. (player flys off screen)----/
		if (!cameraToFar) {
			//----set camera orthographicSize to scale factor---/
			Camera.main.orthographicSize = (zoomFactor.z * zoomScaleFactor);
		} else {
			//do nothing
		}
	}
	
	
	////////////////////////////////////////
	/// Getters and Setters
	////////////////////////////////////////
	public void SetZoomScaleFactor(float zoomScale){
		zoomScaleFactor = zoomScale;
	}
	public float GetZoomScaleFactor(){
		return zoomScaleFactor;
	}
	public void SetPadX(float pad){
		padX = pad;
	}
	public float GetPadX(){
		return padX;
	}
	public void SetPadY(float pad){
		padY = pad;
	}
	public float GetPadY(){
		return padY;
	}
	public void SetCamMoveSpeed(float speed){
		camMoveSpeed = speed;
	}
	public float GetCamMoveSpeed(){
		return camMoveSpeed;
	}
	public void SetMaxDistance(float distance){
		maxDistance = distance;
	}
	public float GetMaxDistance(){
		return maxDistance;
	}
	public void SetLimitCamRange(bool limit){
		limitCamRange = limit;
	} 
	public bool GetLimitCamRange(){
		return limitCamRange;
	}
}
