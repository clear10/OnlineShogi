using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class GuideArea : MonoBehaviour, IPointerClickHandler {
	
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
		if (piece == null) {
			Debug.LogError ("Piece not registed!");
			return;
		}

		RectTransform rect = piece.GetComponent<RectTransform> ();
		Vector2 piecePosition = rect.anchoredPosition;
		rect = this.GetComponent<RectTransform> ();
		Vector2 at = rect.anchoredPosition; // + piecePosition;
		Vector2 tile = piece.GetTilePosition (at);
		Close ();
		piece.Move (tile, false);
	}
	
	public void Close() {
		piece.UnSelect ();
	}
}