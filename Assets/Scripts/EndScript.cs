using UnityEngine;

public class EndScript : MonoBehaviour
{
    [SerializeField] private GameObject endPanel;
    [SerializeField] private GameObject mainPanel;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player.Input.Player>().isCollected)
        {
            endPanel.SetActive(true);
            mainPanel.SetActive(false);
        }
    }
}
