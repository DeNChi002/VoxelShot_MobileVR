using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineTracer : MonoBehaviour {
	
	[SerializeField]
	Image imgLine;

	float totalMove = 0.0f;

	void Update () {

		totalMove = Mathf.Clamp(totalMove - Time.deltaTime, -1.0f, 0.0f);

		imgLine.material.SetTextureOffset("_MainTex", new Vector2(0, totalMove));

		if (totalMove <= -1.0f)
		{
			totalMove = 0.0f;
		}
	}
}
