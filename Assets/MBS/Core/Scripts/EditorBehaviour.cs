#if UNITY_EDITOR
using UnityEngine;

namespace MBS
{
    public abstract class EditorBehaviour : MonoBehaviour
    {
        void OnDestroy()
        {
            if (Event.current != null)
                if (Event.current.commandName == "Delete" || Event.current.commandName == "SoftDelete")
                {
                    DoOnSoftDelete();
                    return;
                }
            DoOnDestroy();
        }

        public virtual void DoOnDestroy() { }

        public virtual void DoOnSoftDelete() { }


    }
}
#endif
