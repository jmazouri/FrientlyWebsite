using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrientlyWebsite.Models.ViewModels
{
    public class BlogPostList
    {
        public List<BlogPost> BlogPosts { get; set; }
        public bool IsAdmin { get; set; }
    }
}
