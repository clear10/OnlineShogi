using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class GameLogic : MonoBehaviour {

	UserInfo me;
	UserInfo first;
	UserInfo last;

	bool isTurnChanged;
	bool isInRoom;
	bool isGameFinish;
	bool isWinner;
	bool isRivalExit;

	public bool IsGameFinish { get { return isGameFinish; } }
	
	static GameLogic instance;
	
	//public DebugText dbg;
	
	public static GameLogic Instance {
		get {
			if(instance == null) {
				GameObject go = new GameObject("Logic");
				instance = go.AddComponent<GameLogic>();
			}
			return instance;
		}
	}

	void Awake() {
		if (instance == null)
			instance = this;
		else
			Destroy (this.gameObject);

		isTurnChanged = false;
		isWinner = true;
		isInRoom = false;
		isRivalExit = false;
		isGameFinish = false;

		DontDestroyOnLoad (this.gameObject);
	}
	
	IEnumerator WaitForPlayer() {
		int level = Application.loadedLevel;
		ShogiNetwork network = ShogiNetwork.Instance;
		while (me.GetState() != UserInfo.State.Playing) {
			network.GetRoomState(me.PlayId, ParseRoomState);
			yield return new WaitForSeconds(1f);
		}
		while (Application.loadedLevel == level)
			yield return null;
		
		GameStart ();
	}

	public void GoSceneLogin(string json) {
		//Debug.Log ("Logout: " + json);
		isInRoom = false;
		StopAllCoroutines ();
		instance = null;
		Application.LoadLevel ("Login");
		Destroy (this.gameObject);
	}

	public void GoSceneMain(string json) {
		Dictionary<string, object> dict = Json.Deserialize (json) as Dictionary<string, object>;
		Debug.Log (json);
		int userId = System.Convert.ToInt32 (dict ["user_id"]);
		int playId = System.Convert.ToInt32 (dict ["play_id"]);
		int state = GetStateInt (dict ["state"].ToString ());
		int role = (dict ["role"].ToString () == "player") ? 1 : 2;
		me = new UserInfo (userId, playId, state, role);

		isInRoom = true;
		Application.LoadLevel ("Main");
		if (state == 1) {
			StartCoroutine (WaitForPlayer ());
			return;
		}
		GameStart ();
	}

	public void GoSceneResult() {
		StopAllCoroutines ();
		GameObject transer = GameObject.Find ("DummyChanger");
		DummySceneTranser dummy = transer.GetComponent<DummySceneTranser> ();
		dummy.OnSceneTransition ();
		//Application.LoadLevel ("Result");
		//StartCoroutine (Wait4Logout ());
		StartCoroutine (Wait4Result ());
	}

	/**
	 * 
	IEnumerator Wait4Logout() {
		int level = Application.loadedLevel;
		while (Application.loadedLevel == level) {
			yield return null;
		}
		ResultText result = ResultText.Instance;
		result.Show (isWinner, isRivalExit);
		History history = History.Instance;
		Debug.Log (history.Dump ());
	}
	*
	*/

	IEnumerator Wait4Result() {
		yield return new WaitForSeconds (0.2f);
		ResultText result = ResultText.Instance;
		UserInfo user = null;
		if (isWinner)
			user = me;
		else {
			if(first.IsWon) user = first;
			if(last.IsWon) user = last;
		}
		result.Show (user, isRivalExit);
		History history = History.Instance;
		Debug.Log (history.Dump ());
	}

	void GameStart () {
		ShogiNetwork network = ShogiNetwork.Instance;
		network.GetUserInfo(me.PlayId, ParseUserInfo);
		StartCoroutine(GameLoopCoroutine());
	}

	IEnumerator GameLoopCoroutine () {
		while(first == null || last == null)
			yield return null;

		isGameFinish = false;
		isTurnChanged = true;
		ShogiNetwork network = ShogiNetwork.Instance;
		PieceController controller = PieceController.Instance;
		controller.Init ();
		History history = History.Instance;
		history.Init ();

		bool isPlayer = me.GetRole () == UserInfo.Role.Player;

		while(!IsGameFinish) {
			if(isPlayer) {
				if((!PromotionWindow.IsShowing) && isTurnChanged) {
					network.GetPiecesInfo(me.PlayId, ParsePieces);
					isTurnChanged = false;
				}
				network.GetBattleInfo(me.PlayId, ParseBattleInfo);
				yield return new WaitForSeconds(1f);
			} else {
				network.GetPiecesInfo(me.PlayId, ParsePieces);
				network.GetBattleInfo(me.PlayId, ParseBattleInfo);
				yield return new WaitForSeconds(1f);
			}
		}
	}

	void ParseRoomState (Dictionary<string, object> json) {
		string state = json["state"].ToString();
		//Debug.Log(state);
		int n = GetStateInt (state);
		if(n == 0) Debug.Log("state error");
		me.SetState(n);
	}

	int GetStateInt(string state) {
		switch(state) {
		case "waiting":
			return 1;
		case "playing":
			return 2;
		case "finish":
			return 3;
		case "exit":
			return 4;
		default:
			return 0;
		}
	}

	void ParseUserInfo (Dictionary<string, object> json) {
		Dictionary<string, object> info = (Dictionary<string, object>)json["first_player"];
		int id = System.Convert.ToInt32(info["user_id"]);
		string name = info ["name"].ToString ();
		const int PLAYER = 1;
		first = new UserInfo (id, me.PlayId, GetStateInt ("playing"), PLAYER);
		first.SetOrder (true);
		first.SetName (name);

		info = (Dictionary<string, object>)json["last_player"];
		id = System.Convert.ToInt32(info["user_id"]);
		name = info ["name"].ToString ();
		last = new UserInfo (id, me.PlayId, GetStateInt ("playing"), PLAYER);
		last.SetOrder (false);
		last.SetName (name);

		if (me.UserId == first.UserId)
			me = first;
		else if (me.UserId == last.UserId)
			me = last;
		else {
			Debug.Log("first: " + first.UserId + ", last: " + last.UserId + ", me: " + me.UserId);
			Debug.Log("You're watcher");
		}

		first.SetTurn(true);
	}

	void OnGameFinish () {
		if (!isInRoom)
			return;
		isGameFinish = true;
		ShogiNetwork.Instance.GetWinner (me.PlayId, ParseWinner);
		Debug.Log ("OnGameFinish");
	}

	void OnRivalExit () {
		if (!isInRoom)
			return;
		isGameFinish = true;
		isRivalExit = true;
		ShogiNetwork.Instance.GetWinner (me.PlayId, ParseWinner);
		Debug.Log ("OnRivalExit");
	}

	void ParseBattleInfo (Dictionary<string, object> json) {
		string state = json["state"].ToString();
		if(state == "finish") {
			OnGameFinish();
			return;
		}
		if(state == "exit") {
			OnRivalExit();
			return;
		}
		int id = System.Convert.ToInt32 (json ["turn_player"]);
		int turn = System.Convert.ToInt32 (json ["turn_count"]);
		int watcher = System.Convert.ToInt32 (json ["watcher_count"]);
		
		BattleInfoPanel panel = BattleInfoPanel.Instance;
		if (panel.Watcher < watcher) {
			panel.IncreaseWatcher();
			if(!NoticePanel.IsShowing)
				NoticePanel.Show();
		}
		if (id == GetTurnPlayer ().UserId) {
			if (!panel.Inited) {
				panel.SetPanel (GetTurnPlayer ().Name, turn);
				return;
			}
		}

		isTurnChanged = true;
		panel.SetPanel (GetTurnPlayer ().Name, turn);

		if(id == first.UserId) {
			first.SetTurn(true);
			last.SetTurn(false);
			return;
		}
		if(id == last.UserId) {
			first.SetTurn(false);
			last.SetTurn(true);
			return;
		}
	}

	void ParsePieces (Dictionary<string, object> json) {
		Dictionary<string, Dictionary<string, object>> dict = new Dictionary<string, Dictionary<string, object>> ();
		for (int i=1; i<=40; i++) {
			dict.Add(i.ToString(), (Dictionary<string, object>)json[i.ToString()]);
		}
		PieceController controller = PieceController.Instance;
		controller.UpdatePieces (dict);
	}

		
	void ParseWinner (Dictionary<string, object> json) {
		int id = System.Convert.ToInt32 (json ["winner"]);
		if (me.GetRole () == UserInfo.Role.Player) {
			if (me.UserId == id) {
				isWinner = true;
				me.Win();
			} else {
				isWinner = false;
				UserInfo user = (me.IsFirst) ? last : first;
				user.Win();
			}
		} else {
			if(first.UserId == id) {
				first.Win();
			} else if(last.UserId == id) {
				last.Win();
			}
		}
		GoSceneResult ();
	}

	bool IsMyTurn() {
		if (me.GetRole () == UserInfo.Role.Player) {
			if(me.IsTurn) {
				return true;
			}
		}
		return false;
	}

	public UserInfo GetTurnPlayer() {
		if (first.IsTurn)
			return first;
		if (last.IsTurn)
			return last;
		return null;
	}

	public UserInfo GetMe() {
		if (me == null)
			Debug.LogError ("me is null");
		return me;
	}

	public UserInfo GetFirstUser () {
		if(first == null)
			Debug.LogError("first is null");
		return first;
	}

	public UserInfo GetLastUser () {
		if(last == null)
			Debug.LogError("last is null");
		return last;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//debug
		//if (Input.GetKeyDown (KeyCode.Space)) {
		//	ShogiNetwork.Instance.LeaveRoom();
		//}
	}
}
