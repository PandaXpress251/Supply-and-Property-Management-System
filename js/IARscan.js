let extractedData = [];

function extractData() {
    const fileInput = document.getElementById("excelFile");
    const file = fileInput.files[0];

    if (!file) {
        alert("⚠ Please select an Excel file before proceeding.");
        return;
    }

    const formData = new FormData();
    formData.append("file", file);

    document.getElementById("loadingIndicator").style.display = 'block';
    document.querySelector("#excelDataTable tbody").innerHTML = '';

    const extractButton = document.querySelector("button[onclick='extractData()']");
    extractButton.disabled = true;
    extractButton.innerHTML = `<i class="fas fa-spinner fa-spin"></i> Extracting...`;

    fetch('/IAR/ExtractExcelData', {
        method: 'POST',
        body: formData
    })
        .then(response => response.text())
        .then(text => {
            try {
                const data = JSON.parse(text);
                if (data.success) {
                    extractedData = data.supplies;
                    populateTable(extractedData);
                } else {
                    alert("❌ Error: " + data.message);
                }
            } catch (err) {
                console.error("Parse Error:", err, text);
                alert("⚠ Unexpected server response. Check console.");
            }
        })
        .catch(error => {
            console.error("Fetch Error:", error);
            alert("❌ Failed to extract Excel data.");
        })
        .finally(() => {
            document.getElementById("loadingIndicator").style.display = 'none';
            extractButton.disabled = false;
            extractButton.innerHTML = "Extract & Preview Data";
        });
}

function populateTable(supplies) {
    const tableBody = document.querySelector("#excelDataTable tbody");
    tableBody.innerHTML = "";

    const categories = ["-Select Type-", "Supply", "Property"];

    supplies.forEach((supply, index) => {
        const categoryOptions = categories.map(category =>
            `<option value="${category}" ${supply.itemCategory === category ? 'selected' : ''}>${category}</option>`
        ).join("");

        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${supply.stockPropNo || ""}</td>
            <td>${supply.itemUnitMeasurement}</td>
            <td>${supply.itemName}</td>
            <td>${supply.itemDescription}</td>
            <td class="text-end">${supply.itemStockQuantity}</td>
            <td class="text-end">₱${parseFloat(supply.itemUnitCost).toFixed(2)}</td>
            <td>
                <select class="form-select form-select-sm" onchange="updateCategory(${index}, this.value)">
                    ${categoryOptions}
                </select>
            </td>
            <td>
                <button class="btn btn-outline-danger btn-sm" onclick="removeImportedSupply(${index})">
                    <i class="fas fa-trash-alt"></i> remove
                </button>
            </td>
        `;
        tableBody.appendChild(row);
    });

    document.getElementById("excelDataTable").style.display = 'block';
    document.getElementById("excelImportHeader").scrollIntoView({ behavior: "smooth", block: "start" });
}

function updateCategory(index, value) {
    extractedData[index].itemCategory = value;
}

function removeImportedSupply(index) {
    extractedData.splice(index, 1);
    populateTable(extractedData);
}

function saveSupplyTransactionOnly() {
    const supplierID = document.getElementById("supplierSelectExcel").value;
    const fundClusterID = document.getElementById("fundClusterSelectExcel").value;

    if (!supplierID || !fundClusterID) {
        alert("❌ Please select both a Supplier and Fund Cluster before saving.");
        return;
    }

    if (extractedData.length === 0) {
        alert("⚠ No data to save. Please extract data first.");
        return;
    }

    if (!confirm("Are you sure you want to save these Supply items?")) return;

    const saveButton = document.getElementById("saveExcelBtn");
    saveButton.disabled = true;
    saveButton.innerHTML = `<i class="fas fa-spinner fa-spin"></i> Saving...`;

    const payload = extractedData.map(item => ({
        ...item,
        SupplierID: parseInt(supplierID),
        FundClusterID: parseInt(fundClusterID)
    }));

    fetch('/IAR/SaveSupplyTransaction', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    })
        .then(response => response.json())
        .then(result => {
            if (result.success) {
                alert(result.message);
                window.location.href = result.url;
            } else {
                alert("❌ " + result.message);
            }
        })
        .catch(error => {
            console.error("Error saving transaction:", error);
            alert("❌ Something went wrong while saving.");
        })
        .finally(() => {
            saveButton.disabled = false;
            saveButton.innerHTML = "Save Supply Transaction";
        });
}
