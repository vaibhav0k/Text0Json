using Newtonsoft.Json.Linq;
using System;

namespace NewCompare.Models
{
    public class Comparer
    {
        public JObject Compare(JToken leftJson, JToken rightJson)
        {
            var difference = new JObject();
            CompareTokens(leftJson, rightJson, difference);
            return difference;
        }

        private void CompareTokens(JToken left, JToken right, JObject difference)
        {
            if (JToken.DeepEquals(left, right))
                return;

            switch (left.Type)
            {
                case JTokenType.Object:
                    var leftObj = (JObject)left;
                    var rightObj = right as JObject;

                    if (rightObj == null)
                    {
                        // If right object is null, mark all properties in left object as added
                        foreach (var leftProp in leftObj.Properties())
                        {
                            difference[leftProp.Name] = new JObject { { "+", leftProp.Value } };
                        }
                        return;
                    }

                    foreach (var leftProp in leftObj.Properties())
                    {
                        var propName = leftProp.Name;
                        var rightProp = rightObj.Property(propName);

                        if (rightProp == null)
                        {
                            // Property present only in left object
                            difference[propName] = new JObject { { "-", leftProp.Value } };
                        }
                        else
                        {
                            // Recursively compare properties
                            var subDifference = new JObject();
                            CompareTokens(leftProp.Value, rightProp.Value, subDifference);
                            if (subDifference.HasValues)
                            {
                                difference[propName] = subDifference;
                            }
                        }
                    }
                    break;

                case JTokenType.Array:
                    var leftArray = (JArray)left;
                    var rightArray = (JArray)right;

                    // Compare array elements recursively
                    for (int i = 0; i < Math.Max(leftArray.Count, rightArray.Count); i++)
                    {
                        var subDifference = new JObject();
                        if (i < leftArray.Count)
                        {
                            CompareTokens(leftArray[i], i < rightArray.Count ? rightArray[i] : null, subDifference);
                            if (subDifference.HasValues)
                            {
                                difference[i.ToString()] = subDifference;
                            }
                        }
                        else if (i < rightArray.Count)
                        {
                            difference[i.ToString()] = new JObject { { "+", rightArray[i] } };
                        }
                    }
                    break;

                default:
                    // Compare primitive types
                    if (!JToken.DeepEquals(left, right))
                    {
                        difference["-"] = left;
                        difference["+"] = right;
                    }
                    break;
            }
        }




        public JObject MarkDifferences(JToken originalLeftJson, JToken originalRightJson, JObject difference)
        {
            MarkDifferencesRecursive(originalLeftJson, originalRightJson, difference);
            return difference;
        }

        private void MarkDifferencesRecursive(JToken originalLeftJson, JToken originalRightJson, JObject difference)
        {
            foreach (var property in difference.Properties())
            {
                if (property.Value is JObject subDifference)
                {
                    var propName = property.Name;
                    if (property.Value["$highlight"] != null && property.Value["$highlight"].ToString() != "red")
                    {
                        if (originalLeftJson is JObject leftObj && originalRightJson is JObject rightObj)
                        {
                            var leftProp = leftObj.Property(propName);
                            var rightProp = rightObj.Property(propName);
                            if (leftProp != null && rightProp != null)
                            {
                                // Recursively mark differences
                                MarkDifferencesRecursive(leftProp.Value, rightProp.Value, subDifference);
                            }
                            else
                            {
                                // Mark as added or removed
                                subDifference.Add("$highlight", leftProp != null ? "added" : "removed");
                            }
                        }
                    }
                }
                else if (property.Value is JArray subArray)
                {
                    // Handle arrays
                    for (int i = 0; i < subArray.Count; i++)
                    {
                        if (subArray[i] is JObject arrayDifference)
                        {
                            // Recursively mark differences
                            MarkDifferencesRecursive(originalLeftJson, originalRightJson, arrayDifference);
                        }
                    }
                }
                else if (property.Value is JValue)
                {
                    // Handle primitive types
                    if (property.Value["$highlight"] == null)
                    {
                        property.Value["$highlight"] = "added";
                    }
                }
            }
        }
    }
}
