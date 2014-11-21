using UnityEngine;
using System.Collections;

namespace  AssemblyCSharpvs
{
	public class BuildingHeight : MonoBehaviour {
		public GameObject mainCamera;
		public GameObject directionalLight;

		string[,] javaClasses;
		//ArrayList arrayBuildingLabels = new ArrayList();
		bool rotationFlag = true;
		
		//Cindy: this is how you call the JSONParser
		//JSONParser parser = ScriptableObject.CreateInstance ("JSONParser");
		//ArrayList allResults = parser.getAllResults ("mockFilePaths.txt", "mockJavaCode.txt");

		// Class name, #LOC, coupled class name, # instances of couplings, comment density, package

		/*string[,] javaClasses = new string[,] {{"ClassA", "120", "ClassC", "123", "10", "PackageA"},
			{"ClassB", "342", "ClassD", "324", "50", "PackageA"} ,
			{"ClassC", "13", "ClassA", "105", "90", "PackageA"} ,
			{"ClassD", "2700", "ClassB", "72", "30", "PackageA"} ,
			{"ClassE", "5", "ClassF", "100", "55", "PackageA"} ,
			{"ClassF", "150", "ClassE", "33", "50", "PackageA"} ,
			{"ClassG", "440", "ClassA", "112", "5", "PackageA"}} ;*/

		private ArrayList arrayBuildingLabels = new ArrayList();
		int numBuildings;
		
		float[] bases;
		float[] heights;
		Color[] colours;
		
		float[,] positions;
		int[,] couplings;

		int[] packages;
		int numPackages;
		string[] packageNames;

		ArrayList allBuildings = new ArrayList();
		
		void Start () {
			JSONParser parser = new JSONParser();
			javaClasses = parser.getAllResults("roboFilePaths.txt", "roboCode.txt");
			// mock file: "mockFilePaths.txt", "mockJavaCode.txt"
			// Robotium: "roboFilePaths.txt", "roboCode.txt"
			// jams music: "javaPaths.txt", "javaText.txt"

			positions = new float[numBuildings,2];

			numBuildings = javaClasses.GetLength(0);
			findPackages ();
			findCouplings ();
			setBuildingDetails ();

			float initialX = 0;
			int classOrder = 0;
			numPackages++;
			Debug.Log ("start - numPackages = " + numPackages);

			positions = new float[numBuildings,2];
			for (int a = 0; a < numPackages; a++) {
				Debug.Log("set positions for each pacakge: "+packageNames[a]);
				Debug.Log ("setPositions("+classOrder+", "+initialX+", "+packages[a]+")");

				int numFirstRow = setPositions(classOrder, initialX, packages[a]);

				if (numFirstRow == 1 && packages[a] == 1){
					initialX += (float)numFirstRow * 3;
				} else {
					initialX += (float)(numFirstRow + 2) * 3;
				}
				classOrder += packages[a];
			}

			// Instantiate example from : http://docs.unity3d.com/Manual/InstantiatingPrefabs.html
			// instantiate building objects and apply building position, scale (height and bases), and colour
			for (int x = 0; x < numBuildings; x++) {
				GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
				Rigidbody buildingRigidBody = building.AddComponent<Rigidbody>();
				buildingRigidBody.isKinematic = true;
				
				GameObject clone = new GameObject("className");

				Rigidbody cloneRigidBody = clone.AddComponent<Rigidbody>();
				
				cloneRigidBody.useGravity = false;
				cloneRigidBody.isKinematic = true;
				
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
				
				allBuildings.Add (building);
				//use: http://forum.unity3d.com/threads/make-textmesh-face-camera.251840/ after scripting the camera
				//someTextMesh.transform.rotation = Camera.main.transform.rotation

				Debug.Log("start - positions: "+javaClasses[x,0]+"("+positions[x,0]+","+positions[x,1]+")");

				building.transform.position = new Vector3(positions[x,0], heights[x]/2 + 0.5f, positions[x,1]);
				building.transform.localScale += new Vector3 (bases[x], heights[x], bases[x]);
				building.renderer.material.color = colours[x];
			}
			
			drawLines ();

		}

		// Set buiding bases, heights, colours, and positions
		void setBuildingDetails () {
			
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
			}  else if(tempHeight <= 300) {
				heights[x] = tempHeight / 10;
			}  else {
				heights[x] = 30 + tempHeight / 500;
			}
		}
		
		void setColours (int x) {
			int commentDensity = int.Parse(javaClasses[x,4]);

			if(commentDensity < 0 || commentDensity > 100){
				Debug.Log("invalid comment density: " +commentDensity+ ", class: " +javaClasses[x,0]);
				
			}  else if (commentDensity < 30){
				colours[x] = Color.white;
				
				
			}  else if (commentDensity < 60){
				colours[x] = Color.blue;
				
			}  else {
				colours[x] = Color.black;
				
			}
		}	
		
