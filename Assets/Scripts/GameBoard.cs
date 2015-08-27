using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour {
	
	Piece[] table = new Piece[9*9];
	
	static GameBoard instance;
	
	public DebugText dbg;
	
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
	
	public void Init(List<Piece> pieces) {
		table = new Piece[9*9];
		foreach (Piece p in pieces) {
			SetPiece(p);
		}
	}
	
	public Piece GetPiece(Vector2 pos) {
		int x = (int)pos.x - 1;
		int y = (int)pos.y - 1;
		int i = x + y * 9;
		if (i >= 81 || i<0) {
			Debug.LogWarning(x + " , " + y);
			return null;
		}
		return (i<81) ? table [i] : null;
	}
	
	public Piece SetPiece(Vector2 pos, Piece p) {
		Piece ret = GetPiece (pos);
		
		int x = (int)pos.x - 1;
		int y = (int)pos.y - 1;
		int i = x + y * 9;
		if (i >= 81 || i<0) {
			Debug.LogWarning(x + " , " + y);
			return ret;
		}
		table [i] = p;
		
		//
		//Dump ();
		//
		
		return ret;
	}
	
	public Piece SetPiece(Piece p) {
		return SetPiece (p.Tile, p);
	}
	
	void Dump() {
		string s = "";
		for (int i=0; i<9; i++) {
			for(int j=8; j>=0; j--) {
				s += (table[i*9 + j] == null) ? "0" : table[i*9 + j].ID.ToString();
				s += ',';
			}
			//dbg.Log(s);
			//s = "";
			s += '\n';
		}
		dbg.Log(s);
	}
}
