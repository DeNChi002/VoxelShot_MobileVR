using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 残弾情報
/// </summary>
public class MagazineInfo : MonoBehaviour {

	static readonly float NUM_IMG_OFFSET = 12.5f;
	static readonly Color COLOR_STANDBY = new Color32(255, 227, 0, 255);
	static readonly Color COLOR_RELOAD = new Color32(72, 223, 0, 255);

	public bool IsReloading { get; private set; }

	[SerializeField]
	GameObject objCanvas;

	[SerializeField]
	Transform rootNum;

	[SerializeField]
	Sprite[] arrayNumSprite;

	[SerializeField]
	Image[] arrayNumImg;

	[SerializeField]
	Image remainingBarFront;

	float defRootX;

	int bulletNum;
	int currentBulletNum;

	float reloadTime;
	float currentTime;

	/// <summary>
	/// 表示有効設定
	/// </summary>
	/// <param name="_enable"></param>
	public void SetDispEnable( bool _enable )
	{
		objCanvas.SetActive(_enable);
	}
	
	/// <summary>
	/// 表示情報設定
	/// </summary>
	/// <param name="_bulletNum"></param>
	/// <param name="_reloadTime"></param>
	public void Set( int _bulletNum, float _reloadTime )
	{
		bulletNum = _bulletNum;
		currentBulletNum = _bulletNum;

		reloadTime = _reloadTime;
		currentTime = 0.0f;

		SetNumImg();
	}

	/// <summary>
	/// 弾丸表示数の減算
	/// </summary>
	public void Sub()
	{
		--currentBulletNum;
		SetNumImg();
	}
	
	/// <summary>
	/// 表示画像更新
	/// </summary>
	void SetNumImg()
	{
		foreach (Image img in arrayNumImg) { img.enabled = false; }

		var tmpRemaining = currentBulletNum;
		var numStr = tmpRemaining.ToString();
		
		for (int i = 0; i < numStr.Length; ++i)
		{
			arrayNumImg[i].enabled = true;
			arrayNumImg[i].sprite = arrayNumSprite[(tmpRemaining % 10)];
			tmpRemaining /= 10;
		}

		rootNum.SetLocalPositionX(defRootX + (numStr.Length - 1) * NUM_IMG_OFFSET);

		remainingBarFront.transform.SetLocalScaleX( (float)currentBulletNum / (float)bulletNum );

		if(currentBulletNum <= 0)
		{
			IsReloading = true;
			remainingBarFront.color = COLOR_RELOAD;
		}
	}

	private void Update()
	{
		if (IsReloading)
		{
			currentTime = Mathf.Clamp(currentTime += Time.deltaTime, 0.0f, reloadTime );
			remainingBarFront.transform.SetLocalScaleX((float)currentTime / (float)reloadTime);

			if (currentTime >= reloadTime)
			{
				IsReloading = false;
				remainingBarFront.color = COLOR_STANDBY;

				currentBulletNum = bulletNum;
				currentTime = 0.0f;
				SetNumImg();
			}
		}
	}

	void Start()
	{
		defRootX = rootNum.localPosition.x;
	}
}
