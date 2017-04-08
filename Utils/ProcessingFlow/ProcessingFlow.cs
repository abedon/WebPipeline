using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public static class ProcessingFlow
    {
        private static Dictionary<string, List<string>> _channelMappings;

        public static void SetUp(Dictionary<string, List<string>> channelMappings)
        {
            _channelMappings = channelMappings;
        }

        public static string GetNextPartName(string currentPartName, string controllerName)
        {
            List<string> partOrderedList;

            try
            {
                partOrderedList = _channelMappings[controllerName.ToLower()];
            }
            catch
            {
                throw new ApplicationException("The channel for controller[" + controllerName + "] is not supported.");
            }

            currentPartName = AdjustInPartName(currentPartName);

            if (currentPartName == Constants.BuiltInPart.INITIALIZING)
                return AdjustOutPartName(partOrderedList[0]);

            if (currentPartName == partOrderedList[partOrderedList.Count - 1])
                return Constants.BuiltInPart.FINALIZING;

            var partname = "";
            for (var i = 0; i < partOrderedList.Count; i++)
            {
                if (currentPartName == partOrderedList[i].ToLower())
                {
                    partname = partOrderedList[i + 1].ToLower();
                    break;
                }
            }

            if (string.IsNullOrEmpty(partname))
            {
                throw new ApplicationException("Can't find import for " + currentPartName + " part. There could be something wrong with the channel definition in the config file.");
            }
            else
            {
                return AdjustOutPartName(partname);
            }
        }

        private static string AdjustOutPartName(string partName)
        {
            if (!partName.EndsWith("part"))
            {
                return partName + "part";
            }

            return partName;
        }

        private static string AdjustInPartName(string partName)
        {
            if (partName.ToLower().StartsWith("parts."))
            {
                partName = partName.Remove(0, 6);
            }

            if (partName.ToLower().EndsWith("part"))
            {
                partName = partName.Remove(partName.Length - 4, 4);
            }

            return partName.ToLower();
        }

        public static ICompositionPart GetPart(string partName, IEnumerable<ICompositionPart> compositionParts)
        {
            return compositionParts.Where(x => x.GetType().ToString().ToLower().EndsWith(partName)).SingleOrDefault();
        }
    }
}
