
let basket = JSON.parse(localStorage.getItem("basket")) || []


function saveBasket() {
    localStorage.setItem("basket", JSON.stringify(basket));
    let total = getBasketTotal();
    localStorage.setItem("basketTotal", total.toString());


}

function clearBasket() {
    localStorage.setItem("basket", JSON.stringify([]));
}

function addToBasket(productId) {
    console.log(productId);
    const exists = basket.find(x => x.productId === productId);

    if (exists) {
        exists.quantity++;
    }
    else {
        basket.push({ productId: productId, quantity: 1 });
    }
    console.log(basket)

    saveBasket();

    document.querySelectorAll(".quantity").forEach(td => {
        const id = td.dataset.id;
        console.log(td.dataset.id);
        td.textContent = getBasketQuantity(id);
    });

    location.reload();
}

function removeFromBasket(productId) {
    console.log(productId);
    const exists = basket.find(x => x.productId === productId);

    if (exists) {
        exists.quantity--;
    }
    if (exists.quantity == 0) {
        basket = basket.filter(x => x.productId !== productId)
    }

    saveBasket();

    document.querySelectorAll(".quantity").forEach(td => {
        const id = td.dataset.id;
        console.log(td.dataset.id);
        td.textContent = getBasketQuantity(id);
    });
    location.reload();
    //sessionStorage.setItem("submitted", "false");
}

function getBasketQuantity(productId) {
    console.log(productId);
    const exists = basket.find(x => x.productId == productId);
    let result = 0;
    if (exists) {
        result = exists.quantity;

    }
    else {
        result = 0;
    }
    console.log("trying")
    return result;
}

function getBasketTotal() {
    var total = 0
    basket.forEach(entry => {
        console.log(entry.productId);
        total += entry.quantity;
    });
    return total;
}

document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".quantity").forEach(td => {
        const id = td.dataset.id;
        console.log(td.dataset.id);
        td.textContent = getBasketQuantity(id);

    });
    saveBasket();
});

function updateBasketTotal() {
    document.querySelectorAll("#basket-total").forEach(a => {
        a.innerHTML += "(" + getBasketTotal() + ")";


    });

}

document.addEventListener("DOMContentLoaded", () => {
    updateBasketTotal();

});
