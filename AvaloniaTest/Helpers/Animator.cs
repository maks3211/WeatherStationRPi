using Avalonia.Animation.Easings;
using Avalonia.Animation;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Helpers
{
    public class Animator
    {


        public Control TargetElement { get; set; }


        public async void AnimateTest()
        {
            Console.WriteLine("NIE Animate idzie");
            if (TargetElement is not null) {
                Console.WriteLine("Animate idzie");
            var animation = new Avalonia.Animation.Animation
            {
                FillMode = FillMode.Forward,
                Duration = TimeSpan.FromMilliseconds(500),
                Easing = new SineEaseInOut(),
                Children =
            {
                new Avalonia.Animation.KeyFrame
                {
                    Setters = { new Setter(Visual.OpacityProperty, 0.0) },
                    Cue = new Cue(1d)
                },             
            }
            };

            await animation.RunAsync(TargetElement);

                var animation1 = new Avalonia.Animation.Animation
                {
                    FillMode = FillMode.Forward,
                    Duration = TimeSpan.FromMilliseconds(500),
                    Easing = new SineEaseInOut(),
                    Children =
            {
                new Avalonia.Animation.KeyFrame
                {
                    Setters = { new Setter(Visual.OpacityProperty, 1.0) },
                    Cue = new Cue(1d)
                },
            }
                };

                await animation1.RunAsync(TargetElement);
            }
        }
    }
}
