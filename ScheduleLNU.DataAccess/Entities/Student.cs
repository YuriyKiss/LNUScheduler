using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ScheduleLNU.DataAccess.Entities
{
    public class Student : IdentityUser
    {
        public Theme SelectedTheme { get; set; }

        public List<Theme> Themes { get; set; }

        public List<EventStyle> EventStyles { get; set; }

        public List<Schedule> Schedules { get; set; }
    }
}
