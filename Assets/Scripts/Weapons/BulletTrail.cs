using General;
using UnityEngine;

namespace Weapons
{
    public class BulletTrail : SingletonMonoBehavior<BulletTrail>
    {
        private Vector3 _startPosition;

        private Vector3 _targetPosition;

        private float _progress;
        
        [SerializeField]private float speed;

        // Start is called before the first frame update
        private void Start()
        {
            _startPosition = transform.position.WithAxis(Axis.Z, 0);
        }

        // Update is called once per frame
        private void Update()
        {
            _progress += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(_startPosition, _targetPosition, _progress);
        }

        public void SetTargetPosition(Vector3 targetPosition)
        {
            _targetPosition = targetPosition.WithAxis(Axis.Z, 0);
        }
    }
}
