// src/components/Basket.js
import React from 'react';
import BasketItem from '../BasketItem';
import DiscountItem from '../DiscountItem';

const Basket = ({ basket, discountItems, updateQuantity, removeFromBasket, removeDiscountItem }) => {
  return (
    <section className="basket">
      <h2>Basket</h2>
      <div className="basket-items">
        <h3>Items</h3>
        {Object.keys(basket).map(key => (
          <BasketItem
            key={key}
            item={basket[key]}
            updateQuantity={updateQuantity}
            removeFromBasket={() => removeFromBasket(key)}
          />
        ))}
      </div>
      <div className="basket-items">
        <h3>Discount Items</h3>
        {Object.keys(discountItems).map(key => (
          <DiscountItem
            key={key}
            item={discountItems[key]}
            removeDiscountItem={() => removeDiscountItem(key)}
          />
        ))}
      </div>
    </section>
  );
};

export default Basket;
