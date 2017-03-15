/// <summary>
/// レイヤー名を定数で管理するクラス
/// </summary>
public static class LAYER_NAME
{
	public const int LAYER_DEFAULT = 0;
	public const int LAYER_TRANSPARENTFX = 1;
	public const int LAYER_IGNORERAYCAST = 2;
	public const int LAYER_WATER = 4;
	public const int LAYER_UI = 5;
	public const int LAYER_GUN = 8;
	public const int DEFAULTMask = 1;
	public const int TRANSPARENTFXMask = 2;
	public const int IGNORERAYCASTMask = 4;
	public const int WATERMask = 16;
	public const int UIMask = 32;
	public const int GUNMask = 256;
}
