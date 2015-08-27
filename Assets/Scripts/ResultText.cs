using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultText : MonoBehaviour {

	[SerializeField] Text text;

	static ResultText instance;
	
	public static ResultText Instance {
		get {
			if(instance == null) {
				GameObject go = new GameObject("result");
				instance = go.AddComponent<ResultText>();
			}
			return instance;
		}
	}

	void Awake() {
		if (instance == null)
			instance = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Show(bool isWinner, bool isRivalExit) {
		string s = "";
		if(isRivalExit)
			s = "対戦相手が退出しました\n";
		s += (isWinner) ? "あなたの勝利です" : "あなたの負けです";

		text.text = s;
	}
}
