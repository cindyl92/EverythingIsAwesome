using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
	public float speed;
	public Camera lowCam;
	public Camera highCam;
	// Use this for initialization
	void Start () {
		lowCam.enabled = true; 
		highCam.enabled = false; 
	
	}
	
	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		
		rigidbody.AddForce (movement * speed * Time.deltaTime);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			lowCam.enabled = !lowCam.enabled;
			highCam.enabled = !highCam.enabled;
		}
		
	}
}
