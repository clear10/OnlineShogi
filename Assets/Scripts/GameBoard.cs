using UnityEngine;
using System.Collections;

public class GameBoard : MonoBehaviour {

	Piece[] table = new Piece[9*9];

	static GameBoard instance;

	public static GameBoard Instance {
		get {
			if(instance == null) {
				GameObject go = new GameObject("GameBoard");
				instance = go.AddComponent<GameBoard>();
			}
			return instance;
		}
	}

	void Awake() {
		if (instance == null)
			instance = this;
		for (int i=0; i<9*9; i++) {
			table[i] = null;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Piece GetPiece(Vector2 pos) {
		int x = (int)pos.x - 1;
		int y = (int)pos.y - 1;
		int i = x + y * 9;
		if (i >= 81 || i<0) {
			Debug.Log(x + " , " + y);
			return null;
		}
		return (i<81) ? table [i] : null;
	}

	public Piece SetPiece(Piece p) {
		Piece ret = GetPiece (p.pos);
		
		int x = (int)p.pos.x - 1;
		int y = (int)p.pos.y - 1;
		int i = x + y * 9;
		if (i >= 81 || i<0) {
			Debug.Log(x + " , " + y);
			return ret;
		}
		table [i] = p;
		return ret;
	}
}
