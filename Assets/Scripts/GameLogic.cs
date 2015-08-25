using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

	UserInfo player;
	UserInfo rival;
	
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
		player = new UserInfo (0, 1, 2, 1);
		rival = new UserInfo (1, 1, 2, 1);
		player.SetOrder (true);
	}
	
	IEnumerator WaitForPlayer() {
		while (player.GetState() != UserInfo.State.Playing) {
			GetRoomState();
			yield return new WaitForSeconds(1f);
		}
		
		GameStart ();
	}

	public void OnFinishParsingRoomState(int state) {
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
