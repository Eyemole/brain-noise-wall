using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveSphere : MonoBehaviour {

	public int correspondingChannel = 0;

	public int MIN_ALPHA = -2;
	public int MAX_ALPHA = 2;
	public float MAX_HEIGHT = 5;

	public Rigidbody rb;

	// Use this for initialization
	void Start () {

		rb = GetComponent<Rigidbody> ();
		
	}
	
	// Update is called once per frame
	void UpdateFixed () {

		Vector3 pos = rb.transform.position;
		float newYPos = (float) ((EEGData.alphaData [correspondingChannel] - MIN_ALPHA)/ (MAX_ALPHA - MIN_ALPHA) * MAX_HEIGHT);

		pos.Set (pos.x, newYPos, pos.z );
		rb.transform.SetPositionAndRotation (pos, rb.transform.rotation);
		
	}
}
