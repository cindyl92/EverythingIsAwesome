using UnityEngine;
using System.Collections;

public class BuildingHeight : MonoBehaviour {
	public GameObject plane;
	public GameObject mainCamera;

<<<<<<< Updated upstream
	//Mock Objects
=======
	//string[,] str = new string[,] {{ClassA, 120, CouplingClassB, 123, CoupClassC, 342, CoupClassD, 324}, ...  

>>>>>>> Stashed changes
	int numBuildings = 4;
	float[] bases = new float[] {0, 1, 2, 0};
	float[] heights = new float[] {10f, 5f, 4f, 12f};
	float[,] positions = new float[,] {{0,0}, {3,1}, {5,3}, {8,2}};
	// http://answers.unity3d.com/questions/209573/how-to-change-material-color-of-an-object.html
	Color[] colours = new Color[]{Color.green, Color.blue, Color.red, Color.yellow};
	
	void Start () {
		float planeX = 0;
		float planeY = 0;
		float planeZ = 0;

		// Instantiate example from : http://docs.unity3d.com/Manual/InstantiatingPrefabs.html
		for (int x = 0; x < numBuildings; x++) {
			GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);

			building.transform.position = new Vector3(positions[x,0], heights[x]/2 + 0.5f, positions[x,1]);
			building.transform.localScale += new Vector3 (bases[x], heights[x], bases[x]);
			building.renderer.material.color = colours[x];

			if (planeX < positions[x,0]){
				planeX = positions[x,0];
			}
			if (planeY < heights[x]){
				planeY = heights[x];
			}
			if (planeZ < positions[x,1]){
				planeZ = positions[x,1];
			}
		}

		// resize and replace plane to cover all the buildings
		plane.transform.localScale = new Vector3(planeX/4, 0, planeZ/4);
		plane.transform.position = new Vector3(planeX/2, 0, planeZ/2);

		// place main camera to show all the buildings
		mainCamera.transform.position = new Vector3 (planeX / 2, planeY / 2, -14);
	}

	void Update () {	
		//capsule.transform.position = Vector3.up * Time.time;
	}
}
