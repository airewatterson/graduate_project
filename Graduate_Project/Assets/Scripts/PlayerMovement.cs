using General;
using UnityEngine;

public class PlayerMovement : SingletonMonoBehavior<PlayerMovement>
{
    [SerializeField]private float moveSp;
    [SerializeField]private float jumpForce = 1;
    private Rigidbody2D _rb2d;
    // Start is called before the first frame update
    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        var movement = Input.GetAxis("Horizontal");
        transform.position += new Vector3(movement, 0, 0) * Time.deltaTime * moveSp;
        if (Mathf.Approximately(0, movement))
        {
            transform.rotation = movement > 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }
        if (Input.GetKeyDown(KeyCode.Space) &&( Mathf.Abs(_rb2d.velocity.y) < 0.001f))
        {
            _rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }



    }
}
