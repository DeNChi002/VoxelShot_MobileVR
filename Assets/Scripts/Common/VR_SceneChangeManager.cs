using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// シーンの切り替え
/// </summary>
public class VR_SceneChangeManager : SingletonMonoBehaviour<VR_SceneChangeManager>
{
	public bool isShowGuid = false;
	public float defaultTime = 10.5f;
	public Color defaultColor = Color.black;


	/// <summary>
	/// シーンを切り替える
	/// </summary>
	/// <param name="_sceneName">遷移先のシーン名</param>
	public void LoadLevel(string _sceneName)
	{
		LoadLevel(_sceneName, defaultTime, defaultColor);
	}

	/// <summary>
	/// シーンを切り替える
	/// </summary>
	/// <param name="_sceneName">遷移先のシーン名</param>
	/// <param name="_time">遷移する時間</param>
	public void LoadLevel(string _sceneName, float _time)
	{
		LoadLevel(_sceneName, _time, defaultColor);
	}

	/// <summary>
	/// シーンを切り替える
	/// </summary>
	/// <param name="_sceneName">遷移先のシーン名</param>
	/// <param name="_time">遷移する時間</param>
	/// <param name="_color">フェードするときの色</param>
	public void LoadLevel(string _sceneName, float _time, Color _color)
	{
		//var resLoader = Resources.Load("Loader") as GameObject;
		//var loader = Instantiate(resLoader).GetComponent<SteamVR_LoadLevel>();
		//loader.levelName = _sceneName;
		//loader.fadeOutTime = 1f;
		//loader.fadeInTime = 1f;
		//loader.Trigger();

		SceneManager.LoadScene(_sceneName);

		//SteamVR_LoadLevel.Begin(
		//	_sceneName,
		//	isShowGuid,
		//	_time,
		//	_color.r,
		//	_color.g,
		//	_color.b,
		//	_color.a
		//	);
	}

	/// <summary>
	/// 現在のシーンを再読み込みする
	/// </summary>
	public void ReLoad()
	{
		LoadLevel(SceneManager.GetActiveScene().name, defaultTime, defaultColor);
	}

	/// <summary>
	/// 現在のシーンを再読み込みする
	/// </summary>
	/// <param name="_time">遷移する時間</param>
	public void ReLoad(float _time)
	{
		LoadLevel(SceneManager.GetActiveScene().name, _time, defaultColor);
	}

}