using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのライフ表示
/// </summary>
public class HealthDisp : SingletonMonoBehaviour<HealthDisp> {

	[SerializeField, Header("ライフ")]
	SpriteRenderer[] arrayHealthIcon;

	[SerializeField, Header("視界エフェクト")]
	SpriteRenderer eyeEffect;

	// プレイヤー耐久値
	int currentHelth = 5;
	// ゲームオーバー判定
	bool isGameOver;

	/// <summary>
	/// 体力値減少
	/// </summary>
	public void Sub()
	{
		return;

		if (Timeboard.Instance.CurrentDispTime <= 0 || isGameOver)
		{   // タイムアップ後は処理しない
			return;
		}

		eyeEffect.enabled = true;

		for (int i = 0; i < currentHelth; ++i)
		{
			arrayHealthIcon[i].enabled = false;
		}

		iTween.Stop( gameObject );
		iTween.ValueTo( gameObject, iTween.Hash("from", eyeEffect.color.a, "to", 0.5f, "time", 0.5f, "onupdate", "OnEyeEffectValue", "oncomplete", "OnEyeEffectSub") );

		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_PLAYER_DAMAGE, ControllerManager.Instance.EyeCameraObj.transform.position);

		--currentHelth;
		for (int i = 0; i < currentHelth; ++i)
		{
			arrayHealthIcon[i].enabled = true;
		}

		// ゲームオーバー
		if (currentHelth <= 0)
		{	// ゲームタイム処理を停止
			Timeboard.Instance.StopClock();

			var endObj = GameObject.Find("EffectGameEnd");
			if (endObj)
			{   // カウントダウン表示が開始されていれば破棄
				Destroy(endObj);
			}

			var obj = Instantiate(Resources.Load("EffectGameOver")) as GameObject;
			obj.transform.parent = ControllerManager.Instance.PlayerUICameraObj.transform;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.SetLocalPositionY(-0.8f);
			obj.transform.SetLocalPositionZ(11.0f);
			obj.transform.localRotation = Quaternion.identity;

			isGameOver = true;
		}
	}
	
	void OnEyeEffectValue( float _value )
	{
		var currentColor = eyeEffect.color;
		currentColor.a = _value;
		eyeEffect.color = currentColor;
	}

	void OnEyeEffectSub()
	{
		if (isGameOver)
		{   // ゲームオーバー時
			WaitAfter( 2.0f, () =>{

				VR_AudioManager.Instance.StopBGM();
				Fader.Instance.Fade(0.0f, 1.0f, 1.5f);

			});

			WaitAfter(4.55f, () => {

				eyeEffect.enabled = false;
				GameManager.Instance.ToResult();
			});
		}
		else
		{
			iTween.ValueTo(gameObject, iTween.Hash("from", 0.5f, "to", 0.0f, "time", 1.0f, "onupdate", "OnEyeEffectValue", "oncomplete", "OnEyeEffectEnd"));
		}
	}

	void OnEyeEffectEnd()
	{
		eyeEffect.enabled = false;
		foreach (SpriteRenderer sr in arrayHealthIcon)
		{
			sr.enabled = false;
		}
	}
}
