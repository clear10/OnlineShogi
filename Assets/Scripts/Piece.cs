﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour, IPointerClickHandler {

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
			/**
			DestroyAllChildren ();
			if(this.gameObject == eventData.pointerEnter) return;
			Vector2 p;


			if(eventData.pointerEnter.GetComponent<Piece>() == null) {
				RectTransform r1 = eventData.pointerEnter.GetComponent<RectTransform>();
				RectTransform r2 = this.GetComponent<RectTransform>();
				p = GetTilePosition(r1.anchoredPosition + r2.anchoredPosition);
			} else {
				RectTransform r = eventData.pointerEnter.GetComponent<RectTransform>();
				p = GetTilePosition(r.anchoredPosition);
			}
			Debug.Log(p);
			Move(p);
			**/
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
			p = new Vector2(-1, 1);
			break;
		case Way.Forward:
			p = new Vector2(0, 1);
			break;
		case Way.ForwardRight:
			p = new Vector2(1, 1);
			break;
		case Way.Left:
			p = new Vector2(-1, 0);
			break;
		case Way.Right:
			p = new Vector2(1, 0);
			break;
		case Way.BackwardLeft:
			p = new Vector2(-1, -1);
			break;
		case Way.Backward:
			p = new Vector2(0, -1);
			break;
		case Way.BackwardRight:
			p = new Vector2(1, -1);
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
			ps.Add(new Vector2(-1, 2));
			ps.Add(new Vector2(1,  2));
			break;
		case Way.ForwardLine:
			for(int y = 1; y < 10; y++) {
				ps.Add(new Vector2(0, y));
			}
			break;
		case Way.Oblique:
			for(int i= 1; i < 10; i++) {
				ps.Add(new Vector2( i,  i));
				ps.Add(new Vector2(-i,  i));
				ps.Add(new Vector2( i, -i));
				ps.Add(new Vector2(-i, -i));
			}
			break;
		case Way.Cross:
			for(int i = 1; i<10; i++) {
				ps.Add(new Vector2(i, 0));
				ps.Add(new Vector2(-i, 0));
				ps.Add(new Vector2(0, i));
				ps.Add(new Vector2(0, -i));
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

	GameObject InstantiateMoveableArea(Transform parent, Vector2 p) {
		if (!owner.IsFirst)
			p *= -1;
		Vector2 target = this.pos - p;
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

}
