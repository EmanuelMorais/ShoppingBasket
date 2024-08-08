import React from 'react';

const ProductList = ({ availableItems, onAddToBasket }) => (
  <aside className="item-grid">
    <h2>Available Items</h2>
    <div className="grid">
      {availableItems.map(item => (
        <div key={item.name} className="item-card">
          <h3>{item.name}</h3>
          <p>€{item.price.toFixed(2)}</p>
          <button onClick={() => onAddToBasket(item)}>Add to Basket</button>
        </div>
      ))}
    </div>
  </aside>
);

export default ProductList;
