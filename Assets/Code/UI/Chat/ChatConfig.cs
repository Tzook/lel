using UnityEngine;

public class ChatConfig
{
    // This is the chat types with the ORDER THEY APPEAR in the dropdown
    public const int TYPE_AREA = 0;
    public const int TYPE_PARTY = 1;
    public const int TYPE_WHISPER = 2;

    // Chat colors
    public static Color COLOR_AREA = new Color(248f / 255f, 248f / 255f, 248f / 255f);
    public static Color COLOR_PARTY = new Color(248 / 255f, 160f / 255f, 160f / 255f);
    public static Color COLOR_WHISPER = new Color(93f / 255f, 183f / 255f, 93f / 255f);
}