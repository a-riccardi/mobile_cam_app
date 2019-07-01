using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ui_debugrotate : MonoBehaviour {

    [SerializeField] Image image;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        image.rectTransform.Rotate(new Vector3(0.0f, 0.0f, Time.deltaTime * 60.0f));
	}
}
