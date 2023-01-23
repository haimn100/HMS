using casa_benjamin.Modules.Shared.Services;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace casa_benjamin.Internalization
{
    public static class I18n
    {
        private static Dictionary<string, Dictionary<string, string>> langDic = new Dictionary<string, Dictionary<string, string>>();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static Dictionary<string, string> shit = new Dictionary<string, string>();

        public static string T(string text, params string[] variables)
        {
           
            Dictionary<string, string> lang = langDic[CacheManager.Instance.AppSettings.language];
            string interpolatedText;
          
            if (lang.TryGetValue(text.ToLower(), out interpolatedText))
            {
                if (variables.Length > 0)
                {
                    for (int i = 0; i < variables.Length; i++)
                    {
                        interpolatedText = interpolatedText.Replace("{{" + i + "}}", variables[i]);
                    }
                }
                return interpolatedText;
            }
            else if (variables.Length > 0)
            {
                for (int i = 0; i < variables.Length; i++)
                {
                    interpolatedText = text.Replace("{{" + i + "}}", variables[i]);
                }
            }
            else
            {
                interpolatedText = text;
            }

            if (!shit.ContainsKey(text.ToLower()))
            {
                shit.Add(text.ToLower(), text);
            }

            logger.Info("missing translation for: ' " + text + "'");
            return interpolatedText;
        }

        public static void LoadLanguages()
        {
            try
            {
                string dir = HttpContext.Current.Server.MapPath("/I18n/Langs");
                foreach (var file in Directory.GetFiles(dir))
                {                   
                    string lang = File.ReadAllText(file);
                    lang = lang.Substring(lang.IndexOf('{'));
                    var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(lang);
                    langDic.Add(new FileInfo(file).Name.Replace(".js",""), dic);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

    }
}