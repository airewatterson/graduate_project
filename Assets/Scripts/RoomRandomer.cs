using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomRandomer : MonoBehaviourPun
{
    [SerializeField] private int roomChoose;

    [SerializeField] private GameObject scene1;
    [SerializeField] private GameObject scene2;
    [SerializeField] private GameObject scene3;
    // Start is called before the first frame update
    private void Start()
    {
        roomChoose = Random.Range(1, 3);
        
    }

    private void Update()
    {
        Chooser();
    }

    private void Chooser()
    {
        switch (roomChoose)
        {
            case 1:
                scene1.SetActive(true);
                scene2.SetActive(false);
                scene3.SetActive(false);
                break;
            case 2:
                scene1.SetActive(false);
                scene2.SetActive(true);
                scene3.SetActive(false);
                break;
            case 3:
                scene1.SetActive(false);
                scene2.SetActive(false);
                scene3.SetActive(true);
                break;
            default:
                scene1.SetActive(false);
                scene2.SetActive(false);
                scene3.SetActive(false);
                break;
        }
    }
}
