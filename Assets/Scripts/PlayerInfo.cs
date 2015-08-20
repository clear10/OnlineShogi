using UnityEngine;
using System.Collections;

public class PlayerInfo {

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

	UIController ui;

	int[] holds = new int[7]{0, 0, 0, 0, 0, 0, 0};

	public int UserId{ get { return userId; } }
	public int PlayId{ get { return playId; } }
	public bool IsFirst{ get { return isFirst; } }
	public bool IsTurn{ get { return isTurn; } }
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

	public void RegistUI(UIController ui) {
		//Debug.LogError ("UI is " + ui);
		this.ui = ui;
	}

	public void GetPiece(Piece p) {
		int num = 0;
		Piece.Kind k = p.kind;
		switch (k) {
		case Piece.Kind.fu:
			num = ++holds[0];
			break;
		case Piece.Kind.kyosha:
			num = ++holds[1];
			break;
		case Piece.Kind.keima:
			num = ++holds[2];
			break;
		case Piece.Kind.gin:
			num = ++holds[3];
			break;
		case Piece.Kind.kin:
			num = ++holds[4];
			break;
		case Piece.Kind.hisya:
			num = ++holds[5];
			break;
		case Piece.Kind.kaku:
			num = ++holds[6];
			break;
		default:
			break;
		}

		ui.UpdateUI (k, num);
	}

	public PlayerInfo() {
		this.userId = -1;
		this.playId = -1;
		this.state = State.Unknown;
		this.role = Role.Unknown;
		this.isFirst = false;
		this.isTurn = false;
	}

	public PlayerInfo(int userId, int playId, int state, int role) {
		this.userId = userId;
		this.playId = playId;
		this.state = (State)state;
		this.role = (Role)role;
		this.isFirst = false;
		this.isTurn = false;
	}

	public void SetOrder(bool isFirst) {
		this.isFirst = isFirst;
	}

	public void SetTurn(bool flag) {
		isTurn = flag;
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
