public class ItemController
{
    /// <summary>
    /// Tutorial from https://youtu.be/2WnAOV7nHW0
    /// </summary>
    
    //指定有哪些物件可供使用
    public enum ItemDefine
    {
        Bomb,
        Landmine,
        Healing,
    }

    public ItemDefine itemDefine;
    public int amount;
}
