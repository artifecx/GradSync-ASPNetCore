﻿using Data.Models;
using System;
using System.Linq;

namespace Data.Interfaces
{
    public interface IAdminRepository
    {
        void Add(Admin model);
        void Update(Admin model);
        void Delete(string adminId);
        Admin FindById(string adminId);
        IQueryable<Admin> GetAll();
        bool IsSuperAdmin(string adminId);
        string GetSuperAdminId();
    }
}
