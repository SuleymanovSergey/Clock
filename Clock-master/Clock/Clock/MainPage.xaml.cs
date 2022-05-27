using System;
using Xamarin.Forms;

namespace Clock
{
    public partial class MainPage : ContentPage
    {
        struct HandParams
        {
            public HandParams(double width, double height, double offset) : this()
            {
                Width = width;
                Height = height;
                Offset = offset;
            }

            public double Width { private set; get; }
            public double Height { private set; get; }
            public double Offset { private set; get; }
        }

        static readonly HandParams secondParams = new HandParams(0.01, 1.1, 0.85);
        static readonly HandParams minuteParams = new HandParams(0.04, 0.8, 0.9);
        static readonly HandParams hourParams = new HandParams(0.07, 0.65, 0.9);

        BoxView[] tickMarks = new BoxView[60];

        public MainPage()
        {
            InitializeComponent();

            for (int i = 0; i < tickMarks.Length; i++)
            {
                tickMarks[i] = new BoxView { Color = Color.Black };
                absoluteLayout.Children.Add(tickMarks[i]);
            }

            Device.StartTimer(TimeSpan.FromSeconds(1.0 / 60), OnTimerTick);
        }

        void OnAbsoluteLayoutSizeChanged(object sender, EventArgs args)
        {
            Point center = new Point(absoluteLayout.Width / 2, absoluteLayout.Height / 2);
            double radius = 0.45 * Math.Min(absoluteLayout.Width, absoluteLayout.Height);

            for (int index = 0; index < tickMarks.Length; index++)
            {
                double sizeX = radius / 20;
                double sizeY = radius / (index % 5 == 0 ? 5 : 20);
                double radians = index * 2 * Math.PI / tickMarks.Length;
                double x = (center.X + radius * Math.Sin(radians) - sizeX / 2) / (index % 5 == 0 ? 1.05 : 1) + (index % 5 == 0 ? sizeX : 0);
                double y = (center.Y - radius * Math.Cos(radians) - sizeY / 2) / (index % 5 == 0 ? 1.05 : 1) + (index % 5 == 0 ? sizeY / 2 : 0);
                AbsoluteLayout.SetLayoutBounds(tickMarks[index], new Rectangle(x, y, sizeX, sizeY));
                tickMarks[index].Rotation = 180 * radians / Math.PI;
                tickMarks[index].CornerRadius = 10;
            }

            LayoutHand(secondHand, secondParams, center, radius);
            LayoutHand(minuteHand, minuteParams, center, radius);
            LayoutHand(hourHand, hourParams, center, radius);
        }

        void LayoutHand(BoxView boxView, HandParams handParams, Point center, double radius)
        {
            double width = handParams.Width * radius;
            double height = handParams.Height * radius;
            double offset = handParams.Offset;

            AbsoluteLayout.SetLayoutBounds(boxView,
                new Rectangle(center.X - 0.5 * width,
                              center.Y - offset * height,
                              width, height));

            boxView.AnchorY = handParams.Offset;
        }

        bool OnTimerTick()
        {
            DateTime dateTime = DateTime.Now;
            hourHand.Rotation = 30 * (dateTime.Hour % 12) + 0.5 * dateTime.Minute;
            minuteHand.Rotation = 6 * dateTime.Minute + 0.1 * dateTime.Second;
            secondHand.Rotation = 6 * (dateTime.Second);
            return true;
        }
    }
}
