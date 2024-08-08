import React from 'react';

const Receipt = ({ total, discount, receiptItems }) => (
  <div>
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
  </div>
);

export default Receipt;
