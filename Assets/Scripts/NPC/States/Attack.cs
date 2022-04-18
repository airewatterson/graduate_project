using System;
using DamageSys;
using NPC.Sight.Scripts;
using UnityEngine;
using Weapons;

namespace NPC.States
{
    [CreateAssetMenu(fileName = "AttackState", menuName = "EnemyStates/Attack", order = 3)]
    public class Attack : AbstractState
    {
        [SerializeField] private Material fovVisible;
        private Transform _target;
        
        public override void OnEnable()
        {
            base.OnEnable();
            StateType = FSMStateType.Attack;
        }

        public override bool EnterState()   
        {
            EnteredState = base.EnterState();
            if (EnteredState)
            {
                
                if(FOV.findPlayer)
                {
                    _target = FOV.visibleTargets[0].transform;
                    if (_target.childCount <= 0)
                    {
                        Debug.LogError("Not founded player yet.");
                    }
                    NavMeshAgent.speed = 0f;
                    Debug.Log("Attacking");
                    fovVisible.color = Color.red;
                    Animator.SetBool("isRunning",false);
                    
                    //Attacking
                    Vector3 relativePos = _target.position - Enemy.transform.position;
                    Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.forward);
                    Enemy.transform.rotation = rotation;
                    
                }
            }

            return EnteredState;
        }

        public override void UpdateState()
        {
            if (EnteredState)
            {
                if (FOV.findPlayer == false)
                {
                    Fsm.EnterState(FSMStateType.Idle);
                }
            }
        }


        
    }
}