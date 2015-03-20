using UnityEngine;
using System.Collections;

// [ExecuteInEditMode]
public class ChildTextureTiler : MonoBehaviour {

	public float scale = 10f;

	// Use this for initialization
	void Start () {
		UpdateTiling ();
	}

	[ContextMenu("Update Tiling")]
	void UpdateTiling() {
		foreach (Transform child in transform) {
			foreach (Object o in child.GetComponents<TextureTilingController>())
				DestroyImmediate (o);
			var textureController = child.gameObject.AddComponent<TextureTilingController>();
			textureController.texture = child.renderer.sharedMaterial.mainTexture;
			textureController.textureToMeshZ = scale;
			textureController.UpdateTiling();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
