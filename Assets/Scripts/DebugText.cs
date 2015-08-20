using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugText : MonoBehaviour {

	Text txt;

	public int lineMax = 9;

	// Use this for initialization
	void Start () {
		txt = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Log(object o) {
		/**
		string[] lines = (txt.text).Split ('\n');
		if (lines.Length + 1 > lineMax) {
			txt.text = "";
			for(int i=1; i<lines.Length; i++) {
				txt.text += lines[i] + '\n';
			}
		}
		string s = o.ToString();
		txt.text += s;
		Debug.Log (s);
		**/

		txt.text = o.ToString ();
		Debug.Log (o);
	}
}
