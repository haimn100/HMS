using System;
using System.Web.Mvc;

namespace casa_benjamin.ModelBinder
{
    public class DateAndTimeModelBinder : IModelBinder
    {      
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var obj = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if(obj != null)
            {
                string val = obj.AttemptedValue;
                return DateTime.Parse(val);
            }
            return null;
        }
    }

    public class NullableDateAndTimeModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var obj = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (obj != null)
            {
                string val = obj.AttemptedValue;
                return DateTime.Parse(val);
            }
            return null;
        }
    }
}