using UnityEngine;

public class ChatConfig
{
    // This is the chat types with the ORDER THEY APPEAR in the dropdown
    public const int TYPE_AREA = 0;
    public const int TYPE_PARTY = 1;
    public const int TYPE_WHISPER = 2;

    public static int[] TYPES = {TYPE_AREA, TYPE_PARTY, TYPE_WHISPER};

    // Chat colors
    public static Color COLOR_AREA = new Color(248f / 255f, 248f / 255f, 248f / 255f);
    public static Color COLOR_PARTY = new Color(248 / 255f, 160f / 255f, 160f / 255f);
    public static Color COLOR_WHISPER = new Color(93f / 255f, 183f / 255f, 93f / 255f);

    // Shortcuts
    public const string SHORTCUT_AREA = "/a";
    public const string SHORTCUT_PARTY = "/p";
    public const string SHORTCUT_WHISPER = "/w";
    public const string SHORTCUT_REPLY = "/r";
    // Keep in this order! it's synced with the types
    public static string[] SHORTCUTS = {SHORTCUT_AREA, SHORTCUT_PARTY, SHORTCUT_WHISPER, /* Keep this one last! */ SHORTCUT_REPLY};

    public const string PLACEHOLDER_WELCOME = "Welcome to XPlora!";
    public const string PLACEHOLDER_HOW_TO_SWITCH = "Switch chats with: " + SHORTCUT_AREA + " (area), " + SHORTCUT_PARTY + " (party), " + SHORTCUT_WHISPER + " (whisper) and " + SHORTCUT_REPLY + " (reply).";
    public const string PLACEHOLDER_AREA = "Talk to everyone...";
    public const string PLACEHOLDER_PARTY = "Party time!";
    public const string PLACEHOLDER_WHISPER = "Whisper. Example: \"Jon Hi there\"";
    public static string[] PLACEHOLDERS = {PLACEHOLDER_AREA, PLACEHOLDER_PARTY, PLACEHOLDER_WHISPER};
}