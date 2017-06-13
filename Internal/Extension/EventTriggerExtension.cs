using UnityEngine;
using UnityEngine.EventSystems;


namespace UniFramework.Extension
{
    public static class EventTriggerExtension
    {
        public static EventTrigger.Entry GetEntry(this EventTrigger self, EventTriggerType type)
        {
            var result = self.triggers.Find(o =>
            {
                return o.eventID == type;

            });
            if (result == null)
            {

                result = new EventTrigger.Entry();
                result.eventID = type;
                self.triggers.Add(result);
            }
            return result;
        }
    }
}