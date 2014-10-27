using System;
using System.Collections;
using System.Net;
using System.IO;

namespace AssemblyCSharpvs
	
{
	//This class will combine the data from the custom and LoC parser with the data from japarser web request
	//The output of this class will be used by JSONParser.cs to get the coupling and lines of code, which will pass into the visualization
	public class JSONCombiner
	{
		
		string filePath;
		string locData;
		public string japarserData;
		
		public JSONCombiner(string filePath, string locData) {
			
			this.filePath = filePath;
			this.locData = locData;
			
		}
		
		//code inspired from http://msdn.microsoft.com/en-us/library/456dfw4f(v=vs.110).aspx
		//Requests data from japarser web api
		//returns the json data for a particular class
		public string JSONRequester (){
			
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
			
			// Display the content.
			Console.WriteLine (japarserData);
			// Clean up the streams and the response.
			reader.Close ();
			response.Close ();
			
			return japarserData;
			
		}
		
		
		//Takes in two strings of data in JSON format from the LoC/custom parser and data from jparser 
		//Returns a combined JSON string
		public String combinedJSONData (string japarserData){
			
			return String.Concat (locData, japarserData);
			
		}
		
		void Start(){
			
			string mockFilePath = "http://google-guice.googlecode.com/svn/trunk/core/src/com/google/inject/Key.java";
			
			//Integrating with Custom Parser Component:
			CustomParser customParser = new CustomParser (mockFilePath);
			int linesOfCode = customParser.createLOCJSON ();
			string mockLocJSON = "linesOfCode:[{lines:"+linesOfCode+"}]";

			JSONCombiner combiner = new JSONCombiner(mockFilePath, mockLocJSON);
			combiner.JSONRequester();
			
			Console.WriteLine(combiner.combinedJSONData(combiner.japarserData));
			
			
		}
		
	}
}


