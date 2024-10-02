using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Messages;

    public class ViewActivatedMessages : ValueChangedMessage<string>
    {
        public ViewActivatedMessages(string value) : base(value)
        {
        }
    }

public class SettingsViewActivatedMessages : ValueChangedMessage<string>
{
    public SettingsViewActivatedMessages(string value) : base(value)
    {
    }
}

