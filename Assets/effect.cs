using UnityEngine;
using System.Collections;

public class effect : MonoBehaviour {
	ParticleSystem part;
	// Use this for initialization
	void Start () {
		part =this.GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!part.isPlaying)
			Destroy (this.gameObject);
	}
}
