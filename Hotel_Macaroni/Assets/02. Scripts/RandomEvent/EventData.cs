using UnityEngine;


public enum EventType
{
    AllFloor,
    _7F,
    _8F,
    _9F,
    _1F,
    _2F,
    SP
}

[CreateAssetMenu(fileName = "EventData", menuName = "ScriptableObjects/EventDataAsset")]
public class EventData : ScriptableObject
{
    public EventType eventType;
    public string eventCode;
    public GameObject eventObject;
}
