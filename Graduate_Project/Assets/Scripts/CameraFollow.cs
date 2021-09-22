using General;
using UnityEngine;

public class CameraFollow : SingletonMonoBehavior<CameraFollow>

{

    [SerializeField] private GameObject followPlayer;
    [SerializeField] private float speed=2.5f;
    public Vector2 followOffset;
    private Vector2 _dis;
    private Rigidbody2D _rb2d;

    // Start is called before the first frame update
    private void Start()
    {
        _dis = CalDis();
        _rb2d = followPlayer.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
   
    private void LateUpdate()
    {
        Vector2 follow = followPlayer.transform.position;

        var position = transform.position;
        var difX = Vector2.Distance(Vector2.right * position.x, Vector2.right * follow.x);
        var difY = Vector2.Distance(Vector2.up * position.y, Vector2.up * follow.y);

        var newPos = position;

        if (Mathf.Abs(difX) >= _dis.x)
        {
            newPos.x = follow.x;

        }
        if (Mathf.Abs(difY) >= _dis.y)
        {
            newPos.y = follow.y;

        }

        var velocity = _rb2d.velocity;
        var moveSp = velocity.magnitude > speed ? velocity.magnitude : speed ;
        transform.position =Vector3.MoveTowards(transform.position,newPos, moveSp * Time.deltaTime);
    }

    private Vector3 CalDis()
    {
        if (Camera.main is { })
        {
            var main = Camera.main;
            var aspect = main.pixelRect;
            var orthographicSize = main.orthographicSize;
            var v2 = new Vector2(orthographicSize * aspect.width / aspect.height,
                orthographicSize);
            v2.x -= followOffset.x;
            v2.y -= followOffset.y;
            return v2;
        }

        return default;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 border = CalDis();
        Gizmos.DrawWireCube(transform.position, new Vector3(border.x * 2, border.y * 2, 1));
    }


}
