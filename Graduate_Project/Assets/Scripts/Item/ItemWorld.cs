using CodeMonkey.Utils;
using General;
using TMPro;
using UnityEngine;

namespace Item
{
    public class ItemWorld : SingletonMonoBehavior<ItemWorld>
    {

        public static ItemWorld SpawnItemWorld(ItemController item , Vector3 position)
        {
            var transform = Instantiate(ItemAssets.Instance.prefabItem, position ,Quaternion.identity);
            
            var itemWorld = transform.GetComponent<ItemWorld>();
            itemWorld.SetItem(item);
            
            return itemWorld;
        }

        public static ItemWorld DropItem(Vector3 dropPosition, ItemController item)
        {
            Vector3 randomDir = UtilsClass.GetRandomDir();
            ItemWorld itemWorld = SpawnItemWorld(item,dropPosition + randomDir * 5f);
            itemWorld.GetComponent<Rigidbody>().AddForce(randomDir * 5f , ForceMode.Impulse);
            return itemWorld;
        }
        
        private ItemController _item;
        private SpriteRenderer _spriteRenderer;
        private TextMeshPro _textMeshPro;

        public override void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
        }

        public void SetItem(ItemController item)
        {
            _item = item;
            _spriteRenderer.sprite = _item.GetSprite();
            if (item.amount>1)
            {
                _textMeshPro.SetText(_item.amount.ToString());
            }
            else
            {
                _textMeshPro.SetText("");
            }
            
        }

        public ItemController GetItem()
        {
            return _item;
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
