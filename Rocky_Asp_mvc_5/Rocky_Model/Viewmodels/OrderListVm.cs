using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_Model.Viewmodels
{
    public class OrderListVm
    {

     public  IEnumerable<OrderHeader> OrderHeaderList { get; set; }

      
        public Enum EnumList { get; set; }

     

    //    public IEnumerable<SelectListItem> StatusList { get; set; } //Раскривающийся список для всех сущнойстей Status

        public string Status { get; set; } //Текущее значение статус



        public IEnumerable<ListStatus1> StatusList { get; set; }

    }
}
