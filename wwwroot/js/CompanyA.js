const selectedItems = {}; // { itemCode: quantity }
const cart = [];
function selectItem(itemCode) {
    document.getElementById(`controls-${itemCode}`).style.display = 'block';
}

function orderItem(itemCode) {

    const qty = parseInt(document.getElementById(`qty-${itemCode}`).value);
    if (qty > 0) {
        selectedItems[itemCode] = qty;
        alert(`Added ${qty} x ${itemCode} to order`);
    } else {
        alert("Quantity must be at least 1");
    }
    const existingItem = cart.find(item => item.ItemCode === itemCode);
    if (existingItem) {
        existingItem.Quantity = qty;
    } else {
        cart.push({ ItemCode: itemCode, Quantity: qty });
        console.log("Current cart:", cart);
    }
}
function submitOrder() {

    if (cart.length === 0) {
        alert("No items selected for order.");
        return;
    }

    fetch('/CompanyA_Simulation' , {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(cart)
    })
        .then(async response => {
            const text = await response.text();
            console.log("Raw response:", text);
            try {
                const data = JSON.parse(text);
                if (data.success) {
                    alert("Order submitted successfully!");
                } else {
                    alert("Error submitting order.");
                }
            } catch (err) {
                console.error("JSON parse error:", err);
                alert("Server response was not valid JSON.");
            }
        });

}
