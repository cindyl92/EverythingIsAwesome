using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AssemblyCSharpvs
{
	class CustomParser
	{
		string filePath;
		string customJSON;


		public CustomParser(string filePath) {	
			this.filePath = filePath;	
		}



		public int createLOCJSON(){
			///code below inspired by http://msdn.microsoft.com/en-us/library/system.io.streamreader.readline%28v=vs.110%29.aspx

			string stringProbe;
			int locCounter = 0;
			bool inLineComment = false;
			bool inBlockComment = false;

			try
			{
				using (StreamReader sr = new StreamReader(filePath))
				{
					while (sr.Peek() >= 0)
					{
						stringProbe = sr.ReadLine(); //reads in the next line from text
						stringProbe = stringProbe.Trim(); //removes all leading and trailing white space from a line
						
						if (stringProbe.Length > 1){ //this assumes that lines with only a } don't count
							
							if ((stringProbe[(stringProbe.Length)-2] == '*') && (stringProbe[(stringProbe.Length)-1] == '/'))
							{ //if the block comment terminates, then inBlockComment = false
								//Console.WriteLine("second to last char = " + stringProbe[stringProbe.Length - 2]);
								//Console.WriteLine("last char = " + stringProbe[stringProbe.Length-1]);
								inBlockComment = false;
								locCounter--;
							}
							
							if ((stringProbe[0] == '/') && (stringProbe[1] == '/'))
							{ //if the line starts with a comment
								inLineComment = true;
							}
							
							if ((stringProbe[0] == '/') && (stringProbe[1] == '*'))
							{
								inBlockComment = true;
							}
							
							if ((!inLineComment) && (!inBlockComment))
							{ //if the line is not blank, then add to counter.
								Console.WriteLine(stringProbe);
								locCounter++;
								inLineComment = false;
								inBlockComment = false;
							}
						}
						inLineComment = false;
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("The process failed: {0}", e.ToString());
			}
			
			return locCounter;
		}
			

		void Start(){

			string mockFilePath = @"C:\Users\Kevin\Desktop\textTest20.txt";

			CustomParser cusPar = new CustomParser(mockFilePath);
			Console.WriteLine (cusPar.createLOCJSON());
			
		}
	}
}