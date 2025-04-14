using System.ComponentModel.DataAnnotations;

namespace DotNetOrderService.Models
{
    public class OrderProductDetail : BaseModel
    {
       public Guid OrderId { get; set; }
       public Guid ProductId { get; set; }

       [MaxLength(150)]
       public string Name { get; set; }  
    }
}
