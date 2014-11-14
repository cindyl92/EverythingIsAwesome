using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssemblyCSharpvs 
{
	public class CustomParser :MonoBehaviour //Debug: need MonoBehaviour so that class can be added to a Unity object as a component
	{
		//Debug: need filePath as public so "C:\Users\Kevin\Desktop\textTest20.txt" (no quotes) can be passed to Unity GUI
		//Change to private in production
		public string filePath;
		private int commentCount;
		private int locCounter;

		//constructor
		public CustomParser(string filePath) {	
			this.filePath = filePath;	
		}

		//Debug only
		public int getCommentCount(){
			return commentCount;
		}

		//commentDensity = (commentCount/(commentCount + LOC)) * 100
		//safeLOC lets this getCommentDensity execute without having to first call createLOCJSON.
		//Refactor: Move createLOCJSON to constructor then create getLOC(). Bruce can then just use getLOC() and getCommentDensity()
		public int getCommentDensity(){
			int safeLOC = this.createLOCJSON ();
			return (int) Math.Round (((double)commentCount)/(commentCount+safeLOC)*100);
		}

		public int createLOCJSON(){

			string stringProbe;
			bool inLineComment = false;
			bool inBlockComment = false;

			//StreamReader implementation inspired by: http://msdn.microsoft.com/en-us/library/system.io.streamreader.readline%28v=vs.110%29.aspx

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
							{
								inLineComment = true;
							}

							//if line being read is the start of a block comment, then increment commentCount
							if ((stringProbe[0] == '/') && (stringProbe[1] == '*'))
							{
								commentCount++;

								//if block comment is only 1 line long, then set inBlockComment to false right away, else set to true
								if ((stringProbe[(stringProbe.Length)-2] == '*') && (stringProbe[(stringProbe.Length)-1] == '/'))
								{
									inBlockComment = false;

								}else{
									inBlockComment = true;
								}

							}


							//if line being read is neither an inline comment nor in a block comment, then increment locCounter,
							//else increment commentCount
							if ((!inLineComment) && (!inBlockComment))
							{
								Console.WriteLine(stringProbe); //debug only
								locCounter++;
								inLineComment = false;
								inBlockComment = false;
							}else{
								commentCount++;   
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

			return locCounter;
		}
			
		void Start(){

			//string mockFilePath = @"C:\Users\Kevin\Desktop\textTest20.txt";
			//CustomParser cusPar = new CustomParser(mockFilePath);
			//Console.WriteLine (cusPar.createLOCJSON());
			Debug.Log ("LOC = " + this.createLOCJSON ());
			Debug.Log ("Comment Count = " + commentCount);
			Debug.Log ("commentDensity = " + this.getCommentDensity ());;
			
		}
	}
}