// src/utils/api.js
export const updateBasketAPI = async (combinedArray, forceRemove) => {
    const response = await fetch(`/api/basket/update?forceRemove=${forceRemove}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(combinedArray),
  });
  if (!response.ok) {
    return response.text().then(text => {
      throw new Error(`HTTP error! Status: ${response.status}, Message: ${text}`);
    });
  }
  return await response.json();
  };
  
  export const calculateTotalAPI = async (combinedArray) => {
    const response = await fetch('/api/basket/calculate', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(combinedArray),
    });
    if (!response.ok) {
      return response.text().then(text => {
        throw new Error(`HTTP error! Status: ${response.status}, Message: ${text}`);
      });
    }
    return await response.json();
  };
  