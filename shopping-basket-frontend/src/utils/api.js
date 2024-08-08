// src/utils/api.js
export const updateBasketAPI = (combinedArray, forceRemove) => {
    return fetch(`/api/basket/update?forceRemove=${forceRemove}`, {
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
      });
  };
  
  export const calculateTotalAPI = (combinedArray) => {
    return fetch('/api/basket/calculate', {
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
      });
  };
  