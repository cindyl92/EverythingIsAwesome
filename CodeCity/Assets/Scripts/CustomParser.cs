using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AssemblyCSharpvs 
{
	public class CustomParser
	{
		private int commentCount;
		private int locCounter;
		private int commentDensity;
		private ArrayList arrayCommentDensity = new ArrayList();
		private ArrayList arrayLOC = new ArrayList();
		
		//constructor: creates arrayCommentDensity and arrayLOC which will be accessed by JSONParser/Combiner 
		public CustomParser(string filePath) {	
			
			string stringProbe;
			string breakSequence = "@#$";
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
						
						//if we reach the end of the page, reset all flags and counters, and add commentDensity and LOC to arrays
						if (stringProbe == breakSequence){
							inLineComment = false;
							inBlockComment = false;
							
							//calculation: commentDensity = (# of comments)/(# of comments + LOC) * 100
							commentDensity = (int) Math.Round (((double)commentCount)/(commentCount+locCounter)*100);
							arrayCommentDensity.Add(commentDensity);
							arrayLOC.Add(locCounter);
							
							locCounter = 0;
							commentCount = 0;
							
							if (sr.Peek () >= 0){
								stringProbe = sr.ReadLine();
								stringProbe = stringProbe.Trim();
							}
							
						}
						
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
			
		} //end of constructor
		
		public ArrayList getArrayLOC(){
			return arrayLOC;
		}
		
		public ArrayList getArrayCommentDensity(){
			return arrayCommentDensity;	
		}
		
		//Debug only.
		public int getCommentCount(){
			return commentCount;
		}
		
		//Debug only.
		public int getLOC(){
			return locCounter;
		}
		
		//Debug only.
		public int getCommentDensity(){
			return commentDensity;
		}
	}
}
