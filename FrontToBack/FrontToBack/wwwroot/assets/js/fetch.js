const addbasketbtn = document.querySelectorAll(".addbasketbtn")

addbasketbtn.forEach(btn => {
    btn.addEventListener("click", function (e) {
        e.preventDefault();

        var endpoint = btn.getAttribute("href");
        fetch(endpoint)
            .then(Response => Response.text())
            .then(data => {
                console.log(data)
            })
    
})