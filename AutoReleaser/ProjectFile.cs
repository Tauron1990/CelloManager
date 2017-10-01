using AutoReleaser.Datastore;
using JetBrains.Annotations;
using Mvvm;

namespace AutoReleaser
{
    public sealed class ProjectFile : BindableBase
    {
        private readonly Options _options;
        private bool _isTest;
        private bool _isApp;

        [UsedImplicitly]
        public string Name { get; }

        [UsedImplicitly]
        public bool IsTest
        {
            get => _isTest;
            set
            {
                if (!SetProperty(ref _isTest, value)) return;

                _options.Tests[Name] = value;
                _options.Update();
            }
        }

        [UsedImplicitly]
        public bool IsApp
        {
            get => _isApp;
            set
            {
                if (!SetProperty(ref _isApp, value)) return;

                _options.Application[Name] = value;
                _options.Update();
            }
        }

        public ProjectFile(string name, Options options)
        {
            Name = name;
            _options = options;

            if (_options.Tests.ContainsKey(name))
                _isTest = _options.Tests[name];
            else
            {
                _isTest = false;
                _options.Tests[name] = false;
            }

            if (_options.Application.ContainsKey(name))
                _isApp = _options.Application[name];
            else
            {
                _isApp = false;
                _options.Application[name] = false;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}