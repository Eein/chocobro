using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chocobro {
  public class Utility {


    public string simpleEncoding = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    // https://developers.google.com/chart/image/docs/data_formats#encoding_data
    // This function scales the submitted values so that
    // maxVal becomes the highest value.
    public string simpleEncode(List<double> valueArray, double maxValue) {
      List<string> chartData = new List<string>();
      chartData.Add("s:");
      for (var i = 0; i < valueArray.Count; i++) {
        var currentValue = valueArray[i];
        if (!Double.IsNaN(currentValue) && currentValue >= 0) {
          var eco = (int)(Math.Round((simpleEncoding.Length - 1) * currentValue / maxValue));
          chartData.Add(simpleEncoding[eco].ToString());
        } else {
          chartData.Add("_");
        }
      }
      return string.Join("", chartData.ToArray());
    }


  }
}
