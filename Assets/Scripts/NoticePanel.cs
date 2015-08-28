using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NoticePanel : MonoBehaviour {

	public static bool IsShowing{ get { return (instance != null); } }
	//public float amplitude = 3f;
	
	static NoticePanel instance = null;
	
	public static NoticePanel Show() {
		if (instance != null)
			return null;
		
		Debug.Log ("NoticePanel");
		
		Transform canvas = GameObject.Find ("Canvas").transform;
		Transform info = canvas.FindChild ("BattleInfoPanel");
		GameObject prefab = Resources.Load<GameObject> ("NoticePanel");
		
		GameObject go = Instantiate (prefab) as GameObject;
		go.transform.SetParent (info, false);
		go.transform.localPosition = new Vector3 (-33, -79, 0);
		instance = go.GetComponent<NoticePanel> ();
		return instance;
	}

	IEnumerator DestroyAfterSeconds(float t) {
		yield return new WaitForSeconds (t-1f);
		StartClearAnimation ();
		//Debug.Log ("2s tatta");

		//instance = null;
		//Destroy (this.gameObject);
	}

	void StartClearAnimation() {
		StartCoroutine (ClearAnimation (1f));
	}

	IEnumerator ClearAnimation(float t) {
		int frame = 30 * (int)t;
		Image target1 = GetComponent<Image> ();
		Text target2 = transform.FindChild("Text").GetComponent<Text> ();
		Image target3 = transform.FindChild ("Image").GetChild (0).GetComponent<Image> ();
		//Color c = null;

		float f1 = target1.color.a;
		float f2 = target2.color.a;
		float f3 = target3.color.a;

		for(int i=1; i<=frame; i++) {
			float a = Mathf.Lerp(f1, 0, (float)i/(float)frame);
			Color c = target1.color;
			target1.color = new Color(c.r, c.g, c.g, a);
			c = target2.color;
			a = Mathf.Lerp(f2, 0, (float)i/(float)frame);
			target2.color = new Color(c.r, c.g, c.g, a);
			c = target3.color;
			a = Mathf.Lerp(f3, 0, (float)i/(float)frame);
			target3.color = new Color(c.r, c.g, c.g, a);

			yield return new WaitForSeconds(t/(float)frame);
		}

		DestroyPanel ();
	}

	void DestroyPanel() {
		instance = null;
		Destroy (this.gameObject);
	}

	// Use this for initialization
	void Start () {
		//Debug.Log ("Start");
		StartCoroutine (DestroyAfterSeconds (3f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
