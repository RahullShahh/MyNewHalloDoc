using DAL.DataModels;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class CreateRequestViewModel
    {
        public string firstname {  get; set; }
        public string lastname { get; set; }
        public string phoneno {  get; set; }
        public string email {  get; set; }
        public DateOnly birthDate { get; set; }

        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipcode { get; set; }
        public string room {  get; set; }

        public List<Region> regions { get; set; }
        public string adminNotes {  get; set; }
    }
}
