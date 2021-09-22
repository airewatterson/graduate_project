using General;
using UnityEngine;

public class TalkButton : SingletonMonoBehavior<TalkButton>
{
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject talkUI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        button.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        button.SetActive(false);
    }

    private void Update()
    {
        if (button.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            talkUI.SetActive(true);
        }
    }

}
