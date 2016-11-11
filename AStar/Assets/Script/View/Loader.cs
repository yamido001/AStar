using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject obj = Resources.Load ("Prefab/MapView", typeof(GameObject)) as GameObject;
		GameObject view = GameObject.Instantiate (obj);
		view.transform.parent = transform;
		view.transform.localScale = new Vector3 (1f, 1f, 1f);
		view.transform.localPosition = new Vector3 (-400f, 270f, 0f);
		MapMediator mediator = view.AddComponent<MapMediator> ();
		mediator.Init ();
	}
}
