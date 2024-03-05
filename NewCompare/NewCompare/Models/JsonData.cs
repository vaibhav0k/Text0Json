using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NewCompare.Models
{
    public class JsonData
    {
        public string LeftJsonString { get; set; }
        public string RightJsonString { get; set; }
    }

    public class DifferenceLeftToRight
    {
        public int Id { get; set; }
        public string LeftJson { get; set; }
        public string RightJson { get; set; }
        public string Differences { get; set; }
    }

    public class DifferenceRightToLeft
    {
        public int Id { get; set; }
        public string LeftJson { get; set; }
        public string RightJson { get; set; }
        public string Differences { get; set; }
    }
}
