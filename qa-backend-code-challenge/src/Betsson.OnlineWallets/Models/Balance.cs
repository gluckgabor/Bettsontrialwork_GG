
namespace Betsson.OnlineWallets.Models
{
    public class Balance
    {
        public decimal Amount { get; set; }

        public static implicit operator decimal(Balance v)
        {
            throw new NotImplementedException();
        }
    }
}
