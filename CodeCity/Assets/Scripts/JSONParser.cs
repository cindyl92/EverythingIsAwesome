using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
namespace  AssemblyCSharpvs
{
	//This class reads the file paths text file created by the decomposer, calls the Custom Parser with the code file created by the decomposer
	//then calls the Japarser API to get JSON data which it parses to return the coupling between classes in the repo. Finally, it combines the 
	//coupling data with the lines of code and comment density from the Custom Parser and passes it to BuildingHeight.cs to render
	public class JSONParser
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
			
			try{
				
				string url = "http://japarser.appspot.com/src?url=" + filePath;
		
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
			
			
			//Check for coupling with method returnTypes
			for (int j = 0; j< numberOfMethods; j++)
			{
				string returnType = N["methods"][j]["returnName"].Value;

				//If the type of the returnType is not equal to the className and it is another class in the project, then we have coupling
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
				string subclassType = N["extendsClasses"][k]["name"].Value;

				//If the type of the superclass is not equal to the className and it is another class in the project, then we have coupling
				if (subclassType != className && allTypes.Contains(subclassType)) 
				{
					//If the class has a coupling instance already, increase the couple count
					if (coupledClasses.Contains(subclassType)){
						int index = coupledClasses.IndexOf(subclassType);
						int value = (int) coupledCounts[index];
						if (index >= 0)
							coupledCounts[index] = value++;
						//otherwise add it to the array and init its count to 1
					}else{
						coupledClasses.Add(subclassType);
						coupledCounts.Add(1);
					}
				}
			}
			
			//Check for coupling with implements interfaces
			for (int l = 0; l< numberOfInterfaces; l++)
			{
				string interfaceType = N["implementsInterfaces"][l]["name"].Value;

				//If the type of the interface is not equal to the className and it is another class in the project, then we have coupling
				if (interfaceType != className && allTypes.Contains(interfaceType)) 
				{
					//If the class has a coupling instance already, increase the couple count
					if (coupledClasses.Contains(interfaceType)){
						int index = coupledClasses.IndexOf(interfaceType);
						int value = (int) coupledCounts[index];
						if (index >= 0)
							coupledCounts[index] = value++;

						//otherwise add it to the array and init its count to 1
					}else{
						coupledClasses.Add(interfaceType);
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
			
			string maxCoupledClass = "no coupling";
			if (maxCount > 0) {
				int indexOfMax = coupledCounts.IndexOf (maxCount);
				
				if (indexOfMax >= 0 && indexOfMax < coupledClasses.Count) {
					maxCoupledClass = (string)coupledClasses [indexOfMax];
				}
			}  
			
			string typeName = N ["qualifiedTypeName"];
			
			string packageName = "unspecified";
			
			if (typeName != null) {
				if (typeName.Length <= className.Length) {
					packageName = "no package";
				} else {
					packageName = typeName.Substring (0, typeName.Length - (className.Length + 1));
				}
			}
			
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
		public string[,] getAllResults (string filePaths, string codeFile) {
			
			//Read the text file and get all the file paths
			ArrayList filePathsArray = this.readFilePaths (filePaths);
			
			//Integrates with the Lines of Code and Comment Density Parser
			CustomParser customParser = new CustomParser (codeFile);
			
			//Get all the Lines of Code
			ArrayList linesOfCodeArray = customParser.getArrayLOC();
			//Get all the Comment Density info
			ArrayList commentDensityArray = customParser.getArrayCommentDensity ();
			
			Debug.Log ("loc array size is " + commentDensityArray.Count);
			
			
			//Get all the class names
			ArrayList allClasses = this.getAllClassNames(filePathsArray);

			string[,] allResults = new string[allClasses.Count,6];
			
			for (int i = 0; i<filePathsArray.Count; i++) {
				string jsonData = (string) allJSONData[i];
				ArrayList result = this.parseforCoupling (allClasses, jsonData, (int)linesOfCodeArray[i], (int)commentDensityArray[i]);
				
				//Add the results to a 2D String Array for the Visualizer to use
				allResults[i, 0] = (string)result[0];
				allResults[i, 1] = (string)result[1];
				allResults[i, 2] = (string)result[2].ToString();
				allResults[i, 3] = (string)result[3].ToString();
				allResults[i, 4] = (string)result[4].ToString();
				allResults[i, 5] = (string)result[5];
				
			}
			
			return allResults;
		}
		
		void Start() {

			//Here are three test files you can use to test the components
			string[,] mockAllResults = this.getAllResults ("mockFilePaths.txt", "mockJavaCode.txt");
			//string[,] mockAllResults = this.getAllResults ("javaPaths.txt", "javaText.txt");
			//string[,] mockAllResults = this.getAllResults ("testPaths.txt", "testCode.txt");
			
			for (int i = 0; i< mockAllResults.GetLength(0); i++){
				
				Debug.Log ("RESULTS FOR CLASS: " + mockAllResults[i,0]);
				Debug.Log ("most coupled class is: " + mockAllResults[i,1]);
				Debug.Log ("number of instances: " + mockAllResults[i,2]);
				Debug.Log ("lines of code: " + mockAllResults[i,3]);
				Debug.Log ("comment density: " + mockAllResults[i,4]);
				Debug.Log ("package: " + mockAllResults[i,5]);					
				
			}
			
			Debug.Log ("Woohoo! Everything is Awesome!");
		}
	}
}

