using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NewCompare.Models;
using System;

namespace NewCompare.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {

            return View(new JsonData());
        }

        [HttpPost]
        public IActionResult CompareJson(JsonData model)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.LeftJsonString) && !string.IsNullOrEmpty(model.RightJsonString))
                {
                    var comparer = new Comparer();
                    JToken leftJson = JToken.Parse(model.LeftJsonString);
                    JToken rightJson = JToken.Parse(model.RightJsonString);

                    // Compare JSON objects
                    JObject leftToRightDifference = comparer.Compare(leftJson, rightJson);

                    // Mark differences with highlights
                    JObject highlightedLeftToRightDifference = comparer.MarkDifferences(leftJson, rightJson, leftToRightDifference);

                    // Pass the original JSON and highlighted differences to the view
                    ViewBag.LeftJson = model.LeftJsonString;
                    ViewBag.RightJson = model.RightJsonString;
                    ViewBag.LeftToRightDifference = highlightedLeftToRightDifference.ToString();

                    return View("Index", model);
                }
                else
                {
                    ViewBag.Error = "Please provide JSON data in both fields.";
                }
            }

            return View("Index", model);
        }


    }
}
