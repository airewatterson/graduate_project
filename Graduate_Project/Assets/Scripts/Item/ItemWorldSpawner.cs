using General;
using UnityEngine.Serialization;

namespace Item
{
    //https://www.youtube.com/watch?v=2WnAOV7nHW0
    public class ItemWorldSpawner : SingletonMonoBehavior<ItemWorldSpawner>
    {
        [FormerlySerializedAs("Item")] public ItemController item;

        
        //Use Start instead of Awake, or will cause mistiming error, like Instance.....
        private void Start()
        {
            ItemWorld.SpawnItemWorld(item,transform.position);
            Destroy(gameObject);
        }
    }
}
