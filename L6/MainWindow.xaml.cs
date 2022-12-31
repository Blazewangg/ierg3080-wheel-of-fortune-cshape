using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WOF
{
    public partial class MainWindow : Window
    {
        static int wheel_slices = 8;
        int money = 1000;
        double status;
        Path[] path = new Path[wheel_slices]; 
        TextBlock[] textBlocks = new TextBlock[wheel_slices];
        Random rand = new Random();
        Dictionary<Color, int> Award = new Dictionary<Color, int>()
        {
            { Colors.Red, 1000 },            //Red
            { Colors.LightSteelBlue, 100 },  //Blue
            { Colors.Aquamarine, 10 },       //Green
            { Colors.Gray, -500 },           //Grey
            { Colors.PeachPuff, 125 },
            { Colors.LightPink, -150 },
            { Colors.LightYellow, 550 },
        };
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            InitializeAdward();
            UpdateWallet();
            start_button.IsEnabled = true;
            stop_button.IsEnabled = false;
        }
        private void InitializeAdward()
        {
            // Initialize the wheel
            List<int> temp_award_allcate = new List<int>();
            for (int i = 0; i < wheel_slices; i++)
            {
                path[i] = pies.Children[2 * i] as Path;
                textBlocks[i] = pies.Children[2*i+1] as TextBlock;
            }
            // Fill adward information to the wheel
            for (int i = 0, rdm; i < wheel_slices; i++)
            {
                do  {
                    rdm = rand.Next(Award.Count);
                } while (temp_award_allcate.Contains(rdm) && temp_award_allcate.Count < Award.Count);
                temp_award_allcate.Add(rdm);
                // Update color and text of award
                path[i].Fill = new SolidColorBrush(Award.Keys.ElementAt(rdm));
                textBlocks[i].Text = Award.Values.ElementAt(rdm).ToString();
            }
        }
        // Every second past, trigger this call
        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            // Float is to increase randomness as it prevents returning constant
            if (status >= 2) status += 0.03 * (1 + status * status * 0.01);
            if (status > 200){
                StopWheelAndGetAdward();
            } else {
                ROTATE.Angle = (ROTATE.Angle + 30.34 / status) % 360;
            }
        }
        private void UpdateWallet(int AmountToChange = 0)
        {
            money += AmountToChange;
            textBlock.Text = "You have:\n" + money.ToString("C");
        }
        private void StartClick(object sender, RoutedEventArgs e)
        {
            status = 1;
            start_button.Foreground = new SolidColorBrush(Colors.Transparent);

            dispatcherTimer.Tick += DispatcherTimerTick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1);
            dispatcherTimer.Start();

            start_button.IsEnabled = false;
            stop_button.IsEnabled = true;
            stop_button.Foreground = new SolidColorBrush(Colors.White);
        }
        private void StopClick(object sender, RoutedEventArgs e)
        {
            status = 2;
            stop_button.Foreground = new SolidColorBrush(Colors.Transparent);
            stop_button.IsEnabled = false;
        }
        private void StopWheelAndGetAdward()
        {
            dispatcherTimer.Tick -= DispatcherTimerTick;
            dispatcherTimer.Stop();

            int awardClass = (int)Math.Floor(wheel_slices * ROTATE.Angle / 360);
            int awardAmount = int.Parse(textBlocks[awardClass].Text);
            MessageBox.Show("You " + (awardAmount >= 0 ? "are lucky. You gained " : "are unlucky. Your wallet just remained ") + awardAmount + "!", "Thank you!");
            UpdateWallet(awardAmount);

            // Allow to restart the game
            start_button.Foreground = new SolidColorBrush(Colors.White);
            start_button.IsEnabled = true;
        }
    }
}
