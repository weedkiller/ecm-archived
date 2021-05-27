using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.Base
{
    public class ValidateDataHelper
    {
        public bool ValidateEmpty(object value, string name = "")
        {
            bool isEmpty = false;
            if (value.GetType() == typeof(string))
            {
                if(string.IsNullOrEmpty(value.ToString()))
                {
                    isEmpty = true;
                }
            }
            else if(value == null)
            {
                isEmpty = true;
            }

            if(isEmpty)
            {
                if(string.IsNullOrEmpty(name))
                {
                    ExceptionHelper.AddException("Value cannot be null or empty.", "EmptyValue");
                }
                else
                {
                    ExceptionHelper.AddException(name + " cannot be null or empty.", "EmptyValue");
                }
            }
            return isEmpty;
        }
    }
}
