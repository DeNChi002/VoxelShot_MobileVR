using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵撃破時スコア表示
/// </summary>
public class EffectDefeatScore : MonoBehaviorExpansion {

	static readonly float NUM_SPRITE_OFFSET = 0.125f;

	[SerializeField]
	Transform rootSprite;

	[SerializeField]
	Sprite[] arrayNumSprite;

	[SerializeField]
	SpriteRenderer[] arraySpriteRenderer;

	float defRootX;

	public void Set( int _score )
	{
		SetNumImg(_score);
		WaitAfter(2.0f, () => { Destroy(gameObject);});
	}

	void SetNumImg( int _score )
	{
		foreach (SpriteRenderer sr in arraySpriteRenderer) { sr.enabled = false; }

		var tmpScore = _score;
		var numStr = tmpScore.ToString();
		for (int i = 0; i < numStr.Length; ++i)
		{
			arraySpriteRenderer[i].enabled = true;
			arraySpriteRenderer[i].sprite = arrayNumSprite[(tmpScore % 10)];
			tmpScore /= 10;
		}
		rootSprite.SetLocalPositionX(defRootX - (numStr.Length - 1) * NUM_SPRITE_OFFSET);
	}

	void Start()
	{
		defRootX = rootSprite.localPosition.x;

		transform.LookAt(ControllerManager.Instance.EyeCameraObj.transform.position);
		transform.SetLocalEulerAnglesX(0.0f);
	}
}
