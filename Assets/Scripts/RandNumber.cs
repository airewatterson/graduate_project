using TMPro;
using UnityEngine;

public class RandNumber : MonoBehaviour
{
    private TextMeshProUGUI _percent;
    // Start is called before the first frame update
    void Start()
    {
        _percent = GetComponent<TextMeshProUGUI>();
        _percent.text = Random.Range(80, 98) + "%";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
