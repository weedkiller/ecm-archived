using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DansLesGolfs
{
    public class CorrectJsonResult : JsonResult
    {
        public IList<JavaScriptConverter> Converters
        {
            get;
            set;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null)
            {
                JavaScriptSerializer serializer = CreateJsonSerializer();

                response.Write(serializer.Serialize(Data));
            }
        }

        private JavaScriptSerializer CreateJsonSerializer()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            ScriptingJsonSerializationSection section = ConfigurationManager.GetSection("system.web.extensions/scripting/webServices/jsonSerialization") as ScriptingJsonSerializationSection;

            if (section != null)
            {
                if ((section.Converters != null) && (section.Converters.Count > 0))
                {
                    IEnumerable<JavaScriptConverter> converters = CreateConvertersFrom(section.Converters);
                    serializer.RegisterConverters(converters);
                }

                serializer.MaxJsonLength = section.MaxJsonLength;
                serializer.RecursionLimit = section.RecursionLimit;
            }

            if ((Converters != null) && (Converters.Count > 0))
            {
                serializer.RegisterConverters(Converters);
            }

            return serializer;
        }

        private static IEnumerable<JavaScriptConverter> CreateConvertersFrom(ConvertersCollection typeDefinitions)
        {
            foreach (Converter definition in typeDefinitions)
            {
                Type type = BuildManager.GetType(definition.Type, false);

                if (type == null)
                {
                    throw new ArgumentException("Unknown type.", definition.Type);
                }

                if (!typeof(JavaScriptConverter).IsAssignableFrom(type))
                {
                    throw new ArgumentException("Unsupported type.", definition.Type);
                }

                yield return (JavaScriptConverter)Activator.CreateInstance(type);
            }

            yield break;
        }
    }
}