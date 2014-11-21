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
//	internal class JSONParserTests
//	{
//
//		ArrayList mockFilePaths = new ArrayList {
//			"https://github.com/RobotiumTech/robotium/blob/master/robotium-solo/src/main/java/com/robotium/solo/ActivityUtils.java",
//			"https://github.com/RobotiumTech/robotium/blob/master/robotium-solo/src/main/java/com/robotium/solo/Asserter.java",
//			"https://github.com/RobotiumTech/robotium/blob/master/robotium-solo/src/main/java/com/robotium/solo/By.java",
//			"https://github.com/RobotiumTech/robotium/blob/master/robotium-solo/src/main/java/com/robotium/solo/Checker.java",
//			"https://github.com/RobotiumTech/robotium/blob/master/robotium-solo/src/main/java/com/robotium/solo/Clicker.java",
//			"https://github.com/RobotiumTech/robotium/blob/master/robotium-solo/src/main/java/com/robotium/solo/Condition.java",
//			"https://github.com/RobotiumTech/robotium/blob/master/robotium-solo/src/main/java/com/robotium/solo/DialogUtils.java",
//			"https://github.com/RobotiumTech/robotium/blob/master/robotium-solo/src/main/java/com/robotium/solo/GLRenderWrapper.java"} ;
//		
//		
//		ArrayList mockClassNames = new ArrayList{"ActivityUtils", "Asserter", "By", "Checker", "Clicker", "Condition", "DialogUtils", "GLRenderWrapper"} ;
//
//
//				//This tests the JSONRequester with a single API call and checks the length of the returned JSONString for verification
//				[Test]
//				[Category("Japarser API Tests")]
//				public void TestJSONRequesterSingleCall() {
//					
//					var parser = new JSONParser();
//					string filePath = (string) mockFilePaths [0];
//					string jsonData = parser.JSONRequester (filePath);
//		
//					Assert.AreEqual (4731, jsonData.Length);
//		
//				}
//
//				//This tests the JSONRequester with a list of API calls and checks the length of the returned JSONStrings for verification
//				[Test]
//				[Category("Japarser API Tests")]
//				public void TestJSONRequesterMultiCall() {
//					
//					var parser = new JSONParser();
//					ArrayList results = new ArrayList ();
//					for (int i = 0; i < mockFilePaths.Count; i++) {
//						string jsonData = parser.JSONRequester((string)mockFilePaths[i]);
//		
//						results.Add(jsonData.Length);
//					}
//		
//					Assert.AreEqual (8, results.Count);
//					Assert.AreEqual (4731, results[0]);
//					Assert.AreEqual (1193, results [1]);
//					Assert.AreEqual (1157, results [2]);
//					Assert.AreEqual (1099, results [3]);
//					Assert.AreEqual (4100, results [4]);
//					Assert.AreEqual (236, results [5]);
//					Assert.AreEqual (1502, results [6]);
//					Assert.AreEqual (1962, results [7]);
//					
//				}
//
//				//This tests the JSONRequester with an invalid filePath and returns an exception
//				[Test]
//				[Category("Japarser API Tests")]
//				public void TestJSONRequesterException() {
//					
//					var parser = new JSONParser();
//		
//					string result = parser.JSONRequester ("invalidFilePath");
//					Assert.AreEqual ("Japarser Exception", result);
//					
//				}
//
//				//This tests the parseforCoupling method in the case that there is no coupling
//				[Test]
//				[Category("Coupling Parser Tests")]
//				public void TestNoCoupling() {
//					
//					var parser = new JSONParser();
//		
//					string jsonData = parser.JSONRequester ((string)mockFilePaths [0]);
//		
//					ArrayList results = parser.parseforCoupling (mockClassNames, jsonData, 392, 25);
//		
//					Assert.AreEqual ("ActivityUtils", results [0]);
//					Assert.AreEqual ("no coupling", results [1]);
//					Assert.AreEqual (0, results [2]);
//		
//		
//				}
//
//				//This tests the parseforCoupling method with one class and there is coupling
//				[Test]
//				[Category("Coupling Parser Tests")]
//				public void TestSingleCoupling() {
//					
//					var parser = new JSONParser();
//					
//					string jsonData = parser.JSONRequester ((string)mockFilePaths [6]);
//					
//					ArrayList results = parser.parseforCoupling (mockClassNames, jsonData, 392, 25);
//					
//					Assert.AreEqual ("DialogUtils", results [0]);
//					Assert.AreEqual ("ActivityUtils", results [1]);
//					Assert.AreEqual (1, results [2]);
//		
//				}
//
//				//This tests the parseforCoupling method with multiple classes, some with coupling and some without
//				[Test]
//				[Category("Coupling Parser Tests")]
//				public void TestMultiCoupling() {
//					
//					var parser = new JSONParser();
//					var allResults = new ArrayList ();
//		
//					for (int i = 0; i< mockFilePaths.Count; i++) {
//						string jsonData = parser.JSONRequester((string)mockFilePaths[i]);
//						ArrayList results = parser.parseforCoupling (mockClassNames, jsonData, 100, 100);
//						allResults.Add (results);
//					}
//		
//					ArrayList result0 = (ArrayList) allResults [0];
//					ArrayList result1 = (ArrayList) allResults [1];
//					ArrayList result2 = (ArrayList) allResults [2];
//					ArrayList result3 = (ArrayList) allResults [3];
//					ArrayList result4 = (ArrayList) allResults [4];
//					ArrayList result5 = (ArrayList) allResults [5];
//					ArrayList result6 = (ArrayList) allResults [6];
//					ArrayList result7 = (ArrayList) allResults [7];
//		
//					Assert.AreEqual (8, allResults.Count);
//		
//					Assert.AreEqual (result0 [0], "ActivityUtils");
//					Assert.AreEqual (result0 [1], "no coupling");
//		
//					Assert.AreEqual (result1 [0], "Asserter");
//					Assert.AreEqual (result1 [1], "ActivityUtils");
//		
//					Assert.AreEqual (result2 [0], "By");
//					Assert.AreEqual (result2 [1], "no coupling");
//		
//					Assert.AreEqual (result3 [0], "Checker");
//					Assert.AreEqual (result3 [1], "no coupling");
//		
//					Assert.AreEqual (result4 [0], "Clicker");
//					Assert.AreEqual (result4 [1], "ActivityUtils");
//		
//					Assert.AreEqual (result5 [0], "Condition");
//					Assert.AreEqual (result5 [1], "no coupling");
//		
//					Assert.AreEqual (result6 [0], "DialogUtils");
//					Assert.AreEqual (result6 [1], "ActivityUtils");
//		
//					Assert.AreEqual (result7 [0], "GLRenderWrapper");
//					Assert.AreEqual (result7 [1], "no coupling");
//		
//				}
//
//				//This tests the parseforCoupling when there is an invalid file path, which will output an error
//				[Test]
//				[Category("Coupling Parser Tests")]
//				public void TestCouplingException() {
//					
//					var parser = new JSONParser();
//		
//					string jsonData = parser.JSONRequester ("invalidFilePath");
//					ArrayList results = parser.parseforCoupling (mockClassNames, jsonData, 100, 100);
//					
//					Assert.AreEqual (results [0], "error");
//					
//				}
//		
//
//				//This tests the getAllClassNames method
//				[Test]
//				[Category("Class Retrieval Tests")]
//				public void TestGetClassNames() {
//					
//					var parser = new JSONParser();
//					
//					ArrayList result = parser.getAllClassNames (mockFilePaths);
//					
//					Assert.AreEqual (8, result.Count);
//					
//					Assert.AreEqual (result[0], "ActivityUtils");
//					Assert.AreEqual (result[1], "Asserter");
//					Assert.AreEqual (result[2], "By");
//					Assert.AreEqual (result[3], "Checker");
//					Assert.AreEqual (result[4], "Clicker");
//					Assert.AreEqual (result[5], "Condition");
//					Assert.AreEqual (result[6], "DialogUtils");
//					Assert.AreEqual (result[7], "GLRenderWrapper");
//		
//					
//				}
//	}
//}