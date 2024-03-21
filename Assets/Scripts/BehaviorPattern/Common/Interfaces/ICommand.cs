namespace Anipen
{
    using System;

    #region Command
    public interface ICommand : IDisposable
    {
        public void Request();
    }
    #endregion

    #region Excutor
    public interface ICommandExecutor
    {
        public CommandType availableType { get; set; }
        public void Execute(ICommand command);
    }
    #endregion
}
