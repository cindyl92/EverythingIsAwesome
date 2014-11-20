using UnityEngine;
using System.Collections;

namespace  AssemblyCSharpvs
{
	public class BuildingHeight : MonoBehaviour {
		public GameObject plane;
		public GameObject mainCamera;
		public GameObject directionalLight;

		string[,] javaClasses;
		// Class name, coupled class name, # couplings, #LOC, comment density, package name
		/*string[,] javaClasses = new string[,] {{"ClassA", "ClassC", "123", "120", "10", "PackageA"},
			{"ClassB", "ClassD", "324", "342", "50", "PackageA"},
			{"ClassC", "ClassA", "105", "13", "90", "PackageA"},
			{"ClassD", "ClassB", "72", "2700", "30", "PackageA"},
			{"ClassE", "ClassF", "100", "5", "55", "PackageA"},
			{"ClassF", "ClassE", "33", "150", "50", "PackageA"},
			{"ClassG", "ClassA", "112", "440", "5", "PackageA"}};	
		// */

		int numBuildings;

		float[] bases;
		float[] heights;
		Color[] colours;

		float[,] positions;
		int[,] couplings;
	
		void Start () {
			JSONParser parser = new JSONParser();
			javaClasses = parser.getAllResults("roboFilePaths.txt", "roboCode.txt");
			// mock file: "mockFilePaths.txt", "mockJavaCode.txt"
			// Robotium: "roboFilePaths.txt", "roboCode.txt"
			// jams music: "javaPaths.txt", "javaCode.txt"

			float planeX = 0;
			float planeY = 0;
			float planeZ = 0;

			numBuildings = javaClasses.GetLength(0);
			setPositions (javaClasses);
			setBuildingDetails (javaClasses);
			findCouplings (javaClasses);

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

			if (planeX == 0){ planeX = 15; }

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

			// initialize float/color arrays
			bases = new float[numBuildings];
			heights = new float[numBuildings];
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

			if(commentDensity < 0 || commentDensity > 100){
				Debug.Log("invalid comment density: " +commentDensity+ ", class: " +javaClasses[x,0]);

			} else if (commentDensity < 30){
				colours[x] = Color.white;

			} else if (commentDensity < 60){
				colours[x] = Color.blue;

			} else {
				colours[x] = Color.black;

			}
		}	
	
		// set building positions based on coupling relations and save them into 2d array - positions[,]
		void setPositions(string[,] javaClasses){
			positions = new float[numBuildings,2];

			int numFirstRow = (int) Mathf.Ceil (numBuildings / 4f);
			//Debug.Log ("numBuildings: " + numBuildings);
			//Debug.Log ("numFirstRow: " + numFirstRow);

			for (int x = 0; x < numFirstRow; x++) {
				positions[x,0] = x * 3;
				positions[x,1] = 0;

				int secondAxisPosition = x + numFirstRow;
				positions[secondAxisPosition,0] = 0;
				positions[secondAxisPosition,1] = x * 3 + 3;

				int thirdAxisPosition = x + 2 * numFirstRow;
				positions[thirdAxisPosition,0] = x * 3 + 3;
				positions[thirdAxisPosition,1] = numFirstRow * 3;

				int fourthAxisPosition = x + 3 * numFirstRow;
				if (fourthAxisPosition < numBuildings) {
					positions[fourthAxisPosition,0] = numFirstRow * 3;
					positions[fourthAxisPosition,1] = (numFirstRow - x -1) * 3;
				}

			}
		}

		// find couplings and save them into 2d array - couplings[,]
		void findCouplings(string[,] javaClasses){
			Debug.Log ("Find couplings");
			couplings = new int[numBuildings,2];
			for (int x = 0; x < numBuildings; x++) {
				if(javaClasses[x,2] != "no coupling"){
					for (int y = 0; y < x; y++) {
						if (javaClasses[y,0] == javaClasses[x,1]) {
							couplings[x,0] = x;
							couplings[x,1] = y;
							Debug.Log("Coupling - "+ javaClasses[x,0]+", "+javaClasses[x,1]);
							break;	
						}
					}
				}
			}
		}

		void Update () {}
	}
}
