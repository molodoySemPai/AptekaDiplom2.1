using System;
using System.Collections.Generic;
using System.Linq;

namespace AptekaDiplom2.State
{
    public class CartState
    {
        // Словарь: ID товара -> Количество
        public Dictionary<int, int> Items { get; set; } = new Dictionary<int, int>();

        // Событие: оповещает меню и страницы, что что-то изменилось
        public event Action? OnChange;

        public void AddToCart(int productId)
        {
            if (Items.ContainsKey(productId))
            {
                Items[productId]++;
            }
            else
            {
                Items[productId] = 1;
            }
            NotifyStateChanged();
        }

        public void RemoveFromCart(int productId)
        {
            if (Items.ContainsKey(productId))
            {
                Items.Remove(productId);
                NotifyStateChanged();
            }
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                RemoveFromCart(productId);
            }
            else
            {
                Items[productId] = quantity;
                NotifyStateChanged();
            }
        }

        public void Clear()
        {
            Items.Clear();
            NotifyStateChanged();
        }

        /// <summary>
        /// Общее количество товарных позиций (уникальных товаров) в корзине.
        /// </summary>
        public int GetCount()
        {
            return Items.Count;
        }

        /// <summary>
        /// Суммарное количество единиц товара в корзине (с учётом количества каждой позиции).
        /// </summary>
        public int GetTotalQuantity()
        {
            return Items.Values.Sum();
        }

        private void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }
    }
}