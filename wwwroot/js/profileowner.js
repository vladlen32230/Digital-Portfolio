document.getElementById("exitB").onclick = (() => {
    fetch("/api/unsign", {
        method: "GET"
    }).then((response) => {
        if (response.ok) {
            window.location.href = "/";
        }
    });
});

const user_id = window.location.pathname.split("/")[2];
document.getElementById("change_infoB").onclick = (() => window.location = user_id + "/changeinfo");
document.getElementById("add_portfolioB").onclick = (() => window.location = user_id + "/addportfolio");
document.getElementById("on_mainB").onclick = (() => window.location.href = "/");
document.getElementById("searchB").onclick = (() => window.location.href = "/search");
fetch("/api/" + user_id + "/getprofileinfo", {
    method: "GET"
}).then(async (response) => {
    const data = await response.json();
    document.getElementById("photoIm").setAttribute("src", data.photo);
    document.getElementById("nameL").innerHTML = data.name;
    document.getElementById("descriptionL").innerHTML = data.description;
    if (data.vk != "") {
        const vkB = document.getElementById("vkB");
        vkB.onclick = (() => window.open("https://vk.com/" + data.vk, '_blank').focus());
        vkB.hidden = false;
    }

    if (data.telegram != "") {
        const telegramB = document.getElementById("telegramB");
        telegramB.onclick = (() => window.open("https://t.me/" + data.telegram, '_blank').focus());
        telegramB.hidden = false;
    }

    document.getElementById("first_skillL").innerHTML = data.first_skill;
    document.getElementById("second_skillL").innerHTML = data.second_skill;
    document.getElementById("third_skillL").innerHTML = data.third_skill;
    for (let portfolio of data.portfolio_infos) {
        const block = document.createElement("div");
        const photo = new Image();
        photo.src = portfolio.photo;
        block.appendChild(photo);
        const name = document.createElement("a");
        name.innerHTML = portfolio.name;
        name.href = user_id + "/p/" + portfolio.id;
        block.appendChild(name);
        document.getElementById("portfolios").appendChild(block);
    }
});