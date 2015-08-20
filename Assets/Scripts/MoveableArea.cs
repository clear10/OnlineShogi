using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MoveableArea : MonoBehaviour, IPointerClickHandler {

	Piece piece;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void RegistPiece(Piece p) {
		piece = p;
	}

	public void OnPointerClick(PointerEventData eventData) {
		//Debug.Log (eventData.pointerEnter.name);
		RectTransform rect = this.GetComponent<RectTransform> ();
		Vector2 screen = rect.anchoredPosition + piece.GetRenderPosition(piece.pos);
		Vector2 tile = piece.GetTilePosition (screen);
		Close ();
		piece.Move (tile);
	}

	public void Close() {
		piece.UnSelect ();
	}
}
