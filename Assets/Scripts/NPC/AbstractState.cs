//tutorial from:https://www.youtube.com/watch?v=21yDDUKCQOI

using NPC.Sight.Scripts;
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
        Attack,
    }
    public abstract class AbstractState : ScriptableObject
    {
        protected NavMeshAgent NavMeshAgent;
        protected Enemy Enemy;
        protected FiniteStateMachine Fsm;
        public Execution Execution { get; protected set; }
        public bool EnteredState { get; protected set; }
        public FSMStateType StateType { get; protected set; }
        protected FieldOfView FOV;
        protected Animator Animator;

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
            successNavMesh = NavMeshAgent != null;
            successNPC = Enemy != null;
            
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
                NavMeshAgent = navMeshAgent;
            }
        }

        public virtual void SetExecutingEnemy(Enemy enemy)
        {
            if (enemy != null)
            {
                Enemy = enemy;
            }
        }

        public virtual void SetExecutingFiniteStateMachine(FiniteStateMachine fsm)
        {
            if (fsm != null)
            {
                 Fsm= fsm;
            }
        }
        
        public virtual void SetExecutingFOV(FieldOfView fov)
        {
            if (fov != null)
            {
                FOV= fov;
            }
        }
        
        public virtual void SetExecutingAnimator(Animator animator)
        {
            if (animator != null)
            {
                Animator= animator;
            }
        }
    }
}
