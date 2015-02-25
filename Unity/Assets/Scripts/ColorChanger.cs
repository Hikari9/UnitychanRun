using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ColorChanger : MonoBehaviour {

	public Color color = Color.white;

	private Color currentColor;
	private Material materialColored;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (color != currentColor) {
			// stop the leaks
			if (materialColored != null)
				UnityEditor.AssetDatabase.DeleteAsset(UnityEditor.AssetDatabase.GetAssetPath(materialColored));

			// create a new material
			materialColored = new Material(Shader.Find ("Diffuse"));
			materialColored.color = currentColor = color;
			this.renderer.material = materialColored;
		}
	}
}
