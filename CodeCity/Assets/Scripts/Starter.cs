using UnityEngine;
using System.Collections;

public class Starter : MonoBehaviour {

  public string repository = "";
  public string prompt = "Please enter a GitHub repository.\nThe format should be: <AUTHOR>/<REPOSITORY> ";
  public string loadingMessage = "loading...please be patient.";
  public bool loading = false;
  public GameObject decomposerObj;
  public GameObject builder;

    void OnGUI() {
        GUI.Label(new Rect (10, 0, 500, 40), prompt);
        repository = GUI.TextField(new Rect(10, 40, 310, 20), repository);

        if (loading) {
            GUI.Label(new Rect (10, 85, 500, 40), loadingMessage);
        }

        if (GUI.Button(new Rect(10,65,80,20), "SUBMIT")) {
            Destroy(decomposerObj); // Destroy existing objects
            Destroy(builder);
            decomposerObj = new GameObject("Decomposer");    // This is creating the decomposing GameObject.
            decomposerObj.AddComponent("Decomposer");  // This is adding the actual Decomposer.js to it.
            decomposerObj.SendMessage("verify", repository);
        }

        if (GUI.Button(new Rect(100,65,80,20), "CANCEL")) {
            Destroy(decomposerObj);
            Destroy(builder);
            loading = false;
        }
    }

    void InProgress() {
        Debug.Log("IN PROGRESS!");
        loading = true;
    }

    void Finished() {
        builder = new GameObject("Builder");
        builder.AddComponent("BuildingHeight");
        loading = false;
    }
}
