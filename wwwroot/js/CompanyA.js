

const selectedItems = {}; // { itemCode: quantity }
const cart = [];
function selectItem(itemCode) {
    const cont = document.getElementById(`controls-${itemCode}`);
    if (cont.style.display === "none" || cont.style.display === "") {
        cont.style.display = "block";
    }
    else {
        cont.style.display = "none";
    }
       

}

function orderItem(itemCode) {

    const qty = parseInt(document.getElementById(`qty-${itemCode}`).value);
    const name = document.getElementById(`name-${itemCode}`).textContent;
    if (qty > 0) {
        selectedItems[itemCode] = qty;
        alert(`Added ${qty} x ${name} to order`);
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
    // We want to can push the button just once, to avoid creating multiple POs
    const button = document.getElementById("PO-button");
    button.disabled = true;
    button.textContent = "Sending...";

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
            setTimeout(updateMessage, 2000); // Change message every 1.5 seconds
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

                            // Changing the information of the next step
                            document.getElementById("page-title").textContent = "Step 3: Let`s do this! ";
                            document.getElementById("page-description").innerHTML =
                                `  The vendor now has a document with your exact order. After everything has been confirmed to be correct,
                                    the order can be shipped from the warehouse to the customer. Tap the "Create Delivery" button to
                                    generate the document used to ship the items. `;

                            // Delete the button for SO
                            const oldButton = document.getElementById("button-so");
                            oldButton.style.display = "none";
                            oldButton.disabled = true;

                            const panel = document.querySelector(".containerB");
                            panel.innerHTML = buildSOPanel(data.salesOrder);

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
                        overlay.style.display = 'none';
                        if (data.success) {

                            // Change the message
                            document.getElementById("page-title").textContent = "Step 4: Company A Receives the Delivery";
                            document.getElementById("page-description").innerHTML =
                                ` The delivery from Company B has now been sent. To complete the process, Company A needs to confirm
                                    that the products have arrived by creating a <strong>Goods Receipt PO (GRPO)</strong>.
                                    This document is used to officially record that the items were received in the warehouse.
                                    It is based on the original Purchase Order and helps update inventory and track the transaction.
                                    Tap the button below to create the GRPO for Company A.`;

                            //Hide the button for Delivery
                            const oldButton = document.getElementById("button-del");
                            oldButton.style.display = "none";
                            oldButton.disabled = true;

                            const panel = document.querySelector(".containerB");
                            panel.innerHTML = buildDeliveryPanel(data.delivery);

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

                            // Update information
                            document.getElementById("page-title").textContent = "Step 5: Company B Issues the Invoice";
                            document.getElementById("page-description").innerHTML =
                                ` Now that Company A has confirmed the receipt of the goods, it’s time for Company B to issue the invoice.
                                    The <strong>A/R Invoice</strong> (Accounts Receivable Invoice) is the document that requests payment for the delivered products.
                                    It finalizes the process on the seller’s side and is based on the delivery and the goods received by Company A.
                                    Tap the button below to create the A/R Invoice for Company B.`;

                            // Hide the button for GRPO
                            const oldButton = document.getElementById("button-grpo");
                            oldButton.style.display = "none";
                            oldButton.disabled = true;

                            const panel = document.querySelector(".containerA");
                            panel.innerHTML = buildGRPOPanel(data.grpo);

                          

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

                            document.getElementById("page-title").textContent = "Step 6: Company A Receives the Invoice";
                            document.getElementById("page-description").innerHTML =
                                ` After Company B has issued the sales invoice, Company A needs to record this in its system by creating an
                                    <strong>A/P Invoice</strong> (Accounts Payable Invoice). This document represents the invoice received from the vendor and is used
                                    to track the amount that needs to be paid.
                                    It is linked to the original Purchase Order and the Goods Receipt previously created, so everything stays connected.
                                    Tap the button below to create the A/P Invoice for Company A and complete the process.`;

                            // Hide the button for AR Invoice
                            const oldButton = document.getElementById("button-ar");
                            oldButton.style.display = "none";
                            oldButton.disabled = true;

                            const panel = document.querySelector(".containerB");
                            panel.innerHTML = buildARInvPanel(data.arInvoice);

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

                            //The last message
                            document.getElementById("page-title").textContent = "Congratulations! The cycle is complete!";
                            document.getElementById("page-description").innerHTML =
                                `Thank you for learning about the Intercompany Process in SAP Business One with us! If you want to restart the simulation, you can return to the main page.`;


                            // Hide the button for AP Invoice
                            const oldButton = document.getElementById("button-ap");
                            oldButton.style.display = "none";
                            oldButton.disabled = true;

                            const panel = document.querySelector(".containerA");
                            panel.innerHTML = buildAPInvPanel(data.apInvoice);

                           

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

function buildSOPanel(salesOrder) {
    let html = `
        <div class="panel slide-in">
            <h2>Sales Order</h2>
            <div>
                <p><strong>Document Number:</strong> ${salesOrder.docNum}</p>
                <p><strong>Customer Code:</strong> ${salesOrder.cardCode}</p>
                <p><strong>Customer Name:</strong> ${salesOrder.cardName}</p>
                <p><strong>Status:</strong> ${salesOrder.docStatus}</p>
                <p><strong>Document Date:</strong> ${salesOrder.docDate}</p>
                <p><strong>Due Date:</strong> ${salesOrder.docDueDate}</p>
            </div>
            <h3>Items</h3>
            <table>
                <thead>
                    <tr>
                        <th>Item Code</th>
                        <th>Item Name</th>
                        <th>Quantity</th>
                        <th>Unit Price</th>
                        <th>Total Price</th>
                    </tr>
                </thead>
                <tbody>`;

    //const salesOrder = response.salesOrder;
    for (const item of salesOrder.items) {
        html += `
                <tr>
                    <td>${item.itemCode}</td>
                    <td>${item.itemName}</td>
                    <td>${item.quantity}</td>
                    <td>${item.price}</td>
                    <td>${(item.quantity * item.price).toFixed(2)}</td>
                </tr>`;
    }


    html += `
                    </tbody>
                </table>
                <p><strong>Total Amount:</strong> ${salesOrder.docTotal}</p>
            </div>

            <div class="button-wrapper">
                <button onclick="CreateDelivery()" class="nextStep" id="button-del">Next step ⇒ Create Delivery</button>
            </div>
        `;
        return html;
}

function buildDeliveryPanel(delivery) {

    let html = `
        <div class="panel slide-in">
            <h2>Delivery</h2>
            <div>
                <p><strong>Document Number:</strong> ${delivery.docNum}</p>
                <p><strong>Customer Code:</strong> ${delivery.cardCode}</p>
                <p><strong>Customer Name:</strong> ${delivery.cardName}</p>
                <p><strong>Status:</strong> ${delivery.docStatus}</p>
                <p><strong>Document Date:</strong> ${delivery.docDate}</p>
                <p><strong>Due Date:</strong> ${delivery.docDueDate}</p>
            </div>
            <h3>Items</h3>
            <table>
                <thead>
                    <tr>
                        <th>Item Code</th>
                        <th>Item Name</th>
                        <th>Quantity</th>
                        <th>Unit Price</th>
                        <th>Total Price</th>
                    </tr>
                </thead>
                <tbody>`;
    for (const item of delivery.items) {
        html += `
                <tr>
                    <td>${item.itemCode}</td>
                    <td>${item.itemName}</td>
                    <td>${item.quantity}</td>
                    <td>${item.price}</td>
                    <td>${(item.quantity * item.price).toFixed(2)}</td>
                </tr>`;
    }
    html += `
                    </tbody>
                </table>
                <p><strong>Total Amount:</strong> ${delivery.docTotal}</p>
            </div>
            <div class="button-wrapper">
                <button onclick="CreateGRPO()" class="nextStep" id="button-grpo">Next step ⇒ Create GRPO</button>
            </div>`;
    return html;
}

function buildGRPOPanel(grpo) {

    let html = `
        <div class="panel slide-in">
            <h2>Goods Receipt PO</h2>
            <div>
                <p><strong>Document Number:</strong> ${grpo.docNum}</p>
                <p><strong>Vendor Code:</strong> ${grpo.cardCode}</p>
                <p><strong>Vendor Name:</strong> ${grpo.cardName}</p>
                <p><strong>Status:</strong> ${grpo.docStatus}</p>
                <p><strong>Document Date:</strong> ${grpo.docDate}</p>
                <p><strong>Due Date:</strong> ${grpo.docDueDate}</p>
            </div>
            <h3>Items</h3>
            <table>
                <thead>
                    <tr>
                        <th>Item Code</th>
                        <th>Item Name</th>
                        <th>Quantity</th>
                        <th>Unit Price</th>
                        <th>Total Price</th>
                    </tr>
                </thead>
                <tbody>`;
    for (const item of grpo.items) {
        html += `
                <tr>
                    <td>${item.itemCode}</td>
                    <td>${item.itemName}</td>
                    <td>${item.quantity}</td>
                    <td>${item.price}</td>
                    <td>${(item.quantity * item.price).toFixed(2)}</td>
                </tr>`;
    }

        html += `
                </tbody>
                </table>
                <div>
                    <p><strong>Total Amount:</strong> ${grpo.docTotal}</p>
                </div>
            </div>
            <div class="next-step-container">
                <button onclick="CreateARInvoice()" class="nextStep" id="button-ar">Next step => Create A/R Invoice</button>
            </div>
        </div>
                `;

    return html;
}

function buildARInvPanel(arInvoice) {

    let html = `
        <div class="panel slide-in">
            <h2>A/R Invoice</h2>
            <div>
                <p><strong>Document Number:</strong> ${arInvoice.docNum}</p>
                <p><strong>Customer Code:</strong> ${arInvoice.cardCode}</p>
                <p><strong>Customer Name:</strong> ${arInvoice.cardName}</p>
                
                <p><strong>Document Date:</strong> ${arInvoice.docDate}</p>
                <p><strong>Due Date:</strong> ${arInvoice.docDueDate}</p>
            </div>
            <h3>Items</h3>
            <table>
                <thead>
                    <tr>
                        <th>Item Code</th>
                        <th>Item Name</th>
                        <th>Quantity</th>
                        <th>Unit Price</th>
                        <th>Total Price</th>
                    </tr>
                </thead>
                <tbody>`;
    for (const item of arInvoice.items) {
        html += `
                <tr>
                    <td>${item.itemCode}</td>
                    <td>${item.itemName}</td>
                    <td>${item.quantity}</td>
                    <td>${item.price}</td>
                    <td>${(item.quantity * item.price).toFixed(2)}</td>
                </tr>`;
    }
    html += `
                    </tbody>
                </table>
                <p><strong>Total Amount:</strong> ${arInvoice.docTotal}</p>
            </div>
            <div class="button-wrapper">
                <button onclick="CreateAPInvoice()" class="nextStep" id="button-ap" >Next step ⇒ Create A/P Invoice</button>
            </div>`;
    return html;
}

function buildAPInvPanel(apInvoice) {
    let html = `
        <div class="panel slide-in">
            <h2>A/P Invoice</h2>
            <div>
                <p><strong>Document Number:</strong> ${apInvoice.docNum}</p>
                <p><strong>Vendor Code:</strong> ${apInvoice.cardCode}</p>
                <p><strong>Vendor Name:</strong> ${apInvoice.cardName}</p>
               
                <p><strong>Document Date:</strong> ${apInvoice.docDate}</p>
                <p><strong>Due Date:</strong> ${apInvoice.docDueDate}</p>
            </div>
            <h3>Items</h3>
            <table>
                <thead>
                    <tr>
                        <th>Item Code</th>
                        <th>Item Name</th>
                        <th>Quantity</th>
                        <th>Unit Price</th>
                        <th>Total Price</th>
                    </tr>
                </thead>
                <tbody>`;
    for (const item of apInvoice.items) {
        html += `
                <tr>
                    <td>${item.itemCode}</td>
                    <td>${item.itemName}</td>
                    <td>${item.quantity}</td>
                    <td>${item.price}</td>
                    <td>${(item.quantity * item.price).toFixed(2)}</td>
                </tr>`;
    }
    html += `
                    </tbody>
                </table>
                <p><strong>Total Amount:</strong> ${apInvoice.docTotal}</p>
            </div>`;
    return html;
}



