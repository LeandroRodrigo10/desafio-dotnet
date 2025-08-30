using System;
using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class SaleItem : BaseEntity
    {
        // Required by EF
        protected SaleItem() {}

        public SaleItem(string sku, string name, int quantity, decimal unitPrice, decimal discount = 0m)
        {
            if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU is required", nameof(sku));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));
            SetQuantity(quantity);
            SetUnitPrice(unitPrice);
            SetDiscount(discount);

            Sku = sku.Trim();
            Name = name.Trim();
        }

        public string Sku { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Discount { get; private set; } // absolute discount per item
        public decimal Total { get; private set; }

        public void SetQuantity(int quantity)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero");
            Quantity = quantity;
            ComputeTotal();
        }

        public void SetUnitPrice(decimal unitPrice)
        {
            if (unitPrice < 0) throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price cannot be negative");
            UnitPrice = unitPrice;
            ComputeTotal();
        }

        public void SetDiscount(decimal discount)
        {
            if (discount < 0) throw new ArgumentOutOfRangeException(nameof(discount), "Discount cannot be negative");
            if (discount > UnitPrice) throw new ArgumentOutOfRangeException(nameof(discount), "Discount cannot exceed unit price");
            Discount = discount;
            ComputeTotal();
        }

        private void ComputeTotal()
        {
            var netUnit = UnitPrice - Discount;
            if (netUnit < 0) netUnit = 0;
            Total = Math.Round(netUnit * Quantity, 2, MidpointRounding.AwayFromZero);
        }
    }
}
