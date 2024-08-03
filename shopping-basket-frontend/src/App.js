import React, { useState, useCallback, useEffect } from 'react';
import './App.css';

// Debounce function to limit API call frequency
const debounce = (func, delay) => {
  let timeoutId;
  return (...args) => {
    if (timeoutId) clearTimeout(timeoutId);
    timeoutId = setTimeout(() => func(...args), delay);
  };
};

function App() {
  const [basket, setBasket] = useState({});
  const [total, setTotal] = useState(null);
  const [discounts, setDiscounts] = useState([]);

  const items = [
    { name: 'Soup', price: 1.5 },
    { name: 'Bread', price: 2.0 },
    { name: 'Milk', price: 1.2 },
    { name: 'Apples', price: 0.5 }
  ];

  const addToBasket = (item) => {
    setBasket(prevBasket => {
      const updatedBasket = {
        ...prevBasket,
        [item.name]: {
          ...prevBasket[item.name],
          quantity: (prevBasket[item.name]?.quantity || 0) + 1,
          price: item.price
        }
      };

      // Call updateBasket with the updated state
      updateBasket(updatedBasket);
      
      return updatedBasket;
    });
  };

  const updateQuantity = (itemName, quantity) => {
    setBasket(prevBasket => {
      const updatedBasket = {
        ...prevBasket,
        [itemName]: {
          ...prevBasket[itemName],
          quantity: parseInt(quantity) || 0
        }
      };

      // Call updateBasket with the updated state
      updateBasket(updatedBasket);
      
      return updatedBasket;
    });
  };

  const removeFromBasket = (itemName) => {
    setBasket(prevBasket => {
      const { [itemName]: removed, ...rest } = prevBasket;
      const updatedBasket = rest;

      // Call updateBasket with the updated state
      updateBasket(updatedBasket);
      
      return updatedBasket;
    });
  };

  // Debounced updateBasket function
  const updateBasket = useCallback(debounce(async (updatedBasket) => {
    console.log('Updating basket with:', updatedBasket);

    try {
      const response = await fetch('/api/basket/update', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(Object.keys(updatedBasket).map(key => ({
          productName: key,
          quantity: updatedBasket[key].quantity,
          unitPrice: updatedBasket[key].price,
          discountApplied: updatedBasket[key].discountApplied || 0
        }))),
      });

      if (!response.ok) {
        throw new Error('Failed to update basket');
      }

      const data = await response.json();
      // Ensure the basket state is updated with the latest data
      setBasket(data.reduce((acc, item) => {
        acc[item.productName] = {
          ...item,
          quantity: item.quantity,
          price: item.unitPrice,
          discountApplied: item.discountApplied || 0
        };
        return acc;
      }, {}));
    } catch (error) {
      console.error('Error updating basket:', error);
    }
  }, 500), []);

  // Effect to call updateBasket whenever the basket changes
  useEffect(() => {
    updateBasket(basket);
  }, [basket, updateBasket]);

  const calculateTotal = () => {
    const basketArray = Object.keys(basket).map(key => ({
      itemName: key,
      quantity: basket[key].quantity,
      unitPrice: basket[key].price 
    }));

    fetch('/api/basket/calculate', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(basketArray),
    })
      .then(response => response.json())
      .then(data => {
        const receipt = data.receipt;
        setTotal(receipt.totalPrice);
        setDiscounts(receipt.discountsApplied || []);
      })
      .catch(error => console.error('Error calculating total:', error));
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>Basket</h1>
      </header>
      <main className="App-main">
        <div className="container">
          <aside className="item-grid">
            <h2>Available Items</h2>
            <div className="grid">
              {items.map(item => (
                <div key={item.name} className="item-card">
                  <h3>{item.name}</h3>
                  <p>€{item.price.toFixed(2)}</p>
                  <button onClick={() => addToBasket(item)}>Add to Basket</button>
                </div>
              ))}
            </div>
          </aside>
          <section className="basket">
            <h2>Basket</h2>
            <div className="basket-items">
              {Object.keys(basket).length === 0 ? (
                <p>Your basket is empty</p>
              ) : (
                Object.keys(basket).map(key => (
                  <div key={key} className="basket-item">
                    <span>{key}</span>
                    <input
                      type="number"
                      min="0"
                      value={basket[key].quantity}
                      onChange={(e) => updateQuantity(key, e.target.value)}
                    />
                    <button onClick={() => removeFromBasket(key)}>Remove</button>
                    {basket[key].discountApplied && (
                      <span className="discount">
                        {basket[key].discountApplied}% off
                      </span>
                    )}
                  </div>
                ))
              )}
            </div>
            <button className="calculate-button" onClick={calculateTotal}>
              Calculate Total
            </button>
            {total !== null && (
              <>
                <h3>Total: €{total.toFixed(2)}</h3>
                {discounts.length > 0 && (
                  <div className="discounts">
                    <h4>Applied Discounts:</h4>
                    <ul>
                      {discounts.map((discount, index) => (
                        <li key={index}>{discount}</li>
                      ))}
                    </ul>
                  </div>
                )}
              </>
            )}
          </section>
        </div>
      </main>
      <footer className="App-footer">
        <p>&copy; 2024 Kantar - Emanuel Morais</p>
      </footer>
    </div>
  );
}

export default App;
