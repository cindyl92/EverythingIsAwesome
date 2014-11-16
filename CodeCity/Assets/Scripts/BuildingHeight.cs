using UnityEngine;
using System.Collections;
//using Math;

namespace  AssemblyCSharpvs
{

public class BuildingHeight : MonoBehaviour {
	public GameObject plane;
	public GameObject mainCamera;

	//Cindy: this is how you call the JSONParser
	//JSONParser parser = ScriptableObject.CreateInstance ("JSONParser");
	//ArrayList allResults = parser.getAllResults ("mockFilePaths.txt", "mockJavaCode.txt");

	// input format
	// Class name, coupled class name, # instances of couplings, #LOC, comment density, package
	string[,] javaClasses = new string[,] {{"ClassA", "120", "ClassB", "123", "10", "PackageA"},
		{"ClassC", "342", "ClassD", "324", "50", "PackageA"},
		{"ClassB", "13", "ClassA", "1005", "2", "PackageA"},
		{"ClassD", "13", "ClassC", "72", "30", "PackageA"}};
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

		setBuildingDetails(javaClasses);

		// Mock Objects for placing and scaling the buildings
		/*numBuildings = 4;
		bases = new float[] {0, 1, 2, 0};
		heights = new float[] {34.5f, 31.0f, 30.9f, 30.7f};
		positions = new float[,] {{0,0}, {3,1}, {5,3}, {8,2}};
		colours = new Color[]{Color.green, Color.blue, Color.red, Color.yellow, Color.cyan, Color.magenta};*/


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
		plane.transform.localScale = new Vector3(planeX/5, 0, planeZ/5);
		plane.transform.position = new Vector3(planeX/2, 0, planeZ/2);

		Debug.Log("Max X = "+ planeX);
		Debug.Log("Max Y = "+ planeY);
		Debug.Log("Max Z = "+ planeZ);

		// place main camera to show all the buildings
		mainCamera.transform.position = new Vector3 (planeX / 2, planeY / 2, -planeY);
	}
	
	void setBuildingDetails (string[,] javaClasses) {
		numBuildings = javaClasses.GetLength(0);
		//Debug.Log ("Number of Buildings = " + numBuildings);
		bases = new float[numBuildings];
		heights = new float[numBuildings];
		//float[,] positions;
		colours = new Color[numBuildings];

		for (int x = 0; x<numBuildings; x++){
			bases[x] = 1f;

			// set height
			float tempHeight = float.Parse(javaClasses[x,3]);
			if(tempHeight < 0){
				Debug.Log("LOC is in negative number");
				return;
			} else if(tempHeight <= 300) {
				heights[x] = tempHeight / 10;
			} else {
				heights[x] = 30 + tempHeight / 500;
			}

			//float[,] positions;

			int commentDensity = int.Parse(javaClasses[x,4]);

			if(commentDensity < 0){
				Debug.Log("Density is in negative number");
			} else if (commentDensity < 50){
				colours[x] = Color.yellow;
			} else {
				colours[x] = Color.red;
			}
		}
		positions = new float[,] {{0,0}, {3,1}, {5,3}, {8,2}};
	}

	void Update () {

		}
	}
}
