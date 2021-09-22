using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    [SerializeField] private GameObject tip;
    [SerializeField] private GameObject beforePic;

    private const string Player = "Player";

    // Start is called before the first frame update
    private void Awake()
    {
        tip.SetActive(false);
        beforePic.SetActive(false);
    }

    // Update is called once per frame

    private void OnTriggerEnter2D(Collider2D col2d)
    {
        if (col2d.CompareTag(Player))
        {
            Debug.Log("player detect");
            tip.SetActive(true);
            
        }

    }
    private void OnTriggerStay2D(Collider2D colstay)
    {
        if (colstay.CompareTag(Player) && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("player watch");
            beforePic.SetActive(true);
        }
    }




    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Player))
        {
            Debug.Log("player out");
            tip.SetActive(false);
           
        }
       
    }

    public void Close()
    {
        beforePic.SetActive(false);
    }





}