namespace Anipen
{
    using System;
    using System.Collections.Generic;

    public class Command : ICommand
    {
        #region Members
        public CommandType excutableType { get; protected set; }
        #endregion

        #region Members : Data
        protected Dictionary<string, object> result;
        #endregion

        #region Properties
        public bool success { get; protected set; }
        public string errMsg { get; protected set; }
        #endregion

        #region Delegate
        protected Action<Dictionary<string, object>> onFinishCommand;
        #endregion

        #region Constructors
        public Command() { result = new Dictionary<string, object>(); }
        #endregion

        #region Methods 
        public virtual void RegisterResultCallback(Action<Dictionary<string, object>> resultcallback) { onFinishCommand = resultcallback; }
        public void AddResultData(string key, object value) { result[key] = value; }

        public void SetResult(bool isSuccess, string errMsg = null)
        {
            success = isSuccess;
            this.errMsg = errMsg;
        }
        #endregion

        #region Implementation : ICommand
        public virtual void Request() { CommandController.Instance.SendCommand(this); }

        public virtual void Finish()
        {
            UnityEngine.Debug.Log("finish command : " + result.Count);
            onFinishCommand?.Invoke(result);
            Dispose();
        }

        public virtual void Dispose()
        {
            result.Clear();
            onFinishCommand = null;
        }
        #endregion
    }
}