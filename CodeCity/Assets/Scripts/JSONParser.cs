using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace  AssemblyCSharpvs 
{

//This class will take the JSON data from JSONCombiner.cs and parse it to get the class with most coupling and the linesOfCode for that class, which will be passed into the visualizer
public class JSONParser
{

	//This method will read a list of all the types in the project and the json data for a particular file and output
	//a list of all the classes that have coupling with it
	ArrayList parseforCoupling (ArrayList allTypes, string jsonData){
		SimpleJSON.JSONNode N = SimpleJSON.JSONNode.Parse (jsonData);

		ArrayList coupledClasses = new ArrayList ();
		ArrayList coupledCounts = new ArrayList ();

		string className = N ["name"].Value;

		int numberOfFields = N ["fields"].Count;
		int numberOfMethods = N ["methods"].Count;

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
					coupledCounts[index] = value++;
				//otherwise add it to the array and init its count to 1
				}else{
					coupledClasses.Add(fieldType);
					coupledCounts.Add(1);
				}
			}
		}

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
					coupledCounts[index] = value++;
					//otherwise add it to the array and init its count to 1
				}else{
					coupledClasses.Add(returnType);
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

		string maxCoupledClass = (string) coupledClasses[indexOfMax];


		ArrayList result = new ArrayList();

		result.Add (className);
		result.Add (maxCoupledClass);
		result.Add (maxCount);
		result.Add (parseForLineCount (jsonData));

		//Returns an array with 4 fields: [0] = class name, [1] = name of most coupled class, [2] = number of times its coupled with that class, [3] = line count 
		return result;


	}

	//This method will return an approximate line count for a particular java class based on the line number of the last method
	int parseForLineCount (string jsonData) {
		SimpleJSON.JSONNode N = SimpleJSON.JSONNode.Parse (jsonData);
		int numberOfMethods = N ["methods"].Count;

		//This will return the line number of the last method in the code
		//While this is not the most accurate line count, it gives us a rough idea of the lines of code temporaily before we complete a full LoC counter

		//once we have the proper line parser from Kevin, we can use the following code:
		//return N ["linesOfCode"][0]["lines"].Value;

		return N ["methods"] [numberOfMethods - 1] ["line"].AsInt;
	}

	void Start() {
		Console.WriteLine ("testing console");

		string mockFilePath = "http://google-guice.googlecode.com/svn/trunk/core/src/com/google/inject/Key.java";
			
			//Integrating with Custom Parser Component:
			CustomParser customParser = new CustomParser (mockFilePath);
			int linesOfCode = customParser.createLOCJSON ();
			string mockLocJSON = "linesOfCode:[{lines:"+linesOfCode+"}]";
			
			//Integrating with Japarser Component (JSONCombiner)
			JSONCombiner combiner = new JSONCombiner (mockFilePath, mockLocJSON);
			combiner.JSONRequester ();
			string jsonData = combiner.combinedJSONData (combiner.japarserData);

			ArrayList mockAllTypes = new ArrayList{"Instrumentation", "ActivityMonitor", "Activity", "Sleeper", "Timer"};
			ArrayList result = this.parseforCoupling (mockAllTypes, jsonData);

			Console.WriteLine (result);
		}

	}
}


