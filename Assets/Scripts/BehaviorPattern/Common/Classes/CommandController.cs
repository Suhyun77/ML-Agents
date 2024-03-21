namespace Anipen
{
    using System.Collections.Generic;
    using UnityEngine;

    public enum CommandType { Network, Messenger, UI, NotificationMessage, Animation }

    public class CommandController : Singleton<CommandController>
    {
        #region Members
        private List<ICommandExecutor> executors;
        #endregion

        #region Constructor
        public CommandController() { executors = new List<ICommandExecutor>(); }
        #endregion

        #region Methods
        public void Register(ICommandExecutor executor)
        {
            if (!executors.Contains(executor))
                executors.Add(executor);
        }

        public void UnRegister(ICommandExecutor excutor) { executors.Remove(excutor); }

        public void SendCommand(Command command)
        {
            //Debug.Log("Current executor count : " + executors.Count);
            Debug.Log("Send command : " + command.GetType());
            foreach (var executor in executors)
            {
                if (command.excutableType == executor.availableType)
                    executor.Execute(command);
            }
        }
        #endregion
    }
}
