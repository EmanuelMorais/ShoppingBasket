import React, { useState } from 'react';
import ProductList from '../components/ProductList';
import BasketItem from '../components/BasketItem';
import DiscountItem from '../components/DiscountItem';
import Receipt from '../components/Receipt';
import { updateBasketAPI, calculateTotalAPI } from '../utils/api';

const AppContainer = () => {
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

  const updateBasket = async (updatedBasket, forceRemove = false) => {
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
  
    try {
      const data = await updateBasketAPI(combinedArray, forceRemove);
  
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
    } catch (error) {
      console.error('Error updating basket:', error);
    }
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

  const calculateTotal = async () => {
    const basketArray = Object.keys(basket).flatMap(key => {
      const item = basket[key];
      const itemInfo = allItems.find(i => i.name === key);
      const unitPrice = itemInfo ? itemInfo.price : 0;

      return Array(item.quantity).fill({
        itemName: key,
        quantity: 1,
        unitPrice: item.unitPrice,
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

    try {
      const data = await calculateTotalAPI(combinedArray);
      const receipt = data.receipt;
      setTotal(receipt.totalPrice.toFixed(2));
      setReceiptItems(receipt.items);
      setDiscount(receipt.discountsApplied.toFixed(2));
    } catch (error) {
      console.error('Error calculating total:', error);
    }
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>Basket</h1>
      </header>
      <main className="App-main">
        <div className="container">
          <ProductList availableItems={availableItems} onAddToBasket={addToBasket} />
          <section className="basket">
            <h2>Basket</h2>
            <div className="basket-items">
              <h3>Items</h3>
              {Object.keys(basket).map(key => (
                <BasketItem
                  key={key}
                  item={basket[key]}
                  onUpdateQuantity={updateQuantity}
                  onRemove={removeFromBasket}
                />
              ))}
            </div>
            <div className="basket-items">
              <h3>Discount Items</h3>
              {Object.keys(discountItems).map(key => (
                <DiscountItem
                  key={key}
                  item={discountItems[key]}
                  onRemove={removeDiscountItem}
                />
              ))}
            </div>
            <button className="calculate-button" onClick={calculateTotal}>
              Calculate Total
            </button>
            <Receipt total={total} discount={discount} receiptItems={receiptItems} />
          </section>
        </div>
      </main>
      <footer className="App-footer">
        <p>&copy; 2024 Kantar - Emanuel Morais</p>
      </footer>
    </div>
  );
};

export default AppContainer;
