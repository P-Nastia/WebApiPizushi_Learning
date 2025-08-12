using Core.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Delivery;

public class DeliveryInfoModel
{
    public string RecipientName { get; set; } = String.Empty;
    public string PhoneNumber { get; set; } = String.Empty;
    public SimpleModel PostDepartment { get; set; } = null;
    public SimpleModel PaymentType { get; set; } = null;
    public SimpleModel City { get; set; } = null;
}
