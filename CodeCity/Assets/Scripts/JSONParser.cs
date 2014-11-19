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
		ArrayList allJSONData;
		
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
			//Debug.Log ("about to request" + filePath);
			
			try{
				
				string url = "http://japarser.appspot.com/src?url=" + filePath;
				
				//this url is temporary for testing the robotium repo until the text file has been fixed with the html file path
				//string url = "http://japarser.appspot.com/src?url=https://github.com/RobotiumTech/robotium/blob/master/" + filePath;
				//Make the request
				WebRequest request = WebRequest.Create (url);
				
				request.Credentials = CredentialCache.DefaultCredentials;
				
				//Get the response
				WebResponse response = request.GetResponse ();
				
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
			
			int numberOfFields = N ["fields"].Count;
			int numberOfMethods = N ["methods"].Count;
			int numberOfSubclasses = N ["extendsClasses"].Count;
			int numberOfInterfaces = N ["implementsInterfaces"].Count;
			
			
			//Check for coupling with fields
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
			
			
			//Check for coupling with method paramTypes and returnTypes
			for (int j = 0; j< numberOfMethods; j++)
			{
				string paramType = N["methods"][j]["paramName"].Value;
				string returnType = N["methods"][j]["returnName"].Value;
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
					
					//If the class has a coupling instance already, increase the couple count
					if (coupledClasses.Contains(returnType)){
						int index = coupledClasses.IndexOf(returnType);
						int value = (int) coupledCounts[index];
						if (index >= 0){
							coupledCounts[index] = value + 1;
						}
						
						//otherwise add it to the array and init its count to 1
					}else{
						coupledClasses.Add(returnType);
						coupledCounts.Add(1);
					}
				}
			}
			
			
			//Check for coupling with extends subclasses
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
			
			//Check for coupling with implements interfaces
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
			
			//Fill the result array
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
			allJSONData = new ArrayList ();
			for (int i = 0; i< filePaths.Count; i++){
				string jsonData = this.JSONRequester((string)filePaths[i]);
				SimpleJSON.JSONNode N = SimpleJSON.JSONNode.Parse(jsonData);
				string className = N ["name"].Value;
				allClassNames.Add(className);
				allJSONData.Add(jsonData);
			}
			return allClassNames;
		}
		
		//Returns the combined results of all the classes in the repo (coupling, lines of code, comment density)
		public ArrayList getAllResults (string filePaths, string codeFile) {
			
			//Read the text file and get all the file paths
			ArrayList filePathsArray = this.readFilePaths (filePaths);
			
			//Integrates with the Lines of Code and Comment Density Parser
			CustomParser customParser = new CustomParser (codeFile);
			
			//Get all the Lines of Code
			ArrayList linesOfCodeArray = customParser.getArrayLOC();
			//Get all the Comment Density info
			ArrayList commentDensityArray = customParser.getArrayCommentDensity ();
			
			ArrayList allResults = new ArrayList ();
			
			//Get all the class names
			ArrayList allClasses = this.getAllClassNames(filePathsArray);
			
			
			for (int i = 0; i<filePathsArray.Count; i++) {
				string jsonData = (string) allJSONData[i];
				ArrayList result = this.parseforCoupling (allClasses, jsonData, (int)linesOfCodeArray[i], (int)commentDensityArray[i]);
				allResults.Add(result);
			}
			
			return allResults;
		}
		
		void Start() {
			
			//ArrayList mockAllResults = this.getAllResults ("roboFilePathsTimes6.txt", "roboCodeTimes6.txt");
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


