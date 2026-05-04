using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agregator.Data
{

    public class Enum
    {
        public int RowId { set; get; }
        public int typeId { set; get; }
        public virtual EnumType enumType { set; get; }
        public string name { set; get; }

    }

    public class EnumType
    {
        public int RowId { set; get; }
        public string name { set; get; }

    }


    public enum PersoneDocumentType
    {
        Passport = 101,
        InternationalPassport = 102,
        MigrationCard = 103


    }
    public enum PersoneAddressType
    {
        Residence = 201,
        Registration = 202
    }

    public enum PersonePhoneType
    {
        Mobile = 301,
    }
    public enum Countries : int
    {
        RUS = 643,
    }

    public enum ContractStatus : int
    {
        Init = 400,
        StartProcess=401,
        ToSign = 410,
        Active = 420,
        Finished = 430,
        ToPay = 440,
        Paid = 450,
        Closed = 460,
        Archived = 470
    }


}
