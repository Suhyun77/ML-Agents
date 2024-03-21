namespace Anipen
{
    using System.Collections.Generic;
    using UnityEngine;

    public class ManagerBase<T> : SingletonGameObject<T>, IManager
        where T : MonoBehaviour
    {
        #region Constant
        protected const string Tag = "manager";
        #endregion

        #region Initialize
        public bool IsInit { get; protected set; }

        public virtual void Init() {
            IsInit = true;
        }
        public virtual void Reset() { }
        #endregion
    }

    public class ManagerWithState<T, U> : ManagerBase<T>, IStateSubject<U>
        where T : MonoBehaviour
        where U : System.Enum
    {
        #region Members
        protected List<IStateObserver<U>> stateObservers = new List<IStateObserver<U>>();
        protected U _currentState;
        protected U _prevState;

        public virtual U CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState.Equals(value))
                    return;

                _prevState = _currentState;
                _currentState = value;

                NotifyChangeState(value);
            }
        }

        public U PrevState => _prevState;
        #endregion

        #region Methods : Public
        public virtual void ReturnToPrevState()
        {
            CurrentState = _prevState;
        }
        #endregion

        #region State Subject
        public void AddObserver(IStateObserver<U> observer)
        {
            if (!stateObservers.Contains(observer))
                stateObservers.Add(observer);
        }
        public void RemoveObserver(IStateObserver<U> observer)
        {
            stateObservers.Remove(observer);
        }
        public virtual void NotifyChangeState(U state)
        {
            foreach (var observer in stateObservers)
                observer.Notify(state);
        }
        #endregion
    }
}