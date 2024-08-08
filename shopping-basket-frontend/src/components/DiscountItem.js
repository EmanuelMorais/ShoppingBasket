import React from 'react';

const DiscountItem = ({ item, onRemove }) => (
  <div className="basket-item">
    <span>{item.name}</span>
    <input
      type="number"
      min="0"
      value={item.quantity}
      readOnly
    />
    <span>
      Price: €{(item.unitPrice * item.quantity - item.discountApplied).toFixed(2)}
    </span>
    <button onClick={() => onRemove(item.name)}>Remove</button>
  </div>
);

export default DiscountItem;
