using UnityEngine;
using System.Collections;

namespace  AssemblyCSharpvs
{

public class BuildingHeight : MonoBehaviour {
	public GameObject plane;
	public GameObject mainCamera;
	public GameObject directionalLight;

	//Cindy: this is how you call the JSONParser
	//JSONParser parser = ScriptableObject.CreateInstance ("JSONParser");
	//ArrayList allResults = parser.getAllResults ("mockFilePaths.txt", "mockJavaCode.txt");

	// input format
	// Class name, coupled class name, # instances of couplings, #LOC, comment density, package
	string[,] javaClasses = new string[,] {{"ClassA", "120", "ClassC", "123", "10", "PackageA"},
		{"ClassB", "342", "ClassD", "324", "50", "PackageA"},
		{"ClassC", "13", "ClassA", "1005", "90", "PackageA"},
		{"ClassD", "13", "ClassB", "72", "30", "PackageA"}};
	
	int numBuildings;

	float[] bases;
	float[] heights;
	Color[] colours;

	int[,] buidlingOrders;
	float[,] positions;
	
	void Start () {
		float planeX = 0;
		float planeY = 0;
		float planeZ = 0;

		setBuildingOrders (javaClasses);
		setBuildingDetails (javaClasses);
		positions = new float[,] {{0,0}, {3,1}, {5,3}, {8,2}};

		// Mock Objects for placing and scaling the buildings
		/*numBuildings = 4;
		bases = new float[] {0, 1, 2, 0};
		heights = new float[] {34.5f, 31.0f, 30.9f, 30.7f};
		positions = new float[,] {{0,0}, {3,1}, {5,3}, {8,2}};
		colours = new Color[]{Color.green, Color.blue, Color.red, Color.yellow, Color.cyan, Color.magenta};*/

		if (numBuildings == null || positions == null) {
			Debug.Log("Cindy - No input or invalid input.");
			return;
		}

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

		//Debug.Log("Max X = "+ planeX);
		//Debug.Log("Max Y = "+ planeY);
		//Debug.Log("Max Z = "+ planeZ);

		// place main camera and directional light to show all the buildings
		mainCamera.transform.position = new Vector3 (planeX / 2, planeY / 2, -planeY);
		directionalLight.transform.position = new Vector3 (planeX / 2, planeY - 10, -10);
	}

	// Set buiding bases, heights, colours, and positions
	void setBuildingDetails (string[,] javaClasses) {
		numBuildings = javaClasses.GetLength(0);
		//Debug.Log ("Number of Buildings = " + numBuildings);
		bases = new float[numBuildings];
		heights = new float[numBuildings];
		//float[,] positions;
		colours = new Color[numBuildings];

		for (int x = 0; x<numBuildings; x++){
			setBases (x);
			setHeights (x);
			setColours (x);
		}
	}

	void setBases (int x) {
		bases[x] = 1f;
	}

	void setHeights (int x) {
		float tempHeight = float.Parse(javaClasses[x,3]);
		if(tempHeight < 0){
			Debug.Log("invalid LOC:" +tempHeight+ ", class: " +javaClasses[x,0]);
			return;
		} else if(tempHeight <= 300) {
			heights[x] = tempHeight / 10;
		} else {
			heights[x] = 30 + tempHeight / 500;
		}
	}

	void setColours (int x) {
		int commentDensity = int.Parse(javaClasses[x,4]);
		
		Color lightBlue = new Color(0.177F, 0.204F, 0.243F, 0.5F);
		Color blue = new Color(0.1F, 0.1F, 0.7F, 0.5F);
		Color darkBlue = new Color(0.1F, 0.1F, 0.1F, 0.5F);
		
		if(commentDensity < 0 || commentDensity > 100){
			Debug.Log("invalid comment density: " +commentDensity+ ", class: " +javaClasses[x,0]);
		} else if (commentDensity < 30){
			//colours[x] = lightBlue;
			colours[x] = Color.white;
			//Debug.Log("light- comment density: " +commentDensity+ ", class: " +javaClasses[x,0]);
		} else if (commentDensity < 60){
			//colours[x] = blue;
			colours[x] = Color.blue;
			//Debug.Log("blue- comment density: " +commentDensity+ ", class: " +javaClasses[x,0]);
		} else {
			//colours[x] = darkBlue;
			colours[x] = Color.black;
			//Debug.Log("dark- comment density: " +commentDensity+ ", class: " +javaClasses[x,0]);
		}
	}
	
	// set building orders based on coupling relations into 2d array
	void setBuildingOrders(string[,] javaClasses) {
		int arrHeight;
		for (int x = 0; x < numBuildings; x++) {
			if(x == 0){
				buidlingOrders[x,0] = x;
			}
			else{
				for(int y = 0; y < x; y++){
					if (javaClasses[y,2] == javaClasses[x,0]){
						//set the building beside
					}
					else {
						//set the building below
					}
				}
			}
		}
	}

	void Update () {

		}
	}
}
