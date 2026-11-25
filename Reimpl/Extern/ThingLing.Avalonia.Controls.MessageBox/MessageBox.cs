using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Threading.Tasks;
using Avalonia.Media;
using JetBrains.Annotations;

namespace ThingLing.Controls
{
    /// <summary>
    /// Displays a message box.
    /// </summary>
    [PublicAPI]
    public class MessageBox
    {
        private const double UniformThickness = 2;

        private static MainWindow CreateDialogWindow(string message, string title = "", bool isOkVisible = true, MessageBoxButton? button = null, MessageBoxImage? icon = null, MessageBoxResult? defaultResult = null)
        {
            MainWindow window = new() { MessageTextBlock = { Text = message }, TitleTextBlock = { Text = title }, OkButton = { IsVisible = isOkVisible}};

            ApplyButton(button, window);
            ApplyIcon(icon, window);
            ApplyDefaultResult(defaultResult, window);
            
            return window;
        }

        private static void ApplyDefaultResult(MessageBoxResult? defaultResult, MainWindow window)
        {
            if (defaultResult is not null)
            {
                window.OkButton.IsDefault = false;
            }
            
            switch (defaultResult)
            {
                case null:
                case MessageBoxResult.None:
                    break;
                case MessageBoxResult.Ok:
                    window.OkButton.IsDefault = true;
                    window.OkButton.BorderThickness = new Thickness(UniformThickness);
                    break;
                case MessageBoxResult.Cancel:
                    window.CancelButton.IsDefault = true;
                    window.CancelButton.BorderThickness = new Thickness(UniformThickness);
                    break;
                case MessageBoxResult.Yes:
                    window.YesButton.IsDefault = true;
                    window.YesButton.BorderThickness = new Thickness(UniformThickness);
                    break;
                case MessageBoxResult.No:
                    window.NoButton.IsDefault = true;
                    window.NoButton.BorderThickness = new Thickness(UniformThickness);
                    break;
                default:
                    window.OkButton.IsDefault = true;
                    window.OkButton.BorderThickness = new Thickness(UniformThickness);
                    break;
            }
        }

        private static void ApplyIcon(MessageBoxImage? icon, MainWindow window)
        {
            switch (icon)
            {
                case null:
                case MessageBoxImage.None:
                    window.IconImage.IsVisible = false;
                    break;
                case MessageBoxImage.Error:
                    window.IconImage.Source =
                        LoadBitmap("avares://ThingLing.Controls.MessageBox/Images/delete.png");
                    break;
                case MessageBoxImage.Stop:
                    window.IconImage.Source =
                        LoadBitmap("avares://ThingLing.Controls.MessageBox/Images/No-entry.png");
                    break;
                case MessageBoxImage.Warning:
                    window.IconImage.Source =
                        LoadBitmap("avares://ThingLing.Controls.MessageBox/Images/Warning.png");
                    break;
                case MessageBoxImage.Information:
                    window.IconImage.Source =
                        LoadBitmap("avares://ThingLing.Controls.MessageBox/Images/Info.png");
                    break;
            }
        }

        private static void ApplyButton(MessageBoxButton? button, MainWindow window)
        {
            switch (button)
            {
                case MessageBoxButton.Ok:
                    window.OkButton.IsVisible = true;
                    break;
                case MessageBoxButton.YesNo:
                    window.YesButton.IsVisible = true;
                    window.NoButton.IsVisible = true;
                    break;
                case MessageBoxButton.YesNoCancel:
                    window.YesButton.IsVisible = true;
                    window.NoButton.IsVisible = true;
                    window.CancelButton.IsVisible = true;
                    break;
                case MessageBoxButton.OkCancel:
                    window.CancelButton.IsVisible = true;
                    window.OkButton.IsVisible = true;
                    break;
                default:
                    window.OkButton.IsVisible = true;
                    break;
            }
        }

        private static async Task<MessageBoxResult> Show(MainWindow window, Window? owner = null)
        {
            if (owner is null && Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                owner = lifetime.MainWindow;
            }
            
            ArgumentNullException.ThrowIfNull(owner);

            return await window.ShowDialog<MessageBoxResult>(owner);
        }
        
        private static IImage LoadBitmap(string uri)
        { 
            return new Bitmap(AssetLoader.Open(new Uri(uri)));
        }

        /// <summary>
        ///  Displays a message box that has a message and that returns a result.
        /// </summary>
        /// <param name="message">A System.String that specifies the text to display.</param>
        /// <returns>A ThingLing.Controls.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static async Task<MessageBoxResult> ShowAsync(string message) => await Show(CreateDialogWindow(message));

        /// <summary>
        ///  Displays a message box that has a message and that returns a result.
        /// </summary>
        /// <param name="owner">The main window that hosts this message box</param>
        /// <param name="message">A System.String that specifies the text to display.</param>
        /// <returns>A ThingLing.Controls.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static async Task<MessageBoxResult> ShowAsync(Window owner, string message) => await Show(CreateDialogWindow(message));

