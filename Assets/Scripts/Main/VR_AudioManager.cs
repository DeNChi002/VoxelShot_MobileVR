using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// オーディオマネージャー
/// </summary>
public class VR_AudioManager : SingletonMonoBehaviour<VR_AudioManager>
{
	//オーディオファイルのパス
	private const string BGM_PATH = "Sounds/BGM";
	private const string SE_PATH = "Sounds/SE";

	//デフォルトの音量
	private const float BGM_VOLUME_DEFULT = 0.3f;
	private const float SE_VOLUME_DEFULT = 1.0f;

	//BGMがフェードするのにかかる時間
	public const float BGM_FADE_SPEED_RATE_HIGH = 0.9f;
	public const float BGM_FADE_SPEED_RATE_LOW = 0.3f;
	private float _bgmFadeSpeedRate = BGM_FADE_SPEED_RATE_HIGH;

    //ボリューム
	public float bgmVolume = BGM_VOLUME_DEFULT;
	public float seVolume = SE_VOLUME_DEFULT;

	//次流すBGM名、SE名
	private string _nextBGMName;
	private string _nextSEName;

	//BGMをフェードアウト中か
	private bool _isFadeOut = false;

	//BGM用、SE用に分けてオーディオソースを持つ
	private AudioSource _bgmSource;
	private List<AudioSourceInfo> audioSourceInfoList = new List<AudioSourceInfo>();
	private const int SE_SOURCE_NUM = 50;

	//全AudioClipを保持
	private Dictionary<string, AudioClip> _bgmDic;
	private Dictionary<string, AudioClip> _seDic;

    /// <summary>
    /// オーディオの詳細
    /// </summary>
	public class AudioSourceInfo
	{
		public AudioSource seSource;
		public GameObject parentObj;
		public GameObject sourceObj;

		public AudioSourceInfo()
		{
			seSource = null;
			parentObj = null;
		}
	}

    /// <summary>
    /// 初期設定
    /// </summary>
	protected override void Awake()
	{
		base.Awake();

		//オーディオソース SE + BGM 作成
		for (int i = 0; i < SE_SOURCE_NUM + 1; i++)
		{
			AudioSourceInfo audioInfo = new AudioSourceInfo();
			GameObject soundObj = new GameObject("AudioObj" + i.ToString());
			audioInfo.parentObj = null;
			audioInfo.seSource = soundObj.AddComponent<AudioSource>();
			audioInfo.seSource.playOnAwake = false;
			audioInfo.sourceObj = soundObj;
			soundObj.transform.SetParent(this.gameObject.transform);


			if (i == 0)
			{
				audioInfo.seSource.spatialBlend = 0.0f;
				audioInfo.seSource.loop = true;
                audioInfo.seSource.volume = BGM_VOLUME_DEFULT;
				_bgmSource = audioInfo.seSource;
			}
			else
			{
				audioInfo.seSource.minDistance = 0.5f;
				audioInfo.seSource.spatialBlend = 1.0f;
				audioInfo.seSource.loop = false;
                audioInfo.seSource.volume = BGM_VOLUME_DEFULT;

                audioSourceInfoList.Add(audioInfo);
			}
		}
		
		//リソースフォルダから全SE&BGMのファイルを読み込みセット
		_bgmDic = new Dictionary<string, AudioClip>();
		_seDic = new Dictionary<string, AudioClip>();

		object[] bgmList = Resources.LoadAll(BGM_PATH);
		object[] seList = Resources.LoadAll(SE_PATH);

		foreach (AudioClip bgm in bgmList)
		{
			_bgmDic[bgm.name] = bgm;
		}
		foreach (AudioClip se in seList)
		{
			_seDic[se.name] = se;
		}
	}



    /// <summary>
    /// BGMを鳴らす
    /// </summary>
    /// <param name="bgmName">BGMの名前</param>
    /// <param name="fadeSpeedRate">フェードのレート</param>
	public void PlayBGM(string bgmName, float fadeSpeedRate = BGM_FADE_SPEED_RATE_HIGH)
	{
		if (!_bgmDic.ContainsKey(bgmName))
		{
			Debug.Log(bgmName + "という名前のBGMがありません");
			return;
		}

		//現在BGMが流れていない時はそのまま流す
		if (!_bgmSource.isPlaying)
		{
			_nextBGMName = "";
			_bgmSource.clip = _bgmDic[bgmName] as AudioClip;
			_bgmSource.volume = BGM_VOLUME_DEFULT;
			_bgmSource.Play();
		}
		//違うBGMが流れている時は、流れているBGMをフェードアウトさせてから次を流す。同じBGMが流れている時はスルー
		else if (_bgmSource.clip.name != bgmName)
		{
			_nextBGMName = bgmName;
			FadeOutBGM(fadeSpeedRate);
        }
    }

	/// <summary>
	/// BGMをすぐに止める
	/// </summary>
	public void StopBGM()
	{
		_bgmSource.Stop();
	}

