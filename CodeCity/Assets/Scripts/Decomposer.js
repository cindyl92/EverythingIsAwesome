#pragma strict
import SimpleJSON;

var repo = "RobotiumTech/Robotium";
var token = "access_token=9c972df855af8fb6245a3182a48787c4d81279b8";
var url = "https://api.github.com/repos/"+ repo + "contents/?";
var dirsToExplore = new Array();
dirsToExplore.Add(url);
var exploredDirs = new Array();
var javas = new Array();

function Start () {
	yield StartCoroutine(search(url + token));
}

function search(currentPath) : IEnumerator {
	Debug.Log(currentPath);
	var www = new WWW(currentPath);
	yield www;
	  
	var response = www.text;
	// Debug.Log(response);
	
	var JSONResponse = JSON.Parse(response);
	//Debug.Log(JSONResponse);
	// Debug.Log(JSONResponse);
	var count = JSONResponse.Count;
	for (var i = 0; i < count; i++) {
		var endpoint = JSONResponse[i];
		// Debug.Log(endpoint["name"].Value);
		if ((endpoint["type"].Value == "file") && (endpoint["name"].Value.Contains("java"))) {
        	javas.Push(endpoint["path"].Value);
        	// Debug.Log("Found a java file in: " + endpoint["name"].Value);
      	}
      	if (endpoint["type"].Value == "dir") {
        	dirsToExplore.Push(endpoint["_links"]["self"].Value);
        	// Debug.Log("found a directory in: " + endpoint["name"].Value);
      	}	
	}
	for (var j = 0; j < dirsToExplore.length; j++){
		var dir = dirsToExplore[j];
		var call = true;
		for (var k = 0; k < exploredDirs.length; k++) {
			if (exploredDirs[k] == dir) call = false;
		}
	    if (call) {
	    	// Debug.Log("recursive call on: " + dir);
	    	dirsToExplore.Pop();
	    	exploredDirs.push(dir);
	    	// Debug.Log(dir);
	    	yield search(dir + "&" + token);
	    }
	}
}

function Update () {

}