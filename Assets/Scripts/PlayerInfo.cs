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

	public int UserId{ get { return userId; } }
	public int PlayId{ get { return playId; } }
	public bool IsFirst{ get { return isFirst; } }
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

	public PlayerInfo() {
		this.userId = -1;
		this.playId = -1;
		this.state = State.Unknown;
		this.role = Role.Unknown;
		this.isFirst = false;
	}

	public PlayerInfo(int userId, int playId, int state, int role) {
		this.userId = userId;
		this.playId = playId;
		this.state = (State)state;
		this.role = (Role)role;
		this.isFirst = false;
	}

	public void SetOrder(bool isFirst) {
		this.isFirst = isFirst;
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
