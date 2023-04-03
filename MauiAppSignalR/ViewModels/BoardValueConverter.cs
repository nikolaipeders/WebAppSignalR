using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppSignalR.ViewModels
{
    public class BoardValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var board = value as string[,];
            var row = int.Parse(parameter.ToString().Split(',')[0]);
            var col = int.Parse(parameter.ToString().Split(',')[1]);
            return board[row, col];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
