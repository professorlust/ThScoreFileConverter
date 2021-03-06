﻿//-----------------------------------------------------------------------
// <copyright file="CloseWindowCommand.cs" company="None">
//     (c) 2015 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ThScoreFileConverter.Commands
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Represents the command to close the specified window.
    /// </summary>
    public class CloseWindowCommand : ICommand
    {
        /// <summary>
        /// Only one instance of this class.
        /// </summary>
        private static readonly ICommand InstanceImpl = new CloseWindowCommand();

        /// <summary>
        /// Prevents a default instance of the <see cref="CloseWindowCommand"/> class from being created.
        /// </summary>
        private CloseWindowCommand()
        {
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static ICommand Instance
        {
            get { return InstanceImpl; }
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">A <see cref="Window"/> instance which will be closed.</param>
        /// <returns><c>true</c> if this command can be executed; otherwise, <c>false</c>.</returns>
        public bool CanExecute(object parameter)
        {
            return parameter is Window;
        }

        /// <summary>
        /// Called when the command is invoked.
        /// </summary>
        /// <param name="parameter">A <see cref="Window"/> instance which is closed.</param>
        public void Execute(object parameter)
        {
            if (this.CanExecute(parameter))
                (parameter as Window).Close();
        }
    }
}
