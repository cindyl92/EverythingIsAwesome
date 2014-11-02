using UnityEngine;
using System.Collections;

public class BuildingHeight : MonoBehaviour {
	public GameObject plane;
	public GameObject mainCamera;

	// input format
	// string[,] javaClasses = new string[,] {{ClassA, 120, CouplingClassB, 123}, {ClassC, 342, CouplingClassD, 324}, ... }
	//  {Class, # Lines of Code, Coupling Class, # of couplings}
	
	int numBuildings;
	float[] bases;
	float[] heights;
	float[,] positions;
	Color[] colours;

	void Start () {
		float planeX = 0;
		float planeY = 0;
		float planeZ = 0;

		// arrangeBuildings(javaClasses);

		// Mock Objects
		// for placing and scaling the buildings
		numBuildings = 4;
		bases = new float[] {0, 1, 2, 0};
		heights = new float[] {10f, 5f, 4f, 12f};
		positions = new float[,] {{0,0}, {3,1}, {5,3}, {8,2}};

		colours = new Color[]{Color.green, Color.blue, Color.red, Color.yellow};

		// Instantiate example from : http://docs.unity3d.com/Manual/InstantiatingPrefabs.html
		// instantiate building objects and apply building position, scale (height and bases), and colour
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
	
	void arrangeBuildings (string[,] javaClasses) {
		//  numBuildings = javaClasses.getLength(0);

		// for (int x = 0; x<numBuildings; x++){
		//  sets:
		//   - bases
		//   - heights
		//   - positions
		// }
	}

	void Update () {}
}
