using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorProject.Shared.Utilities
{
    public class EnumHelper
    {
        public static List<DropDownListItem> ConvertEnumToDropDownSource<T>
            (string initialText, string initialValue)
        {
            List<DropDownListItem> ret = new List<DropDownListItem>();
            var values = Enum.GetValues(typeof(T)).Cast<T>().ToList();

            if (!string.IsNullOrEmpty(initialValue) || !string.IsNullOrEmpty(initialText))
            {
                DropDownListItem ddlInitialItem = new DropDownListItem()
                {
                    Text = initialText,
                    Value = initialValue
                };
                ret.Add(ddlInitialItem);
            }

            for (int i = 0; i < Enum.GetNames(typeof(T)).Length; i++)
            {
                DropDownListItem ddlItem = new DropDownListItem();
                ddlItem.Text = Enum.GetNames(typeof(T))[i];
                ddlItem.Value = values[i].ToString();

                ret.Add(ddlItem);
            }

            return ret;
        }
    }
}