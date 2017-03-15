using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

public static class NameCreator
{
	// 無効な文字を管理する配列
	private static readonly string[] INVALUD_CHARS =
    {
        " ", "!", "\"", "#", "$",
        "%", "&", "\'", "(", ")",
        "-", "=", "^",  "~", "\\",
        "|", "[", "{",  "@", "`",
        "]", "}", ":",  "*", ";",
        "+", "/", "?",  ".", ">",
        ",", "<"
    };

	//メニューバーに表示する名前
	private const string ALL_COMMAND_NAME    = "Tools/Create/AllNameScript";
	private const string AUDIO_COMMAND_NAME  = "Tools/Create/AudioNameScript";
	private const string LAYER_COMMAND_NAME  = "Tools/Create/LayerNameScript";
	private const string SCENE_COMMAND_NAME  = "Tools/Create/SceneNameScript";
	private const string TAG_COMMAND_NAME    = "Tools/Create/TagNameScript";
	private const string EFFECT_COMMAND_NAME = "Tools/Create/EffectNameScript";

	//作成したスクリプトを保存するパス
	private const string AUDIO_EXPORT_PATH  = "Assets/Scripts/GameData/AUDIO_NAME.cs";
	private const string LAYER_EXPORT_PATH  = "Assets/Scripts/GameData/LAYER_NAME.cs";
	private const string SCENE_EXPORT_PATH  = "Assets/SCripts/GameData/SCENE_NAME.cs";
	private const string TAG_EXPORT_PATH    = "Assets/Scripts/GameData/TAG_NAME.cs";
	private const string EFFECT_EXPORT_PATH = "Assets/Scripts/GameData/EFFECT_NAME.cs";

	// ファイル名(拡張子あり)
	private static readonly string AUDIO_FILENAME  = Path.GetFileName(AUDIO_EXPORT_PATH);
	private static readonly string LAYER_FILENAME  = Path.GetFileName(LAYER_EXPORT_PATH);
	private static readonly string SCENE_FILENAME  = Path.GetFileName(SCENE_EXPORT_PATH);
	private static readonly string TAG_FILENAME    = Path.GetFileName(TAG_EXPORT_PATH);
	private static readonly string EFFECT_FILENAME = Path.GetFileName(EFFECT_EXPORT_PATH);

	// ファイル名(拡張子なし)
	private static readonly string AUDIO_FILENAME_WITHOUT_EXTENSION = Path.GetFileNameWithoutExtension(AUDIO_EXPORT_PATH);
	private static readonly string LAYER_FILENAME_WITHOUT_EXTENSION = Path.GetFileNameWithoutExtension(LAYER_EXPORT_PATH);
	private static readonly string SCENE_FILENAME_WITHOUT_EXTENSION = Path.GetFileNameWithoutExtension(SCENE_EXPORT_PATH);
	private static readonly string TAG_FILENAME_WITHOUT_EXTENSION   = Path.GetFileNameWithoutExtension(TAG_EXPORT_PATH);
	private static readonly string EFFECT_FILENAME_WITHOUT_EXTENSION = Path.GetFileNameWithoutExtension(EFFECT_EXPORT_PATH);


	/// <summary>
	/// 全てのスクリプトを作成できるかどうかを取得します
	/// </summary>
	[MenuItem(ALL_COMMAND_NAME, true)]
	public static bool AllNameCanCreate()
	{
		return LayerNameCanCreate() &&
				AudioNameCanCreate() &&
				SceneNameCanCreate() &&
				TagNameCanCreate()&&
				EffectNameCanCreate();
	
	}

	/// <summary>
	/// オーディオのファイル名を定数で管理するクラスを作成できるかどうかを取得します
	/// </summary>
	[MenuItem(AUDIO_COMMAND_NAME, true)]
	private static bool AudioNameCanCreate()
	{
		return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
	}

	/// <summary>
	/// レイヤー名を定数で管理するクラスを作成できるかどうかを取得します
	/// </summary>
	[MenuItem(LAYER_COMMAND_NAME, true)]
	public static bool LayerNameCanCreate()
	{
		return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
	}

	/// <summary>
	/// シーン名を定数で管理するクラスを作成できるかどうかを取得します
	/// </summary>
	[MenuItem(SCENE_COMMAND_NAME, true)]
	public static bool SceneNameCanCreate()
	{
		return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
	}

	/// <summary>
	/// タグ名を定数で管理するクラスを作成できるかどうかを取得します
	/// </summary>
	[MenuItem(TAG_COMMAND_NAME, true)]
	public static bool TagNameCanCreate()
	{
		return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
	}

	/// <summary>
	/// オーディオのファイル名を定数で管理するクラスを作成できるかどうかを取得します
	/// </summary>
	[MenuItem(EFFECT_COMMAND_NAME, true)]
	private static bool EffectNameCanCreate()
	{
		return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
	}

