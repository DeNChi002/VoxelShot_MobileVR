using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム進行管理
/// </summary>
public class Timeboard : SingletonMonoBehaviour<Timeboard> {

	public static readonly float START_TIME = 120.0f;

	public int CurrentDispTime { get; private set; }

	static readonly float NUM_IMG_OFFSET = 125.0f;

	[SerializeField]
	Transform rootNum;

	[SerializeField]
	Sprite[] arrayNumSprite;

	[SerializeField]
	Image[] arrayNumImg;

	[SerializeField]
	float remainingTime = START_TIME;

	float defRootX;
	int startTime;
	
	bool isClockUpdate = false;
	bool isTImeCount = false;

	public void StartClock()
	{
		isClockUpdate = true;
	}

	public void StopClock()
	{
		isClockUpdate = false;
	}

	void Start()
	{
		defRootX = rootNum.localPosition.x;
		CurrentDispTime = (int)remainingTime;
		startTime = (int)remainingTime;

		SetNumImg(CurrentDispTime);
	}

	void Update()
	{
		if (isClockUpdate && remainingTime > 0.0f)
		{
			remainingTime = Mathf.Clamp( remainingTime - Time.deltaTime, 0.0f, startTime);
			if (CurrentDispTime != (int)remainingTime)
			{
				SetNumImg((int)remainingTime);
			}
		}

		if (!isTImeCount && remainingTime <= 4.2f)
		{
			GameManager.Instance.TimeUp();
			isTImeCount = true;
		}
	}

	/// <summary>
	/// 数値表示設定
	/// </summary>
	/// <param name="_time"></param>
	void SetNumImg( int _time )
	{
		foreach (Image img in arrayNumImg) { img.enabled = false; }

		CurrentDispTime = _time;
		var tmpTime = _time;
		var numStr = tmpTime.ToString();
		for (int i = 0; i < numStr.Length; ++i)
		{
			arrayNumImg[i].enabled = true;
			arrayNumImg[i].sprite = arrayNumSprite[(tmpTime % 10)];
			tmpTime /= 10;
		}
		rootNum.SetLocalPositionX(defRootX + (numStr.Length - 1) * NUM_IMG_OFFSET);
	}
}
