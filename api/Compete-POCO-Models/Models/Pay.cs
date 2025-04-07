using compete_poco.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compete_POCO_Models.Models
{
    public class Pay
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public User? User { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = null!;
        public DateTime CreationTime { get; set; }
    }
}
