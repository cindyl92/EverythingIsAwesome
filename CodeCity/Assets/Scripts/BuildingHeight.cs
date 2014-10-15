using UnityEngine;
using System.Collections;

public class BuildingHeight : MonoBehaviour {
	public GameObject building01;
	public GameObject building02;
	//public GameObject capsule;
	
	void Start () {
		// Used example from : http://docs.unity3d.com/ScriptReference/Transform-localScale.html
		float height01 = 2f;
		float height02 = 5f;

		// increase the height of the buildings
		building01.transform.localScale += new Vector3 (0, height01, 0);
		building02.transform.localScale += new Vector3 (0, height02, 0);

		// reposition the buildings to place them on top of the plane
		building01.transform.position += new Vector3 (-3, height01/2f - 0.5f, 0);
		building02.transform.position += new Vector3 (0, height02/2f - 0.5f, 2f);

		// duplicate objects
		//GameObject clone;
		//clone = Instantiate (building01, transform.position, transform.rotation) as GameObject;
	}

	void Update () {	
		//capsule.transform.position = Vector3.up * Time.time;
	}
}
