﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class Piece : MonoBehaviour, IPointerClickHandler {

	[SerializeField] TextAsset jsonFile;
	[SerializeField] GameObject guidePrefab;

	[SerializeField] Transform origin;
	public float sizeX;
	public float sizeY;

	Vector2 tile;

	UserInfo owner;

	int id;
	string kind;
	string kanji;
	bool isPromoted;
	bool canPromote;
	bool isSelected;
	bool isOnBoard;

	public int ID{ get { return id; } }
	public string Kind { get { return kind; } }
	public string Kanji{ get { return kanji; } }
	public bool IsPromoted{ get { return isPromoted; } }
	public bool IsSelected{ get { return isSelected; } }
	public bool OnBoard{ get { return isOnBoard; } }
	public UserInfo Owner { get { return owner; } }
	public Vector2 Tile{ get { return tile; } }

	void Start() {
		//Debug.Log ("start");
		Init ();
	}
	
	public void Init() {
		//Debug.Log ("Init");
		isPromoted = false;
		isSelected = false;
		SetActive (true);
		if (jsonFile == null)
			return;
		string jsonText = jsonFile.text;
		var json = Json.Deserialize (jsonText) as Dictionary<string, object>;
		
		//owner = GameLogic.Instance.Debug1 ();
		//tile = GetTilePosition (transform.localPosition);
		Move (this.tile, false);

		string regularPath = owner.IsFirst ? json ["uprightRegular"].ToString () : json ["reverseRegular"].ToString ();
		Sprite sp = Resources.Load<Sprite> (regularPath);
		Image image = GetComponent<Image> ();
		image.sprite = sp;
		kanji = json ["name"].ToString ();
		canPromote = System.Convert.ToBoolean (json ["promote"]);
	}

	public Sprite GetPromotedSprite() {		
		string jsonText = jsonFile.text;
		var json = Json.Deserialize (jsonText) as Dictionary<string, object>;

		string promotedPath = owner.IsFirst ? json ["uprightPromoted"].ToString () : json ["reversePromoted"].ToString ();
		Sprite sp = Resources.Load<Sprite> (promotedPath);
		return sp;
	}

	public void SetJsonFile(TextAsset asset) {
		this.jsonFile = asset;
	}

	public void SetActive(bool flag) {
		isOnBoard = flag;
		Image image = GetComponent<Image> ();
		if (!flag)
			image.enabled = false;
		else
			image.enabled = true;
	}

	public void SetOwner (UserInfo user) {
		owner = user;
	}

	public void SetID (int id) {
		this.id = id;
	}

	public void SetTilePosition (Vector2 pos) {
		if(pos.x >= 0 && pos.x < 10 && pos.y >= 0 && pos.y < 10)
			tile = pos;
	}
	
	public void SetTileOrigin(Transform t) {
		origin = t;
	}

	public void SetKindName (string s) {
		kind = s;
	}
	
	public void OnPointerClick(PointerEventData eventData) {
		UserInfo me = GameLogic.Instance.GetMe ();
		if (OnBoard) {
			if (me.GetRole () == UserInfo.Role.Watcher)
				return;
			if (!me.IsTurn)
				return;
			if (owner.UserId != me.UserId)
				return;
			if (Owner.IsActed)
				return;
			if (isSelected) {
				UnSelect ();
				return;
			}
			Select ();
			return;
		}
	}

	public void PieceUpdate (Vector2 at, bool isPromote) {
		//Debug.LogWarning ("PieceUpdate called");
		Move(at, false);
		if(isPromote != this.IsPromoted)
			Promote();
	}

	public void PutDown(Vector2 at) {
		Debug.Log ("Put down! " + at);
		UserInfo me = GameLogic.Instance.GetMe ();
		SetOwner (me);
		SetTilePosition (at);
		Init ();
		//SetActive (true);
		Transform canvas = GameObject.Find("Canvas").transform;
		string uiname = "MyUI";
		UIController controller = canvas.FindChild(uiname).GetComponent<UIController>();
		int value = me.GetHolds (Kind);
		controller.UpdateUI (Kind, value - 1);
		ShogiNetwork.Instance.UpdatePieces (this, null);
		History history = History.Instance;
		List<Piece> eq = PieceController.Instance.FindMyEqualPieces (this);
		//bool isExistPieceMoveableSamePlace = false;
		List<Piece> same = null;
		if (eq != null) {
			foreach(Piece p in eq) {
				if(p.IsMoveable(this.Tile)) {
					//isExistPieceMoveableSamePlace = true;
					if(same == null) same = new List<Piece>();
					same.Add(p);
				}
			}
		}
		history.Add (this, same);
		Owner.Act ();
	}

	public void PickUp() {
		if (OnBoard)
			return;
		List<Vector2> area = GetPutableArea ();
		ShowGuideArea (area);
	}

	List<Vector2> GetPutableArea() {
		string jsonText = jsonFile.text;
		var json = Json.Deserialize (jsonText) as Dictionary<string, object>;
		int y = System.Convert.ToInt32 (json ["limitArea"]);
		UserInfo me = GameLogic.Instance.GetMe ();
		List<Vector2> area = new List<Vector2> ();
		if (!me.IsFirst) {
			for (int i=1; i<=9; i++) {
				for (int j=1; j<=9; j++) {
					Vector2 pos = new Vector2 (j, i);
					Piece p = GameBoard.Instance.GetPiece(pos);
					if(p == null) {
						area.Add (pos - Tile);
					}
				}
				if (i == 9 - y)
					break;
			}
		} else {			
			for (int i=9; i>0; i--) {
				for (int j=1; j<=9; j++) {
					Vector2 pos = new Vector2 (j, i);
					Piece p = GameBoard.Instance.GetPiece(pos);
					if(p == null) {
						area.Add (pos - Tile);
					}
				}
				if (i == 1 + y)
					break;
			}
		}

		return area;
	}

	public void Promote() {
		Image image = GetComponent<Image> ();
		image.sprite = GetPromotedSprite ();
		isPromoted = true;
	}

	public void Move(Vector2 at, bool flag = true) {
		if (!this.OnBoard) {
			return;
		}

		RectTransform rect = GetComponent<RectTransform> ();
		Vector2 rend = GetRenderPosition (at);
		if (rect.anchoredPosition == rend)
			return;
		
		CloseGuideArea ();
		rect.anchoredPosition = rend;

		bool isPutDowned = (GameBoard.Instance.GetPiece (this.Tile) == null);

		Vector2 tmp = Tile;
		
		GameBoard.Instance.SetPiece (this.Tile, null);
		tile = at;
		Piece get = GameBoard.Instance.SetPiece (this);

		if(get != null) {
			//Debug.Log("Piece got!!");
			get.CloseGuideArea();
			get.SetTilePosition(Vector2.zero);
			get.SetActive(false);
			this.owner.GetPiece(get);
			//get.transform.localPosition = new Vector3(1000, 1000, 1000);
			//get.gameObject.SetActive(false);
		}

		if ((!owner.IsFirst && (Tile.y >= 7 && Tile.y <= 9)) ||
		    ( owner.IsFirst && (Tile.y >= 1 && Tile.y <= 3)) ||
		    (!owner.IsFirst && (tmp.y >= 7 && tmp.y <= 9)) ||
		    ( owner.IsFirst && (tmp.y >= 1 && tmp.y <= 3)) ) {
			if(!IsPromoted && canPromote && !isPutDowned) {
				UserInfo me = GameLogic.Instance.GetMe();
				if(owner == me) {
					if(get != null) {
						if(get.Owner != me) {
							AskPromotion(get);
							return;
						}
					} else {
						AskPromotion(null);
						return;
					}
				}
			}
		}
		
		if (!flag)
			return;
		
		ShogiNetwork.Instance.UpdatePieces (this, get);
		History history = History.Instance;
		List<Piece> eq = PieceController.Instance.FindMyEqualPieces (this);
		//bool isExistPieceMoveableSamePlace = false;
		List<Piece> same = null;
		if (eq != null) {
			foreach(Piece p in eq) {
				if(p.IsMoveable(this.Tile)) {
					//isExistPieceMoveableSamePlace = true;
					if(same == null) same = new List<Piece>();
					same.Add(p);
				}
			}
		}
		history.Add (this, same);
		Owner.Act ();
	}
	
	public void AskPromotion(Piece get) {
		if(!GameLogic.Instance.IsGameFinish)
			StartCoroutine (PromotionCoroutine (get));
	}
	
	IEnumerator PromotionCoroutine(Piece get) {
		//Vector2 pos = this.Tile;
		PromotionWindow window = PromotionWindow.Show ();
		window.Init (this);
		while (PromotionWindow.IsShowing)
			yield return null;
		//yield return null;

		//if (Tile != pos) {
		//	Move(pos, false);
		//}
		ShogiNetwork.Instance.UpdatePieces (this, get);
		History history = History.Instance;		
		List<Piece> eq = PieceController.Instance.FindMyEqualPieces (this);
		//bool isExistPieceMoveableSamePlace = false;
		List<Piece> same = null;
		if (eq != null) {
			foreach(Piece p in eq) {
				if(p.IsMoveable(this.Tile)) {
					//isExistPieceMoveableSamePlace = true;
					if(same == null) same = new List<Piece>();
					same.Add(p);
				}
			}
		}
		history.Add (this, same);
		Owner.Act ();
	}
	
	void CloseGuideArea() {		
		Transform parent = transform.parent;
		Transform target = parent.GetChild (parent.childCount - 1);
		string s = target.gameObject.name;
		if (s != "GuideArea")
			return;
		Destroy(target.gameObject);
	}

	public bool IsMoveable(Vector2 at) {
		List<Vector2> area = GetMoveableArea ();
		return area.Contains (at);
	}

	List<Vector2> GetMoveableArea() {
		string jsonText = jsonFile.text;
		var json = Json.Deserialize (jsonText) as Dictionary<string, object>;
		string addition = (!isPromoted) ? "Regular" : "Promoted";
		int x = System.Convert.ToInt32 (json ["originX" + addition]);
		int y = System.Convert.ToInt32 (json ["originY" + addition]);
		IList moves = json ["move" + addition] as IList;
		int i = 0;
		int j = 0;
		List<Vector2> area = new List<Vector2> ();
		foreach (object line in moves) {
			IList row = line as IList;
			j = 0;
			foreach (object cell in row) {
				int num = System.Convert.ToInt32 (cell);
				//Debug.Log(num);
				//Debug.Log(string.Format("i: {0}, j: {1}, x: {2}, y: {3}", i, j, x, y));
				int defX = j - x;
				int defY = i - y;
				int posX = 0;
				int posY = 0;
				while (num > 0) {
					posX += defX;
					posY += defY;
					Vector2 pos = new Vector2 (posX, posY);
					//Debug.Log (pos);
					if(!owner.IsFirst) pos *= -1;
					Vector2 test = this.Tile + pos;
					if(test.x < 1 || test.x > 9 || test.y < 1 || test.y > 9) break;
					Piece p = GameBoard.Instance.GetPiece(test);
					if(p != null) {
						if(p.owner == this.owner) {
							break;
						}
						area.Add(pos);
						break;
					}
					area.Add (pos);
					num--;
				}
				j++;
			}
			i++;
		}

		return area;
	}

	GameObject ShowGuideArea(List<Vector2> moveable) {
		if (moveable == null)
			return null;
		if (moveable.Count == 0)
			return null;

		GameObject area = new GameObject ("GuideArea");
		Transform parent = this.transform.parent;
		area.transform.SetParent (parent, false);

		foreach (Vector2 pos in moveable) {
			GameObject go = InstantiateMoveableArea(area.transform, pos);
			if(go == null) continue;
			GuideArea guide = go.GetComponent<GuideArea>();
			guide.RegistPiece(this);
		}

		return area;
	}
	
	GameObject InstantiateMoveableArea(Transform parent, Vector2 p) {
		//if (!owner.IsFirst)
		//	p *= -1;
		Vector2 target = this.tile + p;
		if (target.x < 1 || target.x > 9)
			return null;
		if (target.y < 1 || target.y > 9)
			return null;
		
		GameObject go;
		RectTransform rect;
		go = Instantiate (guidePrefab) as GameObject;
		rect = go.GetComponent<RectTransform> ();
		rect.SetParent(parent, false);
		Vector2 at = GetRenderPosition (this.Tile + p);
		rect.anchoredPosition = at;
		//GuideArea area = go.GetComponent<GuideArea> ();
		return go;
	}

	public Vector2 GetTilePosition(Vector2 rp) {
		Vector2 pos = new Vector2 (rp.x - origin.localPosition.x, rp.y - origin.localPosition.y);
		int x = (int)(-pos.x / sizeX);
		int y = (int)(-pos.y / sizeY);
		return new Vector2 (x+1, y+1);
	}
	
	public Vector2 GetRenderPosition(Vector2 tp) {
		int x = (int)tp.x - 1;
		int y = (int)tp.y - 1;
		Vector2 pos = new Vector2 (-x * sizeX, -y * sizeY);
		pos += new Vector2 (origin.localPosition.x, origin.localPosition.y);
		return (pos);
	}

	public void Select() {
		this.isSelected = true;
		PieceController.Instance.CloseGuideArea ();
		
		if (guidePrefab == null) {
			guidePrefab = Resources.Load<GameObject>("MoveableArea");
		}

		List<Vector2> moveable = GetMoveableArea ();
		ShowGuideArea(moveable); //GameObject area = ShowGuideArea(moveable);
	}
	
	public void UnSelect() {
		this.isSelected = false;
		CloseGuideArea ();
	}

	/**

	public enum Kind {
		fu,
		kyosha,
		keima,
		gin,
		kin,
		hisya,
		kaku,
		oh,

		Unknown,
	};

	public enum Way {
		FForwardSide,
		ForwardLine,
		Oblique,
		Cross,

		ForwardLeft,
		Forward,
		ForwardRight,
		Left,
		//
		Right,
		BackwardLeft,
		Backward,
		BackwardRight,
	};

	public Kind kind = Kind.Unknown;
	List<Way> ways;
	public Vector2 pos;
	int id;
	Vector2 origin = new Vector2 (200f,  201.5f);
	Vector2 limit = new Vector2 (-200f, -198.5f);
	bool isPromote;
	//public bool isMine;
	public bool isSelected = false;
	public bool Active{ get; set; }
	
	GameObject moveableArea;
	
	public float sizeX;
	public float sizeY;

	PlayerInfo owner;

	public int ID{ get { return id; } }
	public bool IsPromote{ get { return isPromote; } }

	// Use this for initialization
	void Start () {
		//parent = transform.parent.GetComponent<RectTransform> ();
		//Set ("", pos, true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Set(string name, Vector2 p, bool promote = false) {
		if(name != "")
			kind = GetKind (name);
		ways = GetWays (kind);
		if (ways == null) {
			Debug.LogError("Kind Error!");
			return;
		}
		//pos = p;
		//isMine = mine;
		isPromote = promote;
		Active = true;

		Image image = GetComponent<Image> ();
		image.sprite = GetSprite ();

		Move (p, false);
	}

	public void Set(Vector2 p, bool promote) {
		//pos = p;
		isPromote = promote;
		Move (p, false);
	}

	public void SetID(int i) {
		this.id = i;
	}

	public void RegistOwner(PlayerInfo o) {
		this.owner = o;
	}

	void Promote() {
		switch (kind) {
		case Kind.hisya:
			ways.Add(Way.ForwardLeft);
			ways.Add(Way.ForwardRight);
			ways.Add(Way.BackwardLeft);
			ways.Add(Way.BackwardRight);
			break;
		case Kind.kaku:
			ways.Add(Way.Forward);
			ways.Add(Way.Left);
			ways.Add(Way.Right);
			ways.Add(Way.Backward);
			break;
		case Kind.gin:
		case Kind.keima:
		case Kind.kyosha:
		case Kind.fu:
			ways = GetWays(Kind.kin); // Eq. Kin
			break;
		default:
			return;
		}
		isPromote = true;
	}

	Sprite GetSprite() {
		if (owner.IsFirst) {
			switch(kind) {
			case Kind.fu:
				return (!isPromote) ? Resources.Load<Sprite>("60x64/sgl08") : Resources.Load<Sprite>("60x64/sgl18");
			case Kind.kyosha:
				return (!isPromote) ? Resources.Load<Sprite>("60x64/sgl07") : Resources.Load<Sprite>("60x64/sgl27");
			case Kind.keima:
				return (!isPromote) ? Resources.Load<Sprite>("60x64/sgl06") : Resources.Load<Sprite>("60x64/sgl26");
			case Kind.gin:
				return (!isPromote) ? Resources.Load<Sprite>("60x64/sgl05") : Resources.Load<Sprite>("60x64/sgl25");
			case Kind.kin:
				return Resources.Load<Sprite>("60x64/sgl04");
			case Kind.hisya:
				return (!isPromote) ? Resources.Load<Sprite>("60x64/sgl02") : Resources.Load<Sprite>("60x64/sgl22");
			case Kind.kaku:
				return (!isPromote) ? Resources.Load<Sprite>("60x64/sgl03") : Resources.Load<Sprite>("60x64/sgl23");
			case Kind.oh:
				return Resources.Load<Sprite>("60x64/sgl01");
			default:
				break;
			}
		}

		switch (kind) {
		case Kind.fu:
			return (!isPromote) ? Resources.Load<Sprite>("60x64/sgl38") : Resources.Load<Sprite>("60x64/sgl58");
		case Kind.kyosha:
			return (!isPromote) ? Resources.Load<Sprite>("60x64/sgl37") : Resources.Load<Sprite>("60x64/sgl57");
		case Kind.keima:
			return (!isPromote) ? Resources.Load<Sprite>("60x64/sgl36") : Resources.Load<Sprite>("60x64/sgl56");
		case Kind.gin:
			return (!isPromote) ? Resources.Load<Sprite>("60x64/sgl35") : Resources.Load<Sprite>("60x64/sgl55");
		case Kind.kin:
			return Resources.Load<Sprite>("60x64/sgl34");
		case Kind.hisya:
			return (!isPromote) ? Resources.Load<Sprite>("60x64/sgl32") : Resources.Load<Sprite>("60x64/sgl52");
		case Kind.kaku:
			return (!isPromote) ? Resources.Load<Sprite>("60x64/sgl33") : Resources.Load<Sprite>("60x64/sgl53");
		case Kind.oh:
			return Resources.Load<Sprite>("60x64/sgl31");
		default:
			break;
		}

		return null;
	}

	List<Way> GetWays(Kind kind) {
		List<Way> w = new List<Way> ();
		switch (kind) {
		case Kind.fu:
			w.Add(Way.Forward);
			return w;
		case Kind.kyosha:
			w.Add(Way.ForwardLine);
			return w;
		case Kind.keima:
			w.Add(Way.FForwardSide);
			return w;
		case Kind.gin:
			w.Add(Way.ForwardLeft);
			w.Add(Way.Forward);
			w.Add(Way.ForwardRight);
			w.Add(Way.BackwardLeft);
			w.Add(Way.BackwardRight);
			return w;
		case Kind.kin:
			w.Add(Way.ForwardLeft);
			w.Add(Way.Forward);
			w.Add(Way.ForwardRight);
			w.Add(Way.Left);
			w.Add(Way.Right);
			w.Add(Way.Backward);
			return w;
		case Kind.hisya:
			w.Add(Way.Cross);
			return w;
		case Kind.kaku:
			w.Add(Way.Oblique);
			return w;
		case Kind.oh:
			w.Add(Way.ForwardLeft);
			w.Add(Way.Forward);
			w.Add(Way.ForwardRight);
			w.Add(Way.Left);
			w.Add(Way.Right);
			w.Add(Way.BackwardLeft);
			w.Add(Way.Backward);
			w.Add(Way.BackwardRight);
			return w;
		default:
			return null;
		}
	}

	Kind GetKind(string s) {
		switch (s) {
		case "fu":
			return Kind.fu;
		case "kyosha":
			return Kind.kyosha;
		case "keima":
			return Kind.keima;
		case "gin":
			return Kind.gin;
		case "kin":
			return Kind.kin;
		case "hisha":
			return Kind.hisya;
		case "kaku":
			return Kind.kaku;
		case "oh":
			return Kind.oh;
		default:
			return Kind.Unknown;
		}
	}

	public void OnPointerClick(PointerEventData eventData) {
		if (GameManager.Instance.player.GetRole () == PlayerInfo.Role.Watcher)
			return;
		if (!owner.IsTurn)
			return;
		if (!GameManager.Instance.player.IsTurn)
			return;
		isSelected = !isSelected;
		if (isSelected) {
			Select ();
			return;
		}

		DestroyAllChildren ();
	}

	public Vector2 GetTilePosition(Vector2 rp) {
		int i = 0;
		int j = 0;
		for (float y = limit.y-sizeY/2f; y < origin.y+sizeY/2f; y+=sizeY) {
			if(rp.y>y) {
				i++;
			}
		}
		for(float x = limit.x-sizeX/2f; x<origin.x+sizeX/2f;x+= sizeX) {
			if(rp.x>x) {
				j++;
			}
		}
		return new Vector2 (10-j, 10-i);
	}

	public Vector2 GetRenderPosition(Vector2 tp) {
		Vector2 target = origin - (new Vector2 ((tp.x - 1) * sizeX, (tp.y - 1) * sizeY));
		return target;
	}

	public void Move(Vector2 tilepos, bool flag = true) {
		if (!this.Active)
			return;
		RectTransform rect = GetComponent<RectTransform> ();
		Vector2 at = GetRenderPosition (tilepos);
		if (rect.anchoredPosition == at)
			return;

		DestroyAllChildren ();
		rect.anchoredPosition = at; //origin - (new Vector2 ((tilepos.x-1) * sizeX, (tilepos.y+0) * sizeY));

		if (tilepos == this.pos)
			return;

		GameBoard.Instance.SetPiece (this.pos, null);
		this.pos = tilepos;
		Piece get = GameBoard.Instance.SetPiece (this);

		if (!flag)
			return;
		GameManager.Instance.UpdatePieces (this, get);
		if(get != null) {
			//get.DestroyAllChildren();
			//get.Move(new Vector2(-2, 0));
			//get.kind
			this.owner.GetPiece(get);
			get.Active = false;
			get.gameObject.SetActive(false);
		}
	}

	public void Select() {		
		Transform parent = transform.parent;
		Transform target = parent.GetChild (parent.childCount - 1);
		string s = target.gameObject.name;
		if (s == "MoveableAreas") {
			if(target.childCount > 0) {
				MoveableArea ma = target.GetChild(0).GetComponent<MoveableArea>();
				ma.Close();
			}
			DestroyAllChildren();
		}

		if (moveableArea == null) {
			moveableArea = Resources.Load<GameObject>("MoveableArea");
		}
		List<GameObject> objs = new List<GameObject> ();
		GameObject area = new GameObject ("MoveableAreas");
		area.transform.SetParent (this.transform.parent, false);
		area.transform.localPosition = this.transform.localPosition;
		foreach (Way w in ways) {
			List<GameObject> tmp = InstantiateMoveableArea(area.transform, w);
			if(tmp != null)
				objs.AddRange(tmp);
		}
	}
	
	public void UnSelect() {
		this.isSelected = false;
	}

	void OnDestroy() {
		Debug.Log ("Destroyed No." + this.ID);
	}

	List<GameObject> InstantiateMoveableArea(Transform parent, Way w) {
		List<GameObject> objs = new List<GameObject> ();
		Vector2 p = Vector2.zero;
		switch (w) {
		case Way.ForwardLeft:
			p = new Vector2(1, -1);
			break;
		case Way.Forward:
			p = new Vector2(0, -1);
			break;
		case Way.ForwardRight:
			p = new Vector2(-1, -1);
			break;
		case Way.Left:
			p = new Vector2(1, 0);
			break;
		case Way.Right:
			p = new Vector2(-1, 0);
			break;
		case Way.BackwardLeft:
			p = new Vector2(1, 1);
			break;
		case Way.Backward:
			p = new Vector2(0, 1);
			break;
		case Way.BackwardRight:
			p = new Vector2(-1, 1);
			break;
		default:
			break;
		}
		if (p != Vector2.zero) {
			//p += pos;
			GameObject go = InstantiateMoveableArea(parent, p);
			if(go == null) return null;
			objs.Add(go);
			return objs;
		}

		List<Vector2> ps = new List<Vector2> ();
		switch (w) {
		case Way.FForwardSide:
			ps.Add(new Vector2(-1, -2));
			ps.Add(new Vector2(1,  -2));
			break;
		case Way.ForwardLine:
			bool fw = true;
			for(int y = 1; y < 10; y++) {
				Vector2 relative = new Vector2(0, -y);
				fw = IsMoveable(relative, ps);
				if(!fw) break;
			}
			break;
		case Way.Oblique:
			bool fl = true;
			bool fr = true;
			bool bl = true;
			bool br = true;
			for(int i= 1; i < 10; i++) {
				Vector2 relative;
				if(fr) {
					relative = new Vector2(-i, -i);
					fr = IsMoveable(relative, ps);
				}
				if(fl) {
					relative = new Vector2(i, -i);
					fl = IsMoveable(relative, ps);
				}
				if(br) {
					relative = new Vector2(-i, i);
					br = IsMoveable(relative, ps);
				}
				if(bl) {
					relative = new Vector2(i, i);
					bl = IsMoveable(relative, ps);
				}
			}
			break;
		case Way.Cross:
			bool left = true;
			bool right = true;
			bool fd = true;
			bool bk = true;
			for(int i = 1; i<10; i++) {
				Vector2 relative;
				if(left) {
					relative = new Vector2(i, 0);
					left = IsMoveable(relative, ps);
				}
				if(right) {
					relative = new Vector2(-i, 0);
					right = IsMoveable(relative, ps);
				}
				if(fd) {
					relative = new Vector2(0, -i);
					fd = IsMoveable(relative, ps);
				}
				if(bk) {
					relative = new Vector2(0, i);
					bk = IsMoveable(relative, ps);
				}
			}
			break;
		default:
			break;
		}

		for (int j=0; j<ps.Count; j++) {
			//ps[j] += pos;
			GameObject go = InstantiateMoveableArea(parent, ps[j]);
			if(go != null)
				objs.Add(go);
		}

		return objs;
	}

	bool IsMoveable(Vector2 relative, List<Vector2> list) {
		Vector2 p = relative;
		if (!owner.IsFirst)
			p *= -1;
		Piece piece = GameBoard.Instance.GetPiece(this.pos + p);
		if(piece != null) {
			if(piece.owner == this.owner) return false;
		}
		list.Add(relative);
		return true;
	}

	GameObject InstantiateMoveableArea(Transform parent, Vector2 p) {
		if (!owner.IsFirst)
			p *= -1;
		Vector2 target = this.pos + p;
		if (target.x < 1 || target.x > 9)
			return null;
		if (target.y < 1 || target.y > 9)
			return null;
		Piece piece = GameBoard.Instance.GetPiece (target);
		if (piece != null) {
			if (piece.owner == this.owner)
				return null;
		}

		GameObject go;
		RectTransform rect;
		go = Instantiate (moveableArea) as GameObject;
		rect = go.GetComponent<RectTransform> ();
		rect.SetParent(parent, false);
		rect.anchoredPosition = new Vector2(p.x * sizeX, p.y * sizeY);
		MoveableArea area = go.GetComponent<MoveableArea> ();
		area.RegistPiece (this);
		return go;
	}

	void DestroyAllChildren() {
		Transform parent = transform.parent;
		Transform target = parent.GetChild (parent.childCount - 1);
		string s = target.gameObject.name;
		if (s == "MoveableAreas")
			Destroy (target.gameObject);
	}

	**/

}
