//tutorial from:https://www.youtube.com/watch?v=21yDDUKCQOI

using UnityEngine;
using UnityEngine.AI;

namespace NPC
{
    public enum Execution
    {
        None,
        Active,
        Completed,
        Terminated,
    }

    public enum FSMStateType
    {
        Idle,
        Patrol,
    }
    public abstract class AbstractState : ScriptableObject
    {
        protected NavMeshAgent _navMeshAgent;
        protected Enemy _enemy;
        protected FiniteStateMachine _fsm;
        public Execution Execution { get; protected set; }
        public bool EnteredState { get; protected set; }
        public FSMStateType StateType { get; protected set; }

        public virtual void OnEnable()
        {
            Execution = Execution.None;
        }

        public virtual bool EnterState()
        {
            var successNavMesh = true;
            var successNPC = true;
            Execution = Execution.Active;
            //檢查NavMeshAgent是否存在
            successNavMesh = _navMeshAgent != null;
            successNPC = _enemy != null;
            
            return successNavMesh & successNPC;
        }

        public abstract void UpdateState();

        public virtual bool LeaveState()
        {
            Execution = Execution.Completed;
            return true;
        }

        public virtual void SetNavMeshAgent(NavMeshAgent navMeshAgent)
        {
            if (navMeshAgent != null)
            {
                _navMeshAgent = navMeshAgent;
            }
        }

        public virtual void SetExecutingEnemy(Enemy enemy)
        {
            if (enemy != null)
            {
                _enemy = enemy;
            }
        }

        public virtual void SetExecutingFiniteStateMachine(FiniteStateMachine fsm)
        {
            if (fsm != null)
            {
                 _fsm= fsm;
            }
        }
        
    }
}
