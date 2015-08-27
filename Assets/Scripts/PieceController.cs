using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PieceController : MonoBehaviour {

	List<Piece> pieces;
	RectTransform board;

	History history;
	public Dictionary<string, Dictionary<string, object>> firstTransform;

	public GameObject piecePrefab;
	public Transform origin;

	public List<string> keys;
	public List<TextAsset> files;

	Dictionary<string, TextAsset> settingFiles;
	
	static PieceController instance;
	
	public static PieceController Instance {
		get {
			if(instance == null) {
				GameObject go = new GameObject("PieceController");
				instance = go.AddComponent<PieceController>();
			}
			return instance;
		}
	}

	void Awake() {
		if (instance == null)
			instance = this;
		if(pieces == null)
			pieces = new List<Piece>();
		if(board == null)
			board = GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
		if(piecePrefab == null)
			piecePrefab = Resources.Load<GameObject>("piece");
		settingFiles = new Dictionary<string, TextAsset>();
	}

	void Start () {
	}

	public void Init() {
		if (keys == null || files == null)
			return;
		if (keys.Count <= 0 || files.Count <= 0)
			return;
		if (keys.Count != keys.Count) {
			Debug.LogWarning("keysとfilesのサイズを一致させてください");
			string s = (keys.Count < files.Count) ? "keys" : "files";
			Debug.LogWarning(s + "のサイズ分だけ作成します");
		}
		
		for (int i=0;;) {
			AddSetting(keys[i], files[i]);
			i++;
			if(i >= keys.Count || i >= files.Count) break;
		}
	}

	public void SetFirstPosition() {
		//pieces.Clear ();
		UpdatePieces (firstTransform);
	}

	public void UpdatePieces (Dictionary<string, Dictionary<string, object>> source) {
		if(pieces == null)
			pieces = new List<Piece>();
		bool flag = pieces.Count == 0;

		UserInfo me = GameLogic.Instance.GetMe ();
		UserInfo first = GameLogic.Instance.GetFirstUser();
		UserInfo last = GameLogic.Instance.GetLastUser();

		for(int i = 1; i <= 40; i++) {
			Dictionary<string, object> piece = source[i.ToString()];

			string name = piece["name"].ToString();
			float x = System.Convert.ToSingle(piece["posx"]);
			float y = System.Convert.ToSingle(piece["posy"]);
			Vector2 pos = new Vector2(x, y);
			bool isPromote = System.Convert.ToBoolean(piece["promote"]);
			//bool isMine = false;
			int id = System.Convert.ToInt32(piece["owner"]);

			UserInfo owner = null;
			if(id == first.UserId) {
				owner = first;
			}
			if(id == last.UserId) {
				owner = last;
			}
			if(owner == null) {
				Debug.LogError("Owner null Error!! " + id + "," + i);
			}

			Piece p = null;

			if(flag) {
				GameObject go = Instantiate(piecePrefab) as GameObject;
				RectTransform rect = go.GetComponent<RectTransform>();
				rect.SetParent(board, false);
				p = go.GetComponent<Piece>();
				p.SetOwner(owner);
				p.SetID(i);
				p.SetTilePosition(pos);
				p.SetTileOrigin(origin);
				p.SetKindName(name);

				TextAsset asset = null;
				if(settingFiles.TryGetValue(name, out asset)) {
					p.SetJsonFile(asset);
				}

				//p.Init();

				pieces.Add(p);
				continue;
			}


			p = pieces[i - 1];
			if(pos.x == p.Tile.x && pos.y == p.Tile.y) {
				//Debug.Log("pos equals tile: " + i);
				continue;
			}

			if(firstTransform != null && firstTransform == source) {
				p.SetActive(true);
				p.SetOwner(owner);
				p.SetTilePosition(pos);
				p.Init();
			}

			if(p.OnBoard) {
				if(p.IsSelected) continue;
				if(pos == Vector2.zero) {
					// p is got.
					continue;
				}
				p.PieceUpdate(pos, isPromote);

				History history = History.Instance;				
				List<Piece> eq = PieceController.Instance.FindMyEqualPieces (p);
				//bool isExistPieceMoveableSamePlace = false;
				List<Piece> same = null;
				if (eq != null) {
					foreach(Piece e in eq) {
						if(p.IsMoveable(p.Tile)) {
							//isExistPieceMoveableSamePlace = true;
							if(same == null) same = new List<Piece>();
							same.Add(e);
						}
					}
				}
				history.Add (p, same);
			} else {
				// putted down.
				Debug.Log("p is not active but moved: " + p.Tile + " -> " + pos);
				if(p.Owner == first) {
					p.SetOwner(last);
				} else if(p.Owner == last) {
					p.SetOwner(first);
				}
				p.SetActive(true);
				p.PieceUpdate(pos, false);
				p.Init();
				Transform canvas = GameObject.Find("Canvas").transform;
				string uiname = "YourUI";
				UIController controller = canvas.FindChild(uiname).GetComponent<UIController>();
				int value = p.Owner.GetHolds (p.Kind);
				controller.UpdateUI (p.Kind, value - 1);
			}
		}
		if (flag || (firstTransform != null && firstTransform == source)) {
			GameBoard.Instance.Init (pieces);
			// copy
			if(flag) {
				if(firstTransform == null)
					firstTransform = new Dictionary<string, Dictionary<string, object>>(source);
				if(me.UserId == last.UserId) {
					Transform canvas = GameObject.Find("Canvas").transform;
					Transform screen = canvas.FindChild("background");
					screen.Rotate(0, 0, 180f);
				}
			}
		}
	}

	public Piece GetByIndex(int index) {
		if (index + 1 > pieces.Count) {
			Debug.LogError("index is over");
			return null;
		}

		return pieces [index];
	}

	public Piece FindPassivePiece(string kind) {
		UserInfo me = GameLogic.Instance.GetMe();
		for (int i=0; i<40; i++) {
			Piece p = pieces[i];
			if(p.Kind == kind && p.Owner != me && !p.OnBoard) {
				return p;
			}
		}

		return null;
	}

	public List<Piece> FindMyEqualPieces(Piece piece) {
		List<Piece> list = new List<Piece> ();
		for(int i=0; i<40; i++) {
			Piece p = pieces[i];
			if(p != piece && p.Kind == piece.Kind && p.Owner == piece.Owner) {
				list.Add(p);
			}
		}
		if (list.Count <= 0)
			return null;

		return list;
	}

	public void CloseGuideArea() {
		Transform parent = GameBoard.Instance.transform;
		Transform target = parent.GetChild (parent.childCount - 1);
		string s = target.gameObject.name;
		if (s == "GuideArea") {
			if(target.childCount > 0) {
				GuideArea ga = target.GetChild(0).GetComponent<GuideArea>();
				ga.Close();
			}
		}
	}

	public void AddSetting (string key, TextAsset file) {
		if(settingFiles.ContainsKey(key)) return;
		settingFiles.Add(key, file);
		UserInfo first = GameLogic.Instance.GetFirstUser();
		UserInfo last = GameLogic.Instance.GetLastUser();
		first.AddPieceKind(key);
		last.AddPieceKind(key);
	}
}