	/// <summary>
	/// 全てのファイル名を定数で管理するクラスを作成します
	/// </summary>
	[MenuItem(ALL_COMMAND_NAME)]
	public static void AllNameCreate()
	{
		AudioNameScriptCreate();
		LayerNameScriptCreate();
		SceneNameScriptCreate();
		TagNameScriptCreate();
		EffectNameScriptCreate();

		Debug.Log("全てのファイル名のクラスを作成しました");
		EditorUtility.DisplayDialog(AUDIO_FILENAME, "全てのファイル名のクラスの作成が完了しました", "OK");
	}

	/// <summary>
	/// オーディオのファイル名を定数で管理するクラスを作成します
	/// </summary>
	[MenuItem(AUDIO_COMMAND_NAME)]
	public static void AudioNameCreate()
	{
		if (!AudioNameCanCreate())
		{
			Debug.Log("AUDIO.csを作成出来ませんでした");
			return;
		}

		AudioNameScriptCreate();
		Debug.Log(AUDIO_FILENAME + "を作成しました");
		EditorUtility.DisplayDialog(AUDIO_FILENAME, "作成が完了しました", "OK");
	}

	/// <summary>
	/// レイヤー名を定数で管理するクラスを作成します
	/// </summary>
	[MenuItem(LAYER_COMMAND_NAME)]
	public static void LayerNameCreate()
	{
		if (!LayerNameCanCreate())
		{
			Debug.Log(LAYER_FILENAME + "の作成に失敗しました");
			return;
		}

		LayerNameScriptCreate();
		Debug.Log(LAYER_FILENAME + "を作成しました");
		EditorUtility.DisplayDialog(LAYER_FILENAME, "作成が完了しました", "OK");
	}

	/// <summary>
	/// シーン名を定数で管理するクラスを作成します
	/// </summary>
	[MenuItem(SCENE_COMMAND_NAME)]
	public static void SceneNameCreate()
	{
		if (!SceneNameCanCreate())
		{
			Debug.Log(SCENE_FILENAME + "の作成に失敗しました");
			return;
		}

		SceneNameScriptCreate();
		Debug.Log(SCENE_FILENAME + "を作成しました");
		EditorUtility.DisplayDialog(SCENE_FILENAME, "作成が完了しました", "OK");
	}

	/// <summary>
	/// タグ名を定数で管理するクラスを作成します
	/// </summary>
	[MenuItem(TAG_COMMAND_NAME)]
	public static void TagNameCreate()
	{
		if (!TagNameCanCreate())
		{
			Debug.Log(TAG_FILENAME + "の作成に失敗しました");
			return;
		}

		TagNameScriptCreate();
		Debug.Log(TAG_FILENAME + "を作成しました");
		EditorUtility.DisplayDialog(TAG_FILENAME, "作成が完了しました", "OK");
	}

	/// <summary>
	/// エフェクトのファイル名を定数で管理するクラスを作成します
	/// </summary>
	[MenuItem(EFFECT_COMMAND_NAME)]
	public static void EffectNameCreate()
	{
		if (!EffectNameCanCreate())
		{
			Debug.Log("EFFET_NAME.csを作成出来ませんでした");
			return;
		}

		EffectNameScriptCreate();
		Debug.Log(EFFECT_FILENAME + "を作成しました");
		EditorUtility.DisplayDialog(EFFECT_FILENAME, "作成が完了しました", "OK");
	}


	/// <summary>
	/// AudioNameスクリプトを作成します
	/// </summary>
	public static void AudioNameScriptCreate()
	{
		StringBuilder builder = new StringBuilder();

		builder.AppendLine("/// <summary>");
		builder.AppendLine("/// オーディオ名を定数で管理するクラス");
		builder.AppendLine("/// </summary>");
		builder.AppendFormat("public static class {0}", AUDIO_FILENAME_WITHOUT_EXTENSION).AppendLine();
		builder.AppendLine("{");

		//指定したパスのリソースを全て取得
		object[] bgmList = Resources.LoadAll("Sounds/BGM");

		List<object> seList = new List<object>();
        object[] sceneSe = Resources.LoadAll("Sounds/SE");
        for (int j = 0; j < sceneSe.Length; j++)
        {
            seList.Add(sceneSe[j]);
        }

		foreach (AudioClip bgm in bgmList)
		{
			builder.Append("\t").AppendFormat(@"  public const string BGM_{0} = ""{1}"";", bgm.name.ToUpper(), bgm.name).AppendLine();
		}

		builder.AppendLine("\t");

		foreach (AudioClip se in seList)
		{
			builder.Append("\t").AppendFormat(@"  public const string SE_{0} = ""{1}"";", se.name.ToUpper(), se.name).AppendLine();
		}

		builder.AppendLine("}");

		string directoryName = Path.GetDirectoryName(AUDIO_EXPORT_PATH);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}

