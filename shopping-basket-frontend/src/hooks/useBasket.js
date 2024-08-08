// src/hooks/useBasket.js
import { useState } from 'react';
import { updateBasketAPI, calculateTotalAPI } from '../utils/api';

const useBasket = () => {
  const [basket, setBasket] = useState({});
  const [discountItems, setDiscountItems] = useState({});
  const [total, setTotal] = useState(null);
  const [discount, setDiscount] = useState(0);
  const [receiptItems, setReceiptItems] = useState([]);

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

  const updateBasket = (updatedBasket, forceRemove = false) => {
    const basketItems = Object.keys(updatedBasket).flatMap(key => {
      const item = updatedBasket[key];
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
      return {
        itemName: key,
        quantity: item.quantity,
        unitPrice: item.unitPrice,
        discountAppliedValue: item.discountApplied || 0,
        discountAppliedName: item.discountAppliedName || ""
      };
    });

    const combinedArray = [...basketItems, ...discountItemsArray];

    updateBasketAPI(combinedArray, forceRemove)
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

  const calculateTotal = () => {
    const basketArray = Object.keys(basket).flatMap(key => {
      const item = basket[key];
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
      return {
        itemName: key,
        quantity: item.quantity,
        unitPrice: item.unitPrice,
        discountAppliedValue: item.discountApplied || 0,
        discountAppliedName: item.discountAppliedName || ""
      };
    });

    const combinedArray = [...basketArray, ...discountItemsArray];

    calculateTotalAPI(combinedArray)
      .then(data => {
        const receipt = data.receipt;
        setTotal(receipt.totalPrice.toFixed(2));
        setReceiptItems(receipt.items);
        setDiscount(receipt.discountsApplied.toFixed(2));
      })
      .catch(error => console.error('Error calculating total:', error));
  };

  return {
    basket,
    discountItems,
    total,
    discount,
    receiptItems,
    addToBasket,
    updateQuantity,
    removeFromBasket,
    removeDiscountItem,
    calculateTotal
  };
};

export default useBasket;
