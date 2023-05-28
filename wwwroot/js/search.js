document.getElementById("on_mainB").onclick = (() => window.location.href = "/");
document.getElementById("findB").onclick = (() => {
    const search_results = document.getElementById("search_results");
    search_results.innerHTML = "";
    const data = new FormData();
    var is_user;
    data.append("Name", document.getElementById("nameI").value);
    data.append("Words", document.getElementById("keywordsI").value);
    if (document.getElementById("portfolioR").checked) {
        data.append("type", "project");
        is_user = false;
    }

    else if (document.getElementById("accountR").checked) {
        data.append("type", "user");
        is_user = true;
    }

    else {
        const statusL = document.getElementById("statusL");
        statusL.innerHTML = "Выберите тип";
        setTimeout(() => statusL.innerHTML = "", 3000);
        return;
    }

    fetch("api/search", {
        method: "POST",
        body: data
    }).then(async (response) => {
        if (response.ok) {
            const data = await response.json();
            for (let result of data) {
                const block = document.createElement("div");
                const photo = new Image();
                if (result.photo != null)
                    photo.src = result.photo;
                block.appendChild(photo);
                const name = document.createElement("a");
                name.innerHTML = result.name;
                if (is_user)
                    name.setAttribute("href", "u/" + result.id);
                else
                    name.setAttribute("href", "u/" + result.author_id + "/p/" + result.id);
                block.appendChild(name);
                const description = document.createElement("label");
                description.innerHTML = result.description;
                block.appendChild(description);
                search_results.appendChild(block);
            }
        }
    })
});