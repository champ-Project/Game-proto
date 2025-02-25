using UnityEngine;

public enum ItemType
{
    none,
    item,
    notePage,
    storyPaper,
    inspectItem
}

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemDataAsset")]
public class ItemData : ScriptableObject
{
    public string itemCode;
    public ItemType itemType;
    public string itemName;
    public Sprite itemImage;
    public string iteminfo;
    [Header("AdditionalOption")]
    public string text;
    public GameObject prefab;

    /*    [System.Serializable]
        public struct ItemData
        {


        }*/
}
