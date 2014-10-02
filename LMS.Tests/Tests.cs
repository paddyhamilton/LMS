using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Domain;
using NUnit.Framework;

namespace LMS.Tests
{
    public class Tests
    {
        [Test]
        public void Testing()
        {
            var userId = Guid.NewGuid();
            var user = User.RegisterUser(userId, "Paddy", "Hamilton", DateTime.Now, "Test@test.com");
        }
    }

    public class User : AggregateRoot
    {
        public string Forename { get; set; }
        public string Surname { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }

        public User()
        {

        }

        private User(Guid userId, string forename, string surname, DateTime dob, string eMailAddress)
            : this()
        {
            Apply(new RegisterUserEvent(userId, forename, surname, dob, eMailAddress));
        }

        public static User RegisterUser(Guid userId, string forename, string surname, DateTime dob, string eMailAddress)
        {
            return new User(userId, forename, surname, dob, eMailAddress);
        }

        public void Handle(RegisterUserEvent e)
        {
            Forename = e.Forename;
            Surname = e.Surname;
            DOB = e.DOB;
            Email = e.Email;
        }
    }

    public class RegisterUserEvent : Event
    {
        public Guid Id { get; private set; }
        public string Forename { get; private set; }
        public string Surname { get; private set; }
        public DateTime DOB { get; private set; }
        public string Email { get; private set; }

        public RegisterUserEvent(Guid userId, string forename, string surname, DateTime dob, string eMailAddress)
        {
            Id = userId;
            Forename = forename;
            Surname = surname;
            DOB = dob;
            Email = eMailAddress;
        }
    }
}