	/// <summary>
	/// 現在流れている曲をフェードアウトさせる
	/// fadeSpeedRateに指定した割合でフェードアウトするスピードが変わる
	/// </summary>
	public void FadeOutBGM(float fadeSpeedRate = BGM_FADE_SPEED_RATE_LOW)
	{
		_bgmFadeSpeedRate = fadeSpeedRate;
		_isFadeOut = true;
	}

	/// <summary>
	/// SE再生、指定された数値以上すでに同一ファイルが再生中の場合、動作をキャンセルする
	/// </summary>
	public void PlaySEAtLimit(int limitNum, string seName, Vector3 playPosition, float _minDistance = 1.0f, float _seVolume = 1.0f, GameObject parentObj = null, bool _is3dSound = true, bool _isLoop = false)
	{
		int currentPlayNum = 0;

		foreach (AudioSourceInfo audioSourceInfo in audioSourceInfoList)
		{
			if (audioSourceInfo.seSource.isPlaying)
			{
				if( string.Compare(seName, audioSourceInfo.seSource.clip.name) == 0)
				{
					++currentPlayNum;
				}
			}
		}

		if( currentPlayNum < limitNum)
		{
			PlaySE( seName, playPosition, _minDistance, _seVolume, parentObj, _is3dSound, _isLoop );
		}
	}

    /// <summary>
    /// SEを鳴らす
    /// </summary>
    /// <param name="seName">SEの名前</param>
    /// <param name="delay">遅延する時間</param>
    public void PlaySE(string seName, Vector3 playPosition, float _minDistance = 1.0f, float _seVolume = 1.0f, GameObject parentObj = null, bool _is3dSound = true, bool _isLoop = false)
    {
        if (!_seDic.ContainsKey(seName))
        {
            Debug.Log(seName + "という名前のSEがありません");
            return;
        }

        bool isEnd = false;

        foreach (AudioSourceInfo audioSourceInfo in audioSourceInfoList)
        {
            if (!audioSourceInfo.seSource.isPlaying)
            {
                audioSourceInfo.seSource.spatialBlend = (_is3dSound) ? 1.0f : 0.0f;
                audioSourceInfo.seSource.minDistance = _minDistance;
                audioSourceInfo.seSource.volume = seVolume;
                audioSourceInfo.seSource.loop = _isLoop;
                audioSourceInfo.seSource.clip = _seDic[seName] as AudioClip;
                audioSourceInfo.sourceObj.transform.position = playPosition;
                audioSourceInfo.parentObj = parentObj;

                audioSourceInfo.seSource.Play();
                //audioSourceInfo.seSource.PlayOneShot(_seDic[seName] as AudioClip);
                isEnd = true;
            }

            if (isEnd)
            {
                break;
            }
        }
    }

    /// <summary>
    /// SEを停止させる
    /// </summary>
    public void StopSE()
    {
        foreach (AudioSourceInfo audioSourceInfo in audioSourceInfoList)
        {
            audioSourceInfo.seSource.Stop();
        }
    }

    /// <summary>
    /// SEを停止させる
    /// </summary>
    /// <param name="_audioName">指定のファイル</param>
    public void StopSE(string _audioName)
    {
        foreach (AudioSourceInfo audioSourceInfo in audioSourceInfoList)
        {
            if (audioSourceInfo.seSource.clip == null)
                continue;

            if (audioSourceInfo.seSource.clip.name == _audioName)
                audioSourceInfo.seSource.Stop();
        }
    }

	/// <summary>
	/// BGMとSEのボリュームを別々に変更&保存
	/// </summary>
	public void ChangeVolume(float BGMVolume, float SEVolume)
	{
		_bgmSource.volume = BGMVolume;
		foreach (AudioSourceInfo audioSourceInfo in audioSourceInfoList)
		{
			audioSourceInfo.seSource.volume = SEVolume;
		}

		bgmVolume = BGMVolume;
		seVolume = SEVolume;

	}

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //追従するオブジェクトがある場合、音を追従させる
        for (int i = 0; i < audioSourceInfoList.Count; i++)
        {
            if (audioSourceInfoList[i].seSource.isPlaying && audioSourceInfoList[i].parentObj != null)
            {
                audioSourceInfoList[i].sourceObj.transform.position = audioSourceInfoList[i].parentObj.transform.position;
            }
        }

        //音のフェード処理
        if (!_isFadeOut)
        {
            return;
        }

        //徐々にボリュームを下げていき、ボリュームが0になったらボリュームを戻し次の曲を流す
        _bgmSource.volume -= Time.deltaTime * _bgmFadeSpeedRate;
        if (_bgmSource.volume <= 0)
        {
            _bgmSource.Stop();
            _bgmSource.volume = BGM_VOLUME_DEFULT;
            _isFadeOut = false;

            //次に再生するBGMがある場合
            if (!string.IsNullOrEmpty(_nextBGMName))
            {
                PlayBGM(_nextBGMName);
            }
        }
    }
}