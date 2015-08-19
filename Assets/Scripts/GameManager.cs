using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class GameManager : MonoBehaviour {

	static GameManager instance;

	public static GameManager Instance {
		get {
			if(instance == null) {
				GameObject go = new GameObject("GameManager");
				instance = go.AddComponent<GameManager>();
			}
			return instance;
		}
	}

	[SerializeField] InputField nameTxt;
	[SerializeField] InputField roomTxt;
	[SerializeField] Button button;

	[SerializeField] DebugText dbg;
	[SerializeField] RectTransform banRect;
	[SerializeField] GameObject piecePrefab;

	const string server = "http://192.168.3.83:3000/";

	public PlayerInfo player;
	public PlayerInfo rival;
	List<Piece> pieces;
	//string callback = "";

	void Awake() {
		if (instance != null) {
			Destroy (this.gameObject);
			return;
		}

		instance = this;
		Init ();
		DontDestroyOnLoad (instance.gameObject);
	}

	void Init() {
		Transform canvas = GameObject.Find ("Canvas").transform;
		nameTxt = canvas.FindChild ("NameField").GetComponent<InputField> ();
		roomTxt = canvas.FindChild ("RoomField").GetComponent<InputField> ();
		button = canvas.FindChild ("Button").GetComponent<Button> ();
		button.onClick.RemoveAllListeners ();
		button.onClick.AddListener (() => instance.JoinRoom ());
		dbg = canvas.FindChild ("debugLog").GetComponent<DebugText> ();
		if(piecePrefab == null)
			piecePrefab = Resources.Load<GameObject> ("piece");
	}

	// Use this for initialization
	void Start () {
		player = new PlayerInfo ();
		rival = new PlayerInfo ();
		pieces = new List<Piece> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			//ParsePieces(callback);
			LeaveRoom();
		}
	}

	public void JoinRoom() {
		StartCoroutine (LogIn ());
	}
	
	public void LeaveRoom() {
		StopAllCoroutines ();
		StartCoroutine (LogOut ());
	}

	IEnumerator LogIn() {
		string req = "users/login";

		string name;
		int room;

		name = nameTxt.text;
		int.TryParse (roomTxt.text, out room);

		WWWForm form = new WWWForm ();
		form.AddField ("name", name);
		form.AddField ("room_no", room);

		WWW www = new WWW (server + req, form);
		yield return www;

		//Debug.Log (www.text);
		SetPlayerInfo (www.text);

		yield return new WaitForSeconds (0.1f);
		GoSceneMain ();
	}
	
	IEnumerator LogOut() {
		string req = "users/logout";

		int playId = player.PlayId;
		int userId = player.UserId;
		if (playId == -1 || userId == -1)
			yield break;
		WWWForm form = new WWWForm ();
		form.AddField ("play_id", playId);
		form.AddField ("user_id", userId);
		WWW www = new WWW (server + req, form);
		yield return www;
		
		//Debug.Log (www.text);
		if (www.text == "[\"true\"]") {
			Debug.Log("Succesfully Logouted!");
			string s = Application.loadedLevelName;
			Application.LoadLevel("Login");
			while(Application.loadedLevelName == s) yield return null;

			Init();
		}
	}

	void SetPlayerInfo(string jsonText) {
		var json = Json.Deserialize (jsonText) as Dictionary<string, object>;

		int userId = System.Convert.ToInt32 (json ["user_id"]); //(int)json ["user_id"];
		int playId = System.Convert.ToInt32 (json ["play_id"]); //(int)json ["play_id"];
		string stateTxt = System.Convert.ToString(json ["state"]);
		string roleTxt = System.Convert.ToString(json ["role"]);

		int state = (stateTxt == "playing") ? 2 : 1;
		int role = (roleTxt == "watcher") ? 2 : 1;

		player = new PlayerInfo (userId, playId, state, role);
		Debug.Log (player.Dump ());
	}

	void GoSceneMain() {
		Application.LoadLevel ("Main");
		GetRoomState ();
		StartCoroutine (WaitForPlayer ());
		//GetUserInfo ();
		//GetBattleInfo ();
		//GetWinner ();
		//GetPiecesInfo ();
	}

	void GameStart() {
		GetUserInfo ();
		StartCoroutine (GameLoopCoroutine ());
	}

	IEnumerator GameLoopCoroutine() {
		while (rival == null)
			yield return null;

		while (!isGameFinish) {
			GetBattleInfo();
			yield return new WaitForSeconds(5f);
			// TODO
		}
	}

	bool isGameFinish{ get { return false; } }

	void OnApplicationQuit() {
		LeaveRoom ();
	}
	
	IEnumerator RequestServer(string req, UnityEngine.Events.UnityAction<string> call = null) {		
		WWW www = new WWW (server + req);
		yield return www;

		//this.callback = www.text;
		Debug.Log (www.text);
		if(call != null)
			call (www.text);
	}
	/**
	IEnumerator RequestServer(string req) {		
		WWW www = new WWW (server + req);
		yield return www;
		
		//this.callback = www.text;
		Debug.Log (www.text);
	}
	**/

	void GetRoomState() {
		int playId = player.PlayId;
		string req = "plays/" + playId.ToString() + "/state";
		StartCoroutine (RequestServer (req));
	}
	
	void GetUserInfo() {
		int playId = player.PlayId;
		string req = "plays/" + playId.ToString() + "/users";
		StartCoroutine (RequestServer (req, ParseUserInfo));
	}

	void GetBattleInfo() {
		int playId = player.PlayId;
		string req = "plays/" + playId.ToString();
		StartCoroutine (RequestServer (req));
	}

	void GetWinner() {
		int playId = player.PlayId;
		string req = "plays/" + playId.ToString() + "/winner";
		StartCoroutine (RequestServer (req));
	}
	
	void GetPiecesInfo() {
		int playId = player.PlayId;
		string req = "plays/" + playId.ToString() + "/pieces";
		StartCoroutine (RequestServer (req, ParsePieces));
	}

	void ParseRoomState(string jsonText) {
		var json = Json.Deserialize (jsonText) as Dictionary<string, object>;
		string state = json ["state"].ToString ();
		int n = (state == "playing") ? 2 : 1;
		player.SetState (n);
	}

	IEnumerator WaitForPlayer() {
		while (player.GetState() != PlayerInfo.State.Playing) {
			GetRoomState();
			yield return new WaitForSeconds(3f);
		}

		GameStart ();
	}

	void ParseUserInfo(string jsonText) {
		var json = Json.Deserialize (jsonText) as Dictionary<string, object>;
		Dictionary<string, object> info = (Dictionary<string, object>)json ["first_player"];
		int id = System.Convert.ToInt32 (info ["user_id"]);
		string rivalName;
		int rivalId;
		if (player.UserId == id) {
			player.SetOrder (true);
			info = (Dictionary<string, object>)json["last_player"];
			rivalId = System.Convert.ToInt32 (info ["user_id"]);
			rivalName = info["name"].ToString();
		} else {
			player.SetOrder (false);
			rivalId = id;
			rivalName = info["name"].ToString();
			info = (Dictionary<string, object>)json["last_player"];
			player.SetName(info["name"].ToString());
		}
		CreateRivalInfo (rivalId, player.PlayId, rivalName, !player.IsFirst);
	}

	void CreateRivalInfo(int userId, int playId, string name, bool isFirst) {
		rival = new PlayerInfo (userId, playId, 2, 1);
		rival.SetOrder (isFirst);
		rival.SetName (name);
	}

	void ParsePieces(string jsonText) {

		var json = Json.Deserialize (jsonText) as Dictionary<string, object>;
		bool flag = pieces.Count == 0;
		int first = 0;
		if (banRect == null) 
			banRect = GameObject.Find ("Canvas").transform.GetChild (0).GetChild (0).GetComponent<RectTransform> ();

		for (int i=1; i<=40; i++) {
			object o = json[i.ToString()];
			Dictionary<string, object> piece = (Dictionary<string, object>)o;

			string name = piece["name"].ToString();
			float x = System.Convert.ToSingle(piece["posx"]);
			float y = System.Convert.ToSingle(piece["posy"]);
			Vector2 pos = new Vector2(x, y);
			bool isPromote = System.Convert.ToBoolean(piece["promote"]);
			bool isMine = false;
			int id = System.Convert.ToInt32(piece["owner"]);
			if(id == player.UserId) isMine = true;
			if(player.GetRole() == PlayerInfo.Role.Watcher) {
				if(i==1) first = id;
				if(first == id) isMine = true;
			}
			if(flag) {
				GameObject go = Instantiate(piecePrefab) as GameObject;
				RectTransform rect = go.GetComponent<RectTransform>();
				rect.SetParent(banRect, false);
				Piece p = go.GetComponent<Piece>();
				p.Set(name, pos, isMine, isPromote);
				pieces.Add(p);
			} else {
				Piece p = pieces[i];
				p.Set(pos, isPromote);
			}
		}

	}

	// TODO
	/**
	void UpdatePieces() {
		int playId = player.PlayId;
		string req = "plays/" + playId.ToString() + "/pieces";
		StartCoroutine (RequestServer (req));
		
		WWWForm form = new WWWForm ();
		form.AddField ("play_id", TODO);
		form.AddField ("user_id", TODO);
		
		WWW www = new WWW (server + req, form);
		yield return www;
		
		Debug.Log (www.text);
		SetPlayerInfo (www.text);
	}
	**/
}
