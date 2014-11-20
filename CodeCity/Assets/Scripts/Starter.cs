using UnityEngine;
using System.Collections;

public class Starter : MonoBehaviour {

  public string repository = "";
  public string prompt = "Please enter a GitHub repository.\nThe format should be: <AUTHOR>/<REPOSITORY> ";

    void OnGUI() {
        GUI.Label(new Rect (10, 0, 500, 40), prompt);
        repository = GUI.TextField(new Rect(10, 40, 310, 20), repository, 25);

        if (GUI.Button(new Rect(10,65,80,20), "SUBMIT")) {
            GameObject decomposerObj = new GameObject("Decomposer");    // This is creating the decomposing GameObject.
            var decomposer = decomposerObj.AddComponent("Decomposer");  // This is adding the actual Decomposer.js to it.
            decomposerObj.SendMessage("verify", repository);
        }

        if (GUI.Button(new Rect(100,65,80,20), "CANCEL")) {
            GameObject decomposerObj = GameObject.Find("Decomposer");
            Destroy(decomposerObj);
        }
    }
}
