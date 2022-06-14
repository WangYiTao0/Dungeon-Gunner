using System.Collections;
using UnityEngine;

public static class HelpUtility 
{
        /// <summary>
        /// Empty string Debug Check
        /// </summary>
        /// <param name="thisObject"></param>
        /// <param name="fieldName"></param>
        /// <param name="stringToCheck"></param>
        /// <returns></returns>
        public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
        {
                if (stringToCheck == "")
                {
                        Debug.Log($"{thisObject.name} の中に {fieldName} はEmptyです。 {fieldName} のvalueは必要です。");
                        return true;
                }

                return false;
        }

        /// <summary>
        /// ListがNullかとうか。　チエックする
        /// </summary>
        /// <param name="thisObject"></param>
        /// <param name="fieldName"></param>
        /// <param name="enumerableObjectToCheck"></param>
        /// <returns> Error が発生したら　return true　</returns>
        public static bool ValidateCheckEnumerableValue(Object thisObject, string fieldName,
                IEnumerable enumerableObjectToCheck)
        {
                bool error = false;
                int count = 0;

                foreach (var item in enumerableObjectToCheck)
                {
                        if (item == null)
                        {
                                Debug.Log($"{thisObject.name} の中に {fieldName} は　nullです");
                                error = true;
                        }
                        else
                        {
                                count++;
                        }
                }

                if (count == 0)
                {
                        Debug.Log($"{thisObject.name} の中に {fieldName} は　no value です");
                        error = false;
                }

                return error;
        }
}