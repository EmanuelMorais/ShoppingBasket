import React, { useState } from 'react';
import './App.css';

function App() {
  const [basket, setBasket] = useState({});
  const [total, setTotal] = useState(null);
  const [discountPercentage, setDiscountPercentage] = useState(0);

  const items = [
    { name: 'Soup', price: 0.65 },
    { name: 'Bread', price: 0.80 },
    { name: 'Milk', price: 1.3 },
    { name: 'Apples', price: 1.0 }
  ];

  const updateBasket = (updatedBasket, forceRemove = false) => {
    const basketItems = Object.keys(updatedBasket).map(key => {
      const item = updatedBasket[key];
      const itemInfo = items.find(i => i.name === key);
      const unitPrice = itemInfo ? itemInfo.price : 0;

      return {
        itemName: key,
        quantity: item.quantity,
        unitPrice: unitPrice,
        discountAppliedValue: item.discountApplied || 0,
        discountAppliedName: item.discountAppliedName || ""
      };
    });

    fetch(`/api/basket/update?forceRemove=${forceRemove}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(basketItems),
    })
      .then(response => {
        if (!response.ok) {
          return response.text().then(text => {
            throw new Error(`HTTP error! Status: ${response.status}, Message: ${text}`);
          });
        }
        return response.json();
      })
      .then(data => {
        const basketState = data.reduce((acc, item) => {
          acc[item.itemName] = {
            ...item,
            quantity: item.quantity,
            unitPrice: item.unitPrice,
            discountApplied: item.discountAppliedValue || 0,
            discountAppliedName: item.discountAppliedName
          };
          return acc;
        }, {});

        setBasket(basketState);

        const totalPrice = data.reduce((sum, item) => {
          const discountedPrice = (item.unitPrice * item.quantity) * (1 - item.discountAppliedValue);
          return sum + discountedPrice;
        }, 0);

        setTotal(totalPrice.toFixed(2));
        setDiscountPercentage(data.discountsApplied * 100);
      })
      .catch(error => {
        console.error('Error updating basket:', error);
      });
  };

  const addToBasket = (item) => {
    setBasket(prevBasket => {
      const updatedBasket = {
        ...prevBasket,
        [item.name]: {
          quantity: (prevBasket[item.name]?.quantity || 0) + 1,
          unitPrice: item.price,
          discountApplied: prevBasket[item.name]?.discountApplied || 0,
          discountAppliedName: prevBasket[item.name]?.discountAppliedName || ""
        }
      };
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
          quantity: parseInt(quantity, 10) || 0
        }
      };
      updateBasket(updatedBasket);
      return updatedBasket;
    });
  };

  const removeFromBasket = (itemName) => {
    setBasket(prevBasket => {
      const { [itemName]: removed, ...rest } = prevBasket;
      updateBasket(rest, true);
      return rest;
    });
  };

  const calculateTotal = () => {
    const basketArray = Object.keys(basket).map(key => {
      const item = basket[key];
      const itemInfo = items.find(i => i.name === key);
      const unitPrice = itemInfo ? itemInfo.price : 0;
      return {
        itemName: key,
        quantity: item.quantity,
        unitPrice: unitPrice,
        discountAppliedValue: item.discountApplied || 0,
        discountAppliedName: item.discountAppliedName || ""
      };
    });
  
    fetch('/api/basket/calculate', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(basketArray),
    })
      .then(response => response.json())
      .then(data => {
        const receipt = data.receipt;
        setTotal(receipt.totalPrice.toFixed(2));
  
        // Calculate the correct discount percentage
        if (receipt.discountsApplied) {
          const correctDiscountPercentage = receipt.discountsApplied * 100;
          setDiscountPercentage(correctDiscountPercentage);
        } else {
          setDiscountPercentage(0);
        }
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
              {Object.keys(basket).map(key => (
                <div key={key} className="basket-item">
                  <span>{key}</span>
                  <input
                    type="number"
                    min="0"
                    value={basket[key].quantity}
                    onChange={(e) => updateQuantity(key, e.target.value)}
                  />
                  <span>
                    Price: €{(basket[key].unitPrice * (1 - (basket[key].discountApplied || 0))).toFixed(2)}
                  </span>
                  <button onClick={() => removeFromBasket(key)}>Remove</button>
                  {basket[key].discountApplied > 0 && (
                    <span className="discount">
                      {Math.round(basket[key].discountApplied * 100)}% off
                    </span>
                  )}
                </div>
              ))}
            </div>
            <button className="calculate-button" onClick={calculateTotal}>
              Calculate Total
            </button>
            {total !== null && (
              <>
                <h3>Total: €{total}</h3>
                {discountPercentage > 0 && (
                  <div className="discount-info">
                    <h4>Discounts Applied: {discountPercentage}% off</h4>
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