		// set building positions based on coupling relations and save them into 2d array - positions[,]
		int setPositions(int classOrder, float initX, int numClasses){
			
			int numFirstRow = (int) Mathf.Ceil (numClasses / 4f);

			int i = 0;
			Debug.Log ("classorder + numFirstRow: " + (numFirstRow + classOrder));
			for (int x = classOrder; x < (numFirstRow + classOrder); x++) {
				Debug.Log("set positions - x = "+x);
				Debug.Log("setPositions");

				//First side
				if (x < (numClasses + classOrder)) {
					Debug.Log("set positions -1st x = "+x);
					positions[x,0] = i * 3 + initX;
					positions[x,1] = 0;
					Debug.Log(javaClasses[x,0]+" - ("+positions[x,0]+","+positions[x,1]+")");
				} else {
					break;
				}

				//Second side
				int secondAxisPosition = x + numFirstRow;
				if (secondAxisPosition < (numClasses + classOrder)) {
					Debug.Log("set positions -2nd x = "+secondAxisPosition);
					positions[secondAxisPosition,0] = 0 + initX;
					positions[secondAxisPosition,1] = i * 3 + 3;
					Debug.Log(javaClasses[secondAxisPosition,0]+" - ("+positions[secondAxisPosition,0]+","+positions[secondAxisPosition,1]+")");
				}

				//Third side
				int thirdAxisPosition = x + 2 * numFirstRow;
				if (thirdAxisPosition < (numClasses + classOrder)) {
					Debug.Log("set positions -3rd x = "+thirdAxisPosition);
					positions[thirdAxisPosition,0] = i * 3 + 3 + initX;
					positions[thirdAxisPosition,1] = numFirstRow * 3;
					Debug.Log(javaClasses[thirdAxisPosition,0]+" - ("+positions[thirdAxisPosition,0]+","+positions[thirdAxisPosition,1]+")");
				}

				//Fourth side
				int fourthAxisPosition = x + 3 * numFirstRow;
				if (fourthAxisPosition < (numClasses + classOrder)) {
					Debug.Log("set positions -4th x = "+fourthAxisPosition);
					positions[fourthAxisPosition,0] = numFirstRow * 3 + initX;
					positions[fourthAxisPosition,1] = (numFirstRow - i -1) * 3;
					Debug.Log(javaClasses[fourthAxisPosition,0]+" - ("+positions[fourthAxisPosition,0]+","+positions[fourthAxisPosition,1]+")");
				}		
				i++;
			}
			return numFirstRow;
		}
		
		// find couplings and save them into 2d array - couplings[,]
		void findCouplings(){
			couplings = new int[numBuildings,2];
			for (int x = 0; x < numBuildings; x++) {
				couplings[x,0] = -1;
				couplings[x,1] = -1;
				if(javaClasses[x,2] != "no coupling"){
					for (int y = 0; y < x; y++) {
						if (javaClasses[y,0] == javaClasses[x,1]) {
							Debug.Log("Coupling - "+ javaClasses[x,0]+", "+javaClasses[x,1]);
							couplings[x,0] = x;
							couplings[x,1] = y;
							break;	
						}
					}
				}
			}
		}

		void findPackages(){
			packages = new int[100];
			packageNames = new string[100];
			numPackages = 0;
			string packageName = javaClasses[0, 5];

			for (int x = 0; x < numBuildings; x++) {
				if (x == 0) {
					packageNames[x] = javaClasses[x,5];
					packages[x] = 1;

				} else if (packageNames[numPackages] == javaClasses[x,5]){
					packages[numPackages]++;

				} else {
					numPackages++;
					packageNames[numPackages] = javaClasses[x,5];
					packages[numPackages] = 1;

				}
				Debug.Log("Package #: "+numPackages+", package name: "+packageNames[numPackages]+", number of classes: "+packages[numPackages]);
			}
		}
	
		void drawLines() {
			for (int i = 0; i<couplings.GetLength(0); i++) {
				
				if (couplings [i, 0] != -1) {
					
					GameObject parentBuilding = (GameObject) allBuildings [i];
					LineRenderer lineRenderer = parentBuilding.AddComponent<LineRenderer> ();
					lineRenderer.material = new Material (Shader.Find ("Particles/Additive"));
					lineRenderer.SetColors (Color.yellow, Color.red);
					lineRenderer.SetWidth (0.1F, 1F);
					lineRenderer.SetVertexCount (2);
					float parentX = positions [i, 0];
					float parentZ = positions [i, 1];
					int childIndex = couplings [i, 1];
					
					float childX = positions [childIndex, 0];
					float childZ = positions [childIndex, 1];
					
					
					
					Vector3 parentPos = new Vector3 (parentX, 1, parentZ);
					Vector3 childPos = new Vector3 (childX, 1, childZ);
					
					
					
					
					//same row, needs to curve
					if (childX == parentX && Mathf.Abs(childZ - parentZ) > 3){
						
						lineRenderer.SetVertexCount(3);
						
						float curveX = parentX;
						
						if (parentX == 0){
							curveX -= 5;
						}else{
							curveX += 5;
						}
						
						
						Vector3 curvePosX = new Vector3(curveX, 1, (parentZ+childZ)/2);
						lineRenderer.SetPosition (0, parentPos);
						lineRenderer.SetPosition (1, curvePosX);
						lineRenderer.SetPosition (2, childPos);
						
						
					}else if (childZ == parentZ && Mathf.Abs(childX - parentX)  > 3){
						
						lineRenderer.SetVertexCount(3);
						
						float curveZ = parentZ;
						
						if (parentZ == 0){
							curveZ -= 5;
						}else{
							curveZ += 5;
						}
						
						Vector3 curvePosZ = new Vector3((parentX+childX)/2f, 1, curveZ);
						lineRenderer.SetPosition (0, parentPos);
						lineRenderer.SetPosition (1, curvePosZ);
						lineRenderer.SetPosition (2, childPos);
						
					}else {	
						
						lineRenderer.SetPosition (0, parentPos);
						lineRenderer.SetPosition (1, childPos);
					}	

				}
				
			}
		}
		
		void Update () {
			
			if (mainCamera.camera.enabled) {
				if (!rotationFlag){
					foreach(TextMesh bl in arrayBuildingLabels){
						bl.transform.rotation = Quaternion.AngleAxis(15, Vector3.right);
					}
					rotationFlag = !rotationFlag;
				}
			}else{
				if (rotationFlag){
					foreach(TextMesh bl in arrayBuildingLabels){
						bl.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
					}
					rotationFlag = !rotationFlag;
				}
			}
		}
	}
}

