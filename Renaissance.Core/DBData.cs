using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Renaissance.Core
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Advert> Adverts { get; set; }
    }

    public class User
    {
        [Key]
        public string UserName { get; set; }
        public int Age { get; set; }
        public byte[] Image { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<Advert> Adverts { get; set; }
    }

    public class Connection
    {
        public string ConnectionID { get; set; }
        public string UserAgent { get; set; }
        public bool Connected { get; set; }
    }

    public class Room
    {
        [Key]
        public string RoomName { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<string> Messages { get; set; }
    }

    public class Advert
    {
        [Key]
        public int Id { get; set; }
        public User User { get; set; }
        public int Whom { get; set; }
        public string Text { get; set; }
    }
}