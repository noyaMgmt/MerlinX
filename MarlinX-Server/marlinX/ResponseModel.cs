using System;
using System.Collections.Generic;
using System.Text;

namespace marlinX
{
    class ResponseModel
    {
        public List<Article> articles { get; set; }
    }

    public class Article
    {
        public string title { get; set; }
        public string description { get; set; }
    }
}
