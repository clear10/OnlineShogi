using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class History {
	List<HistoryInfo> moves;
	int index;
	int max = -1;
	
	static History instance;
	
	public static History Instance {
		get {
			if(instance == null) {
				instance = new History();
			}
			return instance;
		}
	}

	public History() {
		Init ();
	}

	public void Init() {
		moves = new List<HistoryInfo> ();
		index = 0;
		max = -1;
	}

	public void OnGameStop() {
		max = index;
	}

	public void Add(Piece piece, List<Piece> same) {
		//Debug.LogWarning ("History Add");
		//string kanji = piece.Kanji;
		//Vector2 pos = piece.Tile;

		if (max > 0)
			return;

		bool isFirst = piece.Owner.IsFirst;

		string addition = "";

		if (same != null) {
			if(same.Count == 1) {
				if(same[0].Tile.x < piece.Tile.x) {
					addition += isFirst ? "左" : "右";
				}
				if(same[0].Tile.x > piece.Tile.x) {
					addition += !isFirst ? "左" : "右";
				}
			}
		}


		HistoryInfo info = new HistoryInfo (index++, piece, addition);
		//HistoryInfo info = new HistoryInfo (index++, kanji, pos, isFirst, addition);
		moves.Add (info);
	}

	public HistoryInfo Get(int index) {
		if (this.index <= index) {
			Debug.LogError("Index is over");
			return null;
		}

		return moves [index];
	}

	public int Count() {
		return moves.Count;
	}

	public string Dump() {
		string str = "";
		int i = 0;
		foreach (HistoryInfo info in moves) {
			string header = (info.IsFirst) ? "▲" : "△";
			string x = ((int)info.Position.x).ToString();
			string y = Int2Kanji((int)info.Position.y);
			string pos = x + y;
			string add = info.Addition;
			if(i > 0) {
				if(info.Position == moves[i-1].Position) 
					pos = "同";
			}
			string name = info.Kanji;

			str += header + pos + name + add;
			str += "\n";
			i++;
		}

		return str;
	}

	string Int2Kanji(int n) {
		if (n < 1 || n > 9) {
			Debug.LogError("Integer too large");
			return null;
		}
		switch (n) {
		case 1:
			return "一";
		case 2:
			return "二";
		case 3:
			return "三";
		case 4:
			return "四";
		case 5:
			return "五";
		case 6:
			return "六";
		case 7:
			return "七";
		case 8:
			return "八";
		case 9:
			return "九";
		default:
			return "Error";
		}
	}
}

public class HistoryInfo {
	int index;
	int pieceId;
	string kanji;
	Vector2 position;
	bool isFirst;
	bool isPromote;
	string addition;

	public int Index{ get { return this.index; } }
	public int PieceId{ get { return this.pieceId; } }
	public string Kanji{ get { return this.kanji; } }
	public Vector2 Position{ get { return this.position; } }
	public bool IsFirst{ get { return this.isFirst; } }
	public bool IsPromote{ get { return this.isPromote; } }
	public string Addition{ get { return this.addition; } }

	public HistoryInfo(int index, Piece piece, string addition) {
		this.index = index;
		this.pieceId = piece.ID;
		this.kanji = piece.Kanji;
		this.position = piece.Tile;
		this.isFirst = piece.Owner.IsFirst;
		this.isPromote = piece.IsPromoted;
		this.addition = addition;
	}

	/**
	 * 
	public HistoryInfo(int index, string kanji, Vector2 position, bool isFirst, string addition) {
		this.index = index;
		this.kanji = kanji;
		this.position = position;
		this.isFirst = isFirst;
		this.addition = addition;
	}
	*
	*/
}