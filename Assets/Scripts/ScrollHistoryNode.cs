using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollHistoryNode : MonoBehaviour {

	int index;
	//HistoryInfo info;

	// Use this for initialization
	void Start () {
		Button button = GetComponent<Button> ();
		button.onClick.AddListener (() => SendIndex2Controller ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetIndex(int i) {
		index = i;
	}
	
	public void SetText(string txt) {
		transform.GetChild (0).GetComponent<Text> ().text = txt;
	}

	public void SendIndex2Controller() {
		ScrollViewController sc = transform.parent.parent.GetComponent<ScrollViewController> ();
		sc.ReproduceState (this.index);
	}
}
