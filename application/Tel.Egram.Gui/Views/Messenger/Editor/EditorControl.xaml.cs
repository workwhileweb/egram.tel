﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PropertyChanged;

namespace Tel.Egram.Gui.Views.Messenger.Editor
{
    public class EditorControl : UserControl
    {
        public EditorControl()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}