using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// リザルト表示
/// </summary>
public class ResultBoard : SingletonMonoBehaviour<ResultBoard> {

	static readonly float NUM_IMG_OFFSET = 125.0f;

	[SerializeField]
	Transform rootNum;

	[SerializeField]
	Sprite[] arrayNumSprite;

	[SerializeField]
	Image[] arrayNumImg;

	float defRootX;
	// 累計スコア
	int currentScore = 0;
	// 表示用加算スコア
	int localScore = 0;

	public void Set(int _setScore)
	{
		currentScore = _setScore;
		localScore = _setScore;

		SetNumImg();
	}

	public void Add(int _addScore)
	{
		currentScore += _addScore;

		iTween.Stop(gameObject);
		iTween.ValueTo(gameObject, iTween.Hash("from", localScore, "to", currentScore, "time", 0.75f, "onupdate", "SetValue", "EaseType", iTween.EaseType.easeOutCirc));
	}

	void SetNumImg()
	{
		foreach (Image img in arrayNumImg) { img.enabled = false; }

		var tmpScore = localScore;
		var numStr = tmpScore.ToString();
		for (int i = 0; i < numStr.Length; ++i)
		{
			arrayNumImg[i].enabled = true;
			arrayNumImg[i].sprite = arrayNumSprite[(tmpScore % 10)];
			tmpScore /= 10;
		}
		rootNum.SetLocalPositionX(defRootX + (numStr.Length - 1) * NUM_IMG_OFFSET);
	}

	void SetValue(float _score)
	{
		localScore = (int)_score;

		SetNumImg();
	}

	void Start()
	{
		defRootX = rootNum.localPosition.x;
	}
}
