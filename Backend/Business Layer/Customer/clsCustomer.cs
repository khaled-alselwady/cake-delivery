﻿using Business_Layer.Customer.FindMethods;
using CakeDeliveryDTO.CustomerDTOs;
using DataAccessLayer;
using DTOs;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Customer
{
    public class clsCustomer
    {
        public enum enFindBy
        {
            CustomerID,
            Name
        };

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int? CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => string.Concat(FirstName, " ",LastName);
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; }


        public clsCustomer(CustomerDTO customerDto, enMode mode = enMode.AddNew)
        {
            CustomerID = customerDto.CustomerID;
            FirstName = customerDto.FirstName;
            LastName = customerDto.LastName;
            Email = customerDto.Email;
            PhoneNumber = customerDto.PhoneNumber;
            Address = customerDto.Address;
            City = customerDto.City;
            PostalCode = customerDto.PostalCode;
            Country = customerDto.Country;

            Mode = mode;
        }

        // Convert to DTO
        public CustomerDTO TocustomerDto() =>
            new CustomerDTO(CustomerID, FirstName, LastName,FullName, Email, PhoneNumber, Address, City, PostalCode, Country );

        private bool _Add()
        {
            CustomerID = clsCustomerData.Add(new CustomerCreateDTO(FirstName, LastName, Email, PhoneNumber, Address, City, PostalCode, Country));
            return CustomerID.HasValue;
        }

        private bool _Update()
        {
            return clsCustomerData.UpdateCustomer(new CustomerDTO(CustomerID, FirstName, LastName, FirstName +" "+ LastName, Email, PhoneNumber, Address, City, PostalCode, Country));
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_Add())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _Update();
            }

            return false;
        }

        //public static CustomerDTO FindCustomerById(int CustomerId)
        //{
        //    return clsCustomerData.GetCustomerById(CustomerId);
        //}

        //public static CustomerDTO FindCustomerByName(string Name)
        //{
        //    return clsCustomerData.GetCustomerByName(Name);
        //}

        public static CustomerDTO? Find<T>(T data, enFindBy findBy)
        {
            var finder = FindFactory.GetFinder(findBy);
            return finder?.Find(data);
        }

        public static List<CustomerDTO> All()
            => clsCustomerData.GetAllCustomers();

        public static bool Delete(int CustomerID)
            => clsCustomerData.DeleteCustomer(CustomerID);

        public static bool Exists<T>(T data, enFindBy findBy)
        {
            switch (findBy)
            {
                case enFindBy.CustomerID:
                    if (data is int CustomerId)
                    {
                        var Customer = clsCustomerData.GetCustomerById(CustomerId);
                        return Customer != null;
                    }
                    break;

                case enFindBy.Name:
                    if (data is string Name)
                    {
                        var Customer = clsCustomerData.GetCustomerByName(Name);
                        return Customer != null;
                    }
                    break;
            }

            return false;
        }

    }
}

