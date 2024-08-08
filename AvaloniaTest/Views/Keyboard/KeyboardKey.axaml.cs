using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.ComponentModel;
using ZstdSharp.Unsafe;

namespace AvaloniaTest.Views;

public partial class KeyboardKey : UserControl
{
    public static readonly StyledProperty<string> PrimaryTextProperty =
               AvaloniaProperty.Register<KeyboardKey, string>(nameof(PrimaryText));

    public static readonly StyledProperty<string> SecondaryTextProperty =
        AvaloniaProperty.Register<KeyboardKey, string>(nameof(SecondaryText));



    private TextBlock _primaryTextBlock;
    private TextBlock _secondaryTextBlock;
  
    private  bool CapsLook = false;
    private  bool ShiftPressed = true;
    public string PrimaryText
    {
        get => GetValue(PrimaryTextProperty);
        set => SetValue(PrimaryTextProperty, value);
    }

    public string SecondaryText
    {
        get => GetValue(SecondaryTextProperty);
        set => SetValue(SecondaryTextProperty, value);
    }

    public string CurrentText => ShiftPressed ? SecondaryText : PrimaryText;


    public event EventHandler<string> KeyPressed;

    public KeyboardKey()
    {
       AvaloniaXamlLoader.Load(this);
       
    _primaryTextBlock = this.FindControl<TextBlock>("PrimaryTextBlock");
    _secondaryTextBlock = this.FindControl<TextBlock>("SecondaryTextBlock");
    }
   
    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        var textToSend = CurrentText;
        textToSend = CapsLook ? CurrentText.ToUpper() : CurrentText.ToLower();
        KeyPressed?.Invoke( this, textToSend ); 
    }

    public void UpdateText(bool shiftPressed)
    {
        ShiftPressed = shiftPressed;
        _primaryTextBlock.Text = PrimaryText;
        _secondaryTextBlock.Text = SecondaryText;

        _primaryTextBlock.IsVisible = !shiftPressed;
        _secondaryTextBlock.IsVisible = shiftPressed;
        if (SecondaryText == "" & shiftPressed)
        {
            this.IsEnabled = false;
        }
        else {
            this.IsEnabled = true;
        }
        CapsLook = false;
}

    public void ToggleFontSize()
    {
        CapsLook = !CapsLook;
        if (!CapsLook)
        {
            _primaryTextBlock.Text = PrimaryText.ToLower();
            _secondaryTextBlock.Text = SecondaryText.ToLower();
           
        }      
        else
        {
            _primaryTextBlock.Text = PrimaryText.ToUpper();
            _secondaryTextBlock.Text = SecondaryText.ToUpper();  
        }
       
    }

}