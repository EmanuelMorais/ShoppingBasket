import React from 'react';

const BasketItem = ({ item, onUpdateQuantity, onRemove }) => (
  <div className="basket-item">
    <span>{item.name}</span>
    <input
      type="number"
      min="0"
      value={item.quantity}
      onChange={(e) => onUpdateQuantity(item.name, e.target.value)}
    />
    <span>
      Price: €{(item.unitPrice * item.quantity - item.discountApplied).toFixed(2)}
    </span>
    <button onClick={() => onRemove(item.name)}>Remove</button>
    {item.discountApplied > 0 && (
      <span className="discount">
        {Math.round(item.discountApplied * 100)}% off
      </span>
    )}
  </div>
);

export default BasketItem;
