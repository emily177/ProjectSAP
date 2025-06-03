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

function CreateDelivery() {
    // Showing overlay, animation + messages
    const overlay = document.getElementById('loading-overlay');
    const message = document.getElementById('loading-message');
    overlay.style.display = 'block';

    const messages = [
        "Processing Sales Order...",
        "Preparing items for shiping",
        "Creating a Delivery document..",
       
    ]
    let step = 0;
    function updateMessage() {
        if (step < messages.length) {
            message.textContent = messages[step];
            step++;
            setTimeout(updateMessage, 2000); // Change message every 1.5 seconds
        } else {
            sendRequest();
        }
    }
    updateMessage();

    function sendRequest() {
        fetch('/CompanyA_Simulation?handler=CreateDl', {
            method: 'POST',
            
        })
            .then(async response => {
                const text = await response.text();
                console.log("Raw Delivery:", text);
                try {
                    const data = JSON.parse(text);
                    setTimeout(() => {
                        //overlay.style.display = 'none';
                        if (data.success) {
                            alert("Delivery created successfully!");
                            window.location.reload(); // Reload to update the UI
                        } else {
                            alert("There was an error in creating the Delivery");
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

function CreateGRPO() {
    // Showing overlay, animation + messages
    const overlay = document.getElementById('loading-overlay');
    const message = document.getElementById('loading-message');
    overlay.style.display = 'block';

    const messages = [
        "Company A is putting the items in stock...",
        "Adding items in inventory",
        "Creating Goods Receipt PO...",

    ]
    let step = 0;
    function updateMessage() {
        if (step < messages.length) {
            message.textContent = messages[step];
            step++;
            setTimeout(updateMessage, 2000); // Change message every 1.5 seconds
        } else {
            sendRequest();
        }
    }
    updateMessage();

    function sendRequest() {
        fetch('/CompanyA_Simulation?handler=CreateGRPO', {
            method: 'POST',

        })
            .then(async response => {
                const text = await response.text();
                console.log("Raw GRPO:", text);
                try {
                    const data = JSON.parse(text);
                    setTimeout(() => {
                        overlay.style.display = 'none';
                        if (data.success) {
                            alert("Goods Receipt PO created successfully!");
                            window.location.reload(); // Reload to update the UI
                        } else {
                            alert("There was an error in creating GRPO");
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

function CreateARInvoice() {

    // Showing overlay, animation + messages
    const overlay = document.getElementById('loading-overlay');
    const message = document.getElementById('loading-message');
    overlay.style.display = 'block';

    const messages = [
        "Company B is creating an invoice for the items",
        "Checking the delivery",
        "Creating AR Invoice...",

    ]
    let step = 0;
    function updateMessage() {
        if (step < messages.length) {
            message.textContent = messages[step];
            step++;
            setTimeout(updateMessage, 2000); // Change message every 1.5 seconds
        } else {
            sendRequest();
        }
    }
    updateMessage();

    function sendRequest() {
        fetch('/CompanyA_Simulation?handler=CreateARInv', {
            method: 'POST',

        })
            .then(async response => {
                const text = await response.text();
                console.log("Raw ARInvoice:", text);
                try {
                    const data = JSON.parse(text);
                    setTimeout(() => {
                        overlay.style.display = 'none';
                        if (data.success) {
                            alert("AR Invoice was created successfully!");
                            window.location.reload(); // Reload to update the UI
                        } else {
                            alert("There was an error in creating AR Invoice");
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

function CreateAPInvoice() {

    // Showing overlay, animation + messages
    const overlay = document.getElementById('loading-overlay');
    const message = document.getElementById('loading-message');
    overlay.style.display = 'block';

    const messages = [
        "Creating A / P Invoice... loading purchase order data.",
        "Validating supplier and document details...",
        "Saving invoice to SAP Business One...",
        "A/P Invoice is being recorded in Company A." 

    ]
    let step = 0;
    function updateMessage() {
        if (step < messages.length) {
            message.textContent = messages[step];
            step++;
            setTimeout(updateMessage, 2000); // Change message every 1.5 seconds
        } else {
            sendRequest();
        }
    }
    updateMessage();

    function sendRequest() {
        fetch('/CompanyA_Simulation?handler=CreateAPInv', {
            method: 'POST',

        })
            .then(async response => {
                const text = await response.text();
                console.log("Raw APInvoice:", text);
                try {
                    const data = JSON.parse(text);
                    setTimeout(() => {
                        overlay.style.display = 'none';
                        if (data.success) {
                            alert("AP Invoice was created successfully!");
                            window.location.reload(); // Reload to update the UI
                        } else {
                            alert("There was an error in creating AP Invoice");
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


