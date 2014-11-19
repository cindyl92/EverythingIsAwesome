using UnityEngine;
using System.Collections;

public class Starter : MonoBehaviour {

  public string repository = "";

    void OnGUI() {
        repository = GUI.TextField(new Rect(10, 10, 200, 20), repository, 25);

        if (GUI.Button(new Rect(20,40,80,20), "SUBMIT")) {
            GameObject decomposerObj = new GameObject("Decomposer");    // This is creating the decomposing GameObject.
            var decomposer = decomposerObj.AddComponent("Decomposer");  // This is adding the actual Decomposer.js to it.
            decomposerObj.SendMessage("verify", repository);
        }

        if (GUI.Button(new Rect(110,40,80,20), "CANCEL")) {
            GameObject decomposerObj = GameObject.Find("Decomposer");
            Destroy(decomposerObj);
        }
    }
}
