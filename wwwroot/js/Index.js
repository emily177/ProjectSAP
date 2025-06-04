function showDescription(text, buttonId) {
    const desc = document.getElementById("description");
    const descText = document.getElementById("desc-text");
    const arrow = document.getElementById("tooltip-arrow");

    descText.innerText = text;
    desc.classList.add("tooltip-box");

    const button = document.getElementById(buttonId);
    //const rect = button.getBoundingClientRect();
    //const descRect = desc.getBoundingClientRect();

    // Calculăm poziția relativă a centrului butonului față de caseta de descriere
    /*const arrowLeft = rect.left + rect.width / 2 - descRect.left ;*/
    if (button.id === "companyA") {
        arrow.style.left = `55px`;
    }
    else if (button.id === "companyB") {
        arrow.style.left = `422.25px`;
    }
   
}

function resetDescription() {
    const desc = document.getElementById("description");
    const descText = document.getElementById("desc-text");
    descText.innerText = 
    ` An Intercompany simulation is a learning or testing environment designed to
    mimic how multiple companies or business units interact with
    each other within the same organization using SAP Business One,
    which is an ERP (Enterprise Resource Planning) system.
    For this simulation, we will use the minimum of two companies.`
    desc.classList.remove("tooltip-box");
}