        /// <summary>
        /// Displays a message box that has a message and title bar caption; and that returns a result.
        /// </summary>
        /// <param name="message">A System.String that specifies the text to display.</param>
        /// <param name="title">A System.String that specifies the title bar caption to display.</param>
        /// <returns>A ThingLing.Controls.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static async Task<MessageBoxResult> ShowAsync(string message, string title) => await Show(CreateDialogWindow(message, title));

        /// <summary>
        /// Displays a message box that has a message and title bar caption; and that returns a result.
        /// </summary>
        /// <param name="owner">The main window that hosts this message box</param>
        /// <param name="message">A System.String that specifies the text to display.</param>
        /// <param name="title">A System.String that specifies the title bar caption to display.</param>
        /// <returns>A ThingLing.Controls.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static async Task<MessageBoxResult> ShowAsync(Window owner, string message, string title) => await Show(CreateDialogWindow(message, title));
        

        /// <summary>
        /// Displays a message box that has a message and title bar caption; and that returns a result.
        /// </summary>
        /// <param name="message">A System.String that specifies the text to display.</param>
        /// <param name="title">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button"> A ThingLing.Controls.MessageBoxButton value that specifies which button or buttons to display</param>
        /// <returns>A ThingLing.Controls.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static async Task<MessageBoxResult> ShowAsync(string message, string title,
            MessageBoxButton button)
            => await Show(CreateDialogWindow(message, title, false, button));

        /// <summary>
        /// Displays a message box that has a message and title bar caption; and that returns a result.
        /// </summary>
        /// <param name="owner">The main window that hosts this message box</param>
        /// <param name="message">A System.String that specifies the text to display.</param>
        /// <param name="title">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button"> A ThingLing.Controls.MessageBoxButton value that specifies which button or buttons to display</param>
        /// <returns>A ThingLing.Controls.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static async Task<MessageBoxResult> ShowAsync(Window owner, string message, string title,
            MessageBoxButton button)
            => await Show(CreateDialogWindow(message, title, false, button));

        /// <summary>
        /// Displays a message box that has a message and title bar caption; and that returns a result.
        /// </summary>
        /// <param name="message">A System.String that specifies the text to display.</param>
        /// <param name="title">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button"> A ThingLing.Controls.MessageBoxButton value that specifies which button or buttons to display</param>
        /// <param name="icon"> A ThingLing.Controls.MessageBoxImage value that specifies the icon to display.</param>
        /// <returns>A ThingLing.Controls.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static async Task<MessageBoxResult> ShowAsync(string message, string title,
            MessageBoxButton button, MessageBoxImage icon)
            => await Show(CreateDialogWindow(message, title, false, button, icon));

        /// <summary>
        /// Displays a message box that has a message and title bar caption; and that returns a result.
        /// </summary>
        /// <param name="owner">The main window that hosts this message box</param>
        /// <param name="message">A System.String that specifies the text to display.</param>
        /// <param name="title">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button"> A ThingLing.Controls.MessageBoxButton value that specifies which button or buttons to display</param>
        /// <param name="icon"> A ThingLing.Controls.MessageBoxImage value that specifies the icon to display.</param>
        /// <returns>A ThingLing.Controls.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static async Task<MessageBoxResult> ShowAsync(Window owner, string message, string title,
            MessageBoxButton button, MessageBoxImage icon)
            => await Show(CreateDialogWindow(message, title, false, button, icon));

        /// <summary>
        /// Displays a message box that has a message and title bar caption; and that returns a result.
        /// </summary>
        /// <param name="message">A System.String that specifies the text to display.</param>
        /// <param name="title">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button"> A ThingLing.Controls.MessageBoxButton value that specifies which button or buttons to display</param>
        /// <param name="icon"> A ThingLing.Controls.MessageBoxImage value that specifies the icon to display.</param>
        /// <param name="defaultResult"> A ThingLing.Controls.MessageBoxResult value that specifies the default result of the message box.</param>
        /// <returns>A ThingLing.Controls.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static async Task<MessageBoxResult> ShowAsync(string message, string title,
            MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
            => await Show(CreateDialogWindow(message, title, false, button, icon, defaultResult));

        /// <summary>
        /// Displays a message box that has a message and title bar caption; and that returns a result.
        /// </summary>
        /// <param name="owner">The main window that hosts this message box</param>
        /// <param name="message">A System.String that specifies the text to display.</param>
        /// <param name="title">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button"> A ThingLing.Controls.MessageBoxButton value that specifies which button or buttons to display</param>
        /// <param name="icon"> A ThingLing.Controls.MessageBoxImage value that specifies the icon to display.</param>
        /// <param name="defaultResult"> A ThingLing.Controls.MessageBoxResult value that specifies the default result of the message box.</param>
        /// <returns>A ThingLing.Controls.MessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        public static async Task<MessageBoxResult> ShowAsync(Window owner, string message, string title,
            MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
            => await Show(CreateDialogWindow(message, title, false, button, icon, defaultResult));
    }
}