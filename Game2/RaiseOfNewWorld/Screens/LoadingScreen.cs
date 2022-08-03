using Game.Engine;
using Game.Engine.Packageing.ScriptHosting.Scripts;
using Terminal.Gui;

namespace RaiseOfNewWorld.Screens;

public sealed record LoadingParameter(Func<GameManager, IProgress<int>, ValueTask> StartProgress, int Max,
    string Title);

public sealed class LoadingScreen : ScreenBase, IProgress<int>
{
    private int _currentMax = -1;
    private Action<float> _setValue = _ => { };

    public void Report(int value)
    {
        if (_currentMax < 0) return;

        var result = _currentMax / 100d * value;
        result = result switch
        {
            > 100 => 100,
            < 0 => 0,
            _ => result
        };

        _setValue((float)(result / 100));
    }

    public override void Setup(Window container, GameManager gameManager, object? parameter)
    {
        if (parameter is not LoadingParameter loadingParameter)
            throw new InvalidOperationException("Not parameter Provided");

        container.Title = "Laden";

        var label = new Label
        {
            X = Pos.Center(),
            Y = Pos.Top(container) + 6,
            Text = loadingParameter.Title
        };

        _currentMax = loadingParameter.Max;

        var progress = new ProgressBar
        {
            X = Pos.Center(),
            Y = Pos.Top(label) + 2,
            Width = Dim.Percent(50),
            ProgressBarStyle = loadingParameter.Max < 1 ? ProgressBarStyle.MarqueeContinuous : ProgressBarStyle.Blocks
        };

        var cancellationToken = new CancellationTokenSource();

        if (loadingParameter.Max > 0)
        {
            _setValue = f => Application.MainLoop.Invoke(
                () =>
                {
                    progress.Fraction = f;
                    progress.Pulse();
                });
        }
        else
        {
            // ReSharper disable once MethodSupportsCancellation
            Task.Run(Pulser);

            async Task Pulser()
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    const float step = 0.2f;
                    var current = 0f;

                    Application.MainLoop.Invoke(
                        () =>
                        {
                            current += step;
                            if (current > 1)
                                current = 0;

                            progress.Pulse();
                        });
                    await Task.Delay(
                        200,
                        cancellationToken.Token).ConfigureAwait(false);
                }
            }
        }

        container.Add(
            label,
            progress);

        // ReSharper disable once MethodSupportsCancellation
        Task.Run(
            async () =>
            {
                try
                {
                    await loadingParameter.StartProgress(
                        gameManager,
                        this);
                }
                finally
                {
                    cancellationToken.Cancel();
                }
            });
    }
}