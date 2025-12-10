using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotebookApp
{
    public class DataStore
    {
        public static AppUser AppUser = new AppUser
        {
            Id = 1,
            UserName = "Admin",
            Password = "123"
        };
        public static List<Note> Notes = new List<Note>()
        {
        new Note{ Id=1, Description="Her insanın bir öyküsü vardır."},
        new Note{ Id=2, Description="Kibarlık, ruhun görkemidir."},
        new Note{ Id=3, Description="Bir gökyüzü dinleniyor içimizde."},
        new Note{ Id=4, Description="Ruhum düşte kanatlandı."},
        new Note{ Id=5, Description="Ruhum düşte kanatlandı."}
    };

      
    }

    public class Note
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
    public class AppUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Id { get; set; }

    }
}
