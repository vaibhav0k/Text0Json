using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using NewCompare.Models;

namespace NewCompare.Controllers
{
    public class TextController : Controller
    {
        public IActionResult Index()
        {
            var model = new TextToCompare();
            return View(model);
        }

        [HttpPost]
        public IActionResult Compare(TextToCompare model)
        {
            if (model.Text1 != null && model.Text2 != null)
            {
                string[] lines1 = model.Text1.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                string[] lines2 = model.Text2.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                List<string> differences1 = new List<string>();
                List<string> differences2 = new List<string>();
                List<string> completeText1 = new List<string>();
                List<string> completeText2 = new List<string>();

                for (int i = 0; i < Math.Max(lines1.Length, lines2.Length); i++)
                {
                    string line1 = (i < lines1.Length) ? lines1[i] : "";
                    string line2 = (i < lines2.Length) ? lines2[i] : "";

                    if (line1 != line2)
                    {
                        differences1.Add(line1);
                        differences2.Add(line2);
                    }
                    else
                    {
                        differences1.Add("");
                        differences2.Add("");
                    }

                    completeText1.Add(line1);
                    completeText2.Add(line2);
                }

                ViewBag.Differences1 = differences1;
                ViewBag.Differences2 = differences2;
                ViewBag.CompleteText1 = completeText1;
                ViewBag.CompleteText2 = completeText2;
            }
            else
            {
                // Handle the case where one of the texts is null
                // For example, you could display an error message or redirect to a different view
                ViewBag.ErrorMessage = "Please provide text in both fields.";
            }

            // Retain the input texts in the model
            model.Text1 = model.Text1 ?? "";
            model.Text2 = model.Text2 ?? "";

            return View("Index", model);
        }

    }
}
