using UnityEngine;
using System.Collections;

namespace  AssemblyCSharpvs
{

	public class BuildingHeight : MonoBehaviour {
		public GameObject plane;
		public GameObject mainCamera;
		public GameObject directionalLight;

		private ArrayList arrayBuildingLabels = new ArrayList();

		//Cindy: this is how you call the JSONParser
		//JSONParser parser = ScriptableObject.CreateInstance ("JSONParser");
		//ArrayList allResults = parser.getAllResults ("mockFilePaths.txt", "mockJavaCode.txt");

		// input format
		// Class name, #LOC, coupled class name, # instances of couplings, comment density, package
		string[,] javaClasses = new string[,] {{"ClassA", "120", "ClassC", "123", "10", "PackageA"},
			{"ClassB", "342", "ClassD", "324", "50", "PackageA"},
			{"ClassC", "13", "ClassA", "105", "90", "PackageA"},
			{"ClassD", "2700", "ClassB", "72", "30", "PackageA"},
			{"ClassE", "5", "ClassF", "100", "55", "PackageA"},
			{"ClassF", "150", "ClassE", "33", "50", "PackageA"},
			{"ClassG", "440", "ClassA", "112", "5", "PackageA"}};
	
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

			numBuildings = javaClasses.GetLength(0);
			setPositions (javaClasses);
			setBuildingDetails (javaClasses);

			// Instantiate example from : http://docs.unity3d.com/Manual/InstantiatingPrefabs.html
			// instantiate building objects and apply building position, scale (height and bases), and colour
			for (int x = 0; x < numBuildings; x++) {
				GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
				Rigidbody buildingRigidBody = building.AddComponent<Rigidbody>();
				buildingRigidBody.isKinematic = true;
				
				GameObject clone = new GameObject("className");

				TextMesh buildingLabel = clone.AddComponent<TextMesh>();
				buildingLabel.transform.position = new Vector3(positions[x,0], 2*(heights[x]/2 + 0.5f), positions[x,1]);
				//buildingLabel.font = new Font("Arial");
				Font ArialFont = (Font)Resources.GetBuiltinResource (typeof(Font), "Arial.ttf");
				buildingLabel.font = ArialFont;
				buildingLabel.renderer.material = ArialFont.material;
				buildingLabel.text = javaClasses[x,0];
				
				buildingLabel.fontSize = 10;
				buildingLabel.color = Color.white;
				
				arrayBuildingLabels.Add (buildingLabel);
			
				//use: http://forum.unity3d.com/threads/make-textmesh-face-camera.251840/ after scripting the camera
				//someTextMesh.transform.rotation = Camera.main.transform.rotation

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
		/*
		Color lightBlue = new Color(0.177F, 0.204F, 0.243F, 0.5F);
		Color blue = new Color(0.1F, 0.1F, 0.7F, 0.5F);
		Color darkBlue = new Color(0.1F, 0.1F, 0.1F, 0.5F);
		*/
			if(commentDensity < 0 || commentDensity > 100){
				Debug.Log("invalid comment density: " +commentDensity+ ", class: " +javaClasses[x,0]);
			} else if (commentDensity < 30){
			//colours[x] = lightBlue;
				colours[x] = Color.white;
			} else if (commentDensity < 60){
			//colours[x] = blue;
				colours[x] = Color.blue;
			} else {
			//colours[x] = darkBlue;
				colours[x] = Color.black;
			}
		}	
	
		// set building positions based on coupling relations
		void setPositions(string[,] javaClasses){
			int[] numColumns = new int[numBuildings];
			int lastColumn = 0;
			positions = new float[numBuildings,2];

			for (int x = 0; x < numBuildings; x++) {
				if (x == 0) {
					positions[x,0] = 0;
					positions[x,1] = 0;
					numColumns[x] = 0;
				}
				else {
					bool isCoupled = false;
					float rowNumber;
					int coupledClass = 0;

					for (int y = 0; y < x; y++) {
						if (javaClasses[y,0] == javaClasses[x,2]) {
							isCoupled = true;
							coupledClass = y;
							break;
						}
					}

					if (isCoupled){
						Debug.Log(javaClasses[x,0]+" coupling "+javaClasses[coupledClass,0]);
						rowNumber = positions[coupledClass,0] / 3;
						numColumns[(int)rowNumber]++;
						positions[x,0] = positions[coupledClass,0];
						positions[x,1] = positions[coupledClass,1] + 3f * (float) numColumns[(int)rowNumber];
						isCoupled = false;
					} else {
						lastColumn ++;
						numColumns[lastColumn] = 0;
						positions[x,0] = (float) lastColumn * 3;
						positions[x,1] = 0;
					}
					
				}
			}
		}

		void Update () {
			foreach (TextMesh b in arrayBuildingLabels) {
				//someTextMesh.transform.rotation = Camera.main.transform.rotation
				b.transform.rotation = Camera.main.transform.rotation;
				
			}
		}
	}
}
