using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DummySceneTranser : MonoBehaviour {

	public List<TransitionAt> list;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnSceneTransition() {
		StartCoroutine (TransAnimation (1f));
	}

	IEnumerator TransAnimation(float t) {
		int frame = 60;
		List<Vector3> lps = new List<Vector3>();
		List<Vector3> lss = new List<Vector3>();
		foreach (TransitionAt at in list) {
			Transform trans = at.transform;
			lps.Add(trans.localPosition);
			lss.Add(trans.localScale);
		}
		
		GameObject canvas = GameObject.Find ("Canvas");
		GameObject panel = Resources.Load<GameObject> ("ResultPanel");
		panel = Instantiate (panel) as GameObject;
		panel.transform.SetParent (canvas.transform, false);
		Vector3 from = new Vector3 (1200, 0, 0);
		panel.transform.localPosition = from;

		for (int i = 1; i<=frame; i++) {
			panel.transform.localPosition = Vector3.Lerp(from, Vector3.zero, (float)i/(float)frame);
			yield return new WaitForSeconds(t/(float)frame);
		}

		for (int i = 1; i<=frame; i++) {
			int j = 0;
			foreach (TransitionAt at in list) {
				Transform trans = at.transform;
				trans.localPosition = Vector3.Lerp(lps[j], (Vector3)at.atPosition, (float)i/(float)frame);
				trans.localScale = Vector3.Lerp(lss[j], at.atScale, (float)i/(float)frame);
				j++;
			}
			yield return new WaitForSeconds(t/(float)frame);
		}

		GameObject prefab = Resources.Load<GameObject> ("ScrollView");
		GameObject view = Instantiate (prefab) as GameObject;
		view.transform.SetParent (canvas.transform, false);
		ScrollViewController controller = view.GetComponent<ScrollViewController> ();
		controller.StartController ();

		Destroy (panel);

	}
}
