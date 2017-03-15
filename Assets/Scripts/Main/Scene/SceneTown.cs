using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 街中エリアシーン管理クラス
/// </summary>
public class SceneTown : MonoBehaviour {

	void Start()
	{
		VR_AudioManager.Instance.PlayBGM(AUDIO_NAME.BGM_MAIN);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			StartCoroutine(loadTest());


		}
	}

	IEnumerator loadTest()
	{
		var currentScene = SceneManager.GetActiveScene();
		SceneManager.LoadScene("Pause", LoadSceneMode.Additive);

		yield return new WaitForEndOfFrame();

		Time.timeScale = 0.0f;
		var nextScene = SceneManager.GetActiveScene();
		SceneManager.SetActiveScene(nextScene);
	}
}
