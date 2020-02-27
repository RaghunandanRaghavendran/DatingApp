using System;
using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Model;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        public static void SeedUsers(DataContext context){

            if(!context.Users.Any())
            {
              var userdata = System.IO.File.ReadAllText("Data/UserSeeding.json");
              List<User> users = JsonConvert.DeserializeObject<List<User>>(userdata);
              foreach(var user in users) {
                  byte[] passwordHash, passwordSalt;
                     CreatePasswordHash("raghu@123", out passwordHash, out passwordSalt);
                     user.PasswordHash = passwordHash;
                     user.PasswordSalt = passwordSalt;
                     user.UserName = user.UserName.ToLower();
                     context.Users.Add(user);                     
              }
              context.SaveChanges();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}