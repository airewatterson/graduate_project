using UnityEngine;

namespace NPC.States
{
    [CreateAssetMenu(fileName = "IdleState", menuName = "EnemyStates/Idle", order = 1)]
    public class Idle : AbstractState
    {
        [SerializeField] private float _idleDuration = 3f;

        private float _totalDuration;
        public override void OnEnable()
        {
            base.OnEnable();
            StateType = FSMStateType.Idle;
        }

        public override bool EnterState()
        {
            EnteredState = base.EnterState();
            if (EnteredState)
            {
                Debug.Log("IdleState");
                _totalDuration = 0f;
            }
            return EnteredState;
        }

        public override void UpdateState()
        {
            if (EnteredState)
            {
                _totalDuration += Time.deltaTime;

                Debug.Log("Now executing IdleState for" + _totalDuration + "seconds.");

                if (_totalDuration >= _idleDuration)
                {
                    _fsm.EnterState(FSMStateType.Patrol);
                }
            }
            
        }

        public override bool LeaveState()
        { 
            base.LeaveState();
            Debug.Log("Exiting IdleState");
            return true;
        }
    }
}