using UnityEngine;
using System.Collections;

public class Fall : MonoBehaviour {

    bool isFalling;
    public AnimationClip fallingAnimation = null;
	public float triggerPosition = -1.5f;
	public float animationStartTime = 0f;
	[Range(0, 1)]
	public float animationSpeed = 0.3f;

	// Use this for initialization
	void Start () {
        isFalling = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!isFalling && transform.parent.position.y < triggerPosition) {
            isFalling = true;
            // fix camera
            Camera.main.transform.parent = null;

            // set animation to falling animation
            if (fallingAnimation != null) {
				animation[fallingAnimation.name].time = animationStartTime; // set at jump peak animation
				animation[fallingAnimation.name].speed = animationSpeed; // barely move animation
                GetComponentInParent<ArduinoController>().enableAnimation = false; // disable controller animation
                animation.CrossFade(fallingAnimation.name);
				StartCoroutine (Restart());
            }
        }
	}

	IEnumerator Restart() {
		yield return new WaitForSeconds(2.5f);
		Application.LoadLevel ("main");
	}
}
