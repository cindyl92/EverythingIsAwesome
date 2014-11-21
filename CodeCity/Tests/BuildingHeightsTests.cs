//using System;
//using System.Collections.Generic;
//using System.Threading;
//using NUnit.Framework;
//using UnityEngine;
//using AssemblyCSharpvs;
//using System.Collections;
//using NSubstitute;
//
//namespace UnityTest
//{
//	//This file is a Unit Test to test methods in the BuildingHeights.cs class
//	//It requires installing the UnityTestTools Framework to run
//	[TestFixture]
//	internal class BuildingHeightsTests
//	{
//		
//		string[,] mockBuildingData = new string[,] 
//			{{"ClassA", "ClassC", "1", "123", "10", "PackageA"} ,
//			{"ClassB", "ClassD","2",  "350", "50", "PackageA"}  ,
//			{"ClassC", "ClassA","4",  "105", "90", "PackageA"}  ,
//			{"ClassD", "ClassB","3",  "72", "30", "PackageA"}  ,
//			{"ClassE", "ClassF","1",  "100", "55", "PackageA"}  ,
//			{"ClassF", "no coupling","0",  "33", "50", "PackageA"}  ,
//			{"ClassG", "no coupling","0",  "112", "5", "PackageA"}}  ;
//		
//		
//		//Testing the setPositions method in BuildingHeights.cs
//		[Test]
//		public void TestSetPositions() {
//			
//			BuildingHeight visualizer = Substitute.For<BuildingHeight> ();
//			visualizer.javaClasses = mockBuildingData;
//			visualizer.setPositions (mockBuildingData);
//			
//			int sizeOfPositionsArray = visualizer.positions.GetLength (0);
//			
//			float[,] positions = visualizer.positions;
//			
//			Assert.AreEqual (sizeOfPositionsArray, 7);
//
//			//positions is a 2D array with the first element as the x-coordinate and second element as the y-coordinate
//			//Class A
//			Assert.AreEqual (positions [0, 0], 0);
//			Assert.AreEqual (positions [0, 1], 0);
//			
//			//Class B
//			Assert.AreEqual (positions [1, 0], 0);
//			Assert.AreEqual (positions [1, 1], 3);
//			
//			//Class C
//			Assert.AreEqual (positions [2, 0], 0);
//			Assert.AreEqual (positions [2, 1], 6);
//			
//			//Class D
//			Assert.AreEqual (positions [3, 0], 3);
//			Assert.AreEqual (positions [3, 1], 0);
//			
//			//Class E
//			Assert.AreEqual (positions [4, 0], 6);
//			Assert.AreEqual (positions [4, 1], 3);
//			
//			//Class F
//			Assert.AreEqual (positions [5, 0], 6);
//			Assert.AreEqual (positions [5, 1], 3);
//			
//			//Class G
//			Assert.AreEqual (positions [6, 0], 6);
//			Assert.AreEqual (positions [6, 1], 6);
//			
//		}
//		
//
//		//Testing the findCouplings method in BuildingHeights.cs
//		[Test]
//		public void TestFindCoupling() {
//			
//			BuildingHeight visualizer = Substitute.For<BuildingHeight> ();
//			visualizer.javaClasses = mockBuildingData;
//			visualizer.findCouplings (mockBuildingData);
//
//			//couplings is a 2D array that stores the x and z coordinates of the class its coupled with, or -1,-1 if theres no coupling
//			int[,] couplings = visualizer.couplings;
//			
//			Assert.AreEqual (couplings.GetLength(0), 7);
//			
//			//Class A coupled with C
//			Assert.AreEqual (couplings [0, 0], 0);
//			Assert.AreEqual (couplings [0, 1], 6);
//			
//			//Class B coupled with D
//			Assert.AreEqual (couplings [1, 0], 3);
//			Assert.AreEqual (couplings [1, 1], 0);
//			
//			//Class C coupled with A
//			Assert.AreEqual (couplings [2, 0], 0);
//			Assert.AreEqual (couplings [2, 1], 0);
//			
//			//Class D coupled with B
//			Assert.AreEqual (couplings [3, 0], 0);
//			Assert.AreEqual (couplings [3, 1], 3);
//			
//			//Class E coupled with C
//			Assert.AreEqual (couplings [4, 0], 0);
//			Assert.AreEqual (couplings [4, 1], 6);
//			
//			//Class F has no coupling
//			Assert.AreEqual (couplings [5, 0], -1);
//			Assert.AreEqual (couplings [5, 1], -1);
//			
//			//Class G has no coupling
//			Assert.AreEqual (couplings [6, 0], -1);
//			Assert.AreEqual (couplings [6, 1], -1);
//			
//		}
//
//		//Testing the setHeights method in BuildingHeights.cs
//		[Test]
//		public void TestSetHeights() {
//			
//			BuildingHeight visualizer = Substitute.For<BuildingHeight> ();
//			visualizer.javaClasses = mockBuildingData;
//			
//			visualizer.setHeights (0);
//			visualizer.setHeights (1);
//			visualizer.setHeights (2);
//			visualizer.setHeights (3);
//			visualizer.setHeights (4);
//			visualizer.setHeights (5);
//			visualizer.setHeights (6);
//			int sizeOfHeightsArray = (int)visualizer.heights.GetLength(0);
//
//			//We divide the lines of code count by 10 if its less than 300 or divide by 500 and add 30, to prevent huge disparities in rendering
//			float[] heights = visualizer.heights;
//			
//			Assert.AreEqual (sizeOfHeightsArray, 7);
//			
//			Assert.AreEqual (heights[0], 12.3);
//			Assert.AreEqual (heights[1], 30.7);
//			Assert.AreEqual (heights[2], 10.5);
//			Assert.AreEqual (heights[3], 7.2);
//			Assert.AreEqual (heights[4], 10);
//			Assert.AreEqual (heights[5], 3.3);
//			Assert.AreEqual (heights[6], 11.2);
//			
//		}
//
//		//Testing the setColours method in BuildingHeights.cs
//		[Test]
//		public void TestSetColors() {
//			
//			BuildingHeight visualizer = Substitute.For<BuildingHeight> ();
//
//			visualizer.javaClasses = mockBuildingData;
//			visualizer.setColours (0);
//			visualizer.setColours (1);
//			visualizer.setColours (2);
//			visualizer.setColours (3);
//			visualizer.setColours (4);
//			visualizer.setColours (5);
//			visualizer.setColours (6);
//			
//			int sizeOfColoursArray = (int)visualizer.colours.GetLength(0);
//
//			//The color is white if the comment density is less than 30%, blue if its 30-60% and black if its over 60%
//			Color[] colours = visualizer.colours;
//			
//			Assert.AreEqual (sizeOfColoursArray, 7);
//			
//			Assert.AreEqual (colours[0], Color.white);
//			Assert.AreEqual (colours[1], Color.blue);
//			Assert.AreEqual (colours[2], Color.black);
//			Assert.AreEqual (colours[3], Color.blue);
//			Assert.AreEqual (colours[4], Color.blue);
//			Assert.AreEqual (colours[5], Color.blue);
//			Assert.AreEqual (colours[6], Color.white);
//			
//			
//		}
//	}
//}
//
