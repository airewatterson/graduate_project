using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NPC
{
    public class FiniteStateMachine : MonoBehaviour
    {
        private AbstractState _currentState;

        [SerializeField] private List<AbstractState> validStates;
        private Dictionary<FSMStateType, AbstractState> _fsmStates;

        private void Awake()
        {
            _currentState = null;

            _fsmStates = new Dictionary<FSMStateType, AbstractState>();
            Enemy enemy = GetComponent<Enemy>();
            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
            foreach (var state in validStates)
            {
                state.SetExecutingFiniteStateMachine(this);
                state.SetExecutingEnemy(enemy);
                state.SetNavMeshAgent(navMeshAgent);
                
                _fsmStates.Add(state.StateType, state);
            }
        }

        private void Start()
        {
            EnterState(FSMStateType.Idle);
        }

        private void Update()
        {
            _currentState.UpdateState();
        }


        #region STATES

        public void EnterState(AbstractState tract)
        {
            if (tract == null)
            {
                return;
            }

            if (_currentState != null)
            {
                _currentState.LeaveState();
            }

            _currentState = tract;
            _currentState.EnterState();
        }
        
        public void EnterState(FSMStateType tract)
        {
            if (_fsmStates.ContainsKey(tract))
            {
                AbstractState nextState = _fsmStates[tract];
                
                EnterState(nextState);
            }
        }

        #endregion

        
    }
}
