$(document).ready(function () {
    getHoldings();
});

$(document).on("click", ".open-edit-holding", function () {
    $("#inputNewTickerExchange").val($(this).data("bs-tickerexchange"));
    $("#inputNewName").val($(this).data("bs-name"));
    $("#inputNewWeight").val($(this).data("bs-weight"));
});

$(document).on("click", ".delete", function (e) {
    deleteHolding(e.target.id);
    e.preventDefault();
});

$(".add-holding-form").on("submit", function (e) {
    let tickerExchange = $("#inputTickerExchange").val();
    if (tickerExchange === "") {
        alert("Error! Ticker Exchange cannot be empty.");
        e.preventDefault();
        return;
    }
    let name = $("#inputName").val();
    if (name === "") {
        alert("Error! Name cannot be empty.");
        e.preventDefault();
        return;
    }
    let weight = $("#inputWeight").val();
    if (weight === "") {
        alert("Error! Weight cannot be empty.");
        e.preventDefault();
        return;
    }
    let weightValue = parseFloat(weight) / 100;

    if (weightValue <= 0) {
        alert("Error! Weight cannot be 0 or negative.");
        e.preventDefault();
        return;
    }

    const holding = {
        tickerExchange: tickerExchange,
        name: name,
        weight: weightValue
    };

    fetch('https://localhost:7213/api/Holdings', {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(holding)
    })
        .then(response => { if (!response.ok) { throw response } })
        .then(() => {
            getHoldings();
            getLastModifiedDate();
            $("#inputTickerExchange").val("");
            $("#inputName").val("");
            $("#inputWeight").val("");
        })
        .catch(error => {
            alert('Unable to add holding. Make sure there is enough cash in the portfolio or that this holding is not already in the portfolio.');
        });

    $("#inputTickerExchange").val("");
    $("#inputName").val("");
    $("#inputWeight").val("");
    e.preventDefault();
});

$(".edit-holding-form").on("submit", function (e) {
    let holdingNewTickerExchange = $("#inputNewTickerExchange").val();
    let holdingNewName = $("#inputNewName").val();
    let holdingNewWeight = $("#inputNewWeight").val();
    if (holdingNewName === "") {
        alert("Error! Holding name cannot be empty.");
        e.preventDefault();
        return;
    }
    if (holdingNewWeight === "") {
        alert("Error! Holding weight cannot be empty.");
        e.preventDefault();
        return;
    }
    let weightValue = parseFloat(holdingNewWeight) / 100;
    if (weightValue <= 0) {
        alert("Error! Weight cannot be 0 or negative.");
        e.preventDefault();
        return;
    }

    editHolding(holdingNewTickerExchange, holdingNewName, weightValue);

    $("#inputNewTickerExchange").val("");
    $("#inputNewName").val("");
    $("#inputNewWeight").val("");
    e.preventDefault();
});

function getHoldings() {
    fetch('https://localhost:7213/api/Holdings')
        .then(response => response.json())
        .then(data => displayHoldings(data))
        .catch(error => alert('Unable to get holdings.', error));
}

function displayHoldings(data) {
    $('table#holdings tbody').empty();
    let count = 0;
    data.forEach(item => {
        let isCash = count === 0;
        $('table#holdings tbody').append(
            '<tr' + (isCash ? ' class="table-secondary"' : '') + '>' +
            '<th scope="row">' + ++count + '</th>' +
            '<td>' + item.tickerExchange + '</td>' +
            '<td>' + item.name + '</td>' +
            '<td>' + +(item.weight * 100).toFixed(2) + '%</td>' +
            (!isCash ? '<td><button type="button" class="btn btn-link open-edit-holding" data-bs-toggle="modal" data-bs-target="#edit-holding" data-bs-tickerexchange="' + item.tickerExchange + '" data-bs-name="' + item.name + '" data-bs-weight="' + +(item.weight * 100).toFixed(2) + '">Edit</button></td>' : '<td></td>') +
            (!isCash ? '<td><button type="button" id="' + item.tickerExchange + '" class="btn btn-link delete">Delete</button></td>' : '<td></td>') +
            '</tr>');
    });
}

function editHolding(tickerExchange, name, weight) {
    const holding = {
        tickerExchange: tickerExchange,
        name: name,
        weight: weight
    };

    fetch('https://localhost:7213/api/Holdings/' + tickerExchange, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(holding)
    })
        .then(response => { if (!response.ok) { throw response } })
        .then(() => getHoldings())
        .then(() => getLastModifiedDate())
        .catch(error => alert('Unable to edit holding. Make sure there is enough cash in the portfolio.'));
}

function deleteHolding(tickerExchange) {
    fetch('https://localhost:7213/api/Holdings/' + tickerExchange, {
        method: 'DELETE'
    })
        .then(() => getHoldings())
        .then(() => getLastModifiedDate())
        .catch(error => console.error('Unable to delete item.', error));
}

function getLastModifiedDate() {
    fetch('https://localhost:7213/api/LastModifiedDate')
        .then(response => response.json())
        .then(data => displayLastModifiedDate(data))
        .catch(error => alert('Unable to get last modified date', error));
}

function displayLastModifiedDate(data) {
    if (data) {
        var lastModifiedDate = new Date(Date.parse(data));
        const formattedDate = lastModifiedDate.getUTCFullYear() + "/" +
            ("0" + (lastModifiedDate.getUTCMonth() + 1)).slice(-2) + "/" +
            ("0" + lastModifiedDate.getUTCDate()).slice(-2) + " " +
            ("0" + lastModifiedDate.getUTCHours()).slice(-2) + ":" +
            ("0" + lastModifiedDate.getUTCMinutes()).slice(-2) + ":" +
            ("0" + lastModifiedDate.getUTCSeconds()).slice(-2);
        $('div#last-modified-date').text('Last modified: ' + formattedDate);
    }
}