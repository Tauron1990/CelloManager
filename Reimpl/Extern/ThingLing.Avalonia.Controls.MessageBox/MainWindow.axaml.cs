using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ThingLing.Controls
{
    internal partial class MainWindow : Window
    {
        private const double UniformThickness = 2;
        public MessageBoxResult? DefaultResult { get; set; }
        
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            
            InitializeComponent();
        }

        private void Window_OnKeyUp(object? sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close(MessageBoxResult.None);
                    break;
                case Key.Enter:
                    Close(DefaultResult ?? MessageBoxResult.None);
                    break;
                case Key.Left:
                    LeftKeyPressed();
                    break;
                case Key.Right:
                    RightKeyPressed();
                    break;
            }
        }

        // ReSharper disable once CognitiveComplexity
        private void RightKeyPressed()
        {
            if (OkButton.IsVisible && CancelButton.IsVisible)
            {
                if (OkButton.IsDefault)
                {
                    OkButton.IsDefault = false;
                    OkButton.BorderThickness = new Thickness(0);
                    CancelButton.IsDefault = true;
                    CancelButton.Focus();
                }
                else
                {
                    CancelButton.IsDefault = false;
                    CancelButton.BorderThickness = new Thickness(0);
                    OkButton.IsDefault = true;
                    OkButton.Focus();
                }
            }
            else
            {
                switch (YesButton.IsVisible)
                {
                    case true when NoButton.IsVisible && !CancelButton.IsVisible:
                    {
                        if (YesButton.IsDefault)
                        {
                            YesButton.IsDefault = false;
                            YesButton.BorderThickness = new Thickness(0);
                            NoButton.IsDefault = true;
                            NoButton.Focus();
                        }
                        else
                        {
                            NoButton.IsDefault = false;
                            NoButton.BorderThickness = new Thickness(0);
                            YesButton.IsDefault = true;
                            YesButton.Focus();
                        }

                        break;
                    }
                    case true when NoButton.IsVisible && CancelButton.IsVisible:
                    {
                        if (YesButton.IsDefault)
                        {
                            YesButton.IsDefault = false;
                            YesButton.BorderThickness = new Thickness(0);
                            NoButton.IsDefault = true;
                            NoButton.Focus();
                        }
                        else if (NoButton.IsDefault)
                        {
                            NoButton.IsDefault = false;
                            NoButton.BorderThickness = new Thickness(0);
                            CancelButton.IsDefault = true;
                            CancelButton.Focus();
                        }
                        else
                        {
                            CancelButton.IsDefault = false;
                            CancelButton.BorderThickness = new Thickness(0);
                            YesButton.IsDefault = true;
                            YesButton.Focus();
                        }

                        break;
                    }
                }
            }
        }

        // ReSharper disable once CognitiveComplexity
        private void LeftKeyPressed()
        {
            if (OkButton.IsVisible && CancelButton.IsVisible)
            {
                if (OkButton.IsDefault)
                {
                    OkButton.IsDefault = false;
                    OkButton.BorderThickness = new Thickness(0);
                    CancelButton.IsDefault = true;
                    CancelButton.Focus();
                }
                else
                {
                    CancelButton.IsDefault = false;
                    CancelButton.BorderThickness = new Thickness(0);
                    OkButton.IsDefault = true;
                    OkButton.Focus();
                }
            }
            else
            {
                switch (YesButton.IsVisible)
                {
                    case true when NoButton.IsVisible && !CancelButton.IsVisible:
                    {
                        if (YesButton.IsDefault)
                        {
                            YesButton.IsDefault = false;
                            YesButton.BorderThickness = new Thickness(0);
                            NoButton.IsDefault = true;
                            NoButton.Focus();
                        }
                        else
                        {
                            NoButton.IsDefault = false;
                            NoButton.BorderThickness = new Thickness(0);
                            YesButton.IsDefault = true;
                            YesButton.Focus();
                        }

                        break;
                    }
                    case true when NoButton.IsVisible && CancelButton.IsVisible:
                    {
                        if (YesButton.IsDefault)
                        {
                            YesButton.IsDefault = false;
                            YesButton.BorderThickness = new Thickness(0);
                            CancelButton.IsDefault = true;
                            CancelButton.Focus();
                        }
                        else if (NoButton.IsDefault)
                        {
                            NoButton.IsDefault = false;
                            NoButton.BorderThickness = new Thickness(0);
                            YesButton.IsDefault = true;
                            YesButton.Focus();
                        }
                        else
                        {
                            CancelButton.IsDefault = false;
                            CancelButton.BorderThickness = new Thickness(0);
                            NoButton.IsDefault = true;
                            NoButton.Focus();
                        }

                        break;
                    }
                }
            }
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e) => Close(MessageBoxResult.None);

        private void OKButton_Click(object? sender, RoutedEventArgs e) => Close(MessageBoxResult.Ok);

        private void YesButton_Click(object? sender, RoutedEventArgs e) => Close(MessageBoxResult.Yes);

        private void NoButton_Click(object? sender, RoutedEventArgs e) => Close(MessageBoxResult.No);

        private void CancelButton_Click(object? sender, RoutedEventArgs e) => Close(MessageBoxResult.Cancel);

        private void Button_OnGotFocus(object? sender, FocusChangedEventArgs e) => (sender as Button)?.BorderThickness = new Thickness(UniformThickness);

        private void HeaderPanel_OnPointerPressed(object? sender, PointerPressedEventArgs e) => BeginMoveDrag(e);
    }
}