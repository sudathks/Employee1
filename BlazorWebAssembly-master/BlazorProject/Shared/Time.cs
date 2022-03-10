using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorProject.Shared
{
    public class Time
    {
        public int Id { get; set; }
        public DateTime LastUpdateTime { get; set; } = new DateTime();
    }
}
