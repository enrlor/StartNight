using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeSprite : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float worldScreenHeight = Camera.main.fieldOfView / 2.5f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        transform.localScale = new Vector3(
            worldScreenWidth / sr.sprite.bounds.size.x,
            worldScreenHeight / sr.sprite.bounds.size.y, 1);

    }

}