		File.WriteAllText(AUDIO_EXPORT_PATH, builder.ToString(), Encoding.UTF8);
		AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
	}

	/// <summary>
	/// LayerNameスクリプトを作成します
	/// </summary>
	public static void LayerNameScriptCreate()
	{
		var builder = new StringBuilder();

		builder.AppendLine("/// <summary>");
		builder.AppendLine("/// レイヤー名を定数で管理するクラス");
		builder.AppendLine("/// </summary>");
		builder.AppendFormat("public static class {0}", LAYER_FILENAME_WITHOUT_EXTENSION).AppendLine();
		builder.AppendLine("{");

		foreach (var n in InternalEditorUtility.layers.
			Select(c => new { var ="LAYER_"+ RemoveInvalidChars(c).ToUpper(), val = LayerMask.NameToLayer(c) }))
		{
			builder.Append("\t").AppendFormat(@"public const int {0} = {1};", n.var, n.val).AppendLine();
		}
		foreach (var n in InternalEditorUtility.layers.
			Select(c => new { var = RemoveInvalidChars(c).ToUpper(), val = 1 << LayerMask.NameToLayer(c) }))
		{
			builder.Append("\t").AppendFormat(@"public const int {0}Mask = {1};", n.var, n.val).AppendLine();
		}

		builder.AppendLine("}");

		var directoryName = Path.GetDirectoryName(LAYER_EXPORT_PATH);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}

		File.WriteAllText(LAYER_EXPORT_PATH, builder.ToString(), Encoding.UTF8);
		AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
	}

	/// <summary>
	/// SceneNameスクリプトを作成します
	/// </summary>
	public static void SceneNameScriptCreate()
	{
		var builder = new StringBuilder();

		builder.AppendLine("/// <summary>");
		builder.AppendLine("/// シーン名を定数で管理するクラス");
		builder.AppendLine("/// </summary>");
		builder.AppendFormat("public static class {0}", SCENE_FILENAME_WITHOUT_EXTENSION).AppendLine();
		builder.AppendLine("{");

		foreach (var n in EditorBuildSettings.scenes
			.Select(c => Path.GetFileNameWithoutExtension(c.path))
			.Distinct()
			.Select(c => new { var = "SCENE_"+RemoveInvalidChars(c).ToUpper(), val = c }))
		{
			builder.Append("\t").AppendFormat(@"public const string {0} = ""{1}"";", n.var, n.val).AppendLine();
		}

		builder.AppendLine("}");

		var directoryName = Path.GetDirectoryName(SCENE_EXPORT_PATH);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}

		File.WriteAllText(SCENE_EXPORT_PATH, builder.ToString(), Encoding.UTF8);
		AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
	}

	/// <summary>
	/// TagNameスクリプトを作成します
	/// </summary>
	public static void TagNameScriptCreate()
	{
		var builder = new StringBuilder();

		builder.AppendLine("/// <summary>");
		builder.AppendLine("/// タグ名を定数で管理するクラス");
		builder.AppendLine("/// </summary>");
		builder.AppendFormat("public static class {0}", TAG_FILENAME_WITHOUT_EXTENSION).AppendLine();
		builder.AppendLine("{");

		foreach (var n in InternalEditorUtility.tags.
			Select(c => new { var ="TAG_"+ RemoveInvalidChars(c).ToUpper(), val = c }))
		{
			builder.Append("\t").AppendFormat(@"public const string {0} = ""{1}"";", n.var, n.val).AppendLine();
		}

		builder.AppendLine("}");

		var directoryName = Path.GetDirectoryName(TAG_EXPORT_PATH);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}

		File.WriteAllText(TAG_EXPORT_PATH, builder.ToString(), Encoding.UTF8);
		AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
	}

	/// <summary>
	/// EffectNameスクリプトを作成します
	/// </summary>
	public static void EffectNameScriptCreate()
	{
		StringBuilder builder = new StringBuilder();

		builder.AppendLine("/// <summary>");
		builder.AppendLine("/// エフェクト名を定数で管理するクラス");
		builder.AppendLine("/// </summary>");
		builder.AppendFormat("public static class {0}", EFFECT_FILENAME_WITHOUT_EXTENSION).AppendLine();
		builder.AppendLine("{");

		//指定したパスのリソースを全て取得
		object[] EffectList = Resources.LoadAll("Effects/");

		foreach (GameObject effect in EffectList)
		{
			builder.Append("\t").AppendFormat(@"  public const string EFFECT_{0} = ""{1}"";", effect.name.ToUpper(), effect.name).AppendLine();
		}
		
		builder.AppendLine("}");

		string directoryName = Path.GetDirectoryName(EFFECT_EXPORT_PATH);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}

		File.WriteAllText(EFFECT_EXPORT_PATH, builder.ToString(), Encoding.UTF8);
		AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
	}

	/// <summary>
	/// 無効な文字を削除します
	/// </summary>
	public static string RemoveInvalidChars(string str)
	{
		Array.ForEach(INVALUD_CHARS, c => str = str.Replace(c, string.Empty));
		return str;
	}
}
