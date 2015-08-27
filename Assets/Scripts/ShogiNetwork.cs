using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class ShogiNetwork : MonoBehaviour {

	const string server = "http://192.168.3.83:3002/";
	
	static ShogiNetwork instance;
	
	//public DebugText dbg;
	public GameLogic logic;
	
	public static ShogiNetwork Instance {
		get {
			if(instance == null) {
				GameObject go = new GameObject("Network");
				instance = go.AddComponent<ShogiNetwork>();
			}
			return instance;
		}
	}

	void Awake() {
		if (instance == null)
			instance = this;
	}

	public void JoinRoom(LobbyController controller) {
		Debug.Log ("Join room");
		string name = controller.NameText;
		string room = controller.RoomText;
		JoinRoom (name, room);
	}

	void JoinRoom(string name, string room) {
		logic = GameLogic.Instance;
		int no = 0;
		if(int.TryParse(room, out no))
			StartCoroutine (LogIn (name, no, logic.GoSceneMain));
	}
	
	public void LeaveRoom() {
		StopAllCoroutines ();
		UserInfo me = GameLogic.Instance.GetMe ();
		GameLogic logic = GameLogic.Instance;
		StartCoroutine (LogOut(me.PlayId, me.UserId, logic.GoSceneLogin));
	}

	public void UpdatePieces(Piece target, Piece get = null) {
		UserInfo me = GameLogic.Instance.GetMe ();
		StartCoroutine (UpdatePieceCoroutine (target, me.PlayId, me.UserId, get));
	}
	
	IEnumerator LogIn(string name, int room, UnityAction<string> call = null) {
		string req = "users/login";
		
		WWWForm form = new WWWForm ();
		form.AddField ("name", name);
		form.AddField ("room_no", room);
		
		WWW www = new WWW (server + req, form);
		yield return www;

		if (call != null)
			call (www.text);
	}
	
	IEnumerator LogOut(int playId, int userId, UnityAction<string> call = null) {
		string req = "users/logout";
		WWWForm form = new WWWForm ();
		form.AddField ("play_id", playId);
		form.AddField ("user_id", userId);
		WWW www = new WWW (server + req, form);
		yield return www;
		
		//Debug.Log (www.text);
		if (www.text == "[\"true\"]") {
			Debug.Log("Succesfully Logouted!");
			
			if (call != null)
				call (www.text);
		}
	}

	IEnumerator UpdatePieceCoroutine(Piece target, int playId, int userId, Piece get = null) {
		string req = "plays/update";
		
		WWWForm form = new WWWForm ();
		form.AddField ("play_id", playId);
		form.AddField ("user_id", userId);
		form.AddField("move_id", target.ID);
		form.AddField ("posx", (int)target.Tile.x);
		form.AddField ("posy", (int)target.Tile.y);
		form.AddField ("promote", target.IsPromoted.ToString());
		form.AddField ("get_id", (get != null) ? get.ID : -1);
		
		WWW www = new WWW (server + req, form);
		yield return www;
		
		Debug.Log (www.text);
	}

	IEnumerator RequestServer (string req, UnityAction<Dictionary<string, object>> call = null) {
		WWW www = new WWW (server + req);
		yield return www;

		Debug.Log (www.text);
		if(call == null) yield break;
		Dictionary<string, object> dict = ConvertJSON2Dictionary(www.text);
		call(dict);
	}

	public void GetRoomState(int playId, UnityAction<Dictionary<string, object>> call = null) {
		string req = "plays/" + playId.ToString() + "/state";
		StartCoroutine (RequestServer (req, call));
	}

	public void GetUserInfo (int playId, UnityAction<Dictionary<string, object>> call = null) {
		string req = "plays/" + playId.ToString() + "/users";
		StartCoroutine(RequestServer(req, call));
	}

	public void GetBattleInfo (int playId, UnityAction<Dictionary<string, object>> call = null) {
		string req = "plays/" + playId.ToString();
		StartCoroutine(RequestServer(req, call));
	}

	public void GetWinner (int playId, UnityAction<Dictionary<string, object>> call = null) {
		string req = "plays/" + playId.ToString() + "/winner";
		StartCoroutine(RequestServer(req, call));
	}

	public void GetPiecesInfo (int playId, UnityAction<Dictionary<string, object>> call = null) {
		string req = "plays/" + playId.ToString() + "/pieces";
		StartCoroutine(RequestServer(req, call));
	}

	Dictionary<string, object> ConvertJSON2Dictionary (string json) {
		return (Json.Deserialize(json) as Dictionary<string, object>);
	}
}
