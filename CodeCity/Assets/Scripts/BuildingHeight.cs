using UnityEngine;
using System.Collections;

namespace  AssemblyCSharpvs
{
	public class BuildingHeight : MonoBehaviour {
		//public GameObject plane;
		public GameObject mainCamera;
		public GameObject directionalLight;
		
		string[,] javaClasses;
		private ArrayList arrayBuildingLabels = new ArrayList();
		
		//Cindy: this is how you call the JSONParser
		//JSONParser parser = ScriptableObject.CreateInstance ("JSONParser");
		//ArrayList allResults = parser.getAllResults ("mockFilePaths.txt", "mockJavaCode.txt");
		
		// input format
		// Class name, #LOC, coupled class name, # instances of couplings, comment density, package
		/*string[,] javaClasses = new string[,] {{"ClassA", "120", "ClassC", "123", "10", "PackageA"},
			{"ClassB", "342", "ClassD", "324", "50", "PackageA"} ,
			{"ClassC", "13", "ClassA", "105", "90", "PackageA"} ,
			{"ClassD", "2700", "ClassB", "72", "30", "PackageA"} ,
			{"ClassE", "5", "ClassF", "100", "55", "PackageA"} ,
			{"ClassF", "150", "ClassE", "33", "50", "PackageA"} ,
			{"ClassG", "440", "ClassA", "112", "5", "PackageA"}} ;*/
		
		int numBuildings;
		
		float[] bases;
		float[] heights;
		Color[] colours;
		
		float[,] positions;
		int[,] couplings;
		
		ArrayList allBuildings = new ArrayList();
		
		void Start () {
			JSONParser parser = new JSONParser();
			javaClasses = parser.getAllResults("roboFilePaths.txt", "roboCode.txt");
			// mock file: "mockFilePaths.txt", "mockJavaCode.txt"
			// Robotium: "roboFilePaths.txt", "roboCode.txt"
			// jams music: "javaPaths.txt", "javaText.txt"
			
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
				
				allBuildings.Add (building);
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
			
			drawLines ();
			
			if (planeX == 0){ planeX = 15; }
			
			// resize and replace plane to cover all the buildings
			// JONATHAN EDIT: I commented this out (since I don't think we need a plane. correct me if i'm wrong)
			// plane.transform.localScale = new Vector3(planeX/5, 0, planeZ/5);
			// plane.transform.position = new Vector3(planeX/2, 0, planeZ/2);
			
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
			}  else if(tempHeight <= 300) {
				heights[x] = tempHeight / 10;
			}  else {
				heights[x] = 30 + tempHeight / 500;
			}
		}
		
		void setColours (int x) {
			int commentDensity = int.Parse(javaClasses[x,4]);
			float colorPercent = (float)(commentDensity * 0.01);
			Color32 color = new Color (0.2F, 0.3F, 0.4F, colorPercent);
			
			//colours [x] = color;
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
				couplings[x,0] = -1;
				couplings[x,1] = -1;
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
			//foreach (TextMesh b in arrayBuildingLabels) {
			//someTextMesh.transform.rotation = Camera.main.transform.rotation
			// b.transform.rotation = Camera.main.transform.rotation;
			
			//}
		}
	}
}

