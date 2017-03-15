using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : SingletonMonoBehaviour<Fader> {

	[SerializeField]
	Material mat;
	
	public void Fade( float _from, float _to, float _time = 0.5f )
	{
		iTween.ValueTo(gameObject, iTween.Hash("from", _from, "to", _to, "time", _time, "onupdate", "SetValue", "EaseType", iTween.EaseType.linear));
	}

	void SetValue(float _value)
	{
		mat.SetFloat("_Fade", _value);
	}

	void Start()
	{
		mat.SetFloat("_Fade", 0.0f);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, mat);
	}
}
