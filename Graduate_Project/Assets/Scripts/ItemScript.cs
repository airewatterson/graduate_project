using General;
using UnityEngine;

public class ItemScript : SingletonMonoBehavior<ItemScript>
{
    [SerializeField] private GameObject keyboardHint;
    
    public void ApproachItem()
    {
        keyboardHint.SetActive(true);
    }

    public void UnApproachItem()
    {
        keyboardHint.SetActive(false);
    }
}
