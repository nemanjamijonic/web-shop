namespace WebShop.Models.Domain
{
    using System;
    using WebShop.Models.DTO;
    using WebShop.Models.Enums;
    public abstract class User
    {
        protected User()
        {

        }
        protected User(string username, string password, string name, string surname, Gender gender, string email, DateTime birthDate, bool isDeleted)
        {
            Username = username;
            Password = password;
            Name = name;
            Surname = surname;
            Gender = gender;
            Email = email;
            BirthDate = birthDate;
            IsDeleted = isDeleted;
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public Role Role { get; set; }
        public bool IsDeleted { get; set; }

        public void UpdateProfile(UpdateUserProfile updateProfile)
        {
            BirthDate = updateProfile.DateOfBirth;
            Email = updateProfile.Email;
            Gender = updateProfile.Gender;
            Name = updateProfile.Name;
            Password = updateProfile.Password;
            Surname = updateProfile.Surname;
            IsDeleted = false;
        }

        public override string ToString()
        {
            return $"{Username}|{Password}|{Name}|{Surname}|{Gender}|{Email}|{BirthDate.ToString("dd-MM-yyyy")}|{Role}|{IsDeleted}";
        }

    }
}