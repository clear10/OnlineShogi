using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PromotionWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	[SerializeField] Button promoteButton;
	[SerializeField] Button cancelButton;
	[SerializeField] Text text;

	public static bool IsShowing{ get { return (instance != null); } }

	static PromotionWindow instance = null;
	
	public static PromotionWindow Show() {
		if (instance != null)
			return null;

		Debug.Log ("PromotionWindowShow");

		Transform canvas = GameObject.Find ("Canvas").transform;
		GameObject prefab = Resources.Load<GameObject> ("PromotionWindow");

		GameObject go = Instantiate (prefab) as GameObject;
		go.transform.SetParent (canvas, false);
		instance = go.GetComponent<PromotionWindow> ();
		go.SetActive (false);
		return instance;
	}

	public void Close() {
		Destroy (instance.gameObject);
		instance = null;
	}

	public void Init(Piece piece) {
		if (!IsShowing)
			return;
		Sprite promoted = piece.GetPromotedSprite ();
		RectTransform rect;
		Image image = piece.GetComponent<Image> ();
		Sprite cancel = image.sprite;
		image = promoteButton.GetComponent<Image> ();
		image.sprite = promoted;
		if (!piece.Owner.IsFirst) {
			rect = promoteButton.GetComponent<RectTransform> ();
			rect.Rotate (0, 0, 180f);
		}
		image = cancelButton.GetComponent<Image> ();
		image.sprite = cancel;
		if (!piece.Owner.IsFirst) {
			rect = cancelButton.GetComponent<RectTransform> ();
			rect.Rotate (0, 0, 180f);
		}

		promoteButton.onClick.RemoveAllListeners ();
		promoteButton.onClick.AddListener (() => {
			piece.Promote ();
			Close ();
		});
		cancelButton.onClick.RemoveAllListeners ();
		cancelButton.onClick.AddListener (() => this.Close ());

		gameObject.SetActive (true);
	}

	public void OnPointerEnter (PointerEventData eventData) {
	}

	public void OnPointerExit (PointerEventData eventData) {
	}
}
