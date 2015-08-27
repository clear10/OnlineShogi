using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserInfo {
	
	public enum State {
		Unknown = 0,
		
		Waiting,
		Playing,
	};
	
	public enum Role {
		Unknown = 0,
		
		Player,
		Watcher,
	};
	
	int userId;
	int playId;
	
	string name;
	
	State state;
	Role role;
	bool isFirst;
	bool isTurn;
	bool isActed;

	Dictionary<string, int> holds;
	UIController ui;
	
	public int UserId{ get { return userId; } }
	public int PlayId{ get { return playId; } }
	public bool IsFirst{ get { return isFirst; } }
	public bool IsTurn{ get { return isTurn; } }
	public bool IsActed{ get { return isActed; } }
	public string Name{ get { return name; } }
	
	public State GetState() {
		return state;
	}
	
	public Role GetRole() {
		return role;
	}
	
	public void SetState(int i) {
		if (i > 2)
			return;
		this.state = (State)i;
	}
	
	public void SetName(string s) {
		this.name = s;
	}

	public void SetOrder (bool isFirst) {
		this.isFirst = isFirst;
	}

	public void SetTurn (bool flag) {
		isTurn = flag;
		isActed = !flag;
	}

	public void AddPieceKind (string key) {
		int value = 0;
		if(holds.ContainsKey(key)) return;
		holds.Add(key, value);
	}

	public void Act() {
		isActed = true;
	}

	void GetPiece (string key) {
		if(!holds.ContainsKey(key)) return;
		holds[key]++;
		ui.UpdateUI (key, holds [key]);
	}

	public void InitHolds() {
		List<string> keyList = new List<string>(holds.Keys);
		Transform canvas = GameObject.Find("Canvas").transform;
		UserInfo me = GameLogic.Instance.GetMe ();
		string uiname;
		if (me.GetRole () != Role.Player || me.isFirst) {
			uiname = (this.IsFirst) ? "MyUI" : "YourUI";
		} else {
			uiname = this.IsFirst ? "YourUI" : "MyUI";
		}
		//Debug.Log ((me == this).ToString() + ", " + uiname);
		UIController controller = canvas.FindChild(uiname).GetComponent<UIController>();
		ui = controller;
		foreach (string key in keyList) {
			holds[key] = 0;
			ui.UpdateUI (key, holds [key]);
		}
		
		//ui.RefreshAll ();
	}

	public int GetHolds(string kind) {
		if (holds.ContainsKey (kind))
			return holds [kind];
		return 0;
	}
	
	public void GetPiece(Piece p) {
		Transform canvas = GameObject.Find("Canvas").transform;
		UserInfo me = GameLogic.Instance.GetMe ();
		//string uiname = (p.Owner != this) ? "MyUI" : "YourUI";
		string uiname;
		if (me.GetRole () == Role.Player) {
			uiname = (me == p.Owner) ? "YourUI" : "MyUI";
		} else {
			uiname = p.Owner.IsFirst ? "MyUI" : "YourUI";
		}
		UIController controller = canvas.FindChild(uiname).GetComponent<UIController>();
		ui = controller;
		string key = p.Kind;
		Debug.Log (this.name + " Get!-> " + p.ID);
		GetPiece(key);
	}
	
	public UserInfo() {
		this.userId = -1;
		this.playId = -1;
		this.state = State.Unknown;
		this.role = Role.Unknown;
		this.isFirst = false;
		this.isTurn = false;
	}
	
	public UserInfo(int userId, int playId, int state, int role) {
		this.userId = userId;
		this.playId = playId;
		this.state = (State)state;
		this.role = (Role)role;
		this.isFirst = false;
		this.isTurn = false;
		holds = new Dictionary<string, int>();
	}
	
	public string Dump() {
		string ret = "";
		ret += "userId: " + this.userId.ToString ();
		ret += "\n";
		ret += "playId: " + this.playId.ToString ();
		ret += "\n";
		ret += "state: " + this.state.ToString ();
		ret += "\n";
		ret += "role: " + this.role.ToString ();
		ret += "\n";
		
		return ret;
	}
	
}
