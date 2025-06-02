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

    fetch('/CompanyA_Simulation?handler=CreatePO', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(cart)
    })
        .then(async response => {
            const text = await response.text();
            console.log("Raw PO response:", text);
            try {
                const data = JSON.parse(text);
                if (data.success) {
                    console.log("Purchase Order created successfully");
                    window.location.reload();
                } else {
                    alert("Error submitting order.");
                }
            } catch (err) {
                console.error("JSON parse error:", err);
                alert("Server response was not valid JSON.");
            }
        });

}

function CreateSO() {

    // Showing overlay, animation + messages
    const overlay = document.getElementById('loading-overlay');
    const message = document.getElementById('loading-message');
    overlay.style.display = 'block';

    const messages = [
        "Sending order to Company B",
        "Checking inventory...",
        "Creating a Sales Order based on the Purchase Order",
        "Sending back the information from the Sales Order",
        "Almost done..."
    ]

    let step = 0;
    function updateMessage() {
        if (step < messages.length) {
            message.textContent = messages[step];
            step++;
            setTimeout(updateMessage, 1500); // Change message every 1.5 seconds
        } else {
            sendRequest();
        }
    }
    updateMessage();

    function sendRequest() {
        fetch('/CompanyA_Simulation?handler=CreateSO', {
            method: 'POST',
            //headers: { 'Content-Type': 'application/json' },
            //body: JSON.stringify(cart)
        })
            .then(async response => {
                const text = await response.text();
                console.log("Raw response:", text);
                try {
                    const data = JSON.parse(text);
                    setTimeout(() => {
                        overlay.style.display = 'none';

                        if (data.success) {
                            alert("Sales Order created successfully!"); 
                            window.location.reload(); // Reload to update the UI
                        } else {
                            alert("Eroare la trimiterea comenzii.");
                        }
                    }, 1000); 
                } catch (err) {
                    console.error("JSON parse error:", err);
                    alert("Server response was not valid JSON.");
                    overlay.style.display = 'none';
                }
            });
    }
}
