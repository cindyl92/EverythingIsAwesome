using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;

namespace  AssemblyCSharpvs
{


public class JSONParser:ScriptableObject //ScriptableObject is a bit easier to deal with in unit tests than MonoBehavior
{
	
	string stringProbe;
	
	//Reads the filePath text file and returns a list of file paths
	ArrayList readFilePaths (string filePath) {
		
			ArrayList filePathsArray = new ArrayList ();
			try
			{
				using (StreamReader sr = new StreamReader(filePath))
				{
					while (sr.Peek() >= 0)
					{
						stringProbe = sr.ReadLine(); //reads in the next line from text
						filePathsArray.Add (stringProbe);
					}
				}
			}
			catch (Exception e)
			{
				Debug.Log ("The process failed: {0}"+ e.ToString());
			}
			return filePathsArray;	
		}


		//code inspired from http://msdn.microsoft.com/en-us/library/456dfw4f(v=vs.110).aspx
		//Requests data from japarser web api
		//returns the json data for a particular class
		public string JSONRequester (string filePath){

			string japarserData = "";
			
			try{
				string url = "http://japarser.appspot.com/src?url=" + filePath;
				//Make the request
				WebRequest request = WebRequest.Create (url);
				
				request.Credentials = CredentialCache.DefaultCredentials;
				
				//Get the response
				WebResponse response = request.GetResponse ();
				
				// Display the status.
				Console.WriteLine (((HttpWebResponse)response).StatusDescription);
				
				// Get the stream containing content returned by the server.
				Stream dataStream = response.GetResponseStream ();
				
				// Open the stream using a StreamReader for easy access.
				StreamReader reader = new StreamReader (dataStream);
				
				// Read the content.
				japarserData = reader.ReadToEnd ();

				// Clean up the streams and the response.
				reader.Close ();
				response.Close ();
			}catch(Exception e){
				Debug.Log ("exception: "+e); 
			}
			
			return japarserData;
			
		}
	

	//This method will read a list of all the types in the project and the json data for a particular file and output
	//a list of all the classes that have coupling with it
	ArrayList parseforCoupling (ArrayList allTypes, string jsonData, int linesOfCode, int commentDensity){
		SimpleJSON.JSONNode N = SimpleJSON.JSONNode.Parse (jsonData);

		ArrayList coupledClasses = new ArrayList ();
		ArrayList coupledCounts = new ArrayList ();

		string className = N ["name"].Value;
		
			//Debug.Log ("class name: " + className);

		int numberOfFields = N ["fields"].Count;
		int numberOfMethods = N ["methods"].Count;
		int numberOfSubclasses = N ["extendsClasses"].Count;
		int numberOfInterfaces = N ["implementsInterfaces"].Count;

			//Debug.Log ("field count: " + numberOfFields);
			//Debug.Log ("method count: " + numberOfMethods);

		for (int i = 0; i< numberOfFields; i++)
		{
			string fieldType = N["fields"][i]["simpleTypeName"].Value;
			//If the type of the field is not equal to the className and it is another class in the project, then we have coupling
			if (fieldType != className && allTypes.Contains(fieldType)) 
			{
				//If the class has a coupling instance already, increase the couple count
				if (coupledClasses.Contains(fieldType)){
					int index = coupledClasses.IndexOf(fieldType);
					int value = (int) coupledCounts[index];
					if (index >= 0)
					coupledCounts[index] = value++;
				//otherwise add it to the array and init its count to 1
				}else{
					coupledClasses.Add(fieldType);
					coupledCounts.Add(1);
				}
			}
		}

			//Debug.Log ("coupledCounts counts after fields loop: "+coupledCounts.Capacity);

		for (int j = 0; j< numberOfMethods; j++)
		{
			string paramType = N["methods"][j]["paramName"].Value;
			string returnType = N["methods"][j]["returnName"].Value;
				//Debug.Log("returnType: "+returnType);
			//If the type of the field is not equal to the className and it is another class in the project, then we have coupling
			if (paramType != className && allTypes.Contains(paramType)) 
			{
				//If the class has a coupling instance already, increase the couple count
				if (coupledClasses.Contains(paramType)){
					int index = coupledClasses.IndexOf(paramType);
					int value = (int) coupledCounts[index];
					if (index >= 0)
					coupledCounts[index] = value++;
					//otherwise add it to the array and init its count to 1
				}else{
					coupledClasses.Add(paramType);
					coupledCounts.Add(1);
				}
			}

			if (returnType != className && allTypes.Contains(returnType)) 
			{
					//Debug.Log("inside first if");
				//If the class has a coupling instance already, increase the couple count
				if (coupledClasses.Contains(returnType)){
					int index = coupledClasses.IndexOf(returnType);
					int value = (int) coupledCounts[index];
					if (index >= 0){
					coupledCounts[index] = value + 1;
						}

						//Debug.Log("inside second if, new value is:"+coupledCounts[index]);
					//otherwise add it to the array and init its count to 1
				}else{
					coupledClasses.Add(returnType);
					coupledCounts.Add(1);
						//Debug.Log("inside else of second if");
				}
			}
		}


			for (int k = 0; k< numberOfSubclasses; k++)
			{
				string fieldType = N["extendsClasses"][k]["name"].Value;
				//If the type of the field is not equal to the className and it is another class in the project, then we have coupling
				if (fieldType != className && allTypes.Contains(fieldType)) 
				{
					//If the class has a coupling instance already, increase the couple count
					if (coupledClasses.Contains(fieldType)){
						int index = coupledClasses.IndexOf(fieldType);
						int value = (int) coupledCounts[index];
						if (index >= 0)
							coupledCounts[index] = value++;
						//otherwise add it to the array and init its count to 1
					}else{
						coupledClasses.Add(fieldType);
						coupledCounts.Add(1);
					}
				}
			}

			for (int l = 0; l< numberOfInterfaces; l++)
			{
				string fieldType = N["implementsInterfaces"][l]["name"].Value;
				//If the type of the field is not equal to the className and it is another class in the project, then we have coupling
				if (fieldType != className && allTypes.Contains(fieldType)) 
				{
					//If the class has a coupling instance already, increase the couple count
					if (coupledClasses.Contains(fieldType)){
						int index = coupledClasses.IndexOf(fieldType);
						int value = (int) coupledCounts[index];
						if (index >= 0)
							coupledCounts[index] = value++;
						//otherwise add it to the array and init its count to 1
					}else{
						coupledClasses.Add(fieldType);
						coupledCounts.Add(1);
					}
				}
			}

		int maxCount = 0;

		foreach (int i in coupledCounts) {
						if (i > maxCount) {
								maxCount = i;
						}
				}

		int indexOfMax = coupledCounts.IndexOf (maxCount);


			string maxCoupledClass = "no coupling";
		if (indexOfMax >= 0){
		maxCoupledClass = (string) coupledClasses[indexOfMax];

			}

		string typeName = N ["qualifiedTypeName"];
		string packageName = typeName.Substring (0, typeName.Length - (className.Length +1));

		ArrayList result = new ArrayList();

		result.Add (className);
		result.Add (maxCoupledClass);
		result.Add (maxCount);
		result.Add (linesOfCode);
		result.Add (commentDensity);
		result.Add (packageName);

		return result;
	}

