// #pragma strict
import SimpleJSON;
import System.IO;

var repo = "";
var url = "";
var token = "access_token=9c972df855af8fb6245a3182a48787c4d81279b8"; // This is Jonathan's API access token.
var branch = "";
var javas = new Array();

// A pseudo-constructor/init for the decomposer script. 
// This script will self-destruct if we were given an empty repository name.
function verify(repository) {
  repo = repository;
  url = "https://api.github.com/repos/"+ repo + "/contents?";

  if ((repo != "") && (url != "")) {
    Debug.Log("REPOSITORY: " + repo);
    yield StartCoroutine(determineBranch());
    yield decompose();
  } else {
    Destroy(this.gameObject);
    this.enabled = false;
  }
}

// Determines the current branch name of the repository.
// This script will self-destruct if we were unable to determine a branch name (probably because of a bad repository name)
function determineBranch() {

  // Makes a GitHub API call to determine branch name
  var www = new WWW("https://api.github.com/repos/" + repo + "/branches"); 
  yield www;

  if (!String.IsNullOrEmpty(www.error)) {
    Debug.Log("ERROR, CANNOT CONNECT TO REPO: " + repo);
    Destroy(this.gameObject);
  } else {
    var response = www.text;
    var firstBranch = JSON.Parse(response)[0]; // We use the most recently committed branch for the codebase.
    branch = "/" + firstBranch["name"] + "/";
    Debug.Log("BRANCH: " + branch);
  }
}

// Starts a coroutine to begin searching with the repository URL.
// Writes to files only when the decomposition is complete.
function decompose () {
  var starter : Component;
  var camera = GameObject.Find("Main Camera");
  starter = camera.GetComponent("Starter");
  starter.SendMessage("InProgress");
  yield StartCoroutine(search(url + token));
  writeFiles();
}

// The core recursive decomposition function. It's a depth-first search algorithm, which means the output is grouped by packages!
// param: currentPath is the API compatible path of the current directory we're looking at. The first time it's called, it should be the repository's root directory URL.

function search(currentPath) : IEnumerator {
  // Makes an HTTP request for the /contents of the current path.
  var www = new WWW(currentPath);
  yield www;
  var response = www.text;
  Debug.Log("RESPONSE: " + response);

  // Parse the HTTP response as a JSON array. Each object of the array is a JSON representation of a file/directory.
  var JSONResponse = JSON.Parse(response);

  // Now we're looking through the contents for .java files or directories.
  for (var i = 0; i < JSONResponse.Count; i++) {
    var object = JSONResponse[i];

    // Checks if it's a Java file.
    if ((object["type"].Value == "file") && object["path"].Value.Contains(".java")) {
      javas.Push(object["path"].Value);
    }
    
    // Checks if it's a directory. If it is, we recursively call on it.
    if (object["type"].Value == "dir") {
      yield search(object["_links"]["self"].Value + "&" + token);
    }

    // we don't do anything for the rest of the file types.
  }

}

// The writing function, it simply translates the data that we read from the search function into text files.
// Note: We opted to use text files because (especially for larger repos), the data would be too large to store/pass as variables. 
//       The Unity system prefers that we just use temporary .txt files instead.
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
  wrapUp();
}

// This is just how we let Starter.cs know that we're done the entire decomposition process. 
function wrapUp() {
  var starter : Component;
  var camera = GameObject.Find("Main Camera");
  starter = camera.GetComponent("Starter");
  starter.SendMessage("Finished");
}