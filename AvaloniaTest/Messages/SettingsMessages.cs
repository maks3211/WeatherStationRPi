

using CommunityToolkit.Mvvm.Messaging.Messages;
using System;

namespace AvaloniaTest.Messages;

public class ThemeBtnVisMessage : ValueChangedMessage<bool>
{
    public ThemeBtnVisMessage(bool value) : base(value)
    {
    }
}

public class AutoThemeMessage : ValueChangedMessage<bool>
{
    public AutoThemeMessage(bool value) : base(value)
    {
    }
}

public class UnitChangedMessage : ValueChangedMessage<bool>
{
    public UnitChangedMessage(bool value) : base(value)
    {
    }
}
