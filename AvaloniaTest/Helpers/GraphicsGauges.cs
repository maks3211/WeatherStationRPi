using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Helpers
{
    public static class GraphicsGauges
    {
        private static readonly string[] _yellowcolorslist = { "#322c19", "#3d341a", "#534317", "#675114", "#795d12", "#8f6d0f", "#ffbc00" };
        private static readonly string[] _greencolorslist = { "#2b2727", "#2d3029", "#2f372b", "#31402d", "#334a30", "#355132", "#4dba50" };


        public static double GetCircleGaugeValue(double value)
        {
            return (60 - value / 100 * 60);
        }



        public static int GetPointPosition(int start, int minVal, int maxVal, double value)
        {
            double range = maxVal - minVal;
            double width = start + 145;
            int result = (int)(width * value / range);
            if (result > width)
            {
                result = (int)width;
            }
            return result - 2;
        }


        public static void SetBarColors(double value, double max, double min, string place, ObservableCollection<string> axamlColors)
        {
            string[] colors;
            axamlColors.Clear();
            if (place == "out")
            {
                colors = _greencolorslist;
            }
            else
            {
                colors = _yellowcolorslist;
            }
       
            int range = (int)((max - min) / 7);

            if (value <= min + range)
            {
                for (int i = 0; i < 6; i++)
                {
                    axamlColors.Add(colors[i]);
                }
            }
            else if (value > min && value <= min + 2 * range)
            {
                for (int i = 0; i < 5; i++)
                {
                    axamlColors.Add(colors[i]);
                }
                axamlColors.Add(colors[6]);
                axamlColors.Add(colors[6]);
            }
            else if (value > min + 2 * range && value <= min + 3 * range)
            {
                for (int i = 0; i < 4; i++)
                {
                    axamlColors.Add(colors[i + 1]);
                }
                axamlColors.Add(colors[6]);
                axamlColors.Add(colors[6]);
                axamlColors.Add(colors[6]);
            }
            else if (value > min + 3 * range && value <= min + 4 * range)
            {
                for (int i = 0; i < 3; i++)
                {
                    axamlColors.Add(colors[i + 2]);
                }
                axamlColors.Add(colors[6]);
                axamlColors.Add(colors[6]);
                axamlColors.Add(colors[6]);
                axamlColors.Add(colors[6]);
            }
            else if (value > min + 4 * range && value <= min + 5 * range)
            {
                for (int i = 0; i < 2; i++)
                {
                    axamlColors.Add(colors[i + 3]);
                }
                axamlColors.Add(colors[6]);
                axamlColors.Add(colors[6]);
                axamlColors.Add(colors[6]);
                axamlColors.Add(colors[6]);
                axamlColors.Add(colors[6]);
            }
            else if (value > min + 5 * range && value <= min + 6 * range)
            {
                axamlColors.Add(colors[4]);
                for (int i = 0; i < 6; i++)
                {
                    axamlColors.Add(colors[6]);
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    axamlColors.Add(colors[6]);
                }
            }

        }
    }

    }
