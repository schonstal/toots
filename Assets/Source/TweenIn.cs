﻿using UnityEngine;
using System.Collections;

public class TweenIn : MonoBehaviour {
  float originalY = 0;

  void Awake() {
    originalY = transform.position.y;

    transform.position = new Vector3(
      transform.position.x,
      originalY - 4,
      transform.position.z
    );

    transform.localScale = new Vector3(0,0,0);

    float tweenTime = 1f + Random.value * 0.5f;

    iTween.MoveTo(gameObject, iTween.Hash(
      "y", originalY,
      "time", tweenTime,
      "easetype", "easeOutElastic"
    ));

    iTween.ScaleTo(gameObject, iTween.Hash(
      "x", 4,
      "y", 4,
      "z", 0.5,
      "time", tweenTime,
      "easetype", "easeOutElastic"
    ));
  }
}
