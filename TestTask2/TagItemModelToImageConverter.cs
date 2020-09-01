using System;
using System.Globalization ;
using System.Windows.Data ;
using System.Windows.Media.Imaging ;

namespace TestTask2
{
    [ValueConversion( typeof(TagItemModel), typeof(BitmapImage))]
    public class TagItemModelToImageConverter : IValueConverter
    {
        public static TagItemModelToImageConverter Instance = new TagItemModelToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Get the model
            var model = (TagItemModel)value;

            // If the model is null, ignore
            if (model == null)
                return null;
            var image = model.Tag;
            return new BitmapImage(new Uri($"pack://application:,,,/{image}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

