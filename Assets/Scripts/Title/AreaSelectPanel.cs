using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトルのエリア選択
/// </summary>
public class AreaSelectPanel : MonoBehaviorExpansion {

	[SerializeField]// 選択フレーム
	GameObject selectFrameObj;

	bool isSelected = false;

	public void SelectTown(GameObject _obj)
	{
		IconScale(_obj, "Town");
	}

	public void SelectCamp(GameObject _obj)
	{
		IconScale(_obj, "Camp");
	}

	public void SelectBase(GameObject _obj)
	{
		IconScale(_obj, "Base");
	}

	public void SelectAirport(GameObject _obj)
	{
		IconScale(_obj, "Airport");
	}

	void IconScale(GameObject _obj, string _sceneName)
	{
		if (isSelected)
		{
			return;
		}

		selectFrameObj.transform.parent = _obj.transform;
		selectFrameObj.transform.SetLocalPosition(0.0f, 0.0f, 0.0f);
		selectFrameObj.SetActive(true);

		_obj.transform.SetLocalPositionZ(-0.01f);
		_obj.transform.SetLocalScale(1.3f, 1.3f, 1.0f);

		iTween.ScaleTo(_obj, iTween.Hash("x", 1.0f, "y", 1.0f, "time", 0.35f, "EaseType", iTween.EaseType.easeOutBounce));

		isSelected = true;

		WaitAfter(0.4f, ()=> { VR_SceneChangeManager.Instance.LoadLevel(_sceneName); });
	}
}
