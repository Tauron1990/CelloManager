﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Implement
{
    /// <summary>The command line processor.</summary>
    [PublicAPI]
    public class CommandLineProcessor
    {
        /// <summary>The command.</summary>
        private class Command
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Command" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="Command" /> Klasse.
            ///     Initializes a new instance of the <see cref="Command" /> class.
            /// </summary>
            /// <param name="name">
            ///     The name.
            /// </param>
            public Command([NotNull] string name)
            {
                if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
                Name = name;
                Parms = new List<string>();
            }

            #endregion

            #region Public Properties

            /// <summary>Gets the name.</summary>
            /// <value>The name.</value>
            [NotNull]
            public string Name { get; }

            /// <summary>Gets the parms.</summary>
            /// <value>The parms.</value>
            [NotNull]
            public List<string> Parms { get; }

            #endregion
        }

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandLineProcessor" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="CommandLineProcessor" /> Klasse.
        ///     Initializes a new instance of the <see cref="CommandLineProcessor" /> class.
        /// </summary>
        /// <param name="application">
        ///     The application.
        /// </param>
        public CommandLineProcessor([NotNull] CommonApplication application)
        {
            if (application == null) throw new ArgumentNullException(nameof(application));
            _application = application;
            ParseCommandLine();
        }

        #endregion

        #region Fields

        /// <summary>The _application.</summary>
        private readonly CommonApplication _application;

        /// <summary>The _commands.</summary>
        private List<Command> _commands = new List<Command>();

        /// <summary>The _factory.</summary>
        private IShellFactory _factory;

        #endregion

        #region Public Methods and Operators

        /// <summary>The create shell view.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [CanBeNull]
        public object CreateShellView()
        {
            SelectViewFacotry();
            return _factory == null ? null : _factory.CreateView();
        }

        /// <summary>The execute commands.</summary>
        public void ExecuteCommands()
        {
            foreach (var fileCommand in
                _commands.Where(com => com.Name == "FileCommand").ToArray())
            {
                var commandPrcessor = _application.Container.Resolve<IFileCommand>(null, true);
                if (commandPrcessor == null) break;

                var file = fileCommand.Parms[0];

                commandPrcessor.ProcessFile(file);
                _factory = commandPrcessor.ProvideFactory();
            }

            foreach (var command in
                _application.Container.Resolve<ICommandLineService>().Commands)
            {
                var command1 = command;
                var temp = _commands.FirstOrDefault(arg => arg.Name == command1.CommandName);
                if (temp == null) continue;

                command.Execute(temp.Parms.ToArray(), _application.Container);
            }

            _commands = null;
        }

        #endregion

        #region Methods

        /// <summary>The parse command line.</summary>
        private void ParseCommandLine()
        {
            Command current = null;
            var first = true;
            foreach (var arg in _application.GetArgs())
            {
                if (first)
                {
                    first = false;
                    continue;
                }

                if (current == null && arg.ExisFile())
                {
                    var temp = new Command("FileCommand");
                    temp.Parms.Add(arg);
                    _commands.Add(temp);
                }

                if (arg.StartsWith("-", StringComparison.Ordinal))
                {
                    if (current != null) _commands.Add(current);

                    current = new Command(arg.TrimStart('-'));
                }
                else if (current != null)
                {
                    current.Parms.Add(arg);
                }
            }

            if (current != null && !_commands.Contains(current)) _commands.Add(current);
        }

        /// <summary>The select view facotry.</summary>
        private void SelectViewFacotry()
        {
            if (_factory != null) return;

            foreach (var command in
                _application.Container.Resolve<ICommandLineService>()
                    .Commands.Where(
                        command =>
                            command.Factory != null &&
                            _commands.Any(com => com.Name == command.CommandName))
            ) _factory = command.Factory;
        }

        #endregion
    }
}