	//Returns a list of all the class names in the repo
	ArrayList getAllClassNames (ArrayList filePaths) {

			ArrayList allClassNames = new ArrayList ();
			for (int i = 0; i< filePaths.Count-1; i++){
				JSONCombiner jRequest = new JSONCombiner((string)filePaths[i]);
				string jsonData = jRequest.JSONRequester();
				SimpleJSON.JSONNode N = SimpleJSON.JSONNode.Parse(jsonData);
				string className = N ["name"].Value; 
				allClassNames.Add(className);
			}
			return allClassNames;
	}

	//Returns the combined results of all the classes in the repo (coupling, lines of code, comment density)
	public ArrayList getAllResults (string filePaths, string codeFile) {
			CustomParser customParser = new CustomParser (codeFile);
	
			ArrayList filePathsArray = this.readFilePaths (filePaths);
			ArrayList linesOfCodeArray = customParser.getArrayLOC();
			ArrayList commentDensityArray = customParser.getArrayCommentDensity ();
			ArrayList allResults = new ArrayList ();
			ArrayList allClasses = this.getAllClassNames(filePathsArray);

			for (int i = 0; i<filePathsArray.Count-1; i++) {
				string jsonData = this.JSONRequester((string)filePathsArray[i]);
				ArrayList result = this.parseforCoupling (allClasses, jsonData, (int)linesOfCodeArray[i], (int)commentDensityArray[i]);
				allResults.Add(result);
			}

			return allResults;
	}

	void Start() {

			ArrayList mockAllResults = this.getAllResults ("mockFilePaths.txt", "mockJavaCode.txt");

			for (int i = 0; i< mockAllResults.Count; i++){
				ArrayList result = (ArrayList) mockAllResults[i];

				Debug.Log ("RESULTS FOR CLASS: " + result [0]);
				Debug.Log ("most coupled class is: " + result [1]);
				Debug.Log ("number of instances: " + result [2]);
				Debug.Log ("lines of code: " + result [3]);
				Debug.Log ("comment density: " + result [4]);
				Debug.Log ("package: " + result [5]);					

			}

			Debug.Log ("Woohoo! Everything is Awesome!");
		}
	}
}


