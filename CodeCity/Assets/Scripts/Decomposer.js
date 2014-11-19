#pragma strict
import SimpleJSON;
import System.IO;

var repo = "RobotiumTech/Robotium";
// var repo = "psaravan/JamsMusicPlayer";
var token = "access_token=9c972df855af8fb6245a3182a48787c4d81279b8";
var url = "https://api.github.com/repos/"+ repo + "/contents?";
var branch = "";
var javas = new Array();

function Start () {
  yield StartCoroutine(determineBranch());
  yield StartCoroutine(search(url + token));
  writeFiles();
}

function determineBranch() {
  var www = new WWW("https://api.github.com/repos/" + repo + "/branches");
  yield www;
  var response = www.text;
  var firstBranch = JSON.Parse(response)[0];
  branch = "/" + firstBranch["name"] + "/";
  Debug.Log("BRANCH: " + branch);
}

function search(currentPath) : IEnumerator {
  Debug.Log("WORKING ON: " + currentPath);
  var www = new WWW(currentPath);
  yield www;
  var response = www.text;
  Debug.Log("RESPONSE: " + response);
  var JSONResponse = JSON.Parse(response);

  for (var i = 0; i < JSONResponse.Count; i++) {
    var object = JSONResponse[i];

    if ((object["type"].Value == "file") && object["path"].Value.Contains(".java")) {
      // OBJECT IS A JAVA FILE, SAVE IT;
      javas.Push(object["path"].Value);
    }
    
    if (object["type"].Value == "dir") {
      // OBJECT IS A DIR, DELVE IN WITH RECURSIVE CALL
      yield search(object["_links"]["self"].Value + "&" + token);
    }
    // we don't do anything for the rest of the file types
  }
}

function writeFiles() {
  Debug.Log("WRITING START");
  var pathWriter : StreamWriter = new StreamWriter("Temp/javaPaths.txt");
  var textWriter : StreamWriter = new StreamWriter("Temp/javaText.txt");

  for (var file = 0; file < javas.length; file++) {
    pathWriter.WriteLine("https://github.com/" + repo + "/blob" + branch + javas[file]);
    Debug.Log(javas[file]);
    var rawWWW = new WWW("https://raw.githubusercontent.com/" + repo + branch + javas[file]);
    yield rawWWW;
    textWriter.WriteLine(rawWWW.text);
    textWriter.WriteLine("@#$");
  }

  pathWriter.Flush();
  textWriter.Flush();
  pathWriter.Close();
  textWriter.Close();
  Debug.Log("WRITING END");
}

function Update () {

}