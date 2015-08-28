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

	/**
	 * 
	public void Show(bool isWinner, bool isRivalExit) {
		string s = "";
		UserInfo me = GameLogic.Instance.GetMe ();
		bool isPlayer = me.GetRole () == UserInfo.Role.Player;
		if(isRivalExit)
			s = "対戦相手が退出しました\n";
		s += (isPlayer) ? "あなた" : "";
		s += (isWinner) ? "の勝利です" : "の負けです";

		text.text = s;
	}
	*
	*/

	public void Show(UserInfo user, bool isRivalExit) {
		string s = "";
		if (user == null) {
			Debug.LogError("usr null");
		}
		UserInfo me = GameLogic.Instance.GetMe ();
		bool isPlayer = me.GetRole () == UserInfo.Role.Player;
		bool isWinner = me == user;
		if(isRivalExit)
			s = "対戦相手が退出しました\n";
		if (isPlayer) {
			s += "あなた";
		} else {
			s += (user.IsFirst) ? "先手" : "後手";
		}
		if(user.Name != null && user.Name != "")
			s += ("(" + user.Name + ")");
		s += (isWinner) ? "の勝利です" : "の負けです";
		
		text.text = s;
		Debug.Log (s);
	}
}
