using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using Adfnet.Core.Globalization;
using Adfnet.Core.ValueObjects;
using Adfnet.Service;
using Microsoft.AspNetCore.Mvc;

namespace Adfnet.Web.Common
{
    public abstract class CommonController : ControllerBase
    {
        private readonly IMainService _serviceMain;

        protected CommonController(IMainService serviceMain)
        {
            _serviceMain = serviceMain;
        }


        protected List<KeyValue> GlobalizationKeys()
        {
            var resourceManagerDictionary = new ResourceManager(typeof(Dictionary));
            var resourceManagerMessages = new ResourceManager(typeof(Messages));
            var resourceSetDictionary = resourceManagerDictionary.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            if (resourceSetDictionary == null)
            {
                throw new Exception();
            }

            var resourceSetMessages = resourceManagerMessages.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            if (resourceSetMessages == null)
            {
                throw new Exception();
            }

            var keyValuePairs = new List<KeyValue>();
            keyValuePairs.AddRange(from DictionaryEntry entry in resourceSetDictionary let entryValue = entry.Value where entryValue != null where true let key = "glb-dict-" + entry.Key select new KeyValue(key, entryValue.ToString()));

            keyValuePairs.AddRange(from DictionaryEntry entry in resourceSetMessages let value = entry.Value where value != null where true let key = "glb-msg-" + entry.Key select new KeyValue(key, value.ToString()));


            return keyValuePairs;


        }

        protected string GetDictionary(string key)
        {
            return key == "ApplicationName" ? _serviceMain.ApplicationSettings.ApplicationName : Dictionary.ResourceManager.GetString(key);
        }

        protected string GetMessage(string key)
        {
            return Messages.ResourceManager.GetString(key);
        }


        protected string GetMessageByParameter(string key, string parameter)
        {
            return string.Format(Messages.ResourceManager.GetString(key),
                Dictionary.ResourceManager.GetString(parameter));
            //string.Format(Messages.DangerFieldLengthLimit, Dictionary.Username, "8")
        }
        protected string GetMessageByTwoParameter(string key, string parameter1, string parameter2)
        {
            return string.Format(Messages.ResourceManager.GetString(key), Dictionary.ResourceManager.GetString(parameter1), parameter2);
        }


    }
}
