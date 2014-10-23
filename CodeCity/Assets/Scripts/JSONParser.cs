using SimpleJSON;
using System;
using System.Collections;

public class JSONParser
{

	//Right now this is designed to parse 1 java file
	//Will need to expand once we have the JSON Aggregator to combine all files into one giant JSON file to read


	//This method will read a list of all the types in the project and the json data for a particular file and output
	//a list of all the classes that have coupling with it
	ArrayList parseforCoupling (ArrayList allTypes, string jsonData){
		SimpleJSON.JSONNode N = SimpleJSON.JSONNode.Parse (jsonData);

		ArrayList coupledClasses = new ArrayList ();
		string className = N ["name"].Value;

		int numberOfFields = N ["fields"].Count;
		int numberOfMethods = N ["methods"].Count;

		for (int i = 0; i< numberOfFields; i++)
		{
			string fieldType = N["fields"][i]["simpleTypeName"].Value;
			//If the type of the field is not equal to the className and it is another class in the project, then we have coupling
			if (fieldType != className && allTypes.Contains(fieldType) && !coupledClasses.Contains(fieldType)) 
			{
				//Add that fieldType to the array so we know this class has coupling with that type
				coupledClasses.Add(fieldType);
			}
		}

		for (int j = 0; j< numberOfMethods; j++)
		{
			string paramType = N["methods"][j]["paramName"].Value;
			string returnType = N["methods"][j]["returnName"].Value;
			//If the type of the field is not equal to the className and it is another class in the project, then we have coupling
			if (paramType != className && allTypes.Contains(paramType) && !coupledClasses.Contains(paramType)) 
			{
				//Add that paramType to the array so we know this class has coupling with that type
				coupledClasses.Add (paramType);
			}

			if (returnType != className && allTypes.Contains(returnType) && !coupledClasses.Contains(returnType)) 
			{
				//Add that returnType to the array so we know this class has coupling with that type
				coupledClasses.Add(returnType);
			}
		}

		//Returns an arraylist with all the classes that this class is coupled with 
		return coupledClasses;

	}

	//This method will return an approximate line count for a particular java class based on the line number of the last method
	int parseForLineCount (string jsonData) {
		SimpleJSON.JSONNode N = SimpleJSON.JSONNode.Parse (jsonData);
		int numberOfMethods = N ["methods"].Count;

		//This will return the line number of the last method in the code
		//While this is not the most accurate line count, it gives us a rough idea of the lines of code temporaily before we complete a full LoC counter
		return N ["methods"] [numberOfMethods - 1] ["line"].AsInt;
	}

	
}


