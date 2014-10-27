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
		string customJSON; //generate JSON with dependency in Sprint 2

		//constructor
		public CustomParser(string filePath) {	
			this.filePath = filePath;	
		}

		public int createLOCJSON(){
			///line counter inspiration: http://msdn.microsoft.com/en-us/library/system.io.streamreader.readline%28v=vs.110%29.aspx

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
						
						if (stringProbe.Length > 1){ //this will ignore lines of code that only contain one '}'

							//if the line being read is the end of a block comment, then set inBlockComment to false
							if ((stringProbe[(stringProbe.Length)-2] == '*') && (stringProbe[(stringProbe.Length)-1] == '/'))
							{
								inBlockComment = false;
								locCounter--;
							}

							//if line being read is the start of a line comment, then set inLineComment to true
							if ((stringProbe[0] == '/') && (stringProbe[1] == '/'))
							{ //if the line starts with a comment
								inLineComment = true;
							}

							//if line being read is the start of a block comment, then set inBlockComment to true
							if ((stringProbe[0] == '/') && (stringProbe[1] == '*'))
							{
								inBlockComment = true;
							}


							//if line being read is neither an inline comment nor in a block comment, then increment locCounter
							if ((!inLineComment) && (!inBlockComment))
							{
								Console.WriteLine(stringProbe); //debug only
								locCounter++;
								inLineComment = false;
								inBlockComment = false;
							}
						}
						//inline comments end at end of line, so set inLineComment to false before next line read
						inLineComment = false;
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("The process failed: {0}", e.ToString());
			}

			//replace with JSON 
			return locCounter;
		}
			
		void Start(){

			string mockFilePath = @"C:\Users\Kevin\Desktop\textTest20.txt";

			CustomParser cusPar = new CustomParser(mockFilePath);
			Console.WriteLine (cusPar.createLOCJSON());
			
		}
	}
}