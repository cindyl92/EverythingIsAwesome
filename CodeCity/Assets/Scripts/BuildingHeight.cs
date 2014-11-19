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
			Debug.Log ("numBuildings: " + numBuildings);
			setPositions (javaClasses, 0, 0);
			setBuildingDetails (javaClasses);

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
	
		// set building positions based on coupling relations
		void setPositions(string[,] javaClasses, float initialX, float initialZ) {
			//Debug.Log("Cindy - In setPositions");
			int[] numColumns = new int[numBuildings];
			int lastColumn = 0;
			positions = new float[numBuildings,2];

			for (int x = 0; x < numBuildings; x++) {
					//Debug.Log("For - "+javaClasses[x,0]);
				if (x == 0) {
					//Debug.Log ("x = "+x);
					positions[x,0] = initialX;
					//Debug.Log("CL - x pos["+x+",0] :"+positions[x,0]);
					positions[x,1] = initialZ;
					//Debug.Log("CL - x pos["+x+",1] :"+positions[x,1]);
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
						Debug.Log ("1111");
						rowNumber = positions[coupledClass,1] / 3;
						//rowNumber = 1;
						Debug.Log("CL - rowNumber :"+rowNumber);
						Debug.Log ("2222");
						//numColumns[(int)temp] = ;
						numColumns[(int)rowNumber]++;
						Debug.Log ("3333");
						//Debug.Log("coupling - numColumns["+temp+"] = "+numColumns[(int)temp]);
						//positions[x,0] = (float) (positions[y,0] + 3);
						positions[x,0] = positions[coupledClass,0];
						Debug.Log ("4444");
						//Debug.Log("CL - y pos["+x+",0] :"+positions[x,0]);
						//positions[x,1] = positions[y,1];
						positions[x,1] = positions[coupledClass,1] + 3f * (float) numColumns[(int)rowNumber];
						Debug.Log ("5555");
						Debug.Log(javaClasses[x,0]+" coupling "+javaClasses[coupledClass,0]);
						//Debug.Log("CL - y pos["+x+",1] :"+positions[x,1]);
						//		Debug.Log("CL - isCoupled = true - "+javaClasses[x,0]);
						//		Debug.Log("coupling - numColumns["+temp+"] = "+numColumns[(int)temp]);
						isCoupled = false;
					} else {
					//Debug.Log("CL - isCoupled = false - "+javaClasses[x,0]);
						lastColumn ++;
						numColumns[lastColumn] = 0;
					//	Debug.Log("!coupling - numColumns["+lastColumn+"] = "+numColumns[lastColumn]);
						positions[x,0] = (float) lastColumn * 3;
						//Debug.Log("Cindy - x2 pos["+x+",0] :"+positions[x,0]);
						positions[x,1] = 0;
						//Debug.Log("Cindy - x2 pos["+x+",1] :"+positions[x,1]);
					}
					
				}
			}
		}

		void Update () {}
	}
}
