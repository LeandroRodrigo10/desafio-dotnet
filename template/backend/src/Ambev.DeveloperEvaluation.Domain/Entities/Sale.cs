using System;
using System.Collections.Generic;
using System.Linq;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class Sale : BaseEntity
    {
        private readonly List<SaleItem> _items = new();

        // Required by EF
        protected Sale() { }

        public Sale(string number, DateTime date, string customer, string branch)
        {
            if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Number is required", nameof(number));
            if (string.IsNullOrWhiteSpace(customer)) throw new ArgumentException("Customer is required", nameof(customer));
            if (string.IsNullOrWhiteSpace(branch)) throw new ArgumentException("Branch is required", nameof(branch));

            Number = number.Trim();
            Date = date;
            Customer = customer.Trim();
            Branch = branch.Trim();
            Status = SaleStatus.Active;
            RecalculateTotals();
        }

        public string Number { get; private set; } = default!;
        public DateTime Date { get; private set; }
        public string Customer { get; private set; } = default!;
        public string Branch { get; private set; } = default!;
        public SaleStatus Status { get; private set; }

        public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();
        public decimal TotalAmount { get; private set; }

        public void AddItem(string sku, string name, int quantity, decimal unitPrice, decimal discount = 0m)
        {
            EnsureActive();
            var item = new SaleItem(sku, name, quantity, unitPrice, discount);
            _items.Add(item);
            RecalculateTotals();
        }

        public void UpdateItem(Guid itemId, int? quantity = null, decimal? unitPrice = null, decimal? discount = null)
        {
            EnsureActive();
            var item = _items.FirstOrDefault(i => i.Id == itemId) ?? throw new InvalidOperationException("Item not found");
            if (quantity.HasValue) item.SetQuantity(quantity.Value);
            if (unitPrice.HasValue) item.SetUnitPrice(unitPrice.Value);
            if (discount.HasValue) item.SetDiscount(discount.Value);
            RecalculateTotals();
        }

        public void RemoveItem(Guid itemId)
        {
            EnsureActive();
            var removed = _items.RemoveAll(i => i.Id == itemId);
            if (removed == 0) throw new InvalidOperationException("Item not found");
            RecalculateTotals();
        }

        public void Cancel()
        {
            if (Status == SaleStatus.Cancelled) return;
            Status = SaleStatus.Cancelled;
        }

        public void RecalculateTotals()
        {
            TotalAmount = _items.Sum(i => i.Total)
        }

        private void EnsureActive()
        {
            if (Status == SaleStatus.Cancelled)
                throw new InvalidOperationException("Cannot modify a cancelled sale");
        }
    }
}
