import React, { useState } from 'react';
import './App.css';

function App() {
  const [basket, setBasket] = useState({});
  const [discountItems, setDiscountItems] = useState({});
  const [total, setTotal] = useState(null);
  const [discount, setDiscount] = useState(0);
  const [receiptItems, setReceiptItems] = useState([]);

  const availableItems = [
    { name: 'Soup', price: 0.65 },
    { name: 'Bread', price: 0.80 },
    { name: 'Milk', price: 1.30 },
    { name: 'Apples', price: 1.00 }
  ];

  const allItems = [
    ...availableItems,
    { name: 'Bread(offer)', price: 0.80 } 
  ];

  const updateBasket = (updatedBasket, forceRemove = false) => {
    const basketItems = Object.keys(updatedBasket).flatMap(key => {
      const item = updatedBasket[key];
      const itemInfo = allItems.find(i => i.name === key);
      const unitPrice = itemInfo ? itemInfo.price : 0;
  
      return Array(item.quantity).fill({
        itemName: key,
        quantity: 1,
        unitPrice: unitPrice, 
        discountAppliedValue: item.discountApplied || 0,
        discountAppliedName: item.discountAppliedName || ""
      });
    });
  
    const discountItemsArray = Object.keys(discountItems).map(key => {
      const item = discountItems[key];
      const itemInfo = allItems.find(i => i.name === key);
      const unitPrice = itemInfo ? itemInfo.price : 0;
      return {
        itemName: key,
        quantity: item.quantity,
        unitPrice: unitPrice,
        discountAppliedValue: item.discountApplied || 0,
        discountAppliedName: item.discountAppliedName || ""
      };
    });
  
    const combinedArray = [...basketItems, ...discountItemsArray];
  
    fetch(`/api/basket/update?forceRemove=${forceRemove}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(combinedArray),
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
      const consolidatedBasket = {};
  
      data.items.forEach(item => {
        if (consolidatedBasket[item.itemName]) {
          consolidatedBasket[item.itemName].quantity += item.quantity;
        } else {
          consolidatedBasket[item.itemName] = {
            ...item,
            quantity: item.quantity,
            unitPrice: item.unitPrice,
            discountApplied: item.discountAppliedValue || 0,
            discountAppliedName: item.discountAppliedName || ""
          };
        }
      });
  
      const consolidatedDiscountItems = {};
  
      data.discountItems.forEach(item => {
        if (consolidatedDiscountItems[item.itemName]) {
          consolidatedDiscountItems[item.itemName].quantity += item.quantity;
        } else {
          consolidatedDiscountItems[item.itemName] = {
            ...item,
            quantity: item.quantity,
            unitPrice: item.unitPrice,
            discountApplied: item.discountAppliedValue || 0,
            discountAppliedName: item.discountAppliedName || ""
          };
        }
      });
  
      setBasket(consolidatedBasket);
      setDiscountItems(consolidatedDiscountItems);
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

  const removeDiscountItem = (itemName) => {
    setDiscountItems(prevDiscountItems => {
      const updatedDiscountItems = { ...prevDiscountItems };
      if (updatedDiscountItems[itemName]) {
        delete updatedDiscountItems[itemName];
      }

      return updatedDiscountItems;
    });
  };

  const calculateTotal = () => {
    console.log('Basket before calculate:', basket);
    console.log('DiscountItems before calculate:', discountItems);

    const basketArray = Object.keys(basket).flatMap(key => {
      const item = basket[key];
      const itemInfo = allItems.find(i => i.name === key);
      const unitPrice = itemInfo ? itemInfo.price : 0;

      return Array(item.quantity).fill({
        itemName: key,
        quantity: 1,
        unitPrice: item.unitPrice, // Corrected this line
        discountAppliedValue: item.discountApplied || 0,
        discountAppliedName: item.discountAppliedName || ""
      });
    });

    const discountItemsArray = Object.keys(discountItems).map(key => {
      const item = discountItems[key];
      const itemInfo = allItems.find(i => i.name === key);
      const unitPrice = itemInfo ? itemInfo.price : 0;
      return {
        itemName: key,
        quantity: item.quantity,
        unitPrice: unitPrice,
        discountAppliedValue: item.discountApplied || 0,
        discountAppliedName: item.discountAppliedName || ""
      };
    });

    const combinedArray = [...basketArray, ...discountItemsArray];

    console.log('Combined array being sent to backend:', combinedArray);

    fetch('/api/basket/calculate', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(combinedArray),
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
        const receipt = data.receipt;
        setTotal(receipt.totalPrice.toFixed(2));
        setReceiptItems(receipt.items);
        setDiscount(receipt.discountsApplied.toFixed(2));
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
              {availableItems.map(item => (
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
              <h3>Items</h3>
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
                    Price: €{(basket[key].unitPrice * basket[key].quantity - basket[key].discountApplied).toFixed(2)}
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
            <div className="basket-items">
              <h3>Discount Items</h3>
              {Object.keys(discountItems).map(key => (
                <div key={key} className="basket-item">
                  <span>{key}</span>
                  <input
                    type="number"
                    min="0"
                    value={discountItems[key].quantity}
                    readOnly
                  />
                  <span>
                    Price: €{(discountItems[key].unitPrice * discountItems[key].quantity - discountItems[key].discountApplied).toFixed(2)}
                  </span>
                  <button onClick={() => removeDiscountItem(key)}>Remove</button>
                </div>
              ))}
            </div>
            <button className="calculate-button" onClick={calculateTotal}>
              Calculate Total
            </button>
            {total !== null && (
              <>
                <h3>Total: €{total}</h3>
                {discount > 0 && (
                  <div className="discount-info">
                    <h4>Discounts Applied: €{discount}</h4>
                  </div>
                )}
                {receiptItems.length > 0 && (
                  <table className="receipt-table">
                    <thead>
                      <tr>
                        <th>Item</th>
                        <th>Unit Price</th>
                        <th>Quantity</th>
                        <th>Discount</th>
                        <th>Final Price</th>
                      </tr>
                    </thead>
                    <tbody>
                      {receiptItems.map(item => (
                        <tr key={item.itemName}>
                          <td>{item.itemName}</td>
                          <td>€{item.unitPrice.toFixed(2)}</td>
                          <td>{item.quantity}</td>
                          <td>
                            {item.discountApplied > 0
                              ? `€${(item.unitPrice * item.quantity * item.discountApplied).toFixed(2)}`
                              : '€0.00'}
                          </td>
                          <td>€{(item.unitPrice * item.quantity * (1 - item.discountApplied)).toFixed(2)}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
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
