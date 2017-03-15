using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム進行管理
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager> {

	[SerializeField]
	GameObject infoBoard;
	[SerializeField]
	GameObject resultObj;

	[SerializeField]
	ResultBoard resultBoard;

	// 現在のゲームにおけるスコア
	public int CurrentScore { get; private set; }

	public void SelectRetry()
	{
		VR_SceneChangeManager.Instance.LoadLevel(SceneManager.GetActiveScene().name);
	}

	public void SelectTitle()
	{
		VR_SceneChangeManager.Instance.LoadLevel("Title");
	}

	/// <summary>
	/// スコア加算
	/// </summary>
	/// <param name="_add_score">加算値</param>
	public void AddScore( int _add_score )
	{
		CurrentScore += _add_score;
		Scoreboard.Instance.Add(_add_score);
	}

	/// <summary>
	/// タイムアップ
	/// </summary>
	public void TimeUp()
	{
		// カウントダウン表示
		var obj = Instantiate(Resources.Load("EffectGameEnd")) as GameObject;
		obj.transform.parent = ControllerManager.Instance.PlayerUICameraObj.transform;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.SetLocalPositionY(-0.8f);
		obj.transform.SetLocalPositionZ(11.0f);
		obj.transform.localRotation = Quaternion.identity;
		obj.name = "EffectGameEnd";

		WaitAfter(4.9f, () =>
		{
			Destroy(obj);
			VR_AudioManager.Instance.StopBGM();
			Fader.Instance.Fade( 0.0f, 1.0f, 1.5f );
		});

		WaitAfter(7.45f, () =>
		{
			ToResult();
		});
	}

	/// <summary>
	/// リザルト遷移
	/// </summary>
	public void ToResult()
	{
		var tagGameObjs = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject go in tagGameObjs)
		{
			Destroy(go);
		}

		VR_AudioManager.Instance.PlayBGM(AUDIO_NAME.BGM_RESULT);

		infoBoard.SetActive(false);
		resultObj.SetActive(true);

		resultBoard.Set(CurrentScore);

		Fader.Instance.Fade(1.0f, 0.0f, 0.5f);
	}

	void Start()
	{
		WaitAfter( 0.5f, ()=> {

			var obj = Instantiate( Resources.Load("EffectGameStart") ) as GameObject;
			obj.transform.parent = ControllerManager.Instance.PlayerUICameraObj.transform;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.SetLocalPositionY(-0.8f);
			obj.transform.SetLocalPositionZ(11.0f);
			obj.transform.localRotation = Quaternion.identity;
		} );

		WaitAfter( 4.0f, ()=> { 

			Timeboard.Instance.StartClock();
		});
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			VR_SceneChangeManager.Instance.LoadLevel("Title");
		}

		if (Input.GetKeyDown(KeyCode.D))
		{
			HealthDisp.Instance.Sub();
		}
	}
}
