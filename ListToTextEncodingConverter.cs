using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace eTOM
{
    [ValueConversion(typeof(IList), typeof(string))]
    public class ListToTextEncodingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IList valueList = (IList)value;
            string baseUrl = (string)parameter;
            if (valueList == null || baseUrl == null)
                return string.Empty;

            string valueString = "";
            CultureInfo usCulture = new CultureInfo("en-us");

            foreach (object item in valueList)
            {
                double? val = item as double?;
                if (val.HasValue)
                {
                    if (valueString.Length > 0)
                        valueString += ",";
                    valueString += val.Value.ToString(usCulture.NumberFormat);
                }
            }

            return baseUrl + "&chd=t:" + valueString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
