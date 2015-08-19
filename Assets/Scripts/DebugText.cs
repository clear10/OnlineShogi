using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugText : MonoBehaviour {

	Text txt;

	// Use this for initialization
	void Start () {
		txt = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Log(object o) {
		string s = System.Convert.ToString (o);
		txt.text += "\n" + s;
		Debug.Log (s);
	}
}
