using System;

namespace Mwp.Wopi
{
    public class FileCheckoutInfo
    {
        public Guid? CheckoutByUserId { get; set; }
        public string CheckoutUsername { get; set; }
        public DateTime? CheckoutTimestamp { get; set; }
    }
}