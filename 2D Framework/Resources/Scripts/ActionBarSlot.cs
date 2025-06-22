using UnityEngine;

[System.Serializable]
public class ActionBarSlot
{
    public KeyCode hotkey;
    public ItemData item;
    public float lastUseTime = -999f;
}
